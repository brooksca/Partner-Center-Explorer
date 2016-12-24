// -----------------------------------------------------------------------
// <copyright file="CustomerPrincipal.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    using Microsoft.Store.PartnerCenter.Explorer.Configuration;
    using System.Security.Claims;

    /// <summary>
    /// Encapsulates relevant information about the authenticated user.
    /// </summary>
    public class CustomerPrincipal : ClaimsPrincipal
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="CustomerPrincipal"/> class.
        /// </summary>
        public CustomerPrincipal()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerPrincipal"/> class.
        /// </summary>
        /// <param name="principal">A user claims principal created by Azure Active Directory.</param>
        public CustomerPrincipal(ClaimsPrincipal principal) : base(principal)
        {
            CustomerId = principal.FindFirst(Resources.CustomerId)?.Value;
            Email = principal.FindFirst(ClaimTypes.Email)?.Value;
            Name = principal.FindFirst(ClaimTypes.Name)?.Value;
            TenantId = principal.FindFirst(Resources.TenatIdClaimUri)?.Value;
        }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId
        { get; set; }

        /// <summary>
        /// Gets or sets the email address of the authenticated user.
        /// </summary>
        public string Email
        { get; set; }

        /// <summary>
        /// Gets a value indicating whether the authenticated user is an administrator or not.
        /// </summary>
        public bool IsAdmin
        {
            get { return ApplicationConfiguration.AccountId.Equals(TenantId, System.StringComparison.CurrentCultureIgnoreCase); }
        }

        /// <summary>
        /// Gets a value indicating whether the sign in user is a current Partner Center customer or not.
        /// </summary>
        public bool IsCustomer
        {
            get { return !string.IsNullOrEmpty(CustomerId); }
        }

        /// <summary>
        /// Gets or sets the name of the authenticated user. 
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier of the authenticate user. 
        /// </summary>
        public string TenantId
        { get; set; }
    }
}