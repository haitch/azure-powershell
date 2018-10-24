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
    public class GetAzureRMBlueprintAssignment : BlueprintCmdletBase
    {
        #region Parameters
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Subscription { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            if (Name == null)
            {
                ProcessAssignments();
            }
            else
            {
                ProcessNamedAssignments();
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        /// <summary>
        /// Fetch all assignments from the API and write each to the pipeline
        /// </summary>
        private void ProcessAssignments()
        {
            try
            {
                string subscriptionId = Subscription ?? DefaultContext.Subscription.Id;
                var assignments = BlueprintClient.ListBlueprintAssignmentsAsync(subscriptionId).Result;

                WriteObject(assignments, true);
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }

        /// <summary>
        /// Fetch named assignment(s) from the API and write each to the pipeline
        /// </summary>
        private void ProcessNamedAssignments()
        {
            string subscription = Subscription ?? DefaultContext.Subscription.Id;

            foreach (var name in Name)
            {
                try
                {
                    var assignment = BlueprintClient.GetBlueprintAssignmentAsync(subscription, name).Result;

                    WriteObject(assignment);
                }
                catch (Exception ex)
                {
                    WriteExceptionError(ex);
                }
            }
        }
        #endregion Private Methods
    }
}
