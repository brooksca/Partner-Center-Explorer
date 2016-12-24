// -----------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.Web.Http;
    using Unity.WebApi;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver(MvcApplication.UnityContainer);
            config.MapHttpAttributeRoutes();
        }
    }
}