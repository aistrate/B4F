using System;
using System.Collections;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ModelVersion">ModelVersion</see> class
    /// </summary>
    public interface IModelVersion 
	{
        int VersionNumber { get; set; }
        int Key { get; set; }
        IModelBase ParentModel { get; set; }
        DateTime LatestVersionDate { get; set; }
        IModelComponentCollection ModelComponents { get; }
		IModelInstrumentCollection ModelInstruments { get; }
        bool ContainsCashManagementFund { get; }
        short MaxWithdrawalAmountPercentage { get; }
        ILogin CreatedBy { get; set; }

        decimal TotalAllocation();
        ICashManagementFund GetCashManagementFund();
        ITradeableInstrument GetAlternativeCashFund();
        ITradeableInstrument GetCashFundOrAlternative();
        string ToString();
	}
}
