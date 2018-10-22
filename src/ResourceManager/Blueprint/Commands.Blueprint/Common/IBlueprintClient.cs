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
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Common
{
    public interface IBlueprintClient
    {
        IAzureSubscription Subscription { get; }

        Task<IEnumerable<PSBlueprint>> ListBlueprintsAsync(string mgName);

        Task<PSBlueprint> GetBlueprintAsync(string mgName, string blueprintName);

        //IEnumerable<PSPublishedBlueprint> ListPublishedBlueprints(string mgName, string blueprintName);
        Task<IEnumerable<PSPublishedBlueprint>> ListPublishedBlueprintsAsync(string mgName, string blueprintName);

        Task<PSPublishedBlueprint> GetPublishedBlueprintAsync(string mgName, string blueprintName, string version);

        Task<PSPublishedBlueprint> GetLatestPublishedBlueprintAsync(string mgName, string blueprintName);

        Task<IEnumerable<PSBlueprintAssignment>> ListBlueprintAssignmentsAsync(string subscriptionId);

        Task<PSBlueprintAssignment> GetBlueprintAssignmentAsync(string subscriptionId, string blueprintAssignmentName);
    }
}
