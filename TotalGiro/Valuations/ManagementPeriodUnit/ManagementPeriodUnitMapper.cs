using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public enum ManagementFeeLocations
    {
        AverageHolding,
        Unit
    }

    public enum ManagementPeriodUnitTradeStatus
    {
        All = -1,
        ExclTrade = 0,
        InclTrade = 1
    }

    public static class ManagementPeriodUnitMapper
    {
        public static IManagementPeriodUnit GetManagementPeriodUnit(IDalSession session, int unitID)
        {
            IManagementPeriodUnit unit = null;
            string hqlUnit = "from ManagementPeriodUnit U where U.Key = :unitID";
            Hashtable parameters = new Hashtable();
            parameters.Add("unitID", unitID);
            IList<ManagementPeriodUnit> units = session.GetTypedListByHQL<ManagementPeriodUnit>(hqlUnit, parameters);
            if (units != null && units.Count == 1)
                unit = units[0];
            return unit;

        }
        
        public static IList<IManagementPeriodUnit> GetManagementPeriodUnitsForManagementFee(IDalSession session, IAccountTypeInternal account, DateTime startDate, DateTime endDate, ManagementTypes managementType)
        {
            Hashtable parameters = new Hashtable(4);
            parameters.Add("account", account);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            parameters.Add("managementType", (int)managementType);

            string hql = @"from ManagementPeriodUnit PU 
                left join fetch PU.ManagementPeriod MP
                left join fetch PU.UnitParent UP
                left join fetch PU.Transaction T
                where UP.Account = :account  
                and (UP.StartDate >= :startDate)
                and (UP.EndDate <= :endDate)
                and (T is null)
                and MP.ManagementType = :managementType";

            return session.GetTypedListByHQL<IManagementPeriodUnit>(hql, parameters);
        }

        public static IList<IManagementPeriodUnit> GetManagementPeriodUnits(IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            int year, int quarter, ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus, bool includeStornoedUnits)
        {
            return GetManagementPeriodUnits(session, assetManagerId, modelPortfolioId, accountNumber, accountName, year, quarter, managementType, tradeStatus, includeStornoedUnits, null);
        }

        public static IList<IManagementPeriodUnit> GetManagementPeriodUnits(IDalSession session, int[] mgtPeriodIds,
            int year, int quarter, ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus, bool includeStornoedUnits)
        {
            return GetManagementPeriodUnits(session, 0, 0, "", "", year, quarter, managementType, tradeStatus, includeStornoedUnits, mgtPeriodIds);
        }

        private static IList<IManagementPeriodUnit> GetManagementPeriodUnits(IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            int year, int quarter, ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus, bool includeStornoedUnits, int[] mgtPeriodIds)
        {
            string where = "";
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable(1);

            if (assetManagerId > 0)
                where += string.Format(" and A.AccountOwner = {0} ", assetManagerId);
            if (modelPortfolioId > 0)
                where += string.Format(" and M.Key = {0}", modelPortfolioId);
            if (accountNumber != null && accountNumber.Length > 0)
                where += string.Format(" and A.Number LIKE '%{0}%'", accountNumber);
            if (accountName != null && accountName.Length > 0)
                where += string.Format(" and A.ShortName LIKE '%{0}%'", accountName);
            if (mgtPeriodIds != null)
                where += string.Format(" and P.Key IN ({0})",
                    (mgtPeriodIds.Length == 0 ? "0" : string.Join(", ", Array.ConvertAll<int, string>(mgtPeriodIds, id => id.ToString()))));
            if (tradeStatus != ManagementPeriodUnitTradeStatus.All)
                where += string.Format(" and T is {0} null", (tradeStatus== ManagementPeriodUnitTradeStatus.InclTrade ? "not" : ""));
            if (!includeStornoedUnits)
                where += " and IsNull(U.IsStornoed, 0) = 0";
            
            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
            parameters.Add("managementType", managementType);

            string hql = string.Format(@"from ManagementPeriodUnit U 
                left join fetch U.ManagementPeriod P
                left join fetch U.UnitParent PU
                left join fetch U.ManagementFee T
                left join fetch PU.Account A
                {0}
                where PU.Period in (:periods) {1}
                and P.ManagementType = :managementType
                order by A.Key, PU.Period",
                    modelPortfolioId > 0 ? "left join fetch A.ModelPortfolio M " : "",
                    where);
            return session.GetTypedListByHQL<IManagementPeriodUnit>(hql, parameters, parameterLists);
        }

        public static IList GetManagementFees(IDalSession session, ManagementFeeLocations location, int assetManagerId, int remisierId, int remisierEmployeeId,
            int modelPortfolioId, string accountNumber, string accountName, bool showActive,
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus,
            ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus,
            bool includeStornoedUnits)
        {
            return GetManagementFees(session, location, assetManagerId, remisierId, remisierEmployeeId, 
                    modelPortfolioId, accountNumber, accountName, showActive, showInactive, year, quarter, 
                    continuationStatus, managementType, tradeStatus, includeStornoedUnits, null);
        }

        public static IList GetManagementFees(IDalSession session, int[] mgtPeriodIds,
            ManagementFeeLocations location, int year, int quarter, ManagementTypes managementType)
        {
            return GetManagementFees(session, location, 0, 0, 0, 0, "", "", true, true, year, quarter, 
                AccountContinuationStati.All, managementType, ManagementPeriodUnitTradeStatus.All, true, 
                mgtPeriodIds);
        }

        private static IList GetManagementFees(
            IDalSession session, ManagementFeeLocations location, int assetManagerId, int remisierId, int remisierEmployeeId,
            int modelPortfolioId, string accountNumber, string accountName, bool showActive,
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus,
            ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus,
            bool includeStornoedUnits, int[] mgtPeriodIds)
        {
            bool useAccounts;
            string where = getManagementFeesWhereClause(assetManagerId, remisierId, remisierEmployeeId, modelPortfolioId,
                accountNumber, accountName, showActive, showInactive, year, quarter, continuationStatus, tradeStatus,
                includeStornoedUnits, out useAccounts);
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable(1);

            if (mgtPeriodIds != null)
                where += string.Format(" and P.Key IN ({0})",
                    (mgtPeriodIds.Length == 0 ? "0" : string.Join(", ", Array.ConvertAll<int, string>(mgtPeriodIds, id => id.ToString()))));

            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
            parameters.Add("managementType", managementType);

            string hql = string.Format(@"select P.Key, PU.Period, F.FeeType.key, SUM(F.Amount.Quantity)
                from {0} F
                left join F.{1} U
                left join U.UnitParent PU
                left join U.ManagementPeriod P
                {2}
                {3}
                where PU.Period in (:periods) {4}
                and P.ManagementType = :managementType
                group by P.Key, PU.Period, F.FeeType.key",
                 location == ManagementFeeLocations.AverageHolding ? "AverageHoldingFee" : "ManagementPeriodUnitFee",
                 location == ManagementFeeLocations.AverageHolding ? "Unit" : "Parent",
                 useAccounts ? "left join PU.Account A " : "",
                 modelPortfolioId > 0 ? "left join A.ModelPortfolio M " : "",
                 where);

            return session.GetListByHQL(hql, parameters, parameterLists);
        }

        public static IList GetManagementFeeSummary(
            IDalSession session, ManagementFeeLocations location, int assetManagerId, int remisierId, int remisierEmployeeId,
            int modelPortfolioId, string accountNumber, string accountName, bool showActive,
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus,
            ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus,
            bool includeStornoedUnits)

        {
            bool useAccounts;
            string where = getManagementFeesWhereClause(assetManagerId, remisierId, remisierEmployeeId, modelPortfolioId,
                accountNumber, accountName, showActive, showInactive, year, quarter, continuationStatus, tradeStatus,
                includeStornoedUnits, out useAccounts);
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable(1);

            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
            parameters.Add("managementType", managementType);

            string hql = string.Format(@"select PU.Period, F.FeeType.key, SUM(F.Amount.Quantity)
                from {0} F
                left join F.{1} U
                left join U.UnitParent PU
                left join U.ManagementPeriod P
                {2}
                where PU.Period in (:periods)
                and P.ManagementType = :managementType
                {3}
                group by PU.Period, F.FeeType.key",
                 location == ManagementFeeLocations.AverageHolding ? "AverageHoldingFee" : "ManagementPeriodUnitFee",
                 location == ManagementFeeLocations.AverageHolding ? "Unit" : "Parent",
                 useAccounts ? "left join PU.Account A " : "",
                 where);
            
            return session.GetListByHQL(hql, parameters, parameterLists);
        }

        private static string getManagementFeesWhereClause(
            int assetManagerId, int remisierId, int remisierEmployeeId,
            int modelPortfolioId, string accountNumber, string accountName, bool showActive,
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus,
            ManagementPeriodUnitTradeStatus tradeStatus, bool includeStornoedUnits, out bool useAccounts)
        {
            string where = "";
            useAccounts = false;

            if (assetManagerId > 0)
                where += string.Format(" and A.AccountOwner = {0} ", assetManagerId);
            if (remisierId > 0)
                where += string.Format(" and A.RemisierEmployee.Remisier.Key = {0} ", remisierId);
            if (remisierEmployeeId > 0)
                where += string.Format(" and A.RemisierEmployee.Key = {0} ", remisierEmployeeId);
            if (modelPortfolioId > 0)
                where += string.Format(" and A.ModelPortfolio.Key = {0}", modelPortfolioId);
            if (accountNumber != null && accountNumber.Length > 0)
                where += string.Format(" and A.Number LIKE '%{0}%'", accountNumber);
            if (accountName != null && accountName.Length > 0)
                where += string.Format(" and A.ShortName LIKE '%{0}%'", accountName);
            if (showActive && !showInactive)
                where += string.Format(" and A.Status = {0}", (int)AccountStati.Active);
            if (!showActive && showInactive)
                where += string.Format(" and A.Status = {0}", (int)AccountStati.Inactive);

            if (!string.IsNullOrEmpty(where))
                useAccounts = true;

            if (continuationStatus != AccountContinuationStati.All)
            {
                DateTime minDate, maxDate;
                Util.GetDatesFromQuarter(year, quarter, out minDate, out maxDate);

                if (continuationStatus == AccountContinuationStati.Current)
                    where += string.Format(" and (P.endDate is null or P.endDate > '{0}')", maxDate.ToString("yyyy-MM-dd"));
                else
                    where += string.Format(" and P.endDate between '{0}' and '{1}'", minDate.ToString("yyyy-MM-dd"), maxDate.ToString("yyyy-MM-dd"));
            }
            if (tradeStatus != ManagementPeriodUnitTradeStatus.All)
                where += string.Format(" and U.ManagementFee is {0} null", (tradeStatus == ManagementPeriodUnitTradeStatus.InclTrade ? "not" : ""));
            if (!includeStornoedUnits)
                where += " and IsNull(U.IsStornoed, 0) = 0";
            return where;
        }

        public static IList<T> GetManagementFees<T>(IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber, string accountName, 
            int year, int quarter, ManagementTypes managementType)
        {
            return GetManagementFees<T>(session, assetManagerId, modelPortfolioId, accountNumber, accountName, year, quarter, managementType, null);
        }

        public static IList<T> GetManagementFees<T>(IDalSession session, int[] mgtPeriodIds,
            int year, int quarter, ManagementTypes managementType)
        {
            return GetManagementFees<T>(session, 0, 0, "", "", year, quarter, managementType, mgtPeriodIds);
        }

        private static IList<T> GetManagementFees<T>(IDalSession session, int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            int year, int quarter, ManagementTypes managementType, int[] mgtPeriodIds)
        {
            string where = "";
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);

            if (assetManagerId > 0)
                where += string.Format(" and A.AccountOwner = {0} ", assetManagerId);
            if (modelPortfolioId > 0)
                where += string.Format(" and M.Key = {0}", modelPortfolioId);
            if (accountNumber != null && accountNumber.Length > 0)
                where += string.Format(" and A.Number LIKE '%{0}%'", accountNumber);
            if (accountName != null && accountName.Length > 0)
                where += string.Format(" and A.ShortName LIKE '%{0}%'", accountName);
            if (mgtPeriodIds != null)
                where += string.Format(" and P.Key IN ({0})",
                    (mgtPeriodIds.Length == 0 ? "0" : string.Join(", ", Array.ConvertAll<int, string>(mgtPeriodIds, id => id.ToString()))));

            parameters.Add("managementType", managementType);
            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));

            ManagementFeeLocations location = 0;
            if (typeof(T).Equals(typeof(IAverageHoldingFee)))
                location = ManagementFeeLocations.AverageHolding;
            else if (typeof(T).Equals(typeof(IManagementPeriodUnitFee)))
                location = ManagementFeeLocations.Unit;

            // select A.Key, PU.Period, F.FeeType.key, SUM(F.Amount.Quantity)

            string hql = string.Format(@"from {0} F
                left join fetch F.{1} U
                left join fetch U.UnitParent PU
                left join fetch U.ManagementPeriod P
                left join fetch PU.Account A
                {2}
                where PU.Period in (:periods) {3}
                and P.ManagementType = :managementType",
                 location == ManagementFeeLocations.AverageHolding ? "AverageHoldingFee" : "ManagementPeriodUnitFee",
                 location == ManagementFeeLocations.AverageHolding ? "Unit" : "Parent",
                 modelPortfolioId > 0 ? "left join fetch A.ModelPortfolio M " : "",
                 where);

            return session.GetTypedListByHQL<T>(hql, parameters, parameterLists);
        }

        public static IList GetManagementPeriodUnitCorrections(IDalSession session, int[] averageHoldingIds, ManagementTypes managementType)
        {
            Hashtable parameters = new Hashtable(1);
            Hashtable parameterLists = new Hashtable(1);
            parameters.Add("managementType", managementType);
            parameterLists.Add("averageHoldingIds", averageHoldingIds);

            string hql = @"from ManagementPeriodUnitCorrection C 
                    left join fetch C.Unit U
                    left join fetch U.UnitParent PU
                    left join fetch U.ManagementPeriod P
                    where C.AverageHolding.Key in (:averageHoldingIds)
                    and P.ManagementType = :managementType";
            return session.GetListByHQL(hql, parameters, parameterLists);
        }
    }
}
