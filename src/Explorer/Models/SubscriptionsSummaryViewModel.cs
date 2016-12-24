// -----------------------------------------------------------------------
// <copyright file="SubscriptionsSummaryViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a view model for list subscriptions that belong to a specific customer.
    /// </summary>
    public class SubscriptionsSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the customer identifier that owns the subscriptions.
        /// </summary>
        public string CustomerId
        { get; set; }

        /// <summary>
        /// Gets or sets a list of subscriptions.
        /// </summary>
        public IEnumerable<SubscriptionViewModel> Subscriptions
        { get; set; }
    }
}