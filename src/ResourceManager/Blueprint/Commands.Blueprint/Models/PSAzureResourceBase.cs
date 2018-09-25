using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSAzureResourceBase
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public PSAzureResourceBase()
        {
        }

        public PSAzureResourceBase(string Id, string Type, string Name)
        {
            this.Id = Id;
            this.Type = Type;
            this.Name = Name;
        }
    }
}
