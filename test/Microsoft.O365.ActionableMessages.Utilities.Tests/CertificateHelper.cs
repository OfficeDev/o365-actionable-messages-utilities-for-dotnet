// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Microsoft.O365.ActionableMessages.Utilities.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Helper class for dealing with certificates.
    /// </summary>
    public static class CertificateHelper
    {
        /// <summary>
        /// Converts an X.509 certificate to a JSON Web Key.
        /// </summary>
        /// <param name="cert">The certificate to convert.</param>
        /// <returns>The JSON Web Key representation of the certificate.</returns>
        public static JObject ToJsonWebKey(X509Certificate2 cert)
        {
            if (cert == null)
            {
                return new JObject();
            }

            byte[] thumbprint = DataConverter.HexStringToByteArray(cert.Thumbprint);
            string base64UrlThumbprint = DataConverter.ToBase64UrlString(thumbprint);

            List<byte[]> data = DecodeAsnData(cert.GetPublicKey());
            byte[] modulus = new byte[data[0].Length - 1];
            Array.Copy(data[0], 1, modulus, 0, modulus.Length);

            JObject key = JObject.FromObject(new
            {
                kty = "RSA",
                use = "sig",
                kid = base64UrlThumbprint,
                x5t = base64UrlThumbprint,
                n = DataConverter.ToBase64UrlString(modulus),
                e = DataConverter.ToBase64UrlString(data[1]),
                x5c = new JArray(new JValue(Convert.ToBase64String(cert.GetRawCertData())))
            });

            JObject jwk = new JObject();
            jwk.Add(new JProperty("keys", new JArray(key)));

            return jwk;
        }

        /// <summary>
        /// Generates a JSON web token.
        /// </summary>
        /// <param name="signingCert">The certificate with the private key to sign the token.</param>
        /// <param name="subject">Subject of the payload.</param>
        /// <param name="sender">Sender of the payload.</param>
        /// <param name="audience">Audience of the payload.</param>
        /// <returns>The JSON web token with signature.</returns>
        public static string GenerateJsonWebToken(X509Certificate2 signingCert, string subject, string sender, string audience)
        {
            byte[] thumbprint = DataConverter.HexStringToByteArray(signingCert.Thumbprint);
            string base64Thumbprint = DataConverter.ToBase64UrlString(thumbprint);

            JsonWebToken jwt = new Tests.JsonWebToken();
            jwt.Header.Add(new JProperty("typ", O365OpenIdConfiguration.TokenType));
            jwt.Header.Add(new JProperty("alg", O365OpenIdConfiguration.JwtSigningAlgorithm));
            jwt.Header.Add(new JProperty("x5t", base64Thumbprint));
            jwt.Header.Add(new JProperty("kid", base64Thumbprint));

            long currentTime = GetPosixTimeStamp();
            long expiryTime = currentTime + 3600;       // Expire in 1 hour

            jwt.Payload.Add(new JProperty("iat", currentTime));
            jwt.Payload.Add(new JProperty("nbf", currentTime));
            jwt.Payload.Add(new JProperty("exp", expiryTime));
            jwt.Payload.Add(new JProperty("ver", O365OpenIdConfiguration.Version));
            jwt.Payload.Add(new JProperty("appid", O365OpenIdConfiguration.AppId));
            jwt.Payload.Add(new JProperty("iss", O365OpenIdConfiguration.TokenIssuer));
            jwt.Payload.Add(new JProperty("appidacr", O365OpenIdConfiguration.AppAuthContextClassReference));
            jwt.Payload.Add(new JProperty("acr", O365OpenIdConfiguration.AuthContextClassReference));
            jwt.Payload.Add(new JProperty("sub", subject));
            jwt.Payload.Add(new JProperty("sender", sender));
            jwt.Payload.Add(new JProperty("aud", audience));

            //
            // unsignedToken = encodeBase64(header) + '.' + encodeBase64(payload)
            // signature     = HMAC-SHA256(private key, unsignedToken)
            //
            StringBuilder unsignedToken = new StringBuilder();
            unsignedToken.Append(DataConverter.ToBase64UrlString(jwt.Header.ToString(Formatting.None)));
            unsignedToken.Append(".");
            unsignedToken.Append(DataConverter.ToBase64UrlString(jwt.Payload.ToString(Formatting.None)));

            RSACryptoServiceProvider rsa = signingCert.PrivateKey as RSACryptoServiceProvider;
            jwt.Signature = rsa.SignData(UTF8Encoding.UTF8.GetBytes(unsignedToken.ToString()), O365OpenIdConfiguration.HashAlgorithm);

            return jwt.ToString();
        }

        /// <summary>
        /// Decodes ASN.1 DER structure.
        /// </summary>
        /// <param name="encodedData">The base64 DER-encoded data.</param>
        /// <returns>The decoded DER data.</returns>
        private static List<byte[]> DecodeAsnData(byte[] encodedData)
        {
            List<byte[]> data = new List<byte[]>();

            byte type;
            int length;
            int i = 0;

            while (i < encodedData.Length)
            {
                GetTypeLengthValue(encodedData, out type, out length, ref i);

                switch (type)
                {
                    case 0x30:
                        break;

                    default:
                        {
                            byte[] value = new byte[length];
                            Array.Copy(encodedData, i, value, 0, length);
                            i += length;
                            data.Add(value);
                        }
                        break;
                }
            }

            return data;
        }

        /// <summary>
        /// Read the next type-length-value (TLV) from a data stream.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="type">Returns the type of the data.</param>
        /// <param name="length">Returns the length of the data.</param>
        /// <param name="i">
        /// Points to the next byte of the encoded data to read from and 
        /// the index is updated after the read.
        /// </param>
        private static void GetTypeLengthValue(byte[] encodedData, out byte type, out int length, ref int i)
        {
            type = encodedData[i++];
            length = GetLength(encodedData, ref i);
        }

        /// <summary>
        /// Reads the length of the data in a TLV element.
        /// </summary>
        /// <param name="encodedData">The encoded data.</param>
        /// <param name="i">
        /// Points to the next byte of the encoded data to read and
        /// the index is updated after the read.
        /// </param>
        /// <returns>The length of the data in a TLV element.</returns>
        private static int GetLength(byte[] encodedData, ref int i)
        {
            byte b = encodedData[i++];
            int size = b & 0x7f;
            int msb = b & 0x80;

            if (msb == 0)
            {
                return size;
            }

            int c = 0;
            int length = 0;

            while (c++ < size)
            {
                length = (length << 8) | encodedData[i++];
            }

            return length;
        }

        /// <summary>
        /// Gets the POSIX time in seconds, which is the seconds elapsed since 
        /// January 1, 1970 00:00:00 (UTC).
        /// </summary>
        /// <returns></returns>
        private static long GetPosixTimeStamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
