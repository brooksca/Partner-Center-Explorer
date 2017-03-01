// -----------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Configuration;
    using Graph;
    using Logic;

    /// <summary>
    /// Authentication provider for the Microsoft Graph service client.
    /// </summary>
    /// <seealso cref="IAuthenticationProvider" />
    public class AuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// Name of the authentication header to be utilized. 
        /// </summary>
        private const string AuthHeaderName = "Authorization";

        /// <summary>
        /// The type of token being utilized for the authentication request.
        /// </summary>
        private const string TokenType = "Bearer";

        /// <summary>
        /// Provides the ability to request access tokens.
        /// </summary>
        private readonly ITokenManagement tokenMgmt;

        /// <summary>
        /// The customer identifier utilized to scope the Microsoft Graph requests.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationProvider"/> class.
        /// </summary>
        /// <param name="tokenMgmt">Provides the ability to manage access tokens.</param>
        /// <param name="customerId">Identifier for customer whose resources are being accessed.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="tokenMgmt"/> is null.
        /// </exception>
        public AuthenticationProvider(ITokenManagement tokenMgmt, string customerId)
        {
            tokenMgmt.AssertNotNull(nameof(tokenMgmt));
            customerId.AssertNotEmpty(nameof(customerId));

            this.customerId = customerId;
            this.tokenMgmt = tokenMgmt;
        }

        /// <summary>
        /// Performs the necessary authentication and injects the required header.
        /// </summary>
        /// <param name="request">The request being made to the Microsoft Graph API.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            AuthenticationToken token = await this.tokenMgmt.GetAppOnlyTokenAsync(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{this.customerId}",
                ApplicationConfiguration.GraphEndpoint);

            request.Headers.Add(AuthHeaderName, $"{TokenType} {token.Token}");
        }
    }
}