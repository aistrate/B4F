using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class ExactAccount : IExactAccount
    {
        public int Key { get; set; }
        public String AccountNumber { get; set; }
        public string Description { get; set; }
        public string FullDescription { get { return this.AccountNumber + " - " + this.Description; } }
    }
}
