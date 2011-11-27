using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.Notas
{
    public class NotaInstrumentConversion : NotaTransactionBase, INotaInstrumentConversion
    {
        #region Constructor

        internal NotaInstrumentConversion() { }

        public NotaInstrumentConversion(IInstrumentConversion underlyingTx)
            : base(underlyingTx)
        {
        }

        #endregion

        #region Props

        public virtual string Description
        {
            get { return OriginalTransaction.Description; }
        }

        public virtual InstrumentSize ConvertedInstrumentSize
        {
            get { return ((IInstrumentConversion)UnderlyingTx).ConvertedInstrumentSize; }
        }

        public virtual IInstrumentHistory InstrumentTransformation
        {
            get { return ((IInstrumentConversion)OriginalTransaction).InstrumentTransformation; }
        }

        public override string Title
        {
            get { return "Beheer handeling"; }
        }

        #endregion
    }
}
