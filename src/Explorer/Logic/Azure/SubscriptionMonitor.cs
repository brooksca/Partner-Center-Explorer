// -----------------------------------------------------------------------
// <copyright file="SubscriptionMonitor.cs" company="Microsoft">
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
    using Microsoft.Azure.Insights;
    using Microsoft.Azure.Insights.Models;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure;
    using Microsoft.Rest.Azure.OData;
    using Models;

    /// <summary>
    /// Provides the ability to monitor an Azure subscription.
    /// </summary>
    public class SubscriptionMonitor : IDisposable, ISubscriptionMonitor
    {
        private const string ResourceProviderName = "Azure.Health";
        private IInsightsClient client;
        private IExplorerService service;
        private bool disposed;
        private string subscriptionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionMonitor"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        /// <param name="tenantId">Identifier for the tenant.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="tenantId"/> is empty or null.
        /// or 
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public SubscriptionMonitor(IExplorerService service, string tenantId, string subscriptionId)
        {
            service.AssertNotNull(nameof(service));
            tenantId.AssertNotEmpty(nameof(tenantId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            ITokenManagement tokenMgmt = new TokenManagement(service);

            string token = tokenMgmt.GetAppPlusUserToken(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                ApplicationConfiguration.AzureManagementEndpoint).Token;

            this.client = new InsightsClient(new TokenCredentials(token));
            this.service = service;
            this.subscriptionId = subscriptionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionMonitor"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        /// <param name="insightsClient">An instance of <see cref="IInsightsClient"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// or 
        /// <paramref name="insightsClient"/> is null.
        /// </exception>
        public SubscriptionMonitor(IExplorerService service, IInsightsClient insightsClient)
        {
            service.AssertNotNull(nameof(service));
            insightsClient.AssertNotNull(nameof(insightsClient));

            this.client = insightsClient;
            this.service = service;
        }

        private IExplorerService Services => this.service;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a list of incidents for a subscription.
        /// </summary>
        /// <returns>A list of incidents for a subscription.</returns>
        public async Task<List<IncidentModel>> GetIncidentsAsync()
        {
            DateTime queryEndDate;
            DateTime queryStartDate;
            IPage<EventData> events;
            ODataQuery<EventData> query;

            try
            {
                this.client.SubscriptionId = this.subscriptionId;

                queryEndDate = DateTime.UtcNow;
                queryStartDate = DateTime.UtcNow.AddMonths(-1);

                query = new ODataQuery<EventData>(FilterString.Generate<IncidentEvent>(
                    eventData => (eventData.EventTimestamp >= queryStartDate)
                    && (eventData.EventTimestamp <= queryEndDate)
                    && (eventData.ResourceProvider == ResourceProviderName)));

                events = await this.client.Events.ListAsync(query);

                return events.Select(x => new IncidentModel
                {
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.EventName.LocalizedValue,
                    Status = x.Status.LocalizedValue
                }).ToList();
            }
            finally
            {
                events = null;
                query = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.client?.Dispose();
            }

            this.disposed = true;
        }
    }
}