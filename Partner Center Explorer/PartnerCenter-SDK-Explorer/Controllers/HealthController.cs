﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Samples.Azure.Management;
using Microsoft.Samples.Office365.Management.API;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Microsoft.Store.PartnerCenter.Models.Invoices;
using Microsoft.Store.PartnerCenter.Models.Subscriptions;
using Microsoft.Store.PartnerCenter.Samples.Common;
using Microsoft.Store.PartnerCenter.Samples.Common.Models;
using Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Context;
using Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Controllers
{
    /// <summary>
    /// Handles request for the Health views.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    [AuthorizationFilter(ClaimType = ClaimTypes.Role, ClaimValue = "PartnerAdmin")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Handles the index view request.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// customerId
        /// or
        /// subscriptionId
        /// </exception>
        public async Task<ActionResult> Index(string customerId, string subscriptionId)
        {
            Customer customer;
            IAggregatePartner operations;
            SubscriptionHealthModel healthModel;
            Subscription subscription;

            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            try
            {
                operations = await new SdkContext().GetPartnerOperationsAysnc();
                customer = await operations.Customers.ById(customerId).GetAsync();
                subscription = await operations.Customers.ById(customerId).Subscriptions.ById(subscriptionId).GetAsync();

                healthModel = new SubscriptionHealthModel
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customerId,
                    FriendlyName = subscription.FriendlyName,
                    SubscriptionId = subscriptionId,
                    ViewModel = (subscription.BillingType == BillingType.License) ? "Office" : "Azure"
                };

                if (subscription.BillingType == BillingType.Usage)
                {
                    healthModel.HealthEvents = await GetAzureSubscriptionHealthAsync(customerId, subscriptionId);
                }
                else
                {
                    healthModel.HealthEvents = await GetOfficeSubscriptionHealthAsync(customerId);
                }

                return View(healthModel.ViewModel, healthModel);
            }
            finally
            {
                customer = null;
                subscription = null;
            }
        }

        private async Task<List<IHealthEvent>> GetAzureSubscriptionHealthAsync(string customerId, string subscriptionId)
        {
            AuthenticationResult token;

            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            try
            {
                token = await TokenContext.GetAADTokenAsync(
                    $"{AppConfig.Authority}/{customerId}",
                    AppConfig.ManagementUri
                );

                using (Insights insights = new Insights(
                    subscriptionId,
                    token.AccessToken
                ))
                {
                    return await insights.GetHealthEventsAsync();
                }
            }
            finally
            {
                token = null;
            }
        }

        private async Task<List<IHealthEvent>> GetOfficeSubscriptionHealthAsync(string customerId)
        {
            ServiceCommunications comm;

            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            try
            {
                comm = new ServiceCommunications(TokenContext.UserAssertionToken);
                return await comm.GetCurrentStatusAsync(customerId);
            }
            finally
            {
                comm = null;
            }
        }
    }
}