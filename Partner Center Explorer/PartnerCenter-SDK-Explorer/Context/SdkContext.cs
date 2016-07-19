﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Store.PartnerCenter.Extensions;
using Microsoft.Store.PartnerCenter.Samples.Common;
using Microsoft.Store.PartnerCenter.Samples.Common.Context;
using System.Threading.Tasks;

namespace Microsoft.Store.PartnerCenter.Samples.SDK.Explorer.Context
{
    /// <summary>
    /// Context class used to obtain base object to interact with the Partner Center SDK.
    /// </summary>
    public class SdkContext
    {
        private IAggregatePartner _partnerOperations;

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkContext"/> class.
        /// </summary>
        public SdkContext()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkContext"/> class.
        /// </summary>
        /// <param name="partnerOperations">The partner operations.</param>
        public SdkContext(IAggregatePartner partnerOperations)
        {
            _partnerOperations = partnerOperations;
        }

        /// <summary>
        /// Gets the an instance of <see cref="IAggregatePartner"/>.
        /// </summary>
        /// <value>
        /// An instnace of <see cref="IAggregatePartner"/> used by the SDK to perform operations.
        /// </value>
        /// <remarks>
        /// This property utilizes App + User authentication. Various operations with the Partner Center
        /// SDK require App + User authorization. More details regarding Partner Center authentication can be
        /// found at https://msdn.microsoft.com/en-us/library/partnercenter/mt634709.aspx
        /// </remarks>
        public IAggregatePartner PartnerOperations
        {
            get
            {
                if (_partnerOperations != null)
                {
                    return _partnerOperations;
                }

                AuthenticationResult authResult = TokenContext.GetAADToken(
                    $"{AppConfig.Authority}/{AppConfig.AccountId}/oauth2",
                    AppConfig.PartnerCenterApiUri
                    );

                // Authenticate by user context with the partner service
                IPartnerCredentials userCredentials = PartnerCredentials.Instance.GenerateByUserCredentials(
                    AppConfig.ApplicationId,
                    new AuthenticationToken(
                        authResult.AccessToken,
                        authResult.ExpiresOn),
                    delegate
                    {
                        // Token has expired re-authentication to Azure Active Directory is required.
                        AuthenticationResult aadToken = TokenContext.GetAADToken(
                            $"{AppConfig.Authority}/{AppConfig.AccountId}/oauth2",
                            AppConfig.PartnerCenterApiUri
                            );

                        return Task.FromResult(new AuthenticationToken(aadToken.AccessToken, aadToken.ExpiresOn));
                    });

                _partnerOperations = PartnerService.Instance.CreatePartnerOperations(userCredentials);

                return _partnerOperations;
            }
        }
    }
}