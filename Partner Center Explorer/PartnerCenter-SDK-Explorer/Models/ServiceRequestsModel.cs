﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.ServiceRequests;

namespace Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Models
{
    public class ServiceRequestsModel
    {
        public ResourceCollection<ServiceRequest> ServiceRequests
        { get; set; }
    }
}