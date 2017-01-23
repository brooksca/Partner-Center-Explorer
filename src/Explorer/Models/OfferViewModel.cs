// -----------------------------------------------------------------------
// <copyright file="OfferViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    /// <summary>
    /// Represents an offer from Partner Center.
    /// </summary>
    public class OfferViewModel
    {
        /// <summary>
        /// Gets or sets the offer category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the offer description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the offer identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the offer.
        /// </summary>
        public string Name { get; set; }
    }
}