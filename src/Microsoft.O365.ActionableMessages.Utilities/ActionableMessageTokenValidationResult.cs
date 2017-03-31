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

namespace Microsoft.O365.ActionableMessages.Authentication
{
    using System;

    /// <summary>
    /// Represents claims from an actionable message token.
    /// </summary>
    public class ActionableMessageTokenValidationResult
    {
        /// <summary>
        /// Gets or sets the flag to indicate if a token validation operation succeeded.
        /// </summary>
        public bool ValidationSucceeded { get; set; }

        /// <summary>
        /// Gets or sets the email address of the person who performed the action. In some cases,
        /// it will be the hash of the email address.
        /// </summary>
        public string ActionPerformer { get; set; }

        /// <summary>
        /// Gets or sets the email address of the sender of the actionable message.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the exception happened during the token validation.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Constructor of the <see cref="ActionableMessageTokenValidationResult"/> class.
        /// </summary>
        public ActionableMessageTokenValidationResult()
        {
            this.ValidationSucceeded = false;
            this.Exception = null;
        }
    }
}
