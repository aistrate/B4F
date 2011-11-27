using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.AssetManager">AssetManager</see> class
    /// </summary>
    public interface IAssetManager : IManagementCompany
	{
        bool DoNotChargeCommissionWithdrawals { get; }
        bool IsActive { get; set; }
        decimal MgtFeeThreshold { get; }
        IAssetManagerInstrumentCollection AssetManagerInstruments { get; }
        ICashManagementFund CashManagementFund { get; }
        IList<ITradeableInstrument> ActiveInstruments { get; }
        IList<ITradeableInstrument> Instruments { get; }
        IPortfolioModel ClosedModelPortfolio { get; }
        IRemisierCollection Remisiers { get; }
        string BoSymbol { get; set; }
        string GenerateAccountNumber();
        string MgtFeeDescription { get; }
        string MgtFeeFinalDescription { get; }
        bool SupportLifecycles { get; set; }
	}
}
