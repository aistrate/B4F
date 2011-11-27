using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Communicator.Exact
{
    public class LedgerType : ILedgerType
    {
        private LedgerType() { }

        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        
        public virtual string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public virtual LedgerTypes TypeOfLedger { get { return (LedgerTypes)this.Key; } }

        //public LedgerTypes ExportLedgerType { get; set; }

        public override string ToString()
        {
            return Description;
        }

        private int key;
        private string type;
        private string description;
    }
}
