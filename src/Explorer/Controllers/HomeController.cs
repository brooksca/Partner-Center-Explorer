// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using Configuration;
    using Configuration.WebPortal;
    using Filters.Mvc;
    using Logic;
    using Logic.Authentication;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    /// <summary>
    /// Manages the application home page requests.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="service">Provides access to all of the core services.</param>
        /// <exception cref="System.ArgumentNullException">
        /// service   
        /// </exception>
        public HomeController(IExplorerService service) : base(service)
        { }

        /// <summary>
        /// Serves the error page to the browser.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <returns>A view that details the error.</returns>
        public ActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
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

            return View();
        }
    }
}