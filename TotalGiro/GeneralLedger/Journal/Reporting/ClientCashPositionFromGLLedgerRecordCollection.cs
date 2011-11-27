using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public class ClientCashPositionFromGLLedgerRecordCollection : TransientDomainCollection<IClientCashPositionFromGLLedgerRecord>, IClientCashPositionFromGLLedgerRecordCollection
    {

        public ClientCashPositionFromGLLedgerRecordCollection()
            : base() { }

        public ClientCashPositionFromGLLedgerRecordCollection(IClientCashPositionFromGLLedger parent)
            : base()
        {
            Parent = parent;
        }

        public IClientCashPositionFromGLLedger Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                IsInitialized = true;
            }
        }

        private IClientCashPositionFromGLLedger parent;
    }
}
