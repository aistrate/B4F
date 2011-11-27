using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments.Nav;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.VirtualFund">VirtualFund</see> class
    /// </summary>
    public interface IVirtualFund : ISecurityInstrument
    {
        IVirtualFundHoldingsAccount HoldingsAccount { get; set; }
        IVirtualFundTradingAccount TradingAccount { get; set; }
        Money InitialNavPerUnit { get; set; }
        DateTime LastNavDate { get; }
        Money SettledCashPositionInBaseValue { get; }
        Money UnSettledCashPositionInBaseValue { get; }
        Money LastNavPerUnit { get; }
        INavCalculation LastNavCalculation { get; set; }
        decimal TotalParticpations { get; }
        void AssignLastNavCalc();
        IJournal JournalForFund { get; set; }
    }
}
