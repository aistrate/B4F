using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.TBM
{
    class TBMIssueDetails
    {
        private int issueid = 0;
        private string isin = "";
        private string exchangecode = "";
        private string name = "";

        public virtual int IssueId
        {
            get { return issueid; }
            set { issueid = value; }
        }

        public virtual string ISIN
        {
            get { return isin; }
            set { isin = value; }
        }

        public virtual string ExchangeCode
        {
            get { return exchangecode; }
            set { exchangecode = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", isin, name);
        }
    }
}

