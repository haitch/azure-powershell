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

using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Management.Blueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment")]
    public class GetAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        const string SubscriptionScope = "SubscriptionScope";
        const string BlueprintAssignmentByName = "BlueprintAssignmentByName";
        #endregion Sets

        [Parameter(ParameterSetName = BlueprintAssignmentByName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [Parameter(ParameterSetName = SubscriptionScope, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Subscriptin Id.")]
        [ValidateNotNullOrEmpty]
        public string SubscriptionId { get; set; }

        #region Parameters
        [Parameter(ParameterSetName = BlueprintAssignmentByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName) {
                    case SubscriptionScope:
                        WriteObject(BlueprintClient.ListBlueprintAssignmentsAsync(SubscriptionId ?? DefaultContext.Subscription.Id).Result);
                        break;
                    case BlueprintAssignmentByName:
                        WriteObject(BlueprintClient.GetBlueprintAssignmentAsync(SubscriptionId ?? DefaultContext.Subscription.Id, Name).Result);
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        #endregion Cmdlet Overrides
    }
}
