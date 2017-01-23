// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.IO;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Configuration;
    using Configuration.Bundling;
    using Configuration.Manager;
    using Logic;
    using Practices.Unity;
    using Practices.Unity.Mvc;

    /// <summary>
    /// Defines the methods and properties that are common to application objects.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Gets the unity container for the application.
        /// </summary>
        internal static IUnityContainer UnityContainer { get; private set; }

        /// <summary>
        /// Logic required to start the application.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey =
                ApplicationConfiguration.AppInsightsInstrumentationKey;

            UnityContainer = UnityConfig.GetConfiguredContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(UnityContainer));

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FilterConfig.RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            string path = Path.Combine(HttpRuntime.AppDomainAppPath, @"Configuration\WebPortalConfiguration.json");

            IWebPortalConfigurationFactory webPortalConfigFactory = new WebPortalConfigurationFactory();
            ApplicationConfiguration.WebPortalConfigurationManager =
                webPortalConfigFactory.Create(path);

            // Setup the application assets bundles
            ApplicationConfiguration.WebPortalConfigurationManager.UpdateBundles(Bundler.Instance).Wait();

            // Initialize the required services for Partner Center Explorer.
            UnityContainer.Resolve<IExplorerService>().InitializeAsync().Wait();
        }
    }
}