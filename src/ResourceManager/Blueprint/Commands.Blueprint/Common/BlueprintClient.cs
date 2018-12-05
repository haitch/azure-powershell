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
using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BlueprintManagement = Microsoft.Azure.Management.Blueprint;

namespace Microsoft.Azure.Commands.Blueprint.Common
{
    public partial class BlueprintClient : IBlueprintClient
    {
        private readonly BlueprintManagement.IBlueprintManagementClient blueprintManagementClient;

        public IAzureSubscription Subscription { get; private set; }

        // Injection point for unit tests
        public BlueprintClient()
        {
        }

        /// <summary>
        /// Construct a BlueprintClient from an IAzureContext
        /// </summary>
        /// <param name="context"></param>
        public BlueprintClient(IAzureContext context)
            : this(context.Subscription,
                   AzureSession.Instance.ClientFactory.CreateArmClient<BlueprintManagement.BlueprintManagementClient>(context, AzureEnvironment.Endpoint.ResourceManager))
        {
        }

        /// <summary>
        /// Construct a BlueprintClient from a subscription and a BlueprintManagementClient.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="blueprintManagementClient"></param>
        public BlueprintClient(IAzureSubscription subscription,
                               BlueprintManagement.BlueprintManagementClient blueprintManagementClient)
        {
            // Requires.Argument("blueprintManagementClient", blueprintManagementClient).NotNull();

            this.Subscription = subscription;
            this.blueprintManagementClient = blueprintManagementClient;
        }

        private void SetClientIdHeader(string clientRequestId)
        {
            //var client = ((BlueprintManagement.BlueprintManagementClient)this.blueprintManagementClient);
            //
            //client.HttpClient.DefaultRequestHeaders.Remove(Constants.ClientRequestIdHeaderName);
            //client.HttpClient.DefaultRequestHeaders.Add(Constants.ClientRequestIdHeaderName, clientRequestId);
        }

        public async Task<PSBlueprint> GetBlueprintAsync(string mgName, string blueprintName)
        {
            var response = await blueprintManagementClient.Blueprints.GetWithHttpMessagesAsync(mgName, blueprintName);
            
            return PSBlueprint.FromBlueprintModel(response.Body, mgName);
        }

        public async Task<PSBlueprintAssignment> GetBlueprintAssignmentAsync(string subscriptionId, string blueprintAssignmentName)
        {
            var response = await blueprintManagementClient.Assignments.GetWithHttpMessagesAsync(subscriptionId, blueprintAssignmentName);

            return PSBlueprintAssignment.FromAssignment(response.Body, subscriptionId);
        }

        public async Task<PSPublishedBlueprint> GetPublishedBlueprintAsync(string mgName, string blueprintName, string version)
        {
            var response = await blueprintManagementClient.PublishedBlueprints.GetWithHttpMessagesAsync(mgName, blueprintName, version);

            return PSPublishedBlueprint.FromPublishedBlueprintModel(response.Body, mgName);
        }

        public async Task<PSPublishedBlueprint> GetLatestPublishedBlueprintAsync(string mgName, string blueprintName)
        {
            PSPublishedBlueprint latest = null;
            var response = await ListPublishedBlueprintsAsync(mgName, blueprintName);

            foreach (var blueprint in response)
            {
                if (latest == null)
                    latest = blueprint;
                else if (CompareDates(blueprint.Status.LastModified, latest.Status.LastModified) > 0)
                    latest = blueprint;
            }

            return latest;
        }

        public async Task<IEnumerable<PSBlueprintAssignment>> ListBlueprintAssignmentsAsync(string subscriptionId)
        {
            var list = new List<PSBlueprintAssignment>();
            var response = await blueprintManagementClient.Assignments.ListWithHttpMessagesAsync(subscriptionId);

            list.AddRange(response.Body.Select(assignment => PSBlueprintAssignment.FromAssignment(assignment, subscriptionId)));

            while (response.Body.NextPageLink != null)
            {
                response = await blueprintManagementClient.Assignments.ListNextWithHttpMessagesAsync(response.Body.NextPageLink);
                list.AddRange(response.Body.Select(assignment => PSBlueprintAssignment.FromAssignment(assignment, subscriptionId)));
            }

            return list;
        }

        public async Task<IEnumerable<PSBlueprint>> ListBlueprintsAsync(string mgName)
        {
            var list = new List<PSBlueprint>();
            var response = await blueprintManagementClient.Blueprints.ListWithHttpMessagesAsync(mgName);

            list.AddRange(response.Body.Select(bp => PSBlueprint.FromBlueprintModel(bp, mgName)));

            while (response.Body.NextPageLink != null)
            {
                response = await blueprintManagementClient.Blueprints.ListNextWithHttpMessagesAsync(response.Body.NextPageLink);
                list.AddRange(response.Body.Select(bp => PSBlueprint.FromBlueprintModel(bp, mgName)));
            }
 
            return list;
        }

        public async Task<IEnumerable<PSPublishedBlueprint>> ListPublishedBlueprintsAsync(string mgName, string blueprintName)
        {
            var list = new List<PSPublishedBlueprint>();
            var response = await blueprintManagementClient.PublishedBlueprints.ListWithHttpMessagesAsync(mgName, blueprintName);

            list.AddRange(response.Body.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));

            while (response.Body.NextPageLink != null)
            {
                response = await blueprintManagementClient.PublishedBlueprints.ListNextWithHttpMessagesAsync(response.Body.NextPageLink);
                list.AddRange(response.Body.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));
            }

            return list;
        }

        public async Task<PSBlueprintAssignment> DeleteBlueprintAssignmentAsync(string subscriptionId, string blueprintAssignmentName)
        {
            var response = await blueprintManagementClient.Assignments.DeleteWithHttpMessagesAsync(subscriptionId, blueprintAssignmentName);
            
            if (response.Body != null)
            {
                return PSBlueprintAssignment.FromAssignment(response.Body, subscriptionId);
            }

            return null;
        }

        public async Task<PSBlueprintAssignment> CreateOrUpdateBlueprintAssignmentAsync(string subscriptionId, string assignmentName, Assignment assignment)
        {
            var response = await blueprintManagementClient.Assignments.CreateOrUpdateWithHttpMessagesAsync(subscriptionId, assignmentName, assignment);

            if (response.Body != null)
            {
                return PSBlueprintAssignment.FromAssignment(response.Body, subscriptionId);
            }

            return null;
        }
        
        /// <summary>
        /// Compare to nullable DateTime objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>
        /// An integer value that is less than zero if first is earlier than second, greater than zero if first is later than second,
        /// or equal to zero if first is the same as second.
        /// </returns>
        private static int CompareDates(DateTime? first, DateTime? second)
        {
            if (first == null && second == null)
                return 0;
            else if (first == null)
                return -1;
            else if (second == null)
                return 1;

            return DateTime.Compare(first.Value, second.Value);
        }
    }
}
