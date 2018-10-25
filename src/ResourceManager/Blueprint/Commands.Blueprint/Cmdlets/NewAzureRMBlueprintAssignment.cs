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
    public class NewAzureRMBlueprintAssignment : BlueprintCmdletBase
    {
        #region Parameters
        [Parameter(Mandatory = true, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public PSBlueprintBase Blueprint { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Subscription { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNull]
        public Hashtable Parameters { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Lock { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
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
                    var assignment = BlueprintClient.CreateOrUpdateBlueprintAssignmentAsync(Subscription, Name, localAssignment);
                    if (assignment != null)
                        WriteObject(assignment);

                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        #endregion Private Methods
    }
}
