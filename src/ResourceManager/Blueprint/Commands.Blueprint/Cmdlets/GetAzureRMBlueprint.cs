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
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.Azure.Management.ManagementGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Blueprint")]
    public class GetAzureRmBlueprint : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        const string ManagementGroupScope = "ManagementGroupScope";
        const string BlueprintDefinitionByName = "BlueprintDefinitionByName";
        const string BlueprintDefinitionByVersion = "BlueprintDefinitionScopeByVersion";
        //TODO: Add subscription scope for subscription scope up top.
        #endregion Class Constants

        #region Parameters
        //[Parameter(ParameterSetName = BlueprintDefinitionLatestPublished, Mandatory = false, HelpMessage = "Blueprint definition's latest published version.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByVersion, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = ManagementGroupScope, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Management group name.")]
        [ValidateNotNullOrEmpty]
        public string[] ManagementGroupName { get; set; }

        [Parameter(ParameterSetName = BlueprintDefinitionByVersion, Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = BlueprintDefinitionByVersion, Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [ValidateNotNullOrEmpty]
        public string Version { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Blueprint definition's latest published version.")]
        public SwitchParameter LatestPublished { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case ManagementGroupScope:

                        string nextLink = string.Empty;

                        do
                        {
                            //ret = this.AutomationClient.ListAutomationAccounts(this.ManagementGroupName[0], ref nextLink);
                            //this.WriteObject(ret, true);

                        } while (!string.IsNullOrEmpty(nextLink));
                        //WriteObject(BlueprintClient.ListBlueprintsAsync(ManagementGroupName[0]).Result);
                        break;
                    case BlueprintDefinitionByName:
                        if (LatestPublished)
                        {
                            //WriteObject(BlueprintClient.GetLatestPublishedBlueprintAsync(ManagementGroupName, Name).Result);
                        }
                        else
                        {
                            //WriteObject(BlueprintClient.GetBlueprintAsync(ManagementGroupName, Name).Result);
                        }
                        break;
                    case BlueprintDefinitionByVersion:
                        //WriteObject(BlueprintClient.GetPublishedBlueprintAsync(ManagementGroupName, Name, Version).Result);
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
