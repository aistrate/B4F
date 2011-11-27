using System;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Instruments.Prices
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Prices.PriceDetail">PriceDetail</see> class
    /// </summary>
    public interface IPriceDetail
	{
        Price Price { get; set;  }
		DateTime Date { get; }
        bool IsOldDate { get; }
        bool WasOldDateBy(DateTime referenceDate);
	}
}
