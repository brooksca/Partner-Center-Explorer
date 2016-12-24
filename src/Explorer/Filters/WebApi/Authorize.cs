// -----------------------------------------------------------------------
// <copyright file="Authorize.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Filters.WebApi
{
    using Logic.Authentication;
    using System.Web.Http.Controllers;

    /// <summary>
    /// Implements portal authorization for Web API controllers.
    /// </summary>
    public class Authorize : System.Web.Http.AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Authorize"/> class.
        /// </summary>
        /// <param name="userRole">The user role to give access to.</param>
        public Authorize(UserRole userRole = UserRole.Any)
        {
            UserRole = userRole;
        }

        /// <summary>
        /// Gets or sets the user role which is allowed access.
        /// </summary>
        public UserRole UserRole
        { get; set; }

        /// <summary>
        /// Authorizes an incoming request based on the user role.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>True if authorized, false otherwise.</returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            CustomerPrincipal principal = actionContext.RequestContext.Principal as CustomerPrincipal;
            return new AuthorizationPolicy().IsAuthorized(principal, UserRole);
        }
    }
}