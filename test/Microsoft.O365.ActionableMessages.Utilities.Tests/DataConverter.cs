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
    using System.Text;

    /// <summary>
    /// Conversion functions for various kind of data.
    /// </summary>
    public static class DataConverter
    {
        /// <summary>
        /// Converts a hex string to byte array.
        /// </summary>
        /// <param name="hex">The hex string to convert.</param>
        /// <returns>The byte array of the converted hex string.</returns>
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new ArgumentException("String length must be even.", "hex");
            }

            int dataSize = hex.Length / 2;
            byte[] data = new byte[dataSize];
            int i = 0;

            while (i < dataSize)
            {
                byte b1 = Convert.ToByte(hex[i * 2].ToString(), 16);
                byte b2 = Convert.ToByte(hex[i * 2 + 1].ToString(), 16);
                data[i++] = (byte)((b1 << 4) | b2);
            }

            return data;
        }

        /// <summary>
        /// Converts a byte array to base64 url representation.
        /// </summary>
        /// <param name="data">Byte array to convert.</param>
        /// <returns>The base64 url encoded string.</returns>
        public static string ToBase64UrlString(byte[] data)
        {
            return Convert.ToBase64String(data)
                .Replace("=", "")       // Remove padding.
                .Replace('+', '-')
                .Replace('/', '_');
        }

        /// <summary>
        /// Converts a string to Base64 URL representation.
        /// </summary>
        /// <param name="s">String to convert.</param>
        /// <returns>The base64 url encoded string.</returns>
        public static string ToBase64UrlString(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            return ToBase64UrlString(data);
        }
    }
}
