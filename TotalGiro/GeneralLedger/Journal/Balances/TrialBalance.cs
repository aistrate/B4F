using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Collection.Generic;

namespace B4F.TotalGiro.GeneralLedger.Journal.Balances
{
    public class TrialBalance
    {
        public TrialBalance()
        {

        }

        public DateTime Key { get; set; }

        public IList<TrialBalanceRecord> Records
        {
            get
            {
                return (IList<TrialBalanceRecord>)records;
            }

        }

        private PersistentGenericBag<TrialBalanceRecord> records;
    }
}
