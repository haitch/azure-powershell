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
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.Azure.Management.Blueprint.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class NewAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        const string CreateUpdateBlueprintAssignment = "BlueprintAssignment";
        #endregion

        #region Parameters
        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition object.")]
        [ValidateNotNull]
        public PSBlueprintBase Blueprint { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Subscription ID to assign Blueprint. Can be a comma delimited list of subscription ID strings.")]
        [ValidateNotNullOrEmpty]
        public string[] SubscriptionId { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Region for managed identity to be created in. Learn more at aka.ms/blueprintmsi")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Artifact parameters.")]
        [ValidateNotNull]
        public Hashtable Parameters { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Lock resources. Learn more at aka.ms/blueprintlocks")]
        public SwitchParameter Lock { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            //TODO: Move below to another function block and call through CreateNewAssignment()
            try
            {
                AssignmentLockSettings lockSettings = new AssignmentLockSettings { Mode = PSLockMode.None.ToString() };
                if (Lock)
                    lockSettings.Mode = PSLockMode.AllResources.ToString();

                var localAssignment = new Assignment
                {
                    Identity = new ManagedServiceIdentity { Type = "SystemAssigned" },
                    Location = Location,
                    BlueprintId = Blueprint.Id,
                    Locks = lockSettings,
                    Parameters = new Dictionary<string, ParameterValueBase>(),
                    ResourceGroups = new Dictionary<string, ResourceGroupValue>()
                };

                foreach (var key in Parameters.Keys)
                {
                    var value = new ParameterValue(Parameters[key], null);
                    localAssignment.Parameters.Add(key.ToString(), value);
                }

                if (ShouldProcess(Name))
                {
                    var assignmentResult = BlueprintClient.CreateOrUpdateBlueprintAssignmentAsync(SubscriptionId[0],Name, localAssignment).Result;
                    if (assignmentResult != null) {
                        WriteObject(assignmentResult);
                    }
                    else
                    {
                       //TODO: Need a way to let user know about why assingment failed.
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        private void CreateNewAssignment()
        {
            /* TODO:
               1.  Register Blueprint RP in the subscription.
                       Register-AzureRmResourceProvider
               2.  Grant Owner Permission
                   a. doing AAD query to resolve Azure Blueprint SPN,
                       AppId: f71766dc-90d9-4b7d-bd9d-4499c4331c3f
                       cmdlet: Get-AzureRmADServicePrincipal
                   b. grant owner permission to this SPN
                       New-AzureRmRoleAssignment -ObjectId "from previous step" -Scope "/subscriptions/{}" -RoleDefinitionName "Owner"

               3.  Call BlueprintAssignment.CreateOrUpdate()
           */

        }
        #endregion Private Methods
    }
}
