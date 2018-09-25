using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSManagedServiceIdentity
    {
        public string Type { get; set; }
        public string PrincipalId { get; set; }
        public string TenantId { get; set; }
    }
}
