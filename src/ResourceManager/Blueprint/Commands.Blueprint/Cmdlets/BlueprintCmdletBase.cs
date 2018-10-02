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

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.ResourceManager.Common;
using Microsoft.Azure.Management.Blueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    public class BlueprintCmdletBase : AzureRMCmdlet
    {
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

        /// <summary>
        /// Gets the current Subscription ID.
        /// </summary>
        protected string SubscriptionId
        {
            get { return DefaultContext.Subscription.Id; }
        }
    }
}
