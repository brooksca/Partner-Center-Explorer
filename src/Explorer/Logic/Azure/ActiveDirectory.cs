// -----------------------------------------------------------------------
// <copyright file="ActiveDirectory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using Authentication;
    using Configuration;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the ability to interact with Azure Active Directory.
    /// </summary>
    public class ActiveDirectory : IActiveDirectory
    {
        private IActiveDirectoryClient _client;
        private IExplorerService _service;
        private ITokenManagement _tokenMgmt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <param name="tenantId">The tenant identifer for the customer.</param>
        /// <exception cref="ArgumentNullException">
        /// service 
        /// </exception>
        /// <exception cref="ArgumentException">
        /// tenantId
        /// </exception>
        public ActiveDirectory(IExplorerService service, string tenantId)
        {
            string token;

            service.AssertNotNull(nameof(service));
            tenantId.AssertNotEmpty(nameof(tenantId));

            _service = service;
            _tokenMgmt = new TokenManagement(service);

            token = _tokenMgmt.GetAppOnlyToken(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                Resources.ADAppOnlyKey,
                ApplicationConfiguration.ActiveDirectoryGraphEndpoint).Token;

            _client = new ActiveDirectoryClient(
                new Uri(new Uri(ApplicationConfiguration.ActiveDirectoryGraphEndpoint), tenantId),
                async () => await Task.FromResult(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <param name="tenantId">The tenant identifer for the customer.</param>
        /// <param name="assertionToken">A valid JSON Web Token (JWT).</param>
        /// <exception cref="ArgumentException">
        /// tenantId
        /// or
        /// assertionToken
        /// </exception>
        public ActiveDirectory(IExplorerService service, string tenantId, string assertionToken)
        {
            string token;

            assertionToken.AssertNotEmpty(nameof(assertionToken));
            service.AssertNotNull(nameof(service));
            tenantId.AssertNotEmpty(nameof(tenantId));

            _service = service;
            _tokenMgmt = new TokenManagement(service);
            token = _tokenMgmt.GetAppPlusUserToken(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                ApplicationConfiguration.ActiveDirectoryGraphEndpoint,
                assertionToken).Token;

            _client = new ActiveDirectoryClient(
                new Uri(new Uri(ApplicationConfiguration.ActiveDirectoryGraphEndpoint), tenantId),
                async () => await Task.FromResult(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <param name="tenantId">The tenant identifer for the customer.</param>
        /// <param name="code">Authorization code received from the service authorization endpoint.</param>
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

            _service = service; 
            _tokenMgmt = new TokenManagement(service, key);
            token = _tokenMgmt.GetTokenByAuthorizationCode(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                code,
                ApplicationConfiguration.ActiveDirectoryGraphEndpoint,
                redirectUri).Token;

            _client = new ActiveDirectoryClient(
                new Uri(new Uri(ApplicationConfiguration.ActiveDirectoryGraphEndpoint), tenantId),
                async () => await Task.FromResult(token));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDirectory"/> class.
        /// </summary>
        /// <param name="client">An instance of <see cref="IActiveDirectoryClient"/>.</param>
        /// <param name="service">Provides access to the application core services.</param>
        /// <exception cref="ArgumentNullException">
        /// client
        /// or
        /// service
        /// </exception>
        public ActiveDirectory(IActiveDirectoryClient client, IExplorerService service)
        {
            client.AssertNotNull(nameof(client));
            service.AssertNotNull(nameof(service));

            _client = client;
            _service = service;
        }

        /// <summary>
        /// Obtains a list of directory roles that the specified directory is associated with.
        /// </summary>
        /// <param name="objectId">Object identifier for the object to be checked.</param>
        /// <returns>A list of directory that the specified object identifier is associated with.</returns>
        /// <exception cref="ArgumentException">
        /// objectId
        /// </exception>
        public async Task<List<IDirectoryRole>> GetDirectoryRolesAsync(string objectId)
        {
            DirectoryRole role;
            IPagedCollection<IDirectoryObject> roles;
            List<IDirectoryRole> values;

            objectId.AssertNotEmpty(objectId);

            try
            {
                roles = await _client.Users.GetByObjectId(objectId).MemberOf.ExecuteAsync();
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
        /// Obtains a list service configuration records for all domains that are associated with the tenant.
        /// </summary>
        /// <returns>A list of service configuration records for all domains associated with the tenant.</returns>
        public async Task<List<IDomainDnsRecord>> GetDomainDnsRecordsAsync()
        {
            IPagedCollection<IDomainDnsRecord> records;
            List<IDomainDnsRecord> models;

            try
            {
                records = await _client.DomainDnsRecords.ExecuteAsync();
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
    }
}