// -----------------------------------------------------------------------
// <copyright file="SubscriptionViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using PartnerCenter.Models.Subscriptions;

    /// <summary>
    /// Represents a view model for a specific subscription.
    /// </summary>
    public class SubscriptionViewModel
    {
        /// <summary>
        /// Gets or sets the name of the company that owns the subscription.
        /// </summary>
        public string CompanyName
        { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier that owns the sbuscription.
        /// </summary>
        public string CustomerId
        { get; set; }

        /// <summary>
        /// Gets or sets the subscription's friendly name.
        /// </summary>
        public string FriendlyName
        { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string SubscriptionId
        { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity
        { get; set; }

        /// <summary>
        /// Gets or sets the subscription status.
        /// </summary>
        public SubscriptionStatus Status
        { get; set; }

        /// <summary>
        /// Gets or sets the units defining Quantity for the subscription.
        /// </summary>
        public string UnitType
        { get; set; }
    }
}