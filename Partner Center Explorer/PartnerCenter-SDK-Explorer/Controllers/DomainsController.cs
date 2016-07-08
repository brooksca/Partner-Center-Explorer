﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Samples.AzureAD.Graph.API;
using Microsoft.Store.PartnerCenter.Samples.Common;
using Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Context;
using Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Controllers
{
    [AuthorizationFilter(ClaimType = ClaimTypes.Role, ClaimValue = "PartnerAdmin")]
    public class DomainsController : Controller
    {
        private SdkContext _context;

        [HttpGet]
        public async Task<PartialViewResult> ConfigurationRecords(string customerId, string domain)
        {
            AuthenticationResult token;
            ConfigurationRecordsModel domainDetailsModel;
            GraphClient client;

            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException("customerId");
            }
            else if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domain");
            }

            try
            {
                token = TokenContext.GetAADToken(
                    string.Format(
                        "{0}/{1}",
                        AppConfig.Authority,
                        customerId
                    ),
                    AppConfig.GraphUri
                );

                client = new GraphClient(token.AccessToken);

                domainDetailsModel = new ConfigurationRecordsModel()
                {
                    ServiceConfigurationRecords = await client.GetServiceConfigurationRecordsAsync(customerId, domain)
                };

                return PartialView(domainDetailsModel);
            }
            finally
            {
                client = null;
                token = null;
            }
        }

        public async Task<JsonResult> IsDomainAvailable(string primaryDomain)
        {
            if (string.IsNullOrEmpty(primaryDomain))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            bool exists = await Context.PartnerOperations.Domains.ByDomain(primaryDomain + ".onmicrosoft.com").ExistsAsync();

            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        private SdkContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new SdkContext();
                }

                return _context;
            }
        }
    }
}