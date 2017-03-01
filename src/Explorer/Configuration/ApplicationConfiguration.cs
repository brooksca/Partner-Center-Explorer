// -----------------------------------------------------------------------
// <copyright file="ApplicationConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using Manager;

    /// <summary>
    /// Provides quick access to configurations stored in different places such as web.config
    /// </summary>
    public static class ApplicationConfiguration
    {
        /// <summary>
        /// A lazy reference to client configuration.
        /// </summary>
        private static readonly Lazy<IDictionary<string, dynamic>> ClientConfig = new Lazy<IDictionary<string, dynamic>>(
            () => WebPortalConfigurationManager.GenerateConfigurationDictionary().Result);

        /// <summary>
        /// Gets the Azure Active Directory endpoint.
        /// </summary>
        public static string ActiveDirectoryEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];

        /// <summary>
        /// Gets the Azure Active Directory (AAD) Graph Endpoint
        /// </summary>
        public static string ActiveDirectoryGraphEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryGraphEndpoint"];

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        public static string AppInsightsInstrumentationKey => ConfigurationManager.AppSettings["AppInsights.InstrumentationKey"];

        /// <summary>
        /// Gets the Azure Resource Manager (ARM) API endpoint.
        /// </summary>
        public static string AzureManagementEndpoint => ConfigurationManager.AppSettings["AzureManagementEndpoint"];

        /// <summary>
        /// Gets the client configuration dictionary. 
        /// </summary>
        public static IDictionary<string, dynamic> ClientConfiguration => ClientConfig.Value;

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        public static string ApplicationId => ConfigurationManager.AppSettings["Explorer.ApplicationId"];

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        public static string ApplicationSecret => ConfigurationManager.AppSettings["Explorer.ApplicationSecret"];

        /// <summary>
        /// Gets the application tenant identifier.
        /// </summary>
        public static string ApplicationTenantId => ConfigurationManager.AppSettings["Explorer.ApplicationTenantId"];

        /// <summary>
        /// Gets the Microsoft Graph endpoint.
        /// </summary>
        public static string GraphEndpoint => ConfigurationManager.AppSettings["GraphEndpoint"];

        /// <summary>
        /// Gets a value indicating whether or not the Partner Center tenant is an integration sandbox. 
        /// </summary>
        public static bool IsSandboxEnvironment
            => Convert.ToBoolean(ConfigurationManager.AppSettings["PartnerCenter.IsIntegrationSandbox"]);

        /// <summary>
        /// Gets the endpoint value for the Partner Center API.
        /// </summary>
        public static string PartnerCenterEndpoint => ConfigurationManager.AppSettings["PartnerCenterEndpoint"];

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        public static string PartnerCenterApplicationId => ConfigurationManager.AppSettings["PartnerCenter.ApplicationId"];

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        public static string PartnerCenterApplicationSecret => ConfigurationManager.AppSettings["PartnerCenter.ApplicationSecret"];

        /// <summary>
        /// Gets the tenant identifier for the Partner Center Azure AD application. 
        /// </summary>
        public static string PartnerCenterApplicationTenantId => ConfigurationManager.AppSettings["PartnerCenter.ApplicationTenantId"];

        /// <summary>
        /// Gets the Redis Cache connection string.
        /// </summary>
        public static string RedisConnection => ConfigurationManager.AppSettings["Redis.Connnection"];

        /// <summary>
        /// Gets or sets the web portal configuration manager instance.
        /// </summary>
        public static WebPortalConfigurationManager WebPortalConfigurationManager
        {
            get
            {
                return HttpContext.Current.Application["WebPortalConfigurationManager"] as WebPortalConfigurationManager;
            }

            set
            {
                HttpContext.Current.Application["WebPortalConfigurationManager"] = value;
            }
        }
    }
}