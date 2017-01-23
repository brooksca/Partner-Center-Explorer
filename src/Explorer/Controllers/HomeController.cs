// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Configuration;
    using Configuration.WebPortal;
    using Filters.Mvc;
    using Logic;
    using Logic.Authentication;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides the ability to manage requests for the home page.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="service">Provides access to all of the core services.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is null.  
        /// </exception>
        public HomeController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Serves the error page to the browser.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <returns>A view that details the error.</returns>
        public ActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return this.View();
        }

        /// <summary>
        /// Serves the single page application to the browser.
        /// </summary>
        /// <returns>The single page application markup.</returns>
        [@Authorize(UserRole = UserRole.Any)]
        public async Task<ActionResult> Index()
        {
            PluginsSegment clientVisiblePlugins = await ApplicationConfiguration.WebPortalConfigurationManager.GeneratePlugins();
            IDictionary<string, dynamic> clientConfiguration = new Dictionary<string, dynamic>(ApplicationConfiguration.ClientConfiguration);

            ViewBag.IsAuthenticated = Request.IsAuthenticated ? "true" : "false";
            ViewBag.Templates = (await ApplicationConfiguration.WebPortalConfigurationManager.AggregateStartupAssets()).Templates;

            clientConfiguration["DefaultTile"] = "Home";
            clientConfiguration["Tiles"] = clientVisiblePlugins.Plugins;

            if (Request.IsAuthenticated)
            {
                ViewBag.UserName = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst("name").Value ?? "Unknown";
                ViewBag.Email = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(ClaimTypes.Name)?.Value ??
                    ((ClaimsIdentity)HttpContext.User.Identity).FindFirst(ClaimTypes.Email)?.Value;
            }

            ViewBag.Configuratrion = JsonConvert.SerializeObject(
                clientConfiguration,
                new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.Default });

            return this.View();
        }
    }
}