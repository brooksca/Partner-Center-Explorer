// -----------------------------------------------------------------------
// <copyright file="CustomerSummaryViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using PartnerCenter.Models.Customers;

    /// <summary>
    /// Provides a summary of the customer.
    /// </summary>
    public class CustomerSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the customer's billing profile.
        /// </summary>
        public CustomerBillingProfile BillingProfile
        { get; set; }

        /// <summary>
        /// Gets or sets the customer's company profile.
        /// </summary>
        public CustomerCompanyProfile CompanyProfile
        { get; set; }

        /// <summary>
        /// Gets or sets the customer's commerce identifier.
        /// </summary>
        public string CommerceId
        { get; set; }

        /// <summary>
        /// Gets or sets the customer's identifier.
        /// </summary>
        public string Id
        { get; set; }
    }
}