using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Instruments.ExRates
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ExRates.ExRate">ExRate</see> class
    /// </summary>
    public interface IExRate
	{
		ICurrency Currency { get; }
        decimal Rate { get; set; }
		DateTime RateDate { get; }
		decimal Bid { get; }
		decimal Ask { get; }
		decimal PriceFactor { get; }
		decimal GetExRate();
		decimal GetExRate(Side side);
        bool IsOldDate { get; }
        bool WasOldDateBy(DateTime referenceDate);

	}
}
