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
        private const string ManagementGroupScope = "ManagementGroupScope";
        private const string BlueprintDefinitionByName = "BlueprintDefinitionByName";
        private const string BlueprintDefinitionByLatestPublished = "BlueprintDefinitionByLatestPublished";
        private const string BlueprintDefinitionByVersion = "BlueprintDefinitionByVersion";

        #endregion Class Constants

        #region Parameters

        [Parameter(ParameterSetName = BlueprintDefinitionByVersion, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = ManagementGroupScope, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Management group name.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByLatestPublished, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupName { get; set; }

        [Parameter(ParameterSetName = BlueprintDefinitionByVersion, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = BlueprintDefinitionByLatestPublished, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Get latest published version for a Blueprint.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = BlueprintDefinitionByVersion, Position = 2, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [ValidateNotNullOrEmpty]
        public string Version { get; set; }

        [Parameter(ParameterSetName = BlueprintDefinitionByLatestPublished, Position = 2, Mandatory = false, HelpMessage = "Blueprint definition name.")]
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
                        IEnumerable<string> mgList = string.IsNullOrEmpty(ManagementGroupName)
                            ? GetManagementGroupsForCurrentUser()
                            : new string[] { ManagementGroupName };

                        foreach (var bp in BlueprintClient.ListBlueprints(mgList))
                            WriteObject(bp);
                        break;
                    case BlueprintDefinitionByName:
                        WriteObject(BlueprintClient.GetBlueprint(ManagementGroupName, Name));
                        break;
                    case BlueprintDefinitionByVersion:
                        WriteObject(BlueprintClient.GetPublishedBlueprint(ManagementGroupName, Name, Version));
                        break;
                    case BlueprintDefinitionByLatestPublished:
                        WriteObject(BlueprintClient.GetLatestPublishedBlueprint(ManagementGroupName, Name));
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }

        #endregion Cmdlet Overrides
        private IEnumerable<string> GetManagementGroupsForCurrentUser()
        {
            var responseList = ManagementGroupsClient.ManagementGroups.List();
            return responseList.Select(managementGroup => managementGroup.Name).ToList();
        }

    }
}
