using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.ExternalInterfaces
{
    public class ExternalInterface : IExternalInterface
    {

        public int Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
