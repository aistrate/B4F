using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Valuations
{
    public enum ValuationTypesFilterOptions
    {
        Security,
        Monetary,
        All
    }
    
    /// <summary>
    /// This class is used to instantiate Valuation companies objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class ValuationMapper
    {
        public static IList<IValuationMutation> GetValuationMutations(IDalSession session, int accountID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("accountID", accountID);
            return session.GetTypedListByNamedQuery<IValuationMutation>(
                "B4F.TotalGiro.Valuations.GetValuationMutationData",
                parameters);
        }

        public static IList<ISecurityValuationMutation> GetSecurityValuationMutations(IDalSession session, int accountID, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("accountID", accountID);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<ISecurityValuationMutation>(
                "B4F.TotalGiro.Valuations.GetSecurityValuationMutationData",
                parameters);
        }

        public static IList<IMonetaryValuationMutation> GetMonetaryValuationMutations(IDalSession session, int accountID, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(3);
            parameters.Add("accountID", accountID);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IMonetaryValuationMutation>(
                "B4F.TotalGiro.Valuations.GetMonetaryValuationMutationData",
                parameters);
        }

        public static IList<IValuationCashMutation> GetValuationCashMutations(IDalSession session, int accountID, DateTime[] dates)
        {
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("accountID", accountID);
            parameterLists.Add("dates", dates);
            return session.GetTypedListByNamedQuery<IValuationCashMutation>(
                "B4F.TotalGiro.Valuations.GetValuationCashMutationDataByDates",
                parameters, parameterLists);
        }

        public static IList<IValuationCashMutation> GetValuationCashMutations(IDalSession session, int accountID, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(3);
            parameters.Add("accountID", accountID);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IValuationCashMutation>(
                "B4F.TotalGiro.Valuations.GetValuationCashMutationDataByStartEndDate",
                parameters);
        }

        #region GetValuations

        public static IList<IValuation> GetValuations(IDalSession session, int accountID, DateTime[] dates)
        {
            return GetValuations(session, accountID, dates, false, ValuationTypesFilterOptions.All);
        }

        public static IList<IValuation> GetValuations(IDalSession session, int accountID, DateTime[] dates, bool includeClosedPositions, ValuationTypesFilterOptions filterOption)
        {
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("accountID", accountID);
            parameterLists.Add("dates", dates);
            if (includeClosedPositions)
                parameters.Add("includeClosedPositions", 1);
            switch (filterOption)
            {
                case ValuationTypesFilterOptions.Security:
                    parameters.Add("instrumentFilterTradeableInstruments", 1);
                    break;
                case ValuationTypesFilterOptions.Monetary:
                    parameters.Add("instrumentFilterCurrencies", 1);
                    break;
            }

            return session.GetTypedListByNamedQuery<IValuation>(
                "B4F.TotalGiro.Valuations.GetValuationData",
                parameters, parameterLists);
        }

        public static List<IValuation> GetValuations(IDalSession session, int accountID, int instrumentID, DateTime[] dates,
                                          bool includeClosedPositions)
        {
            return GetValuations(session, accountID, instrumentID, dates, includeClosedPositions, false);
        }

        public static List<IValuation> GetValuations(IDalSession session, int accountID, int instrumentID, DateTime[] dates, 
                                                     bool includeClosedPositions, bool includeChildInstruments)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parameterLists = new Hashtable();
            parameters.Add("AccountID", accountID);
            parameters.Add("InstrumentID", instrumentID);
            parameterLists.Add("Dates", dates);

            string hql = string.Format(@"from Valuation V 
                                         left join fetch {0} I
                                         {1}
                                         where V.Account.Key = :AccountID 
                                           and I.Key = :InstrumentID 
                                           and V.Date in (:Dates) 
                                           {2} 
                                         order by I.Key, V.Date",
                                       (includeChildInstruments ? "V.Instrument.topParentInstrument" : "V.Instrument"),
                                       (includeClosedPositions ? "" : "left join fetch V.ValuationMutation VM"),
                                       (includeClosedPositions ? "" : "and VM.Size.Quantity != 0"));

            return session.GetTypedListByHQL<IValuation>(hql, parameters, parameterLists);
        }

        public static IList<IValuation> GetValuations(IDalSession session, int accountID, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(3);
            parameters.Add("accountID", accountID);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IValuation>(
                "B4F.TotalGiro.Valuations.GetValuationData",
                parameters);
        }

        #endregion

        public static List<IValuationTotalPortfolio> GetValuationsTotalPortfolio(IDalSession session, int accountId, DateTime[] dates)
        {
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("AccountId", accountId);
            parameterLists.Add("Dates", dates);
            return session.GetTypedListByNamedQuery<IValuationTotalPortfolio>(
                "B4F.TotalGiro.Valuations.GetValuationTotalPortfolioData",
                parameters, parameterLists);
        }

        public static IList<IDepositWithdrawal> GetDepositsWithdrawals(IDalSession session, int accountID, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("accountID", accountID);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            return session.GetTypedListByNamedQuery<IDepositWithdrawal>(
                "B4F.TotalGiro.Valuations.GetDepositsWithdrawalData",
                parameters);
        }

        public static IList<ValuationCashType> GetValuationCashTypes(IDalSession session)
        {
            return session.GetTypedList<ValuationCashType>();
        }

        #region Reporting

        public static PortfolioDevelopment GetPortfolioDevelopment(IDalSession session, IAccountTypeInternal account, DateTime startDate, DateTime endDate, int modelID)
        {
            IList<IValuation> valuations = GetValuations(session, account.Key, new DateTime[] { startDate.AddDays(-1), endDate }, true, ValuationTypesFilterOptions.All);
            IList<ISecurityValuationMutation> securityMutations = GetSecurityValuationMutations(session, account.Key, startDate, endDate);
            IList<IValuationCashMutation> cashMutations = GetValuationCashMutations(session, account.Key, startDate, endDate);
            IList<IDepositWithdrawal> depositsWithdrawals = GetDepositsWithdrawals(session, account.Key, startDate, endDate);
            IList<ValuationCashType> valuationCashTypes = GetValuationCashTypes(session);
            PortfolioDevelopment portDev = new PortfolioDevelopment(account, valuations, securityMutations, cashMutations, depositsWithdrawals, valuationCashTypes, startDate, endDate, modelID);
            return portDev;
        }

        /// <summary>
        /// This method takes a valuation collection as input and returns the same collection but with the cash valuations aggregated.
        /// </summary>
        /// <param name="valuations">The original valuation collection</param>
        /// <returns>A collection with valuations</returns>
        public static IList<IValuation> GetValuationsPortfolioOverview(IList<IValuation> valuations)
        {
            ValuationCollection cashValuations = null;
            foreach (IValuation valuation in valuations)
            {
                if (valuation.Instrument.IsCash)
                {
                    if (cashValuations == null)
                        cashValuations = new ValuationCollection();
                    cashValuations.Add(valuation);
                }
            }

            if (cashValuations != null && cashValuations.Count > 0)
            {

                IValuation aggCashValuation = new AggregatedCashValuation(cashValuations, true);
                foreach (IValuation v in cashValuations)
                {
                    valuations.Remove(v);
                }
                valuations.Add(aggCashValuation);
                return valuations;
            }
            else
                return valuations;
        }

        #endregion
    }
}
