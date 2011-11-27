using System;
namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.MutualFund">MutualFund</see> class
    /// </summary>
    public interface IMutualFund : ISecurityInstrument
	{
		string AdminFee { get; set; }
		decimal BuyCost { get; set; }
		decimal CapitalisationCost { get; set; }
		int DecimalPlaces { get; }
		string DisplayToString(decimal Quantity);
		bool Dividend { get; set; }
		string RatingMS { get; set; }
		decimal SellCost { get; set; }
	}
}
