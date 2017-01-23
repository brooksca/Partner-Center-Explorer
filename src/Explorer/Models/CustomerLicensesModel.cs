﻿// -----------------------------------------------------------------------
// <copyright file="CustomerLicensesModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    /// <summary>
    /// The customer licenses view model.
    /// </summary>
    public class CustomerLicensesModel
    {
        /// <summary>
        /// Gets or sets the license creation date. 
        /// </summary>
        public string CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the customer license identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the offer name.
        /// </summary>
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets the total number of licenses for this customer license.
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the customer license status like None, Active, Suspended or Deleted
        /// </summary>
        public string Status { get; set; }
    }
}