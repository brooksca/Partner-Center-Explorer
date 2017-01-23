// -----------------------------------------------------------------------
// <copyright file="CustomerPrincipal.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Authentication
{
    using System.Security.Claims;
    using Configuration;

    /// <summary>
    /// Encapsulates relevant information about the authenticated user.
    /// </summary>
    public class CustomerPrincipal : ClaimsPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerPrincipal"/> class.
        /// </summary>
        public CustomerPrincipal()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerPrincipal"/> class.
        /// </summary>
        /// <param name="principal">A user claims principal created by Azure Active Directory.</param>
        public CustomerPrincipal(ClaimsPrincipal principal) : base(principal)
        {
            this.CustomerId = principal.FindFirst("CustomerId")?.Value;
            this.Email = principal.FindFirst(ClaimTypes.Email)?.Value;
            this.Name = principal.FindFirst(ClaimTypes.Name)?.Value;
            this.TenantId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the authenticated user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets a value indicating whether the authenticated user is an administrator or not.
        /// </summary>
        public bool IsAdmin => ApplicationConfiguration.ApplicationTenantId.Equals(this.TenantId, System.StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether the sign in user is a current Partner Center customer or not.
        /// </summary>
        public bool IsCustomer => !string.IsNullOrEmpty(this.CustomerId);

        /// <summary>
        /// Gets or sets the name of the authenticated user. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier of the authenticate user. 
        /// </summary>
        public string TenantId { get; set; }
    }
}