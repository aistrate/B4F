using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Communicator.ExternalInterfaces;

namespace B4F.TotalGiro.Communicator
{
	public class SymbolMapper
	{
		public static IInstrumentSymbol GetSymbol(IDalSession session, ITradeableInstrument Instrument, IExternalInterface externalInterface)
		{
			List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Instrument.Key", Instrument.Key));
            expressions.Add(Expression.Eq("ExternalInterface", externalInterface));
			IList result = session.GetList(typeof(InstrumentSymbol), expressions);
			return (result.Count > 0) ? (IInstrumentSymbol)result[0] : null;
		}

	}
}
