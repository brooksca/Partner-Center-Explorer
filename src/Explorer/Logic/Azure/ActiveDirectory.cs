// -----------------------------------------------------------------------
// <copyright file="ActiveDirectory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Authentication;
    using Configuration;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;

    /// <summary>
    /// Provides the ability to interact with Azure Active Directory.
    /// </summary>
    public class ActiveDirectory : IActiveDirectory
    {
        /// <summary>
        /// Key for interfacing with the cache service when utilizing App Only authentication.  
        /// </summary>
        private const string AdAppOnlyKey = "AppOnly::AzureAD";

        /// <summary>
        /// Provides the ability to interact with Azure AD. 
        /// </summary>
        private readonly IActiveDirectoryClient client;

        /// <summary>
        /// Provides access to the core application services. 
        /// </summary>
        private IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <param name="tenantId">The tenant identifier for the customer.</param>
        /// <param name="authTokenType">Type of access token to be utilized.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="tenantId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public ActiveDirectory(IExplorerService service, string tenantId, TokenType authTokenType = TokenType.AppOnly)
        {
            string token;

            service.AssertNotNull(nameof(service));
            tenantId.AssertNotEmpty(nameof(tenantId));

            ITokenManagement tokenMgmt = new TokenManagement(service);
            this.service = service;

            if (authTokenType == TokenType.AppOnly)
            {
                token = tokenMgmt.GetAppOnlyToken(
                    $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                    AdAppOnlyKey,
                    ApplicationConfiguration.ActiveDirectoryGraphEndpoint).Token;
            }
            else
            {
                token = tokenMgmt.GetAppPlusUserToken(
                    $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                    ApplicationConfiguration.ActiveDirectoryGraphEndpoint).Token;
            }

            this.client = new ActiveDirectoryClient(
                new Uri(new Uri(ApplicationConfiguration.ActiveDirectoryGraphEndpoint), tenantId),
                async () => await Task.FromResult(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <param name="tenantId">The tenant identifier for the customer.</param>
        /// <param name="code">Authorization code received from the service authorization endpoint.</param>
        /// <param name="key">Key to access the resource in the cache.</param>
        /// <param name="redirectUri">Redirect URI used for obtain the authorization code.</param>
        /// <exception cref="ArgumentException">
        /// tenantId
        /// or
        /// code
        /// or 
        /// key
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// redirectUri
        /// or
        /// service
        /// </exception>
        public ActiveDirectory(IExplorerService service, string tenantId, string code, string key, Uri redirectUri)
        {
            string token;

            service.AssertNotNull(nameof(service));
            tenantId.AssertNotEmpty(nameof(tenantId));
            code.AssertNotEmpty(nameof(code));
            key.AssertNotEmpty(nameof(key));
            redirectUri.AssertNotNull(nameof(redirectUri));

            ITokenManagement tokenMgmt = new TokenManagement(service, key);
            this.service = service;

            token = tokenMgmt.GetTokenByAuthorizationCode(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                code,
                ApplicationConfiguration.ActiveDirectoryGraphEndpoint,
                redirectUri).Token;

            this.client = new ActiveDirectoryClient(
                new Uri(new Uri(ApplicationConfiguration.ActiveDirectoryGraphEndpoint), tenantId),
                async () => await Task.FromResult(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="client">An instance of <see cref="IActiveDirectoryClient"/>.</param>
        /// <param name="service">Provides access to the application core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> is null.
        /// or
        /// <paramref name="service"/> is null.
        /// </exception>
        public ActiveDirectory(IActiveDirectoryClient client, IExplorerService service)
        {
            client.AssertNotNull(nameof(client));
            service.AssertNotNull(nameof(service));

            this.client = client;
            this.service = service;
        }

        /// <summary>
        /// Gets a list of directory roles that the specified directory is associated with.
        /// </summary>
        /// <param name="objectId">Object identifier for the object to be checked.</param>
        /// <returns>A list of directory that the specified object identifier is associated with.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="objectId"/> is empty or null.
        /// </exception>
        public async Task<List<IDirectoryRole>> GetDirectoryRolesAsync(string objectId)
        {
            DirectoryRole role;
            IPagedCollection<IDirectoryObject> roles;
            List<IDirectoryRole> values;

            objectId.AssertNotEmpty(objectId);

            try
            {
                roles = await this.client.Users.GetByObjectId(objectId).MemberOf.ExecuteAsync();
                values = new List<IDirectoryRole>();

                do
                {
                    foreach (IDirectoryObject membership in roles.CurrentPage)
                    {
                        role = membership as DirectoryRole;

                        if (role != null)
                        {
                            values.Add(role);
                        }
                    }

                    roles = await roles.GetNextPageAsync();
                }
                while (roles != null && roles.MorePagesAvailable);

                return values;
            }
            finally
            {
                role = null;
                roles = null;
            }
        }

        /// <summary>
        /// Gets a list service configuration records for all domains that are associated with the tenant.
        /// </summary>
        /// <returns>A list of service configuration records for all domains associated with the tenant.</returns>
        public async Task<List<IDomainDnsRecord>> GetDomainDnsRecordsAsync()
        {
            IPagedCollection<IDomainDnsRecord> records;
            List<IDomainDnsRecord> models;

            try
            {
                records = await this.client.DomainDnsRecords.ExecuteAsync();
                models = new List<IDomainDnsRecord>();

                do
                {
                    models.AddRange(records.CurrentPage.Select(record => new DomainDnsRecord()
                    {
                        Label = record.Label,
                        RecordType = record.RecordType,
                        Ttl = record.Ttl
                    }));

                    records = await records.GetNextPageAsync();
                }
                while (records != null && records.MorePagesAvailable);

                return models;
            }
            finally
            {
                records = null;
            }
        }

        /// <summary>
        /// Gets a list of domains associated with the tenant.
        /// </summary>
        /// <returns>A list of domains associated with the tenant.</returns>
        public async Task<List<IDomain>> GetDomainRecordsAsync()
        {
            IPagedCollection<IDomain> records;
            List<IDomain> models;

            try
            {
                records = await this.client.Domains.ExecuteAsync();
                models = new List<IDomain>();

                do
                {
                    models.AddRange(records.CurrentPage);
                }
                while (records != null && records.MorePagesAvailable);

                return models;
            }
            finally
            {
                records = null;
            }
        }
    }
}