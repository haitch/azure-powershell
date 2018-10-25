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

using Microsoft.Azure.Management.Blueprint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSBlueprintAssignment : PSSubscriptionResourceBase
    {
        public string Location { get; set; }

        public PSManagedServiceIdentity Identity { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string BlueprintId { get; set; }
        public IDictionary<string, PSParameterValueBase> Parameters { get; set; }
        public IDictionary<string, PSResourceGroupValue> ResourceGroups { get; set; }
        public PSAssignmentStatus Status { get; set; }
        public PSAssignmentLockSettings Locks { get; set; }
        public PSAssignmentProvisioningState ProvisioningState {get; set; }    //TODO: private set?

        /// <summary>
        /// Create a PSBluprintAssignment object from an Assignment model.
        /// </summary>
        /// <param name="assignment">Assignment object from which to create the PSBlueprintAssignment.</param>
        /// <param name="subscriptionId">ID of the subscription the assignment is associated with.</param>
        /// <returns>A new PSBlueprintAssignment object.</returns>
        internal static PSBlueprintAssignment FromAssignment(Assignment assignment, string subscriptionId)
        {
            var psAssignment = new PSBlueprintAssignment
            {
                Name = assignment.Name,
                SubscriptionId = subscriptionId,
                Location = assignment.Location,
                Identity = new PSManagedServiceIdentity
                            {
                                PrincipalId = assignment.Identity.PrincipalId,
                                TenantId = assignment.Identity.TenantId,
                                Type = assignment.Type
                            },
                DisplayName = assignment.DisplayName,
                Description = assignment.Description,
                BlueprintId = assignment.BlueprintId,
                ProvisioningState = PSAssignmentProvisioningState.Unknown,
                Status = new PSAssignmentStatus(),
                Locks = new PSAssignmentLockSettings { Mode = PSLockMode.Unknown },
                Parameters = new Dictionary<string, PSParameterValueBase>(),
                ResourceGroups = new Dictionary<string, PSResourceGroupValue>()
            };

            if (DateTime.TryParse(assignment.Status.TimeCreated, out DateTime timeCreated))
                psAssignment.Status.TimeCreated = timeCreated;
            if (DateTime.TryParse(assignment.Status.LastModified, out DateTime lastModified))
                psAssignment.Status.LastModified = lastModified;

            if (Enum.TryParse(assignment.ProvisioningState, true, out PSAssignmentProvisioningState state))
                psAssignment.ProvisioningState = state;

            if (Enum.TryParse(assignment.Locks.Mode, true, out PSLockMode lockMode))
                psAssignment.Locks.Mode = lockMode;

            foreach (var item in assignment.Parameters)
                psAssignment.Parameters.Add(item.Key, new PSParameterValueBase { Description = item.Value.Description });

            foreach (var item in assignment.ResourceGroups)
                psAssignment.ResourceGroups.Add(item.Key, new PSResourceGroupValue { Name = item.Value.Name, Location = item.Value.Location });

            return psAssignment;
        }
    }
}
