using System;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    ///// <summary>
    ///// What property of the order is the commission calculations using 
    ///// </summary>
    //public enum CommCalcBasedOn
    //{
    //    /// <summary>
    //    /// It is using the order value to calculate the commission
    //    /// </summary>
    //    Value,
    //    /// <summary>
    //    /// It is using the order size to calculate the commission
    //    /// </summary>
    //    Size
    //}

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Fees.CommCalculations.CommCalc">CommCalc</see> class
    /// </summary>
    public interface ICommCalc
	{
		string Name { get; set; }
		ICurrency CommCurrency { get; set; }
		int Key { get; set; }
		Money FixedSetup { get; set; }
		Money MinValue { get; set; }
		Money MaxValue { get; set; }
        //CommCalcBasedOn CalcBasedOn { get; set; }
		FeeCalcTypes CalcType { get; }
        IAssetManager AssetManager { get; set; }

        Money Calculate(ICommClient client);
	}
}
