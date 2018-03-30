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

namespace Microsoft.O365.ActionableMessages.Utilities
{
    /// <summary>
    /// Class with constants for O365 OpenID configuration.
    /// </summary>
    public static class O365OpenIdConfiguration
    {
        /// <summary>
        /// The URL of the O365 OpenID Connect metadata endpoint.
        /// </summary>
        public const string MetadataUrl = "https://substrate.office.com/sts/common/.well-known/openid-configuration";

        /// <summary>
        /// The value of the "iss" claim in the token.
        /// </summary>
        public const string TokenIssuer = "https://substrate.office.com/sts/";

        /// <summary>
        /// The value of the "appid" claim in the token.
        /// </summary>
        public const string AppId = "48af08dc-f6d2-435f-b2a7-069abd99c086";

        /// <summary>
        /// The value of the "ver" claim in the token.
        /// </summary>
        public const string Version = "STI.ExternalAccessToken.V1";

        /// <summary>
        /// The type of token issued by O365.
        /// </summary>
        public const string TokenType = "JWT";

        /// <summary>
        /// The signing algorithm for JWT tokens.
        /// </summary>
        public const string JwtSigningAlgorithm = "RS256";

        /// <summary>
        /// The hash algorithm used to sign the JWT token.
        /// </summary>
        public const string HashAlgorithm = "SHA256";

        /// <summary>
        /// The value of the "appidacr" claim in the token.
        /// </summary>
        public const string AppAuthContextClassReference = "2";

        /// <summary>
        /// The value of the "acr" claim in the token.
        /// </summary>
        public const string AuthContextClassReference = "0";
    }
}
