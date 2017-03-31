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
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the JSON Web Token.
    /// </summary>
    public class JsonWebToken
    {
        /// <summary>
        /// Gets the header JSON object.
        /// </summary>
        public JObject Header { get; private set; }

        /// <summary>
        /// Gets the payload JSON object.
        /// </summary>
        public JObject Payload { get; private set; }

        /// <summary>
        /// Gets or sets the signature of this token.
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// Constructor of the <see cref="JsonWebToken"/> class.
        /// </summary>
        public JsonWebToken()
        {
            this.Header = new JObject();
            this.Payload = new JObject();
        }

        /// <summary>
        /// Returns the JSON web token in base64-encoded string.
        /// </summary>
        /// <returns>The base64-encoded string of this token.</returns>
        public override string ToString()
        {
            StringBuilder jwt = new StringBuilder();
            jwt.Append(DataConverter.ToBase64UrlString(this.Header.ToString(Formatting.None)));
            jwt.Append(".");
            jwt.Append(DataConverter.ToBase64UrlString(this.Payload.ToString(Formatting.None)));
            jwt.Append(".");
            jwt.Append(DataConverter.ToBase64UrlString(this.Signature));

            return jwt.ToString();
        }
    }
}
