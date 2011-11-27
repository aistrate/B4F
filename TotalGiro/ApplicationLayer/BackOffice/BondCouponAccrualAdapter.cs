using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.CorporateAction;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;
using NHibernate.Linq;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Stichting.Login;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice
{
    public static class BondCouponAccrualAdapter
    {
        public static void ProcessBondPositions(DateTime upToDate)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.AccruedInterest);

            // Get all bond positions
            Hashtable parameters = new Hashtable(2);
            parameters.Add("bondSecCategoryId", SecCategories.Bond);
            parameters.Add("upToDate", upToDate);
            IList<int> accountKeys = session.GetTypedListByNamedQuery<int>(
                "B4F.TotalGiro.ApplicationLayer.BackOffice.GetBondPositions",
                parameters);

            if (accountKeys != null && accountKeys.Count > 0)
            {
                foreach (int accountId in accountKeys)
                {
                    IDalSession session2 = NHSessionFactory.CreateSession();
                    IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session2, accountId);
                    bool success = false;
                    foreach (IFundPosition pos in account.Portfolio.PortfolioInstrument.Where(x => x.Instrument.SecCategory.Key == SecCategories.Bond))
                    {
                        IBond bond = pos.Instrument as IBond;
                        if (bond == null || bond.SecCategory.Key != SecCategories.Bond)
                            throw new ApplicationException(string.Format("The instrument {0} is not a bond.", pos.Instrument.DisplayIsinWithName));

                        Hashtable parameters2 = new Hashtable(3);
                        parameters2.Add("positionId", pos.Key);
                        parameters2.Add("calcDate", Util.IsNotNullDate(pos.LastBondCouponCalcDate) ? pos.LastBondCouponCalcDate : new DateTime(2000,1,1));
                        parameters2.Add("statusId", BondCouponPaymentStati.Settled);
                        IList<IBondCouponPaymentDailyCalculation> oldCalculations = session.GetTypedListByNamedQuery<IBondCouponPaymentDailyCalculation>(
                            "B4F.TotalGiro.ApplicationLayer.BackOffice.GetBondCouponPaymentDailyCalculationsForPosition",
                            parameters2);

                        success = processBondPosition(pos, null, upToDate, oldCalculations, lookups, session);
                    }
                    if (success)
                        session2.Update(account);
                }
            }

            // Get payments that did not process
            parameters = new Hashtable(1);
            parameters.Add("statusActive", BondCouponPaymentStati.Active);
            IList<IBondCouponPayment> bipsToCancel = session.GetTypedListByNamedQuery<IBondCouponPayment>(
                "B4F.TotalGiro.ApplicationLayer.BackOffice.GetActiveBondCouponPaymentsWithClosedPositions",
                parameters);
            if (bipsToCancel != null && bipsToCancel.Count > 0)
            {
                foreach (IBondCouponPayment bip in bipsToCancel)
                {
                    bip.Cancel();
                    session.Update(bip);
                }
            }
        }

        public static bool ProcessBondPosition(IDalSession session, IAccountTypeInternal account, ITradeableInstrument instrument, IExchange exchange, DateTime upToDate, IGLLookupRecords lookups)
        {
            bool success = false;
            IFundPosition pos = account.Portfolio.PortfolioInstrument.Where(x => x.Instrument.Key == instrument.Key).FirstOrDefault();

            if (pos != null)
            {
                IList<IBondCouponPaymentDailyCalculation> oldCalculations = null;
                if (pos.BondCouponCalculations != null)
                    oldCalculations = pos.BondCouponCalculations
                        .Where(x => x.CalcDate > upToDate && x.Parent.Status == BondCouponPaymentStati.Settled && x.Parent.StornoBooking == null)
                        .ToList();

                IBondCouponPaymentDailyCalculation lastCalc =  pos.BondCouponPayments.Get(e => e.ActivePayment).Get(e => e.DailyCalculations).Get(e => e.LastCalculation);

                success = processBondPosition(pos, exchange, lastCalc != null ? lastCalc.CalcDate : upToDate, oldCalculations, lookups, session);
            }
            // Check for cancel
            InstrumentSize size = pos.PositionTransactions.Where(x => x.TransactionDate <= upToDate).Select(x => x.Size).Sum();
            if (size != null && size.IsZero && pos.BondCouponPayments.ActivePayment != null)
            {
                IBondCouponPayment bip = pos.BondCouponPayments.ActivePayment;
                if (Util.DateBetween(bip.CouponHistory.StartAccrualDate, bip.CouponHistory.EndAccrualDate, upToDate))
                    success = bip.Cancel();
            }
            return success;
        }

        private static bool processBondPosition(
            IFundPosition pos, IExchange exchange, DateTime upToDate,
            IList<IBondCouponPaymentDailyCalculation> oldCalculations, IGLLookupRecords lookups, 
            IDalSession session)
        {
            bool success = false;
            IBond bond = pos.Instrument as IBond;
            if (bond == null || bond.SecCategory.Key != SecCategories.Bond)
                throw new ApplicationException(string.Format("The instrument {0} is not a bond.", pos.Instrument.DisplayIsinWithName));

            if (bond.DoesPayInterest)
            {
                DateTime calcDate;
                if (Util.IsNotNullDate(pos.LastBondCouponCalcDate))
                    calcDate = pos.LastBondCouponCalcDate.AddDays(1);
                else
                    calcDate = pos.OpenDate;

                while (calcDate <= upToDate)
                {
                    if (exchange == null)
                        exchange = bond.DefaultExchange ?? bond.HomeExchange;

                    InstrumentSize size = pos.PositionTransactions.Where(x => x.TransactionDate <= calcDate).Select(x => x.Size).Sum();
                    if (size != null && size.IsNotZero)
                    {
                        if (!Util.IsWeekendOrHoliday(calcDate, exchange.ExchangeHolidays))
                        {
                            DateTime settlementDate = bond.GetSettlementDate(calcDate, exchange);

                            IBondCouponPayment bip = null;
                            DateTime lastCouponDate = bond.LastCouponDate(settlementDate);
                            if (Util.IsNullDate(lastCouponDate))
                                lastCouponDate = bond.IssueDate;
                            DateTime nextCouponDate = bond.NextCouponDate(settlementDate);
                            if ((bond.IsPerpetual || bond.MaturityDate >= settlementDate) &&
                                Util.IsNotNullDate(lastCouponDate) && lastCouponDate <= settlementDate &&
                                Util.IsNotNullDate(nextCouponDate) && nextCouponDate >= settlementDate)
                            {
                                // Per position -> Does have an Active BondCouponPayment
                                bip = pos.BondCouponPayments.GetBondCouponPaymentByDate(settlementDate);
                                if (bip == null)
                                {
                                    ICouponHistory couponHistory = bond.Coupons.GetCouponByDate(settlementDate);
                                    if (couponHistory == null)
                                    {
                                        couponHistory = new CouponHistory(bond, lastCouponDate, nextCouponDate);
                                        bond.Coupons.AddCoupon(couponHistory);
                                    }
                                    int journalID = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultAccruedInterestJournal")));
                                    IJournal journal = JournalMapper.GetJournal(session, journalID);
                                    string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
                                    IMemorialBooking memorialBooking = new MemorialBooking(journal, nextJournalEntryNumber);
                                    memorialBooking.TransactionDate = couponHistory.EndAccrualDate;
                                    bip = new BondCouponPayment(pos, couponHistory, memorialBooking);
                                    pos.BondCouponPayments.AddPayment(bip);
                                }
                                if (bip != null)
                                {
                                    // Add interest accrual 
                                    bip.CalculateDailyInterest(size, calcDate, settlementDate,
                                        oldCalculations != null && oldCalculations.Count > 0 ? oldCalculations.Where(x => x.CalcDate == calcDate).ToList() : null,
                                        lookups);
                                }
                            }

                            // If coupon payment date equals settlementDate -> set to status -> to-be-settled
                            if (bip != null && nextCouponDate <= settlementDate)
                                bip.SetToBeSettled(calcDate, settlementDate);


                            // If coupon payment date -> pay (unsettled to settled)
                            // Settle the interest
                            List<IBondCouponPayment> bipsToBeSettled  = pos.BondCouponPayments.ToBeSettledPayments(calcDate);
                            if (bipsToBeSettled != null && bipsToBeSettled.Count > 0)
                            {
                                foreach (IBondCouponPayment bipToBeSettled in bipsToBeSettled)
                                    bipToBeSettled.SettleInterest(calcDate);
                            }
                            success = true;
                        }
                        pos.LastBondCouponCalcDate = calcDate;
                    }
                    calcDate = calcDate.AddDays(1);
                }
            }
            return success;
        }

        public static bool StornoBondTransaction(IDalSession session, ITransaction storno, IInternalEmployeeLogin employee)
        {
            bool success = false;
            if (storno.Approved)
            {
                IList<IBondCouponPayment> bondPaymentsToStorno = storno.GetBondPaymentsToStorno();
                if (bondPaymentsToStorno != null && bondPaymentsToStorno.Count > 0)
                {
                    IGLLookupRecords lookups = null;
                    int journalID = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultAccruedInterestJournal")));
                    IJournal journal = JournalMapper.GetJournal(session, journalID);
                    foreach (IBondCouponPayment payment in bondPaymentsToStorno.OrderByDescending(x => x.CouponHistory.StartAccrualDate))
                    {
                        if (payment.Status == BondCouponPaymentStati.Active || payment.Status == BondCouponPaymentStati.ToBeSettled)
                        {
                            if (lookups == null)
                                lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.AccruedInterest);
                            payment.Cancel();
                            success = true;
                        }
                        else if (payment.Status == BondCouponPaymentStati.Settled)
                        {
                            string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);
                            IMemorialBooking memorialBooking = new MemorialBooking(journal, nextJournalEntryNumber);
                            memorialBooking.TransactionDate = payment.CouponHistory.EndAccrualDate;
                            payment.Storno(employee, storno.StornoReason, memorialBooking);
                            success = true;
                        }
                    }
                }
            }
            return success;
        }

        public static DataSet GetCoupons(string isin, string instrumentName, int currencyNominalId, DateTime dateFrom, DateTime dateTo)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();
                if (Util.IsNotNullDate(dateFrom))
                    parameters.Add("dateFrom", dateFrom);
                if (Util.IsNotNullDate(dateTo))
                    parameters.Add("dateTo", dateTo);
                if (!string.IsNullOrEmpty(isin))
                    parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
                if (!string.IsNullOrEmpty(instrumentName))
                    parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
                if (currencyNominalId > 0)
                    parameters.Add("currencyNominalId", currencyNominalId);

                List<ICouponHistory> list = session.GetTypedListByNamedQuery<ICouponHistory>(
                    "B4F.TotalGiro.ApplicationLayer.BackOffice.GetCoupons",
                    parameters);

                return list.Select(c => new
                {
                    c.Key,
                    c.Description,
                    InstrumentName = c.Instrument.Name,
                    ISIN = c.Instrument.DisplayIsin
                })
                .ToDataSet();
            }
        }

        public static DataSet GetCouponPaymentDetails(int couponId)
        {
            DataSet ds = null;
            if (couponId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    Hashtable parameters = new Hashtable();
                    parameters.Add("couponId", couponId);

                    List<IBondCouponPayment> list = session.GetTypedListByNamedQuery<IBondCouponPayment>(
                        "B4F.TotalGiro.ApplicationLayer.BackOffice.GetCouponPaymentsByCouponHistoryID",
                        parameters);

                    ds = list.Select(c => new
                    {
                        c.Key,
                        AccountNumber = c.Position.Account.Number,
                        AccountName = c.Position.Account.ShortName,
                        PositionID = c.Position.Key,
                        PositionSize = c.Position.Size.Quantity,
                        AccruedAmount = c.TotalAmountUnSettled.GetS(e => e.DisplayString),
                        LastCalcDate = c.DailyCalculations.LastCalculation.CalcDate,
                        Calculations_Count = c.DailyCalculations.Count,
                        c.Status
                    })
                    .ToDataSet();
                }
            }
            return ds;
        }

        public static DataSet GetCouponPaymentCalculations(int couponId, int positionId)
        {
            DataSet ds = null;
            if (couponId != 0 && positionId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    Hashtable parameters = new Hashtable();
                    parameters.Add("couponId", couponId);
                    parameters.Add("positionId", positionId);

                    List<IBondCouponPaymentDailyCalculation> list = session.GetTypedListByNamedQuery<IBondCouponPaymentDailyCalculation>(
                        "B4F.TotalGiro.ApplicationLayer.BackOffice.GetCouponPaymentCalculations",
                        parameters);

                    ds = list.Select(c => new
                    {
                        c.Key,
                        c.CalcDate,
                        c.SettlementDate,
                        PositionSize = c.PositionSize.GetS(e => e.DisplayString),
                        CalculatedAccruedInterestUpToDate = c.CalculatedAccruedInterestUpToDate.GetS(e => e.DisplayString),
                        DailyInterest = c.DailyInterest.GetS(e => e.DisplayString)
                    })
                    .ToDataSet();
                }
            }
            return ds;
        }
    }
}
