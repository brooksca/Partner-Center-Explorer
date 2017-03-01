// -----------------------------------------------------------------------
// <copyright file="GraphClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Graph;
    using Security;

    /// <summary>
    /// Provides the ability to interact with the Microsoft Graph.
    /// </summary>
    /// <seealso cref="IGraphClient" />
    public class GraphClient : IGraphClient
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Provides access to the Microsoft Graph API.
        /// </summary>
        private readonly IGraphServiceClient client;

        /// <summary>
        /// Identifier of the customer.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="customerId">Identifier for customer whose resources are being accessed.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public GraphClient(IExplorerService service, string customerId)
        {
            service.AssertNotNull(nameof(service));
            customerId.AssertNotEmpty(nameof(customerId));

            ITokenManagement tokenMgmt = new TokenManagement(service);

            this.customerId = customerId;
            this.service = service;
            this.client = new GraphServiceClient(new AuthenticationProvider(tokenMgmt, customerId));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="client">Provides the ability to interact with the Microsoft Graph.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// or
        /// <paramref name="client"/> is null.
        /// </exception>
        public GraphClient(IExplorerService service, IGraphServiceClient client)
        {
            service.AssertNotNull(nameof(service));
            client.AssertNotNull(nameof(client));

            this.service = service;
            this.client = client;
        }

        /// <summary>
        /// Gets a list of directory roles that the specified directory is associated with.
        /// </summary>
        /// <param name="objectId">Object identifier for the object to be checked.</param>
        /// <returns>A list of directory roles that the specified object identifier is associated with.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="objectId"/> is empty or null.
        /// </exception>
        public async Task<List<DirectoryRole>> GetDirectoryRolesAsync(string objectId)
        {
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            IUserMemberOfCollectionWithReferencesPage directoryGroups;
            List<DirectoryRole> roles;
            List<DirectoryRole> directoryRoles;
            bool morePages;

            objectId.AssertNotEmpty(nameof(objectId));

            try
            {
                startTime = DateTime.Now;

                directoryGroups = await this.client.Users[objectId].MemberOf.Request().GetAsync();
                roles = new List<DirectoryRole>();

                do
                {
                    directoryRoles = directoryGroups.CurrentPage.OfType<DirectoryRole>().ToList();

                    if (directoryRoles.Count > 0)
                    {
                        roles.AddRange(directoryRoles);
                    }

                    morePages = directoryGroups.NextPageRequest != null;

                    if (morePages)
                    {
                        directoryGroups = await directoryGroups.NextPageRequest.GetAsync();
                    }
                }
                while (morePages);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", this.customerId },
                    { "ObjectId", objectId }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfRoles", roles.Count }
                };

                this.service.Telemetry.TrackEvent("GetDirectoryRolesAsync", eventProperties, eventMeasurements);

                return roles;
            }
            finally
            {
                directoryGroups = null;
                directoryRoles = null;
                eventMeasurements = null;
                eventProperties = null;
            }
        }
    }
}