using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Models
{
    public class PSBlueprintResourceStatusBase
    {
        public string TimeCreated { get; set; }     //TODO: Make this a DateTime?
        public string LastModified { get; set; }    //TODO: Make this a DateTime?
    }
}
