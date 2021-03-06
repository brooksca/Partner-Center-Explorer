﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Models
{
    /// <summary>
    /// Model that represents audit record log search.
    /// </summary>
    public class AuditSearchModel
    {
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date for the audit records search.
        /// </value>
        [DataType(DataType.DateTime)]
        public DateTime EndDate
        { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date for the audit records search.
        /// </value>
        [DataType(DataType.DateTime)]
        public DateTime StartDate
        { get; set; }
    }
}