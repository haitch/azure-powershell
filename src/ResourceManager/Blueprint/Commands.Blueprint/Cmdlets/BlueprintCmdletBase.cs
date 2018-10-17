// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.ResourceManager.Common;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.Azure.Management.ManagementGroups;
using System;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    public class BlueprintCmdletBase : AzureRMCmdlet
    {
        #region Obsolete
        private const string ARMFrontDoor = "https://management.azure.com";

        private IBlueprintManagementClient client;

        /// <summary>
        /// Gets the Blueprint Client object.
        /// </summary>
        protected IBlueprintManagementClient Client
        {
            get
            {
                if (client == null)
                    client = new BlueprintManagementClient(new Uri(ARMFrontDoor),
                                                           AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(DefaultProfile.DefaultContext));

                return client;
            }
        }
        #endregion Obsolete

        #region Properties
        /// <summary>
        /// The blueprint client.
        /// </summary>
        private IBlueprintClient blueprintClient;

        public IBlueprintClient BlueprintClient
        {
            get
            {
                return blueprintClient = blueprintClient ?? new BlueprintClient(DefaultProfile.DefaultContext);
            }
            set
            {
                blueprintClient = value;
            }
        }

        private IManagementGroupsAPIClient managementGroupsApiClient;
        public IManagementGroupsAPIClient ManagementGroupsClient
        {
            get
            {
                return managementGroupsApiClient ??
                      (managementGroupsApiClient =
                        AzureSession.Instance.ClientFactory.CreateArmClient<ManagementGroupsAPIClient>(DefaultProfile.DefaultContext,
                                                                                                       AzureEnvironment.Endpoint.ResourceManager));
            }
            set
            {
                managementGroupsApiClient = value;
            }
        }
        #endregion Properties

        #region Cmdlet Overrides
        protected override void WriteExceptionError(Exception ex)
        {
            var aggEx = ex as AggregateException;

            if (aggEx != null && aggEx.InnerExceptions != null)
            {
                foreach (var e in aggEx.Flatten().InnerExceptions)
                    WriteExceptionError(e);

                return;
            }

            base.WriteExceptionError(ex);
        }
        #endregion Cmdlet Overrides

    }
}
