using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Collection.Generic;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public class ClientCashPositionFromGLLedger : IClientCashPositionFromGLLedger
    {
        public ClientCashPositionFromGLLedger()
        {

        }

        public DateTime Key { get; set; }



        public virtual IClientCashPositionFromGLLedgerRecordCollection Records
        {
            get
            {
                IClientCashPositionFromGLLedgerRecordCollection records1 = (IClientCashPositionFromGLLedgerRecordCollection)records.AsList();
                if (records1.Parent == null) records1.Parent = this;
                return records1;
            }
        }

        private IDomainCollection<IClientCashPositionFromGLLedgerRecord> records = new ClientCashPositionFromGLLedgerRecordCollection();
    }
}
