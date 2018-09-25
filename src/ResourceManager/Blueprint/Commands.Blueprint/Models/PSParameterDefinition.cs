using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSParameterDefinition
    {
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string StrongType { get; set; }
        public object DefaultValue { get; set; }
        public IList<object> AllowedValues { get; set; }
    }
}
