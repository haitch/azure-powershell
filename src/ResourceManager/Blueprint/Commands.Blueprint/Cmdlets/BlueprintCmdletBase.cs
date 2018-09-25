using Microsoft.Azure.Commands.ResourceManager.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    public class BlueprintCmdletBase : AzureRMCmdlet
    {
        public const string ARMFrontDoor = "https://management.azure.com";
    }
}
