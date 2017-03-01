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
    using Configuration;
    using Microsoft.Azure.Insights;
    using Microsoft.Azure.Insights.Models;
    using Models;
    using Rest;
    using Rest.Azure;
    using Rest.Azure.OData;
    using Security;

    /// <summary>
    /// Provides the ability to monitor an Azure subscription.
    /// </summary>
    public class SubscriptionMonitor : IDisposable, ISubscriptionMonitor
    {
        /// <summary>
        /// Name of the resource provider to be queried. 
        /// </summary>
        private const string ResourceProviderName = "Azure.Health";

        /// <summary>
        /// Provides the ability to interact with the Azure Monitor API. 
        /// </summary>
        private readonly IInsightsClient client;

        /// <summary>
        /// Provides the ability to access core services.
        /// </summary>
        private readonly IExplorerService services;
        
        /// <summary>
        /// The subscription identifier that should be utilized in querying the health.
        /// </summary>
        private readonly string subscriptionId;

        /// <summary>
        /// The tenant identifier that owns the subscription.
        /// </summary>
        private readonly string tenantId;

        /// <summary>
        /// A flag indicating whether or not this object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionMonitor"/> class.
        /// </summary>
        /// <param name="services">Provides access to the core application services.</param>
        /// <param name="tenantId">Identifier for the tenant.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="tenantId"/> is empty or null.
        /// or 
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is null.
        /// </exception>
        public SubscriptionMonitor(IExplorerService services, string tenantId, string subscriptionId)
        {
            services.AssertNotNull(nameof(services));
            tenantId.AssertNotEmpty(nameof(tenantId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            ITokenManagement tokenMgmt = new TokenManagement(services);

            string token = tokenMgmt.GetAppPlusUserToken(
                $"{ApplicationConfiguration.ActiveDirectoryEndpoint}/{tenantId}",
                ApplicationConfiguration.AzureManagementEndpoint).Token;

            this.client = new InsightsClient(new TokenCredentials(token));
            this.services = services;
            this.subscriptionId = subscriptionId;
            this.tenantId = tenantId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionMonitor"/> class.
        /// </summary>
        /// <param name="services">Provides access to core application services.</param>
        /// <param name="insightsClient">An instance of <see cref="IInsightsClient"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is null.
        /// or 
        /// <paramref name="insightsClient"/> is null.
        /// </exception>
        public SubscriptionMonitor(IExplorerService services, IInsightsClient insightsClient)
        {
            services.AssertNotNull(nameof(services));
            insightsClient.AssertNotNull(nameof(insightsClient));

            this.client = insightsClient;
            this.services = services;
        }

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
            DateTime startTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            IPage<EventData> events;
            ODataQuery<EventData> query;

            try
            {
                startTime = DateTime.Now;
                this.client.SubscriptionId = this.subscriptionId;

                queryEndDate = DateTime.UtcNow;
                queryStartDate = DateTime.UtcNow.AddMonths(-1);

                query = new ODataQuery<EventData>(FilterString.Generate<IncidentEvent>(
                    eventData => (eventData.EventTimestamp >= queryStartDate)
                    && (eventData.EventTimestamp <= queryEndDate)
                    && (eventData.ResourceProvider == ResourceProviderName)));

                events = await this.client.Events.ListAsync(query);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "SubscriptionId", this.subscriptionId },
                    { "TenantId", this.tenantId }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfEvents", events.Count() }
                };

                this.services.Telemetry.TrackEvent("GetIncidentsAsync", eventProperties, eventMeasurements);

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

            this.client.Dispose();

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
}