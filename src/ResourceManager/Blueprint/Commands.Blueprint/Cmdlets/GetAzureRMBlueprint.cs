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
    public class GetAzureRMBlueprint : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        const string ParameterSetDefault = "Default";
        const string ParameterSetByVersion = "ByVersion";
        const string ParameterSetLatestPublished = "LatestPublished";
        #endregion Class Constants

        #region Parameters
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetDefault)]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetByVersion)]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetLatestPublished)]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = ParameterSetDefault)]
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetByVersion)]
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetLatestPublished)]
        [ValidateNotNullOrEmpty]
        public string[] ManagementGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetByVersion)]
        [ValidateNotNullOrEmpty]
        public string BlueprintVersion { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetLatestPublished)]
        public SwitchParameter LatestPublished { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case ParameterSetDefault:
                        HandleDefaultParameterSet();
                        break;

                    case ParameterSetByVersion:
                        EnsureSingleManagementGroup();
                        HandleByVersionParameterSet();
                        break;

                    case ParameterSetLatestPublished:
                        EnsureSingleManagementGroup();
                        HandleLatestPublishedParameterSet();
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        /// <summary>
        /// Handle the "Default" parameter set.
        /// </summary>
        /// <remarks>
        /// The "Default" parameter set fetches either all blueprints or named blueprints.
        /// Both published and draft blueprints are retrieved.
        /// </remarks>
        private void HandleDefaultParameterSet()
        {
            if (ManagementGroupName == null || ManagementGroupName.Length > 1)
            {
                ProcessBlueprintsFromAllManagementGroups();
            }
            else
            {
                if (Name == null)
                {
                    ProcessBlueprintsFromManagementGroup(ManagementGroupName[0]);
                }
                else
                {
                    ProcessNamedBlueprints(ManagementGroupName[0]);
                }
            }
        }

        /// <summary>
        /// Handle the "ByVersion" parameter set.
        /// </summary>
        /// <remarks>
        /// The "ByVersion" parameter set filters published blueprints by version name.
        /// The "Name" parameter is required for this parameter set.
        /// </remarks>
        private void HandleByVersionParameterSet()
        {
            foreach (var name in Name)
            {
                try
                {
                    var blueprint = BlueprintClient.GetPublishedBlueprintAsync(ManagementGroupName[0], name, BlueprintVersion).Result;
                    WriteObject(blueprint);
                }
                catch (Exception)
                {
                    // If only a single Name is given this is an error, othwise the error is ignored.
                    if (Name.Length == 1)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the "LatestPublished" parameter set.
        /// </summary>
        /// <remarks>
        /// The "LatestPublished" parameter set filters published blueprints by latest published date.
        /// </remarks>
        private void HandleLatestPublishedParameterSet()
        {
            foreach (var name in Name)
            {
                try
                {
                    var blueprint = BlueprintClient.GetLatestPublishedBlueprintAsync(ManagementGroupName[0], name).Result;

                    if (blueprint != null)
                    {
                        WriteObject(blueprint);
                    }
                }
                catch (Exception)
                {
                    // If only a single Name is given this is an error, othwise the error is ignored.
                    if (Name.Length == 1)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Fetch all blueprints from the given management group
        /// </summary>
        private async Task<IEnumerable<PSBlueprint>> GetAllBlueprintsFromManagementGroup(string mgName)
        {
            return await BlueprintClient.ListBlueprintsAsync(mgName);
        }

        private void ProcessBlueprintsFromManagementGroup(string mgName)
        {
            var result = GetAllBlueprintsFromManagementGroup(mgName).Result;

            if (result != null)
            {
                foreach (var bp in result)
                {
                    WriteObject(bp);
                }
            }
        }

        /// <summary>
        /// Fetch named blueprint(s) from the API and write each to the pipeline
        /// </summary>
        private void ProcessNamedBlueprints(string mgName)
        {
            foreach (var name in Name)
            {
                try
                {
                    var blueprint = BlueprintClient.GetBlueprintAsync(mgName, name).Result;
                    WriteObject(blueprint);
                }
                catch (Exception)
                {
                    // If only a single Name is given this is an error, othwise the error is ignored.
                    if (Name.Length == 1)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Fetch the list of Management Groups and iterate over them.
        /// If no names were given, output all blueprints from all groups.
        /// If names were given, output only the named blueprints from each group.
        /// </summary>
        private void ProcessBlueprintsFromAllManagementGroups()
        {
            var taskList = new List<Task<IEnumerable<PSBlueprint>>>();

            foreach (var mgName in GetManagementGroupNames())
            {
                taskList.Add(GetAllBlueprintsFromManagementGroup(mgName));
            }

            foreach (var task in taskList)
            {
                var result = task.Result;

                if (result != null)
                {
                    if (Name == null)
                    {
                        foreach (var bp in result)
                        {
                            WriteObject(bp);
                        }
                    }
                    else
                    {
                        foreach (var bp in result)
                        {
                            foreach (var name in Name)
                            {
                                if (name.Equals(bp.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    WriteObject(bp);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return a collection of Management Group names.
        /// If the ManagementGroupName property contains any names, the value of
        /// the property is returned, otherwise a list of names is queried
        /// via the API.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetManagementGroupNames()
        {
            if (ManagementGroupName != null && ManagementGroupName.Length > 0)
            {
                return ManagementGroupName;
            }

            var response = ManagementGroupsClient.ManagementGroups.List();
            return response.Select(managementGroup => managementGroup.Name).ToList();
        }

        private void EnsureSingleManagementGroup()
        {
            if (ManagementGroupName == null || ManagementGroupName.Length != 1)
            {
                throw new ArgumentException("One ManagementGroupName must be provided.");
            }
        }
        #endregion Private Methods
    }
}
