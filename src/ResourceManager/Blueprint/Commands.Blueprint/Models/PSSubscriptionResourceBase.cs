using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSSubscriptionResourceBase
    {
        public string Name { get; set; }
        public string SubscriptionId { get; set; }
    }
}
