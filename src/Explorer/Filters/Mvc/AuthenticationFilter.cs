// -----------------------------------------------------------------------
// <copyright file="AuthenticationFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Filters.Mvc
{
    using Logic.Authentication;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Filters;

    public class AuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// Called when authorization occurs.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            filterContext.Principal = new CustomerPrincipal(HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal);
        }

        /// <summary>
        /// Called when authorization challenge occurs.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        { }
    }
}