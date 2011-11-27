using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Notas
{
    public class NotaTransfer : NotaTransactionBase, INotaTransfer
    {
        #region Constructor

        internal NotaTransfer() { }

        public NotaTransfer(ITransactionNTM underlyingTx)
            : base(underlyingTx)
        {
        }

        #endregion

        #region Props

        public override string Title
        {
            get { return "Positiemutaties"; }
        }

        public virtual ITradeableInstrument Instrument
        {
            get { return (ITradeableInstrument)ValueSize.Underlying; }
        }

        #endregion
    }
}
