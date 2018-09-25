using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSResourceGroupDefinition
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string StrongType { get; set; }
        public IList<string> DependsOn { get; set; }
    }
}
