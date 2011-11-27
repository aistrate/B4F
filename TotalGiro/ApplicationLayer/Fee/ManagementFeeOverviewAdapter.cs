using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.ManagementPeriodUnits.ReportData;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.ApplicationLayer.Fee
{
    public static class ManagementFeeOverviewAdapter
    {
        #region Overview

        public static DataSet GetUnitsOverview(
            int assetManagerId, int remisierId, int remisierEmployeeId, int modelPortfolioId, 
            string accountNumber, string accountName, bool showActive, bool showInactive, 
            int year, int quarter, AccountContinuationStati continuationStatus, ManagementTypes managementType,
            ManagementPeriodUnitTradeStatus tradeStatus, bool includeStornoedUnits,
            int maximumRows, int pageIndex, string sortColumn)
        {
            string propertyList =
                string.Format("Key, {0}Account.Key, Account.Number, Account.ShortName, Account.ModelPortfolio.ShortName, Account.Status, TradeID, ManagementEndDate, Error, Period1, MgtPeriod1.TotalValue.DisplayString, MgtPeriod1.FeeAmount.DisplayString, MgtPeriod1.HasMessage, MgtPeriod1.Message, Period2, MgtPeriod2.TotalValue.DisplayString, MgtPeriod2.FeeAmount.DisplayString, MgtPeriod2.HasMessage, MgtPeriod2.Message, Period3, MgtPeriod3.TotalValue.DisplayString, MgtPeriod3.FeeAmount.DisplayString, MgtPeriod3.HasMessage, MgtPeriod3.Message, AllowCreateTransaction",
                (managementType == ManagementTypes.KickBack ? "Account.RemisierEmployee.Remisier.Name, Account.RemisierEmployee.Employee.FullName, IsKickBackExported, " : ""));

            string bareSortColumn = sortColumn.Split(' ')[0];
            bool ascending = !(sortColumn.Split(' ').Length == 2 && sortColumn.Split(' ')[1] == "DESC");

            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            IList allUnitFeeItems;
            if (isHqlSortingNeeded(bareSortColumn))
            {
                allUnitFeeItems = GetUnitFeeOverviewList(session,
                                    assetManagerId, remisierId, remisierEmployeeId, modelPortfolioId, accountNumber, accountName, 
                                    showActive, showInactive, year, quarter, continuationStatus, managementType,
                                    tradeStatus, includeStornoedUnits, null, bareSortColumn, ascending, true);
                if (allUnitFeeItems != null && allUnitFeeItems.Count > 0)
                {
                    var items = from a in allUnitFeeItems.Cast<object[]>()
                                group a by (int)a[0] into g
                                select new { ID = g.Key, AccountStatus = from b in g orderby b[3] descending select b[3], TradeID = from b in g orderby b[4] descending select b[4] };

                    ds = new DataSet("UnitFeeKeys");
                    DataTable dt = new DataTable();
                    ds.Tables.Add(dt);
                    dt.Columns.Add("Key", typeof(int));
                    dt.Columns.Add("AllowCreateTransaction", typeof(bool));

                    foreach (var item in items)
                    {
                        bool allowCreateTransaction = false;
                        AccountStati s = (AccountStati)item.AccountStatus.First();
                        object t = item.TradeID.First();
                        if (s == AccountStati.Active && (t == null || string.IsNullOrEmpty(t.ToString())))
                            allowCreateTransaction = true;

                        dt.Rows.Add(new object[] { item.ID, allowCreateTransaction });
                    }

                    //ds = DataSetBuilder.CreateDataSetFromHibernateList(allUnitFeeItems, "Key");
                }
            }
            else
            {
                allUnitFeeItems = GetUnitFeeOverviewList(session,
                                    assetManagerId, remisierId, remisierEmployeeId, modelPortfolioId, accountNumber, accountName, 
                                    showActive, showInactive, year, quarter, continuationStatus, managementType,
                                    tradeStatus, includeStornoedUnits, null, bareSortColumn, ascending, false);
                if (allUnitFeeItems != null)
                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(allUnitFeeItems, "Key, " + bareSortColumn.Replace('_', '.'));
                if (ds != null)
                    Util.SortDataTable(ds.Tables[0], sortColumn);

                session.Close();
                session = NHSessionFactory.CreateSession();
            }

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                int[] mgtPeriodIds = Util.GetPageKeys(ds.Tables[0], maximumRows, pageIndex, "Key");
                IList pageUnitFeeItems = GetUnitFeeOverviewList(session, 0, 0, 0, 0, null, null, false, false,
                                        year, quarter, continuationStatus, managementType, tradeStatus, includeStornoedUnits, mgtPeriodIds, bareSortColumn, ascending, false);
                DataSetBuilder.MergeDataTableWithBusinessObjectList(ds.Tables[0], pageUnitFeeItems, "Key", propertyList);
            }
            session.Close();
            return ds;
        }

        private static bool isHqlSortingNeeded(string sortColumn)
        {
            string[] hqlSortColumns = new string[] { "KEY", "ACCOUNT_NUMBER", "ACCOUNT_SHORTNAME", "MANAGEMENTENDDATE", "TRADEID", "ACCOUNT_REMISIEREMPLOYEE_REMISIER_NAME", "ACCOUNT_REMISIEREMPLOYEE_EMPLOYEE_FULLNAME" };
            sortColumn = sortColumn.ToUpper();
            foreach (string col in hqlSortColumns)
                if (col == sortColumn)
                    return true;
            return false;
        }

        private static string GetUnitFeeOverviewWhereClause(
            int assetManagerId, int remisierId, int remisierEmployeeId,
            int modelPortfolioId, string accountNumber, string accountName, bool showActive,
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus,
            ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus,
            bool includeStornoedUnits, Hashtable parameters, bool useParametersOnly)
        {
            string where = "";

            if (assetManagerId > 0)
            {
                parameters.Add("assetManagerId", assetManagerId);
                if (!useParametersOnly)
                    where += " and A.AccountOwner = :assetManagerId";
            }
            if (remisierId > 0)
            {
                parameters.Add("remisierId", remisierId);
                if (!useParametersOnly)
                    where += " and A.RemisierEmployee.Remisier.Key = :remisierId";
            }
            if (remisierEmployeeId > 0)
            {
                parameters.Add("remisierEmployeeId", remisierEmployeeId);
                if (!useParametersOnly)
                    where += " and A.RemisierEmployee.Key = :remisierEmployeeId";
            }
            if (modelPortfolioId > 0)
            {
                parameters.Add("modelPortfolioId", modelPortfolioId);
                if (!useParametersOnly)
                    where += " and A.ModelPortfolio.Key = :modelPortfolioId";
            }
            if (accountNumber != null && accountNumber.Length > 0)
            {
                parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber));
                if (!useParametersOnly)
                    where += " and A.Number like :accountNumber";
            }
            if (accountName != null && accountName.Length > 0)
            {
                parameters.Add("accountName", Util.PrepareNamedParameterWithWildcard(accountName));
                if (!useParametersOnly)
                    where += " and A.ShortName like :accountName";
            }
            if (showActive && !showInactive)
            {
                parameters.Add("accountStatus", (int)AccountStati.Active);
                if (!useParametersOnly)
                    where += " and A.Status = :accountStatus";
            }
            else if (!showActive && showInactive)
            {
                parameters.Add("accountStatus", (int)AccountStati.Inactive);
                if (!useParametersOnly)
                    where += " and A.Status = :accountStatus";
            }
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

        public static IList GetUnitFeeOverviewList(
            IDalSession session, int assetManagerId, int remisierId, int remisierEmployeeId, 
            int modelPortfolioId, string accountNumber, string accountName, bool showActive, 
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus, 
            ManagementTypes managementType,ManagementPeriodUnitTradeStatus tradeStatus, 
            bool includeStornoedUnits, int[] mgtPeriodIds, string sortColumn, 
            bool ascending, bool keysOnly)
        {
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable();
            string where = GetUnitFeeOverviewWhereClause(assetManagerId, remisierId, remisierEmployeeId,
                modelPortfolioId, accountNumber, accountName, showActive, showInactive, year, quarter,
                continuationStatus, managementType, tradeStatus, includeStornoedUnits, parameters, !keysOnly);

            if (mgtPeriodIds != null)
                where += string.Format(" and P.Key IN ({0})",
                    (mgtPeriodIds.Length == 0 ? "0" : string.Join(", ", Array.ConvertAll<int, string>(mgtPeriodIds, id => id.ToString()))));

            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
            parameters.Add("managementType", managementType);

            if (keysOnly)
            {
                string orderBy = "order by A.Number", contactsJoin = "";
                bool sortOnRemisier = false;
                if (sortColumn != "")
                {
                    string ascendingStr = (ascending ? "ASC" : "DESC");
                    sortColumn = sortColumn.ToUpper();

                    string sortProperty = "";
                    switch (sortColumn)
                    {
                        case "KEY":
                            sortProperty = "P.Key";
                            break;
                        case "ACCOUNT_NUMBER":
                            sortProperty = "A.Number";
                            break;
                        case "ACCOUNT_SHORTNAME":
                            sortProperty = "CN.Name";
                            contactsJoin = @"left join A.bagOfAccountHolders AH
                                         left join AH.Contact C
                                         left join C.CurrentNAW CN";
                            where += " and AH.IsPrimaryAccountHolder = true";
                            break;
                        case "MANAGEMENTENDDATE":
                            sortProperty = "P.endDate";
                            break;
                        case "TRADEID":
                            sortProperty = "T.Key";
                            break;
                        case "ACCOUNT_REMISIEREMPLOYEE_REMISIER_NAME":
                            sortProperty = "R.Name";
                            sortOnRemisier = true;
                            break;
                        case "ACCOUNT_REMISIEREMPLOYEE_EMPLOYEE_FULLNAME":
                            sortProperty = "E.Employee.LastName";
                            sortOnRemisier = true;
                            break;
                    }

                    if (sortProperty != "")
                        orderBy = string.Format("order by {0} {1}", sortProperty, ascendingStr);
                }

                string hql = string.Format(@"select distinct P.Key, P.endDate, A.Number, A.Status, T.Key{0}{5}
                from ManagementPeriodUnit U 
                left join U.ManagementPeriod P
                left join U.UnitParent PU
                left join U.ManagementFee T
                left join PU.Account A 
                {1}
                {2}
                {6}
                where (A.Family.managementTypesCharged & :managementType) <> 0
                and PU.Period in (:periods) {3}
                and P.ManagementType = :managementType {4}",
                        (contactsJoin != "" ? ", CN.Name" : ""),
                        contactsJoin,
                        modelPortfolioId > 0 ? "left join A.ModelPortfolio M " : "",
                        where, orderBy,
                        (sortOnRemisier ? ", R.Name, E.Employee.LastName" : ""),
                        (sortOnRemisier ? "left join A.RemisierEmployee E left join E.Remisier R" : ""));
                return session.GetListByHQL(hql, parameters, parameterLists);
            }
            else
            {
                string queryName;
                if (managementType == ManagementTypes.KickBack)
                    queryName = "B4F.TotalGiro.ApplicationLayer.Fee.ManagementPeriodUnitsForMgtFee";
                else
                    queryName = "B4F.TotalGiro.ApplicationLayer.Fee.ManagementPeriodUnitsForKickBack";

                IList<IManagementPeriodUnit> units = session.GetTypedListByNamedQuery<IManagementPeriodUnit>(queryName, where, parameters, parameterLists);
                if (units != null && units.Count > 0)
                {
                    IList avgHldFees = ManagementPeriodUnitMapper.GetManagementFees(session, mgtPeriodIds, ManagementFeeLocations.AverageHolding, year, quarter, managementType);
                    IList unitFees = ManagementPeriodUnitMapper.GetManagementFees(session, mgtPeriodIds, ManagementFeeLocations.Unit, year, quarter, managementType);
                    ICurrency currency = InstrumentMapper.GetBaseCurrency(session);

                    var fees = from a in avgHldFees.Cast<object[]>().Union(unitFees.Cast<object[]>())
                               select new { MgtPeriodID = (int)a[0], Period = (int)a[1], FeeType = (FeeTypes)a[2], FeeAmount = (decimal)a[3] };

                    var stuff = from u in units
                                join f in fees on new { MgtPeriodID = u.ManagementPeriod.Key, u.Period } equals new { f.MgtPeriodID, f.Period } into x
                                from f in x.DefaultIfEmpty()
                                group new { u, fp = (f != null ? new { Period = (int)f.Period, FeeAmount = new Money((decimal)f.FeeAmount, currency) } : null) } by u.ManagementPeriod
                                    into g
                                    select new { ManagementPeriod = g.Key, Units = (from gi in g select gi.u).ToList(), FeeAmounts = (from gj in g select gj.fp).ToList() };

                    List<UnitFeeOverview> list = new List<UnitFeeOverview>();
                    UnitFeeOverview.Year = year.ToString();
                    UnitFeeOverview.Quarter = string.Format("Q{0}", quarter);
                    UnitFeeOverview.Periods = Util.GetPeriodsFromQuarter(year, quarter);

                    //stuff.ToList().ForEach(p => list.Add(new UnitFeeOverview(p.Account, p.Units)));
                    foreach (var pair in stuff)
                    {
                        UnitFeeOverview item = new UnitFeeOverview(pair.ManagementPeriod, pair.Units);
                        foreach (var fee in pair.FeeAmounts)
                        {
                            if (fee != null)
                                item.AddFeeDetails(fee.Period, fee.FeeAmount);
                        }
                        list.Add(item);
                    }
                    return list;
                }
            }
            return null;
        }

        #endregion

        #region Summary

        public static DataSet GetUnitFeeOverviewSummary(
            int assetManagerId, int remisierId, int remisierEmployeeId,
            int modelPortfolioId, string accountNumber, string accountName, bool showActive,
            bool showInactive, int year, int quarter, AccountContinuationStati continuationStatus,
            ManagementTypes managementType, ManagementPeriodUnitTradeStatus tradeStatus,
            bool includeStornoedUnits)
        {
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable();
            string where = GetUnitFeeOverviewWhereClause(assetManagerId, remisierId, remisierEmployeeId,
                modelPortfolioId, accountNumber, accountName, showActive, showInactive, year, quarter,
                continuationStatus, managementType, tradeStatus, includeStornoedUnits, parameters, true) +
                "\ngroup by PU.Period";

            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
            parameters.Add("managementType", managementType);

            IDalSession session = NHSessionFactory.CreateSession();
            IList summary = session.GetListByNamedQuery("B4F.TotalGiro.ApplicationLayer.Fee.ManagementPeriodUnitsForSummary", where, parameters, parameterLists);

            IList avgHldFees = ManagementPeriodUnitMapper.GetManagementFeeSummary(
                session, ManagementFeeLocations.AverageHolding, assetManagerId, remisierId, remisierEmployeeId,
                modelPortfolioId, accountNumber, accountName, showActive, showInactive, year, quarter,
                continuationStatus, managementType, tradeStatus, includeStornoedUnits);
            IList unitFees = ManagementPeriodUnitMapper.GetManagementFeeSummary(
                session, ManagementFeeLocations.Unit, assetManagerId, remisierId, remisierEmployeeId,
                modelPortfolioId, accountNumber, accountName, showActive, showInactive, year, quarter,
                continuationStatus, managementType, tradeStatus, includeStornoedUnits);

            ICurrency currency = InstrumentMapper.GetBaseCurrency(session);

            if (managementType == ManagementTypes.ManagementFee)
            {
                var fees = from a in avgHldFees.Cast<object[]>().Union(unitFees.Cast<object[]>())
                           group a by (int)a[0] into g
                           select new
                           {
                               Period = g.Key,
                               TotalFeeAmount = g.Sum(a => (decimal)a[2]),
                               TotalFixedFeeAmount = g.Where(a => (FeeTypes)a[1] == FeeTypes.FixedFee).Sum(a => (decimal)a[2]),
                               TotalOtherFeeAmount = g.Where(a => (FeeTypes)a[1] != FeeTypes.FixedFee).Sum(a => (decimal)a[2])
                           };

                //select new { Period = (int)a[0], TotalFeeAmount = (decimal)a[1] };

                var sum = from a in summary.Cast<object[]>()
                          select new { Period = (int)a[0], TotalValue = (decimal)a[1] };

                var stuff = from s in sum
                            join f in fees on s.Period equals f.Period into x
                            from f in x.DefaultIfEmpty()
                            group new
                            {
                                s,
                                v = (decimal)s.TotalValue,
                                fp = (f != null ? (decimal)f.TotalFeeAmount : 0M),
                                ff = (f != null ? (decimal)f.TotalFixedFeeAmount : 0M),
                                fo = (f != null ? (decimal)f.TotalOtherFeeAmount : 0M)
                            } by s.Period
                                into g
                                select new
                                {
                                    Period = g.Key,
                                    TotalValue = new Money((decimal)g.Sum(s => s.v), currency),
                                    TotalFee = new Money((decimal)g.Sum(f => f.fp), currency),
                                    TotalFixedFee = new Money((decimal)g.Sum(f => f.ff), currency),
                                    TotalOtherFee = new Money((decimal)g.Sum(f => f.fo), currency)
                                };

                //"Period, TotalValue.DisplayString, TotalFee.DisplayString");
                return stuff
                        .Select(c => new
                        {
                            c.Period,
                            TotalValue = c.TotalValue != null ? c.TotalValue.DisplayString : "",
                            TotalFee = c.TotalFee != null ? c.TotalFee.DisplayString : "",
                            FixedFee = c.TotalFixedFee != null ? c.TotalFixedFee.DisplayString : "",
                            OtherFee = c.TotalOtherFee != null ? c.TotalOtherFee.DisplayString : ""
                        })
                        .ToDataSet();
            }
            else
            {
                var fees = from a in avgHldFees.Cast<object[]>().Union(unitFees.Cast<object[]>())
                           group a by (int)a[0] into g
                           select new
                           {
                               Period = g.Key,
                               TotalFeeAmount = g.Sum(a => (decimal)a[2])
                           };

                //select new { Period = (int)a[0], TotalFeeAmount = (decimal)a[1] };

                var sum = from a in summary.Cast<object[]>()
                          select new { Period = (int)a[0], TotalValue = (decimal)a[1] };

                var stuff = from s in sum
                            join f in fees on s.Period equals f.Period into x
                            from f in x.DefaultIfEmpty()
                            group new
                            {
                                s,
                                v = (decimal)s.TotalValue,
                                fp = (f != null ? (decimal)f.TotalFeeAmount : 0M)
                            } by s.Period
                                into g
                                select new
                                {
                                    Period = g.Key,
                                    TotalValue = new Money((decimal)g.Sum(s => s.v), currency),
                                    TotalFee = new Money((decimal)g.Sum(f => f.fp), currency)
                                };

                //"Period, TotalValue.DisplayString, TotalFee.DisplayString");
                return stuff
                        .Select(c => new
                        {
                            c.Period,
                            TotalValue = c.TotalValue != null ? c.TotalValue.DisplayString : "",
                            TotalFee = c.TotalFee != null ? c.TotalFee.DisplayString : ""
                        })
                        .ToDataSet();
            }
        }

        #endregion

        #region Other Data

        public static DataSet GetManagementFeeTransactionData(string tradeIds)
        {
            int[] keys = null;
            if (!string.IsNullOrEmpty(tradeIds))
                keys = (from a in tradeIds.Split(',')
                              select Convert.ToInt32(a)).ToArray();

            //"Key, TransactionDate, ValueSize.DisplayString, Tax.DisplayString, StornoTransaction.Key, Description, CreationDate"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return GeneralOperationsBookingMapper.GetBookings(session, keys)
                        .Cast<IManagementFee>()
                        .Select(c => new
                        {
                            c.Key,
                            TransactionDate =
                                c.GeneralOpsJournalEntry.TransactionDate,
                            NettAmount =
                                c.NettAmount,
                            TaxAmount =
                                c.TaxAmount,
                            IsStornoed =
                                (c.StornoBooking != null),
                            StornoBookingKey =
                                (c.StornoBooking != null ? c.StornoBooking.Key : 0),
                            c.Description,
                            c.CreationDate
                          })
                          .ToDataSet();
            }
        }

        public static DataSet GetManagementPeriodUnits(int managementPeriodID, int year, int quarter, ManagementTypes managementType)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                ManagementPeriodUnitMapper.GetManagementPeriodUnits(session, new int[] { managementPeriodID }, year, quarter, managementType, ManagementPeriodUnitTradeStatus.All, true).ToList(),
                "Key, UnitParent.Period, UnitParent.StartDate, UnitParent.EndDate, UnitParent.TotalValue.DisplayString, ModelPortfolio.ModelName, IsStornoed, FeesCalculated, RulesFound, DocumentsSentByPost, Success, IsEditable, Message");

            int[] periods = new int[] { managementPeriodID };

            IList avgHldFees = ManagementPeriodUnitMapper.GetManagementFees(session, periods, ManagementFeeLocations.AverageHolding, year, quarter, managementType);
            IList unitFees = ManagementPeriodUnitMapper.GetManagementFees(session, periods, ManagementFeeLocations.Unit, year, quarter, managementType);
            ICurrency currency = InstrumentMapper.GetBaseCurrency(session);

            var fees = from a in avgHldFees.Cast<object[]>().Union(unitFees.Cast<object[]>())
                       select new { Period = (int)a[1], FeeType = (FeeTypes)a[2], FeeAmount = new Money((decimal)a[3], currency) };

            var usedFeeTypes = (from a in fees
                                group a by a.FeeType into b
                                orderby b.Key
                                select (int)b.Key).ToArray();
            
            int[] feeTypesToShow;
            if (managementType == ManagementTypes.ManagementFee)   
                feeTypesToShow = usedFeeTypes.Union( new int[] { (int)FeeTypes.ManagementFee, (int)FeeTypes.FixedFee }).ToArray();
            else
                feeTypesToShow = usedFeeTypes.Union( new int[] { (int)FeeTypes.KickbackFee }).ToArray();

            int colCount = ds.Tables[0].Columns.Count;
            string feeTypesInDataset = string.Join(",", feeTypesToShow.Select(a => a.ToString()).ToArray());
            
            for (int i = 1; i < 5; i++)
        	    ds.Tables[0].Columns.Add(string.Format("Fee_{0}", i), typeof(string));  
    	    ds.Tables[0].Columns.Add("UsedFeeTypes", typeof(string));

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["UnitParent_Period"] != System.DBNull.Value)
                {
                    for (int i = 0; i < feeTypesToShow.Count(); i++)
                    {
                        int period = Convert.ToInt32(row["UnitParent_Period"]);
                        int feeType = feeTypesToShow[i];
                        var relevantFees = from a in fees 
                                           where a.Period == period && (int)a.FeeType == feeType
                                           select a;
                        if (relevantFees != null && relevantFees.Count() == 1)
                            row[colCount + i] = relevantFees.ElementAt(0).FeeAmount.DisplayString;
	                }
                    row[colCount + 4] = feeTypesInDataset;
                }
            }

            session.Close();

            return ds;
        }

        public static DataSet GetManagementFeeDetails(int managementPeriodID, int year, int quarter, ManagementTypes managementType)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            int[] periods = new int[] { managementPeriodID };

            IList avgHldFees = ManagementPeriodUnitMapper.GetManagementFees(session, periods, ManagementFeeLocations.AverageHolding, year, quarter, managementType);
            IList unitFees = ManagementPeriodUnitMapper.GetManagementFees(session, periods, ManagementFeeLocations.Unit, year, quarter, managementType);
            ICurrency currency = InstrumentMapper.GetBaseCurrency(session);

            var fees = from a in avgHldFees.Cast<object[]>().Union(unitFees.Cast<object[]>())
                       select new { Key = string.Format("{0}_{1}_{2}", (int)a[0], (int)a[1], (int)a[2]), SortKey = string.Format("{0}_{1}", (int)a[1], (int)a[2]), Period = (int)a[1], FeeType = (FeeTypes)a[2], FeeAmount = new Money((decimal)a[3], currency) };

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                fees.OrderBy(a => a.SortKey).ToList(),
                "Key, Period, FeeType, FeeAmount.DisplayString");


            session.Close();

            return ds;
        }

        public static DataSet GetAverageHoldingFees(int unitID, ManagementTypes managementType)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            Hashtable parameters = new Hashtable(2);
            parameters.Add("unitID", unitID);
            parameters.Add("managementType", managementType);

            IList<IAverageHolding> holdings = session.GetTypedListByNamedQuery<IAverageHolding>("B4F.TotalGiro.ApplicationLayer.Fee.AverageHoldingsByUnit", parameters);
            if (holdings != null && holdings.Count > 0)
            {
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    holdings.ToList(),
                    "Key, Period, BeginDate, EndDate, Instrument.Name, AverageValue.DisplayString, SkipFees, CreationDate, DisplayMessage");

                parameters.Remove("managementType");
                IList<IAverageHoldingFee> fees = session.GetTypedListByNamedQuery<IAverageHoldingFee>("B4F.TotalGiro.ApplicationLayer.Fee.AverageHoldingFeesByUnit", parameters);

                var usedFeeTypes = (from a in fees
                                    group a by a.FeeType.Key into b
                                    orderby b.Key
                                    select (int)b.Key).ToArray();

                int[] feeTypesToShow;
                if (managementType == ManagementTypes.ManagementFee)
                    feeTypesToShow = usedFeeTypes.Union(new int[] { (int)FeeTypes.ManagementFee }).ToArray();
                else
                    feeTypesToShow = usedFeeTypes.Union(new int[] { (int)FeeTypes.KickbackFee }).ToArray();
                string feeTypesInDataset = string.Join(",", feeTypesToShow.Select(a => a.ToString()).ToArray());

                int colCount = ds.Tables[0].Columns.Count;
                for (int i = 1; i < 4; i++)
                    ds.Tables[0].Columns.Add(string.Format("Fee_{0}", i), typeof(string));
                ds.Tables[0].Columns.Add("UsedFeeTypes", typeof(string));

                foreach (DataRow row in ds.Tables[0].Rows)
                    row[colCount + 3] = feeTypesInDataset;

                if (fees != null && fees.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["Key"] != System.DBNull.Value)
                        {
                            for (int i = 0; i < feeTypesToShow.Count(); i++)
                            {
                                int avhHldID = Convert.ToInt32(row["Key"]);
                                int feeType = feeTypesToShow[i];
                                var relevantFees = from a in fees
                                                   where a.Parent.Key == avhHldID && (int)a.FeeType.Key == feeType
                                                   select a;
                                if (relevantFees != null && relevantFees.Count() > 0)
                                {
                                    IAverageHoldingFee fi = relevantFees.ElementAt(0);
                                    if (fi != null)
                                    {
                                        if (managementType == ManagementTypes.ManagementFee)
                                        {
                                            row[colCount + i] = string.Format("{0}{1}", (fi.FeeCalcSource == null ? "" : fi.FeeCalcSource.DisplayString + " -> "), fi.Amount.DisplayString);
                                            if (!string.IsNullOrEmpty(fi.DisplayMessage))
                                                row[colCount - 1] += fi.DisplayMessage;
                                        }
                                        else
                                        {
                                            row[colCount + i] = string.Format("{0}{1}", (fi.FeePercentageUsed > 0M ? fi.FeePercentageUsed.ToString("0.####") + "% -> " : ""), fi.Amount.DisplayString);
                                            if (!string.IsNullOrEmpty(fi.DisplayMessage))
                                                row[colCount - 1] += fi.DisplayMessage;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ds;
        }

        #endregion

        #region Methods

        public static bool RecalculateFees(int unitID)
        {
            bool success = false;
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementPeriodUnit unit = ManagementPeriodUnitMapper.GetManagementPeriodUnit(session, unitID);
            if (unit != null)
            {
                switch (unit.ManagementPeriod.ManagementType)
                {
                    case B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.ManagementFee:
                        FeeFactory feeFactory = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Fee, true);
                        unit.Success = feeFactory.CalculateFeePerUnit(session, unit);
                        break;
                    case B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.KickBack:
                        unit.Success = FeeFactory.CalculateKickBackOnUnit(session, unit);
                        break;
                }

                if (unit.Success)
                    success = session.Update(unit);
            }
            session.Close();
            return success;
        }

        public static bool CreateMgtFeeTransactions(BatchExecutionResults results, int[] mgtPeriodIds, int year, int quarter, ManagementTypes managementType, bool createCashInitiatedOrders)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DateTime minDate, maxDate;
            Util.GetDatesFromQuarter(year, quarter, out minDate, out maxDate);
            int mgtPeriodId = 0;
            int journalID = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultManagementFeeJournal")));
            IJournal journalMgmtFee = JournalMapper.GetJournal(session, journalID);
            IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.ManagementFee);

            if (mgtPeriodIds != null && mgtPeriodIds.Length > 0)
            {
                FeeFactory feeFactory = FeeFactory.GetInstance(session, FeeFactoryInstanceTypes.Fee, true);
                decimal taxPercentage = feeFactory.GetHistoricalTaxRate(maxDate).StandardRate;

                for (int i = 0; i < mgtPeriodIds.Length; i++)
                {
                    try
                    {
                        mgtPeriodId = mgtPeriodIds[i];
                        if (createMgtFeeTransactionForMP(journalMgmtFee, lookups, mgtPeriodId, year, quarter, managementType, feeFactory, minDate, maxDate, taxPercentage, createCashInitiatedOrders))
                            results.MarkSuccess();
                    }
                    catch (Exception ex)
                    {
                        results.MarkError(
                            new ApplicationException(string.Format("Error creating management fee transaction for management period {0}.", mgtPeriodId), ex));
                    }
                }
            }
            session.Close();
            return true;
        }

        private static bool createMgtFeeTransactionForMP(IJournal journalMgmtFee, IGLLookupRecords lookups, int mgtPeriodId, int year, int quarter, ManagementTypes managementType, FeeFactory feeFactory, DateTime minDate, DateTime maxDate, decimal taxPercentage, bool createCashInitiatedOrders)
        {
            bool retVal = false;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable(2);

            Util.GetDatesFromQuarter(year, quarter, out minDate, out maxDate);
                
            parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
            parameters.Add("mgtPeriodId", mgtPeriodId);
            parameters.Add("managementType", managementType);
            IList<IManagementPeriodUnit> unitsUnsorted = session.GetTypedListByNamedQuery<IManagementPeriodUnit>(
                "B4F.TotalGiro.ApplicationLayer.Fee.ManagementPeriodUnitsForMgtFeeTransactionCreation", 
                parameters, 
                parameterLists);

            if (unitsUnsorted != null && unitsUnsorted.Count > 0)
            {
                var unitsByMP = from a in unitsUnsorted
                          group a by a.ManagementPeriod into g
                          select new { ManagementPeriod = g.Key, Units = g.ToList() };

                if (unitsByMP.Count() != 1)
                    throw new ApplicationException("The number of management periods is not what is expected.");
                
                IManagementPeriod managementPeriod = unitsByMP.First().ManagementPeriod;
                IList<IManagementPeriodUnit> units = unitsByMP.First().Units;

                DateTime feeStartDate = minDate;
                DateTime feeEndDate = maxDate;

                if (managementPeriod.StartDate > minDate)
                    feeStartDate = managementPeriod.StartDate;
                if (Util.IsNotNullDate(managementPeriod.EndDate) && managementPeriod.EndDate < maxDate)
                    feeEndDate = managementPeriod.EndDate.Value;

                if (feeStartDate.Year != feeEndDate.Year)
                    throw new ApplicationException("The year of the start date and end date for the management fee should equal");

                string mgtFeeDescription;
                decimal mgtFeeThreshold;
                getMgtFeeInfo(managementPeriod, feeEndDate, year, quarter, out mgtFeeDescription, out mgtFeeThreshold);

                if (units != null && units.Count > 0)
                {
                    // check the number of units
                    int expectedUnitCount = Util.DateDiff(DateInterval.Month, feeStartDate, feeEndDate) + 1;
                    if (expectedUnitCount != units.Count)
                        throw new ApplicationException(string.Format("The number of units {0} does not equal the number of expected units {1}.", units.Count, expectedUnitCount));

                    // check if all have the same management period
                    var mps = from a in units
                              group a by a.ManagementPeriod into g
                              select g.Key;
                    if (mps.Count() != 1)
                        throw new ApplicationException("Not all units belong to the same management period.");

                    if (mps.First().Key != managementPeriod.Key)
                        throw new ApplicationException("The management period is not ok.");

                    if (Util.GetPeriodFromDate(managementPeriod.StartDate) == Util.GetPeriodFromDate(feeStartDate) && managementPeriod.StartDate.Day != feeStartDate.Day)
                        throw new ApplicationException(string.Format("The start date of the management period ({0}) does not equal the presented start date ({1}).", managementPeriod.StartDate.ToString("yyyy-MM-dd"), feeStartDate.ToString("yyyy-MM-dd")));

                    if (managementPeriod.EndDate.HasValue)
                    {
                        if (feeEndDate > managementPeriod.EndDate)
                            throw new ApplicationException(string.Format("The end date of the management period ({0}) does not equal the presented end date ({1}).", managementPeriod.EndDate.Value.ToString("yyyy-MM-dd"), feeEndDate.ToString("yyyy-MM-dd")));
                        else if (Util.GetPeriodFromDate(managementPeriod.EndDate.Value) == Util.GetPeriodFromDate(feeEndDate) && managementPeriod.EndDate.Value.Day != feeEndDate.Day)
                            throw new ApplicationException(string.Format("The end date of the management period ({0}) does not equal the presented end date ({1}).", managementPeriod.EndDate.Value.ToString("yyyy-MM-dd"), feeEndDate.ToString("yyyy-MM-dd")));
                    }

                    IAccountTypeCustomer account = (IAccountTypeCustomer)managementPeriod.Account;
                    if (!account.ExitFeePayingAccount.Equals(account))
                        mgtFeeDescription += string.Format(" voor {0}", account.Number);

                    string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journalMgmtFee);
                    IMemorialBooking memorialBooking = new MemorialBooking(journalMgmtFee, nextJournalEntryNumber);
                    memorialBooking.TransactionDate = feeEndDate.AddDays(1);
                    memorialBooking.Description = mgtFeeDescription;

                    IManagementFee mgtFee = new ManagementFee(account, feeStartDate, feeEndDate, units, memorialBooking, taxPercentage, lookups);
                    if (mgtFee != null && mgtFee.NettAmount != null && mgtFee.NettAmount.IsNotZero)
                    {
                        if (mgtFee.NettAmount.Quantity < mgtFeeThreshold)
                        {
                            ITradeableInstrument instrument;
                            account = account.ExitFeePayingAccount;
                            if (createCashInitiatedOrders && mgtFee.NeedToCreateCashInitiatedOrder(out instrument))
                            {
                                if (instrument != null)
                                {
                                    OrderAmountBased order = new OrderAmountBased(account, mgtFee.Components.TotalAmount, instrument, true, feeFactory, true);
                                    order.OrderInfo = mgtFeeDescription;
                                    mgtFee.CashInitiatedOrder = order;
                                }
                                else
                                {
                                    // Sell from the biggest position
                                    IFundPosition pos = account.Portfolio.PortfolioInstrument.Where(x => x.Size.IsGreaterThanZero).OrderByDescending(x => x.CurrentValue).FirstOrDefault();
                                    if (pos != null && (pos.CurrentBaseValue + mgtFee.NettAmount).IsGreaterThanOrEqualToZero)
                                    {
                                        OrderAmountBased order = new OrderAmountBased(account, mgtFee.Components.TotalAmount, pos.Instrument, true, feeFactory, true);
                                        order.OrderInfo = mgtFeeDescription;
                                        mgtFee.CashInitiatedOrder = order;
                                    }
                                }
                            }

                            if (mgtFee.CashInitiatedOrder == null && (account.Portfolio.TotalValue() + mgtFee.TotalAmount).IsLessThanZero)
                                throw new ApplicationException(string.Format("Could not create a management fee booking for account {0} since the total portfolio value ({1}) is insufficient.", account.DisplayNumberWithName, account.Portfolio.TotalValue().DisplayString));

                            mgtFee.BookLines();
                            GeneralOperationsBookingMapper.Update(session, mgtFee);
                            retVal = true;
                        }
                    }
                }
            }
            session.Close();
            return retVal;
        }

        private static bool getMgtFeeInfo(IManagementPeriod mp, DateTime feeEndDate, int year, int quarter, out string mgtFeeDescription, out decimal mgtFeeThreshold)
        {
            bool success = false;
            mgtFeeDescription = "";
            mgtFeeThreshold = 0;
            ICustomerAccount account = (ICustomerAccount)mp.Account;
            IAssetManager am = account.AccountOwner as IAssetManager;
            if (am != null)
            {
                if (account.FinalManagementEndDate == feeEndDate)
                    mgtFeeDescription = string.Format(am.MgtFeeFinalDescription, quarter, year);
                else
                    mgtFeeDescription = string.Format(am.MgtFeeDescription, quarter, year);

                mgtFeeThreshold = am.MgtFeeThreshold * -1M;
                success = true;
            }
            return success;
        }

        #endregion

        #region Display Results

        public static string FormatErrorsForCreateMgtFeeTransactions(BatchExecutionResults results)
        {
            const int MAX_ERRORS_DISPLAYED = 25;

            string message = "<br/>";

            if (results.SuccessCount == 0 && results.ErrorCount == 0)
                message += "No new management fee transactions need to be created";
            else
            {
                if (results.SuccessCount > 0)
                    message += string.Format("{0} management fee transactions were successfully created.<br/><br/><br/>", results.SuccessCount);

                if (results.ErrorCount > 0)
                {
                    string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                        string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                    message += string.Format("{0} errors occured while creating management fee transactions{1}:<br/><br/><br/>",
                                             results.ErrorCount, tooManyErrorsMessage);

                    int errors = 0;
                    foreach (Exception ex in results.Errors)
                    {
                        if (++errors > MAX_ERRORS_DISPLAYED)
                            break;
                        message += Utility.GetCompleteExceptionMessage(ex) + "<br/>";
                    }
                }
            }

            return message;
        }

        #endregion

        #region Export KickBack Data

        public static int ExportKickBackReportData(int assetManagerId, int year, int quarter)
        {
            
            int exportCount = 0;
            DateTime beginDate, endDate;
            Util.GetDatesFromQuarter(year, quarter, out beginDate, out endDate);
            IDalSession session = NHSessionFactory.CreateSession();

            if (session.Session.GetNamedQuery("B4F.TotalGiro.ApplicationLayer.Fee.ValuationRunsAfterDate").SetDateTime("date", endDate).UniqueResult<int>() == 0)
                throw new ApplicationException(string.Format("It is not possible to export the kickback data since the Valuation Job did not pass {0}.", endDate.ToString("dd-MM-yyyy")));
            
            IList list = GetUnitFeeOverviewList(
                session, assetManagerId, 0, 0, 0, "", "", true, true, year, quarter,
                AccountContinuationStati.All, ManagementTypes.KickBack,
                ManagementPeriodUnitTradeStatus.All, false, null, "", false, false);

            //List<IKickBackExport> exportList = (from a in list.Cast<UnitFeeOverview>()
            //              where a.KickBackReportData != null && a.KickBackReportData.IsValid
            //              select a.KickBackReportData).ToList();

            List<IKickBackExport> exportWithKickback = list.Cast<UnitFeeOverview>().Where(u => u.IsValid).SelectMany(u => u.Units).Select(u => u.Value.KickBackReportData).ToList();
            // Remove doubles
            var doubles = (from a in exportWithKickback
                    group a by new { Account = a.Account, Period = a.Period } into g
                    select new { AccountPeriod = g.Key, Items = g.ToArray() }).Where(u => u.Items.Count() > 1);
            if (doubles.Count() > 0)
            {
                foreach (var pair in doubles)
                {
                    KickBackExport unDoubledItem = new KickBackExport(pair.AccountPeriod.Account, pair.AccountPeriod.Period, pair.Items);
                    foreach (IKickBackExport item in pair.Items)
                    {
                        exportWithKickback.Remove(item);
                    }
                    exportWithKickback.Add(unDoubledItem);
                }
            }
            // Add accounts without kickback but with a remisier attached
            List<IKickBackExport> exportNoKickback = getExportAccountDataNoKickBack(assetManagerId, year, quarter, beginDate, endDate);

            List<IKickBackExport> exportList = null;
            if (exportWithKickback != null && exportNoKickback != null)
                exportList = exportWithKickback.Union(exportNoKickback).ToList();
            else if (exportWithKickback != null)
                exportList = exportWithKickback;
            else if (exportNoKickback != null)
                exportList = exportNoKickback;

            if (exportList != null && exportList.Count > 0)
            {
                exportCount = exportList.Count;
                session.Insert(exportList);
            }

            session.Close();
            return exportCount;
        }

        private static List<IKickBackExport> getExportAccountDataNoKickBack(int assetManagerId, int year, int quarter, DateTime beginDate, DateTime endDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                List<IKickBackExport> exportList = null;
                session.TimeOut = 0;

                Hashtable parameters = new Hashtable(4);
                parameters.Add("managementType", (int)ManagementTypes.KickBack);
                parameters.Add("beginDate", beginDate);
                parameters.Add("endDate", endDate);
                parameters.Add("assetManagerId", assetManagerId);

                Hashtable parameterLists = new Hashtable(1);
                parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));

                IList<ICustomerAccount> accounts = session.GetTypedListByNamedQuery<ICustomerAccount>(
                    "B4F.TotalGiro.ApplicationLayer.Fee.AccountsForExportAccountDataNoKickBack",
                    parameters,
                    parameterLists);

                if (accounts != null && accounts.Count > 0)
                {
                    exportList = new List<IKickBackExport>();

                    foreach (ICustomerAccount account in accounts)
                    {
                        parameters = new Hashtable(3);
                        parameters.Add("accountId", account.Key);
                        parameters.Add("beginDate", (account.ManagementStartDate > beginDate ? account.ManagementStartDate : beginDate));
                        parameters.Add("endDate", (Util.IsNotNullDate(account.ManagementEndDate) && account.ManagementEndDate < endDate ? account.ManagementEndDate : endDate));
                        IList values = session.GetListByNamedQuery(
                            "B4F.TotalGiro.ApplicationLayer.Fee.AvgValuesForExportAccountDataNoKickBack",
                            parameters);

                        var av = from a in values.Cast<object[]>()
                                 select new { AccountID = (int)a[0], TotalValue = (decimal)a[1], Period = ((int)a[2] * 100) + (int)a[3] };

                        IEnumerable<IKickBackExport> range = (
                            from a in accounts
                            join v in av on a.Key equals v.AccountID
                            select new KickBackExport(a, v.Period, v.TotalValue)
                                ).Select(c => (IKickBackExport)c);

                        exportList.AddRange(range);
                    }
                }
                return exportList;
            }
        }

        #endregion

    }
}
