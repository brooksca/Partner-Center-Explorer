// -----------------------------------------------------------------------
// <copyright file="ITokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a manaagement interface for retrieving access tokens.
    /// </summary>
    public interface ITokenManagement
    {
        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <param name="key">Key to be utilized for the access token caching strategy.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// key
        /// or
        /// resource
        /// </exception>
        AuthenticationToken GetAppOnlyToken(string authority, string key, string resource);

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="key">Key to be utilized for the access token caching strategy.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// key
        /// or
        /// resource
        /// </exception>
        Task<AuthenticationToken> GetAppOnlyTokenAsync(string authority, string key, string resource);

        /// <summary>
        /// Gets an access token from the authority using app + user authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <param name="token">Assertion token representing the user.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// resource
        /// or
        /// token
        /// </exception>
        AuthenticationToken GetAppPlusUserToken(string authority, string resource, string token);

        /// <summary>
        /// Gets an access token from the authority using app + user authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <param name="token">Assertion token represting the user.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// resource
        /// or 
        /// token
        /// </exception>
        Task<AuthenticationToken> GetAppPlusUserTokenAsync(string authority, string resource, string token);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// authority
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        IPartnerCredentials GetPartnerCenterAppOnlyCredentials(string authority);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// authority
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority);

        /// <summary>
        /// Gets an access token utilizing an authorization code. 
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="code">Authorization code received from the service authorization endpoint.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <param name="redirectUri">Redirect URI used for obtain the authorization code.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// code
        /// or
        /// resource
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// redirectUri
        /// </exception>
        AuthenticationToken GetTokenByAuthorizationCode(string authority, string code, string resource, Uri redirectUri);

        /// <summary>
        /// Gets an access token utilizing an authorization code. 
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="code">Authorization code received from the service authorization endpoint.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <param name="redirectUri">Redirect URI used for obtain the authorization code.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// code
        /// or
        /// resource
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// redirectUri
        /// </exception>
        Task<AuthenticationToken> GetTokenByAuthorizationCodeAsync(string authority, string code, string resource, Uri redirectUri);
    }
}