using Microsoft.Azure.Management.Blueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSBlueprint : PSAzureResourceBase
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public PSBlueprintStatus Status { get; set; }
        public string TargetScope { get; set; }
        public IDictionary<string, PSParameterDefinition> Parameters { get; set; }
        public IDictionary<string, PSResourceGroupDefinition> ResourceGroups { get; set; }
        public object Versions { get; set; }
        public object Layout { get; set; }
    }
}
