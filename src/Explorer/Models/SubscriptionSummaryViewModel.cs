// -----------------------------------------------------------------------
// <copyright file="SubscriptionSummaryViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// View model used to display summary information for a subscription.
    /// </summary>
    public class SubscriptionSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the company name that owns the subscription.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier that owns the subscription.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the friendly for the subscription.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets a list of active incidents for the subscription.
        /// </summary>
        public List<IncidentModel> Incidents { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string SubscriptionId { get; set; }
    }
}