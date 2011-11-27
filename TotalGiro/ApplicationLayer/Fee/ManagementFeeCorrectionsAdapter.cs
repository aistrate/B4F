using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.ManagementPeriodUnits.Corrections;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Fee
{
    [Flags()]
    public enum CorrectionOpenCloseStati
    {
        All = -1,
        Open = 0,
        Closed = 1
    }

    public static class ManagementFeeCorrectionsAdapter
    {
        public static DataSet GetManagementFeeCorrections(
            int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
            bool showActive, bool showInactive, int year, int quarter,
            CorrectionOpenCloseStati openCloseStatus, ManagementTypes managementType)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            string where = "";
            Hashtable parameterLists = new Hashtable(1);
            Hashtable parameters = new Hashtable();

            if (assetManagerId > 0)
                parameters.Add("assetManagerId", assetManagerId);
            if (modelPortfolioId > 0)
                parameters.Add("modelPortfolioId", modelPortfolioId);
            if (accountNumber != null && accountNumber.Length > 0)
                parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber));
            if (accountName != null && accountName.Length > 0)
                parameters.Add("accountName", Util.PrepareNamedParameterWithWildcard(accountName));
            if (showActive && !showInactive)
                parameters.Add("accountStatus", (int)AccountStati.Active);
            if (!showActive && showInactive)
                parameters.Add("accountStatus", (int)AccountStati.Inactive);
            
            // TODO -> possibility to fix the difference with extra journalentrylines
            if (openCloseStatus != CorrectionOpenCloseStati.All)
            {
                if (openCloseStatus == CorrectionOpenCloseStati.Open)
                    where += " and (IsNull(C.Skip, 0) = 0)";
                else
                    where += " and (IsNull(C.Skip, 0) = 1)";
            }

            if (year != 0 || quarter != 0)
            {
                if (year != 0 && quarter != 0)
                    parameterLists.Add("periods", Util.GetPeriodsFromQuarter(year, quarter));
                else if (year != 0)
                    parameterLists.Add("periods", Util.GetPeriodsFromYear(year));
                else if (quarter != 0)
                    parameterLists.Add("periods", Util.GetPeriodsFromQuarter(DateTime.Today.Year, quarter));
            }
            parameters.Add("managementType", managementType);

            List<ManagementPeriodUnitCorrection> list = session.GetTypedListByNamedQuery<ManagementPeriodUnitCorrection>(
                "B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeCorrections",
                where,
                parameters, 
                parameterLists);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                list,
                "AverageHolding.Key, AverageHolding.Account.Key, AverageHolding.Account.Number, AverageHolding.Account.ShortName, AverageHolding.Account.Status, Unit.UnitParent.Account.ModelPortfolio.ModelName, AverageHolding.Period, AverageHolding.BeginDate, AverageHolding.EndDate, AverageHolding.Instrument.Name, AverageHolding.AverageValue.DisplayString, AverageHolding.PreviousHolding.AverageValue.DisplayString, Skip, IsOpen");

            session.Close();
            return ds;
        }

        public static DataSet GetManagementFeeTransactionData(int tradeId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            //if (tradeId != 0)
            //{
            //    // TODO
            //    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
            //        ObsoleteTransactionMapper.GetTransactions(session, new int[] { tradeId }),
            //        "Key, TransactionDate, ValueSize.DisplayString, Tax.DisplayString, StornoTransaction.Key, Description, CreationDate");

            //    if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    {
            //        ds.Tables[0].Columns.Add("SettleDifference", typeof(string));

            //        Hashtable parameters = new Hashtable(2);
            //        parameters.Add("tradeId", tradeId);
            //        parameters.Add("feeType", FeeTypes.SettleDifference);
            //        IList<MgtFeeBreakupLine> list = session.GetTypedListByNamedQuery<MgtFeeBreakupLine>(
            //            "B4F.TotalGiro.ApplicationLayer.Fee.ManagementFeeTransactionData",
            //            parameters);
            //        if (list != null && list.Count > 0)
            //            ds.Tables[0].Rows[0]["SettleDifference"] = list[0].Amount.DisplayString;
            //    }

            //}
            session.Close();
            return ds;
        }

        public static DataSet GetAverageHoldingFees(int averageHoldingID, ManagementTypes managementType)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            if (averageHoldingID != 0)
            {
                Hashtable parameters = new Hashtable(4);
                parameters.Add("averageHoldingID", averageHoldingID);
                parameters.Add("managementType", managementType);
                IList<IAverageHoldingFee> feeItems = session.GetTypedListByNamedQuery < IAverageHoldingFee>(
                    "B4F.TotalGiro.ApplicationLayer.Fee.AverageHoldingFees", 
                    parameters);
                if (feeItems != null && feeItems.Count > 0)
                {
                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                        feeItems.ToList(),
                        "Key, FeeType, CalculatedAmount.DisplayString, CalculatedAmount.Quantity, Amount.DisplayString, Amount.Quantity, PreviousCalculatedFeeAmount.DisplayString, FeeCalcSource.DisplayString, DisplayMessage, CreationDate");
                }
            }
            return ds;
        }

        public static bool SkipAverageHoldingCorrections(BatchExecutionResults results, int[] avgHoldingIds, ManagementTypes managementType)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            if (avgHoldingIds != null && avgHoldingIds.Length > 0)
            {
                try
                {
                    IList list = ManagementPeriodUnitMapper.GetManagementPeriodUnitCorrections(session, avgHoldingIds, managementType);
                    if (list != null)
                    {
                        int counter = 0;
                        foreach (IManagementPeriodUnitCorrection correction in list)
                        {
                            correction.Skip = true;
                            counter++;
                        }

                        if (session.Update(list))
                            results.MarkSuccess(counter);
                    }
                }
                catch (Exception ex)
                {
                    results.MarkError(
                        new ApplicationException("Error skipping average holding corrections.", ex));
                }
            }
            session.Close();
            return true;
        }

        #region Display Results

        public static string FormatErrorsForSkipAverageHoldingCorrections(BatchExecutionResults results)
        {
            const int MAX_ERRORS_DISPLAYED = 25;

            string message = "<br/>";

            if (results.SuccessCount == 0 && results.ErrorCount == 0)
                message += "No new average holding corrections need to be skipped";
            else
            {
                if (results.SuccessCount > 0)
                    message += string.Format("{0} average holding corrections were successfully skipped.<br/><br/><br/>", results.SuccessCount);

                if (results.ErrorCount > 0)
                {
                    string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                        string.Format(" (only the first {0} are shown)", MAX_ERRORS_DISPLAYED) : "");

                    message += string.Format("{0} errors occured while skipping average holding corrections{1}:<br/><br/><br/>",
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

    }
}
