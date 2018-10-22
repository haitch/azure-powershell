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

using Microsoft.Azure.Commands.Blueprint.Cmdlets;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Commands.Common.Test.Mocks;
using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.ResourceManager.Blueprint.Test.UnitTests
{
    [TestClass]
    public class GetAzureBlueprintTest : RMTestBase
    {
        private Mock<IBlueprintClient> mockBlueprintClient;

        private MockCommandRuntime mockCommandRuntime;

        private GetAzureRMBlueprint cmdlet;

        [TestInitialize]
        public void SetupTest()
        {
            this.mockBlueprintClient = new Mock<IBlueprintClient>();
            this.mockCommandRuntime = new MockCommandRuntime();
            this.cmdlet = new GetAzureRMBlueprint
            {
                BlueprintClient = this.mockBlueprintClient.Object,
                CommandRuntime = this.mockCommandRuntime
            };
        }

        [TestMethod]
        public void GetAzureBlueprintAllAccountsSuccessfull()
        {
            // Setup
            string mgName = "resourceGroup";
            string nextLink = string.Empty;

            this.mockBlueprintClient.Setup(f => f.ListBlueprintsAsync(mgName)).Returns( (string a) => Task.FromResult<IEnumerable<PSBlueprint>>(new List<PSBlueprint>()));

            // Test
            this.cmdlet.ManagementGroupName = new string[] { mgName };
            this.cmdlet.ExecuteCmdlet();

            // Assert
            this.mockBlueprintClient.Verify(f => f.ListBlueprintsAsync(mgName), Times.Once());
        }
    }
}
