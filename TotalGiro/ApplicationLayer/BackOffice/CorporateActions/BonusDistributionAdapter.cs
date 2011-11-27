using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.History;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transactions;
using System.Globalization;
using B4F.TotalGiro.Instruments.CorporateAction;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions
{
    public static class BonusDistributionAdapter
    {
        public static DataSet GetBonusDistributions(int instrumentKey, DateTime startDate, DateTime endDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return CorporateActionHistoryMapper.GetBonusDistributionList(session, instrumentKey, startDate, endDate)
                    .Select(d => new
                    {
                        d.Key,
                        InstrumentName = d.Instrument.Name,
                        d.DistributionDate,
                        UnitsAllocated = d.TotalSizeDistributed.DisplayString
                    })
                    .ToDataSet();
            }

        }

//        public static BonusDistributionDetails GetBonusDistributionDetails(int bonusID)
//        {
//            using (IDalSession session = NHSessionFactory.CreateSession())
//            {
//                IInstrumentHistoryBonusDistribution history = InstrumentHistoryMapper.GetInstrumentHistoryBonusDistribution(session, bonusID);
//                return new BonusDistributionDetails(history);
//            }
//        }

        public static DataSet GetTradeableInstruments()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return Instruments.InstrumentMapper.GetTradeableInstrumentsForDropDownList(session)
                .Select(p => new
                {
                    Key = p.Key,
                    Description = p.Value
                })
                .OrderBy(o => o.Description)
                .ToDataSet().AddEmptyFirstRow();
            }
        }

//        public static DataSet GetBonusDistributionList(int instrumentHistoryID)
//        {
//            using (IDalSession session = NHSessionFactory.CreateSession())
//            {
//                IList<IBonusDistribution> distributions = InstrumentHistoryMapper.GetBonusDistributions(session, instrumentHistoryID);

//                return distributions.Select(d => new
//                {
//                    d.Key,
//                    Account = d.AccountA.DisplayNumberWithName,
//                    SizeAllocated = d.ValueSize.DisplayString,
//                    PreviousSize = d.PreviousSize.DisplayString,
//                    d.BonusPercentage,
//                    BonusPercentageDisplay = Decimal.Multiply(100.0000000m, d.BonusPercentage).ToString("P7", CultureInfo.CreateSpecificCulture("nl-NL"))
//                })
//                .ToDataSet();

//            }

//        }

        public static DataSet GetInternalAccounts()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetAccounts<IAccountTypeInternal>(session, AccountTypeReturnClass.Internal)
                .Select(p => new
                {
                    Key = p.Key,
                    Description = p.DisplayNumberWithName
                })
                .OrderBy(o => o.Description)
                .ToDataSet().AddEmptyFirstRow();
            }
        }

//        public class BonusDistributionDetails
//        {
//            public BonusDistributionDetails() { }
//            public BonusDistributionDetails(ICorporateActionBonusDistribution bonus)
//            {
//                if (bonus != null)
//                {
//                    this.Key = bonus.Key;
//                    this.Fund = bonus.Instrument as ITradeableInstrument;
//                    this.FundID = bonus.Instrument.Key;
//                    this.ChangeDate = bonus.ChangeDate;
//                    this.TotalSizeDistributed = bonus.TotalSizeDistributed != null ? bonus.TotalSizeDistributed.Quantity : 0m;
//                    this.Account = bonus.CounterAccount;
//                    this.Accountid = bonus.CounterAccount != null ? bonus.CounterAccount.Key : 0;
//                    this.TotalHoldingsAtDate = bonus.TotalHoldingsAtDate != null ? bonus.TotalHoldingsAtDate.Quantity : 0m;
//                    this.SizeToDistribute = bonus.SizeToDistribute != null ? bonus.SizeToDistribute.Quantity : 0m;

//                }
//            }
//            public int Key;
//            public ITradeableInstrument Fund;
//            public int FundID;
//            public DateTime ChangeDate;
//            public Decimal TotalSizeDistributed;
//            public IAccountTypeInternal Account;
//            public int Accountid;
//            public decimal TotalHoldingsAtDate;
//            public decimal SizeToDistribute;


//        }
    }



}
