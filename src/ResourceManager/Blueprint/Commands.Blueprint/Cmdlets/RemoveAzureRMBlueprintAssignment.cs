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
    [Cmdlet(VerbsCommon.Remove, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class RemoveAzureRMBlueprintAssignment : BlueprintCmdletBase
    {
        #region Parameters
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "Default")]
        [ValidateNotNull]
        public string[] Name { get; set; }

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "InputObject")]
        public PSBlueprintAssignment[] InputObject { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        [ValidateNotNullOrEmpty]
        public string Subscription { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            switch (ParameterSetName)
            {
                case "Default":
                    HandleDefaultParameterSet();
                    break;

                case "InputObject":
                    HandleInputObjectParameterSet();
                    break;
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        /// <summary>
        /// Handle the "Default" parameter set.
        /// </summary>
        /// <remarks>
        /// The "Default" parameter set removes assignments by name(s) either given
        /// on the command line by the -Name parameter or input through the pipeline.
        /// Optionally, a subscription ID can be provided through the -Subscription
        /// parameter.
        /// </remarks>
        private void HandleDefaultParameterSet()
        {
            string subscription = Subscription ?? DefaultContext.Subscription.Id;

            foreach (var name in Name)
                PerformDelete(subscription, name);
        }

        /// <summary>
        /// Handle the "InputObject" parameter set.
        /// </summary>
        /// <remarks>
        /// The "InputObject" parameter set removes assignments referred to by
        /// PSBlueprintAssignment object(s) either given on the command line by
        /// the -InputObject parameter or input through the pipeline.
        /// </remarks>
        private void HandleInputObjectParameterSet()
        {
            foreach (var assignment in InputObject)
                PerformDelete(assignment.SubscriptionId, assignment.Name);
        }

        /// <summary>
        /// Perform the deletion of a blueprint assignment.
        /// </summary>
        /// <param name="subscription">The subscription ID for the assignment.</param>
        /// <param name="name">The name of the assignment</param>
        private void PerformDelete(string subscription, string name)
        {
            try
            {
                if (ShouldProcess(subscription + "/" + name))
                {
                    var assignment = Client.Assignments.Delete(subscription, name);

                    if (assignment != null)
                        WriteObject(PSBlueprintAssignment.FromAssignment(assignment, subscription));
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        #endregion Private Methods
    }
}
