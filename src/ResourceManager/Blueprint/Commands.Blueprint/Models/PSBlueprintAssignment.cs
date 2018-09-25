using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSBlueprintAssignment : PSAzureResourceBase
    {
        public string Location { get; set; }

        public PSManagedServiceIdentity Identity { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string BlueprintId { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public IDictionary<string, PSResourceGroupValue> ResourceGroups { get; set; }
        public PSAssignmentStatus Status { get; set; }
        public PSAssignmentLockSettings Locks { get; set; }
        public string ProvisioningState {get; set; }    //TODO: private set?
    }
}
