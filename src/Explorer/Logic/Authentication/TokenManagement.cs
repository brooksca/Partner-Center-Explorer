// -----------------------------------------------------------------------
// <copyright file="TokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    using Cache;
    using Configuration;
    using IdentityModel.Clients.ActiveDirectory;
    using Models;
    using PartnerCenter.Extensions;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Management interface for retrieving access tokens.
    /// </summary>
    public class TokenManagement : ITokenManagement
    {
        private IExplorerService _service;
        private readonly string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenManagement"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <exception cref="ArgumentNullException">
        /// service 
        /// </exception>
        public TokenManagement(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));
            _service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenManagement"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <param name="key">A unique indentifier for the cache entry.</param>
        /// <exception cref="ArgumentException">
        /// key
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// service 
        /// </exception>
        public TokenManagement(IExplorerService service, string key)
        {
            key.AssertNotEmpty(nameof(key));
            service.AssertNotNull(nameof(service));

            _service = service;
            _key = key;
        }

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipent of the requested token.</param>
        /// <returns>An instnace of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// resource
        /// </exception>
        public AuthenticationToken GetAppOnlyToken(string authority, string key, string resource)
        {
            authority.AssertNotEmpty(nameof(authority));
            key.AssertNotEmpty(nameof(key));
            resource.AssertNotEmpty(nameof(resource));

            return SynchronousExecute(() => GetAppOnlyTokenAsync(authority, key, resource));
        }

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
        public async Task<AuthenticationToken> GetAppOnlyTokenAsync(string authority, string key, string resource)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            key.AssertNotEmpty(nameof(key));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                // If the Redis Cache connection string is not populated then utilize the constructor
                // that only requires the authority. That constructor will utilize a in-memory caching
                // feature that is built-in into ADAL.
                if (string.IsNullOrEmpty(ApplicationConfiguration.RedisConnection))
                {
                    authContext = new AuthenticationContext(authority);
                }
                else
                {
                    tokenCache = new DistributedTokenCache(resource, key);
                    authContext = new AuthenticationContext(authority, tokenCache);
                }

                authResult = await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        ApplicationConfiguration.ApplicationId,
                        ApplicationConfiguration.ApplicationSecret));

                return new AuthenticationToken(authResult.AccessToken, authResult.ExpiresOn);
            }
            finally
            {
                authContext = null;
                authResult = null;
                tokenCache = null;
            }
        }

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
        public AuthenticationToken GetAppPlusUserToken(string authority, string resource, string token)
        {
            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));
            token.AssertNotEmpty(nameof(token));

            return SynchronousExecute(() => GetAppPlusUserTokenAsync(authority, resource, token));
        }

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
        public async Task<AuthenticationToken> GetAppPlusUserTokenAsync(string authority, string resource, string token)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));
            token.AssertNotEmpty(nameof(token));

            try
            {
                if (string.IsNullOrEmpty(ApplicationConfiguration.RedisConnection))
                {
                    authContext = new AuthenticationContext(authority);
                }
                else
                {
                    tokenCache = new DistributedTokenCache(resource, _key);
                    authContext = new AuthenticationContext(authority, tokenCache);
                }

                authResult = await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        ApplicationConfiguration.ApplicationId,
                        ApplicationConfiguration.ApplicationSecret),
                    new UserAssertion(token, "urn:ietf:params:oauth:grant-type:jwt-bearer"));

                return new AuthenticationToken(authResult.AccessToken, authResult.ExpiresOn);
            }
            finally
            {
                authContext = null;
                authResult = null;
                tokenCache = null;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        public IPartnerCredentials GetPartnerCenterAppOnlyCredentials(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            return SynchronousExecute(() => GetPartnerCenterAppOnlyCredentialsAsync(authority));
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        public async Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            string key = "Resource::AppOnly::PartnerCenter";

            // Attempt to obtain the Partner Center token from the cache.
            IPartnerCredentials credentials =
                 await _service.CachingService.FetchAsync<PartnerCenterTokenModel>(
                     CacheDatabaseType.Authentication, key);

            if (credentials != null && !credentials.IsExpired())
            {
                return credentials;
            }

            // The access token has expired, so a new one must be requested.
            credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                ApplicationConfiguration.PartnerCenterApplicationId,
                ApplicationConfiguration.PartnerCenterApplicationSecret,
                ApplicationConfiguration.AccountId);

            await _service.CachingService.StoreAsync(
                CacheDatabaseType.Authentication, key, credentials);

            return credentials;
        }

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
        public AuthenticationToken GetTokenByAuthorizationCode(string authority, string code, string resource, Uri redirectUri)
        {
            authority.AssertNotEmpty(nameof(authority));
            code.AssertNotEmpty(nameof(code));
            redirectUri.AssertNotNull(nameof(redirectUri));
            resource.AssertNotEmpty(nameof(resource));

            return SynchronousExecute(() => GetTokenByAuthorizationCodeAsync(authority, code, resource, redirectUri));

        }

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
        public async Task<AuthenticationToken> GetTokenByAuthorizationCodeAsync(string authority, string code, string resource, Uri redirectUri)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            code.AssertNotEmpty(nameof(code));
            redirectUri.AssertNotNull(nameof(redirectUri));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                // If the Redis Cache connection string is not populated then utilize the constructor
                // that only requires the authority. That constructor will utilize a in-memory caching
                // feature that is built-in into ADAL.
                if (string.IsNullOrEmpty(ApplicationConfiguration.RedisConnection))
                {
                    authContext = new AuthenticationContext(authority);
                }
                else
                {
                    tokenCache = new DistributedTokenCache(resource, _key);
                    authContext = new AuthenticationContext(authority, tokenCache);
                }

                authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(code,
                    redirectUri,
                    new ClientCredential(
                        ApplicationConfiguration.ApplicationId,
                        ApplicationConfiguration.ApplicationSecret),
                    resource);

                return new AuthenticationToken(authResult.AccessToken, authResult.ExpiresOn);
            }
            finally
            {
                authContext = null;
                tokenCache = null;
            }
        }

        /// <summary>
        /// Synchronously executes an asynchronous function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        private static T SynchronousExecute<T>(Func<Task<T>> operation)
        {
            try
            {
                return Task.Run(async () => await operation()).Result;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}