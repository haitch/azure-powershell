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
        /// Construct a BlueprintClient from a subscriptin and a BlueprintManagementClient.
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
            var list = new List<PublishedBlueprint>();
            var response = await blueprintManagementClient.PublishedBlueprints.ListWithHttpMessagesAsync(mgName, blueprintName);

            list.AddRange(response.Body);

            while (response.Body.NextPageLink != null)
            {
                response = await blueprintManagementClient.PublishedBlueprints.ListNextWithHttpMessagesAsync(response.Body.NextPageLink);
                list.AddRange(response.Body);
            }

            PublishedBlueprint latest = null;

            foreach (var bp in list)
            {
                if (latest == null)
                    latest = bp;
                else if (CompareDateStrings(bp.Status.LastModified, latest.Status.LastModified) > 0)
                    latest = bp;
            }

            if (latest != null)
                return PSPublishedBlueprint.FromPublishedBlueprintModel(latest, mgName);

            return null;
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

        #if false
        public IEnumerable<PSPublishedBlueprint> ListPublishedBlueprints(string mgName, string blueprintName)
        {
            var list = new List<PSPublishedBlueprint>();
            var response = blueprintManagementClient.PublishedBlueprints.ListWithHttpMessagesAsync(mgName, blueprintName);

            list.AddRange(response.Result.Body.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));

            while (response.Result.Body.NextPageLink != null)
            {
                response = blueprintManagementClient.PublishedBlueprints.ListNextWithHttpMessagesAsync(response.Result.Body.NextPageLink);
                list.AddRange(response.Result.Body.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));
            }

            return list;
        }
        #endif
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
    }
}
