// -----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using Filters.Mvc;
    using System.Web.Http.Filters;
    using System.Web.Mvc;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthenticationFilter());
            filters.Add(new AiHandleErrorAttribute());
        }

        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new Filters.WebApi.AuthenticationFilter());
            filters.Add(new Filters.WebApi.ErrorHandler());
        }
    }
}