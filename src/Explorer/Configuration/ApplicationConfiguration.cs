// -----------------------------------------------------------------------
// <copyright file="ApplicationConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Configuration
{
    using Manager;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using Telemetry;

    /// <summary>
    /// Provides quick access to configurations stored in different places such as web.config
    /// </summary>
    public static class ApplicationConfiguration
    {
        /// <summary>
        /// Gets the account identitifer value.
        /// </summary>
        /// <remarks>
        /// This value can be found in Partner Center on the App Management page. 
        /// </remarks>
        public static string AccountId => ConfigurationManager.AppSettings["AccountId"];

        /// <summary>
        /// Gets the Azure Active Directory endpoint.
        /// </summary>
        /// <remarks>
        /// This value should either be https://login.microsoftonline.com or https://login.windows.net
        /// </remarks>
        public static string ActiveDirectoryEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];

        /// <summary>
        /// Gets the Azure Active Directory (AAD) Graph Endpoint
        /// </summary>
        /// <remarks>
        /// This value is typically configured to https://graph.windows.net
        /// </remarks>
        public static string ActiveDirectoryGraphEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryGraphEndpoint"];

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        public static string AppInsightsInstrumentationKey => ConfigurationManager.AppSettings["AppInsights.InstrumentationKey"];

        /// <summary>
        /// Gets the client configuration dictionary. 
        /// </summary>
        public static IDictionary<string, dynamic> ClientConfiguration => clientConfiguration.Value;

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        /// <remarks>
        /// This is the ClientId of the application being used for authentication purposes.
        /// </remarks>
        public static string ApplicationId => ConfigurationManager.AppSettings["Explorer.ApplicationId"];

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        /// <remarks>
        /// This is the secret key for the application being used for authentication purposes.
        /// </remarks>
        public static string ApplicationSecret => ConfigurationManager.AppSettings["Explorer.ApplicationSecret"];

        /// <summary>
        /// Gets the endpoint value for the Partner Center API.
        /// </summary>
        /// <remarks>
        /// This value is typically set to https://api.partnercenter.microsoft.com
        /// </remarks>
        public static string PartnerCenterEndpoint => ConfigurationManager.AppSettings["PartnerCenterEndpoint"];

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        /// <remarks>
        /// This is the ClientId of the application being used for authentication purposes.
        /// </remarks>
        public static string PartnerCenterApplicationId => ConfigurationManager.AppSettings["PartnerCenter.ApplicationId"];

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        /// <remarks>
        /// This is the secret key for the application being used for authentication purposes.
        /// </remarks>
        public static string PartnerCenterApplicationSecret => ConfigurationManager.AppSettings["PartnerCenter.ApplicationSecret"];

        /// <summary>
        /// Gets the Redis Cache connection string.
        /// </summary>
        /// <remarks>
        /// This is an optional value. It is used by <see cref="DistributedTokenCache"/> in order to cache tokens.
        /// Obtaining tokens is an expensive operations. Using a presistent caching mechanism allows us to cache tokens
        /// and not worry about a system reboot impacting the performance of the application. Additional information
        /// can be found at https://azure.microsoft.com/en-us/documentation/articles/guidance-multitenant-identity-token-cache/.
        /// </remarks>
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

        private static readonly Lazy<IDictionary<string, dynamic>> clientConfiguration = new Lazy<IDictionary<string, dynamic>>(
            () => WebPortalConfigurationManager.GenerateConfigurationDictionary().Result);
    }
}