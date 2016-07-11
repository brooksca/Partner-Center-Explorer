﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.PowerBI.Api.V1;
using Microsoft.PowerBI.Api.V1.Models;
using Microsoft.PowerBI.Security;
using Microsoft.Rest;
using Microsoft.Store.PartnerCenter.Samples.Common;
using Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Controllers
{
    [AuthorizationFilter(ClaimType = ClaimTypes.Role, ClaimValue = "PartnerAdmin")]
    public class ReportsController : Controller
    {
        private IPowerBIClient _client;

        public ReportsController()
        { }

        public ReportsController(IPowerBIClient powerbiClient)
        {
            _client = powerbiClient;
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Reports()
        {
            ODataResponseListReport response;
            ReportsModel reportsModel;

            try
            {
                using (IPowerBIClient client = GetPowerBIClient())
                {
                    response = client.Reports.GetReports(
                        PowerBIConfig.WorkspaceCollectionName,
                        PowerBIConfig.WorkspaceId
                    );

                    reportsModel = new ReportsModel()
                    {
                        Reports = response.Value.ToList()
                    };

                    return PartialView(reportsModel);
                }
            }
            finally
            {
                response = null;
            }
        }

        public async Task<ActionResult> Report(string reportId)
        {
            ODataResponseListReport response;
            PowerBIToken embedToken;
            Report report;
            ReportModel reportModel;

            if (string.IsNullOrEmpty(reportId))
            {
                throw new ArgumentNullException("reportId");
            }

            try
            {
                using (IPowerBIClient client = GetPowerBIClient())
                {
                    response = await client.Reports.GetReportsAsync(
                        PowerBIConfig.WorkspaceCollectionName,
                        PowerBIConfig.WorkspaceId
                    );

                    report = response.Value.FirstOrDefault(r => r.Id == reportId);

                    embedToken = PowerBIToken.CreateReportEmbedToken(
                        PowerBIConfig.WorkspaceCollectionName,
                        PowerBIConfig.WorkspaceId,
                        report.Id
                    );

                    reportModel = new ReportModel()
                    {
                        AccessToken = embedToken.Generate(PowerBIConfig.AccessKey),
                        Report = report
                    };

                    return View(reportModel);
                }
            }
            finally
            {
                embedToken = null;
                report = null;
                response = null;
            }
        }

        private IPowerBIClient GetPowerBIClient()
        {
            TokenCredentials credentials;

            if (_client == null)
            {
                credentials = new TokenCredentials(
                    PowerBIConfig.AccessKey,
                    "Appkey"
                );

                _client = new PowerBIClient(credentials)
                {
                    BaseUri = PowerBIConfig.BaseUri
                };
            }

            return _client;
        }
    }
}