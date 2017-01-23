// -----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.Web.Http.Filters;
    using System.Web.Mvc;
    using Filters.Mvc;

    /// <summary>
    /// Provides the ability to configure filtering.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the specified global filters.
        /// </summary>
        /// <param name="filters">A collection of global filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthenticationFilter());
            filters.Add(new AiHandleErrorAttribute());
        }

        /// <summary>
        /// Registers the specified filters.
        /// </summary>
        /// <param name="filters">A collection of HTTP filters.</param>
        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new Filters.WebApi.AuthenticationFilter());
            filters.Add(new Filters.WebApi.ErrorHandler());
        }
    }
}