// -----------------------------------------------------------------------
// <copyright file="SubscriptionMonitor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Office
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// Provides the ability to monitor an Office subscription.
    /// </summary>
    public class SubscriptionMonitor : ISubscriptionMonitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionMonitor"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        /// <param name="tenantId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="tenantId"/> is empty or null.
        /// or 
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public SubscriptionMonitor(IExplorerService service, string tenantId, string subscriptionId)
        {
            service.AssertNotNull(nameof(service));
            tenantId.AssertNotEmpty(nameof(tenantId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));
        }

        /// <summary>
        /// Gets a list of active incidents for a subscription.
        /// </summary>
        /// <returns>A list of active incidents for a subscription.</returns>
        public async Task<List<IncidentModel>> GetIncidentsAsync()
        {
            await Task.FromResult(0);
            return new List<IncidentModel>();
        }
    }
}