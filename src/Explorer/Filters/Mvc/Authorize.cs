// -----------------------------------------------------------------------
// <copyright file="Authorize.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Filters.Mvc
{
    using Logic.Authentication;
    using System.Web;
    using System.Web.Mvc;

    public class Authorize : AuthorizeAttribute
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
        /// Gets or set the user role which is allowed access.
        /// </summary>
        public UserRole UserRole
        { get; set; }

        /// <summary>
        /// Authorizes an incoming request based on the user role.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>True if authorized, false otherwise.</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var principal = httpContext.User as CustomerPrincipal;
            return new AuthorizationPolicy().IsAuthorized(principal, UserRole);
        }

        /// <summary>
        /// Handles any unauthorized requests.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // send back a 403
                filterContext.HttpContext.Response.StatusCode = 403;
            }
            else
            {
                // use the default handling for web pages
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}