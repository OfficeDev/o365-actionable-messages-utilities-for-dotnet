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
    using System.IdentityModel.Tokens;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.IdentityModel.Protocols;
    using Microsoft.O365.ActionableMessages.Authentication;
    using Moq;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="ActionableMessageTokenValidator"/>.
    /// </summary>
    public class TokenValidatorTests
    {
        private const string TestCertPath = @"data\signingcert.pfx";
        private const string TestOpenIdConfigurationPath = @"data\openidconfiguration.json";

        /// <summary>
        /// Unit tests for validating valid JSON web tokens.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        [Fact]
        public async Task TestValidToken()
        {
            var testCert = GetTestCert();
            var openIdConfig = GetTestO365OpenIdConnectConfiguration(testCert);

            CancellationToken cancelToken;
            Mock<IConfigurationManager<OpenIdConnectConfiguration>> mockConfigManager = new Mock<IConfigurationManager<OpenIdConnectConfiguration>>();
            mockConfigManager.Setup(cm => cm.GetConfigurationAsync(cancelToken)).ReturnsAsync(openIdConfig);

            ActionableMessageTokenValidator validator = new ActionableMessageTokenValidator(mockConfigManager.Object);
            string token = CertificateHelper.GenerateJsonWebToken(testCert, "john@contoso.com", "nicole@contoso.com", "https://api.contoso.com");
            ActionableMessageTokenValidationResult result = await validator.ValidateTokenAsync(token, "https://api.contoso.com");

            Assert.Equal(true, result.ValidationSucceeded);
            Assert.Equal("john@contoso.com", result.ActionPerformer);
            Assert.Equal("nicole@contoso.com", result.Sender);
        }

        /// <summary>
        /// Unit tests for validating invalid JSON web tokens.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        [Fact]
        public async Task TestInvalidTokens()
        {
            ActionableMessageTokenValidator validator = new ActionableMessageTokenValidator();
            ActionableMessageTokenValidationResult result = await validator.ValidateTokenAsync("abc", "https://www.microsoft.com");
            Assert.Equal(false, result.ValidationSucceeded);
        }

        /// <summary>
        /// Unit tests for validating null or empty JSON web tokens.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>An asynchronous task.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task TestNullOrEmptyToken(string token)
        {
            ActionableMessageTokenValidator validator = new ActionableMessageTokenValidator();
            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.ValidateTokenAsync(token, null));
        }

        /// <summary>
        /// Gets the instance of <see cref="OpenIdConnectConfiguration"/> with the given certificate.
        /// </summary>
        /// <param name="cert">The certificate with the signing public keys.</param>
        /// <returns>The instance of <see cref="OpenIdConnectConfiguration"/>.</returns>
        private static OpenIdConnectConfiguration GetTestO365OpenIdConnectConfiguration(X509Certificate2 cert)
        {
            OpenIdConnectConfiguration config = new OpenIdConnectConfiguration(GetTestOpenIDConfiguration().ToString());
            JObject jwk = CertificateHelper.ToJsonWebKey(cert);
            JsonWebKeySet jwks = new JsonWebKeySet(jwk.ToString());

            foreach (SecurityToken token in jwks.GetSigningTokens())
            {
                config.SigningTokens.Add(token);
            }

            return config;
        }

        /// <summary>
        /// Gets the certificate for testing.
        /// </summary>
        /// <returns></returns>
        private static X509Certificate2 GetTestCert()
        {
            Uri fileUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filePath = Uri.UnescapeDataString(fileUrl.AbsolutePath);
            X509Certificate2 cert = new X509Certificate2(Path.Combine(Path.GetDirectoryName(filePath), TestCertPath));

            return cert;
        }

        /// <summary>
        /// Gets the Open ID configuration for testing.
        /// </summary>
        /// <returns></returns>
        private static JObject GetTestOpenIDConfiguration()
        {
            Uri fileUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string filePath = Uri.UnescapeDataString(fileUrl.AbsolutePath);

            using (StreamReader sr = new StreamReader(Path.Combine(Path.GetDirectoryName(filePath), TestOpenIdConfigurationPath)))
            {
                return JObject.Parse(sr.ReadToEnd());
            }
        }
    }
}
