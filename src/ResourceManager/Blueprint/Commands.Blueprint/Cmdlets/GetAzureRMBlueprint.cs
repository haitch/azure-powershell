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
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = ParameterSetDefault)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSetByVersion)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSetLatestPublished)]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetDefault)]
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetByVersion)]
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetLatestPublished)]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetByVersion)]
        [ValidateNotNullOrEmpty]
        public string VersionName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetLatestPublished)]
        public SwitchParameter LatestPublished { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            switch (ParameterSetName)
            {
                case ParameterSetDefault:
                    HandleDefaultParameterSet();
                    break;

                case ParameterSetByVersion:
                    HandleByVersionParameterSet();
                    break;

                case ParameterSetLatestPublished:
                    HandleLatestPublishedParameterSet();
                    break;
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        /// <summary>
        /// Handle the "Default" parameter set.
        /// </summary>
        /// <remarks>
        /// The "Default" parameter set fetches either all blueprints or named blueprints.
        /// Both published and draft blueprints are used.
        /// </remarks>
        private void HandleDefaultParameterSet()
        {
            if (Name == null)
            {
                ProcessBlueprints();
            }
            else
            {
                ProcessNamedBlueprints();
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
                    var bp = Client.PublishedBlueprints.Get(ManagementGroupName, name, VersionName);

                    WriteObject(PSPublishedBlueprint.FromPublishedBlueprintModel(bp, ManagementGroupName));
                }
                catch (Exception ex)
                {
                    WriteExceptionError(ex);
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
                Management.Blueprint.Models.PublishedBlueprint latest = null;

                try
                {
                    var list = Client.PublishedBlueprints.List(ManagementGroupName, name);

                    while (true)
                    {
                        foreach (var bp in list)
                        {
                            if (latest == null)
                                latest = bp;
                            else if (CompareDateStrings(bp.Status.LastModified, latest.Status.LastModified) > 0)
                                latest = bp;
                        }

                        if (list.NextPageLink == null)
                            break;

                        list = Client.PublishedBlueprints.ListNext(list.NextPageLink);
                    }

                    if (latest != null)
                        WriteObject(PSPublishedBlueprint.FromPublishedBlueprintModel(latest, ManagementGroupName));
                }
                catch (Exception ex)
                {
                    WriteExceptionError(ex);
                }
            }
        }

        /// <summary>
        /// Fetch all blueprints from the API and write each to the pipeline
        /// </summary>
        private void ProcessBlueprints()
        {
            try
            {
                var list = Client.Blueprints.List(ManagementGroupName);

                while (true)
                {
                    foreach (var bp in list)
                        WriteObject(PSBlueprint.FromBlueprintModel(bp, ManagementGroupName));

                    if (list.NextPageLink == null)
                        return;

                    list = Client.Blueprints.ListNext(list.NextPageLink);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }

        /// <summary>
        /// Fetch named blueprint(s) from the API and write each to the pipeline
        /// </summary>
        private void ProcessNamedBlueprints()
        {
            foreach (var name in Name)
            {
                try
                {
                    var bp = Client.Blueprints.Get(ManagementGroupName, name);

                    WriteObject(PSBlueprint.FromBlueprintModel(bp, ManagementGroupName));
                }
                catch (Exception ex)
                {
                    WriteExceptionError(ex);
                }
            }
        }
    
        /// <summary>
        /// Compare two strings representing date/time values
        /// </summary>
        /// <param name="first">First string to compare.</param>
        /// <param name="second">Second string to compare</param>
        /// <returns>
        /// An integer value that is less than zero if first is earlier than second, greater than zero if first is later than second,
        /// or equal to zero if first is the same as second.
        /// </returns>
        /// <remarks>
        /// In the event that one or both strings cannot be parsed into a DateTime object, the unparsable string will be
        /// treated as a null DateTime? object. If both strings are unparsable they will be considered equal. If one
        /// string is unparsable it will be considered earlier than the one that is successfully parsed. Otherwise the
        /// two strings are parsed into DateTime objects and compared with the DateTime.Compare method.
        /// </remarks>
        private static int CompareDateStrings(string first, string second)
        {
            DateTime?   dtFirst = null;
            DateTime?   dtSecond = null;
            DateTime    dt = new DateTime();

            if (DateTime.TryParse(first, out dt))
                dtFirst = dt;
            if (DateTime.TryParse(second, out dt))
                dtSecond = dt;

            if (dtFirst == null && dtSecond == null)
                return 0;
            else if (dtFirst == null)
                return -1;
            else if (dtSecond == null)
                return 1;

            return DateTime.Compare(dtFirst.Value, dtSecond.Value);
        }
        #endregion Private Methods
    }
}
