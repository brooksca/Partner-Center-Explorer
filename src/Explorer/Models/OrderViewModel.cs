// -----------------------------------------------------------------------
// <copyright file="OrderViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Logic;

    /// <summary>
    /// The order view model (orders contain subscriptions and optionally the customer identifier).
    /// </summary>
    public class OrderViewModel
    {
        /// <summary>
        /// Gets or sets the customer identifier. 
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the operation type for the order. 
        /// </summary>
        [Required]
        public CommerceOperationType OperationType { get; set; }

        /// <summary>
        /// Gets or sets the subscriptions the customer ordered.
        /// </summary>
        [Required]
        public IEnumerable<OrderSubscriptionItemViewModel> Subscriptions { get; set; }
    }
}