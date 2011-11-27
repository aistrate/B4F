using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Journal.Reporting
{
    public interface IClientCashPositionFromGLLedgerRecordCollection : IList<IClientCashPositionFromGLLedgerRecord>
    {
        IClientCashPositionFromGLLedger Parent { get; set; }
    }
}
