using System;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.CashManagementFund">CashManagementFund</see> class
    /// </summary>
    public interface ICashManagementFund : ITradeableInstrument
	{
		int DecimalPlaces { get; }
		string DisplayToString(decimal Quantity);
	}
}
