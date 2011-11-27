using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Reports.Documents;
using B4F.TotalGiro.Reports.Financial;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.Valuations.ReportedData;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public static class FinancialReportAdapter
    {
        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                                  int year, string propertyList)
        {
            return AccountFinderAdapter.GetCustomerAccounts(assetManagerId, modelPortfolioId, accountNumber, accountName, false, true, true, year, 
                                                            true, false, propertyList);
        }

        public static int PrintReports(int[] accountIds, DateTime beginDate, DateTime endDate, 
            bool portfolioDevelopment, bool portfolioOverview, bool portfolioSummary, bool transactionOverview, bool moneyMutations, bool chartCover,
            string concerning, string description, int managementCompanyId, int reportLetterYear, string reportLetterTypeName)
        {
            ReportLetterTypes reportLetterType = ReportLetterMapper.GetReportLetterType(reportLetterTypeName);
            int reportLetterId = getLatestReportLetterId(managementCompanyId, reportLetterYear, reportLetterType);

            // Yearly Reports always contain a logo (for now); showLogo is set here to true to avoid calling Report Server two times
            // (we just make a copy of the "Online" PDF as the "Post" version)
            bool showLogo = (reportLetterType == ReportLetterTypes.EOY ? true : getShowLogoByDefault(managementCompanyId));

            int[] accountIdsToPrint = null;
            int printedCount = 0;
            
            foreach (ReportStatuses oldReportStatus in Report.ReportStatusList)
                if (accountIds.Length > 0)
                {
                    accountIdsToPrint = getAccountIdsByReportStatus(reportLetterYear, reportLetterType, accountIds, oldReportStatus);
                    if (oldReportStatus == ReportStatuses.PrintSuccess)
                        accountIdsToPrint = getDifference(accountIds, accountIdsToPrint);

                    if (accountIdsToPrint.Length > 0)
                    {
                        printedCount += printReportsForAccounts(accountIdsToPrint, beginDate, endDate,
                            portfolioDevelopment, portfolioOverview, portfolioSummary, transactionOverview, moneyMutations, chartCover,
                            concerning, description, managementCompanyId, reportLetterYear, reportLetterType,
                            reportLetterId, showLogo);

                        accountIds = getDifference(accountIds, accountIdsToPrint);
                    }
                }

            return printedCount;
        }

        private static int printReportsForAccounts(int[] accountIds, DateTime beginDate, DateTime endDate,
            bool portfolioDevelopment, bool portfolioOverview, bool portfolioSummary, bool transactionOverview, bool moneyMutations, bool chartCover,
            string concerning, string description, int managementCompanyId, int reportLetterYear, ReportLetterTypes reportLetterType,
            int reportLetterId, bool showLogo)
        {
            string periodName = string.Format("{0}_{1}", reportLetterYear, reportLetterType.ToString());
            DateTime[] monthDates = new DateTime[] { beginDate, endDate };
            int printedCount = 0;

            if (accountIds.Length > 0)
            {
                ReportExecutionWrapper wrapper = getReportWrapper(beginDate, endDate, managementCompanyId, reportLetterType);

                foreach (int accountId in accountIds)
                {
                    IDalSession session = NHSessionFactory.CreateSession();

                    try
                    {
                        IList totalPortfolio = ValuationMapper.GetValuationsTotalPortfolio(session, accountId, monthDates);
                        if (totalPortfolio.Count > 0)
                        {
                            ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);

                            string pdfOnlineReportsFolder = getPdfReportsFolder(account.AccountOwner, "Online", periodName, account.ModelPortfolioName),
                                   pdfPostReportsFolder = getPdfReportsFolder(account.AccountOwner, "Post", periodName, account.ModelPortfolioName);
                            string pdfFileName = string.Format("{0:d4}_{1}_{2}_{3}_{4}.pdf",
                                                               account.AccountHolders.PrimaryAccountHolder.Contact.Key,
                                                               account.Number, reportLetterType.ToString(), reportLetterId, reportLetterYear);
                            string pdfOnlineFullPath = string.Format(@"{0}\{1}", pdfOnlineReportsFolder, pdfFileName),
                                   pdfPostFullPath = string.Format(@"{0}\{1}", pdfPostReportsFolder, pdfFileName);
                            bool needsSendByPost = account.NeedsSendByPost(ReportLetterMapper.ToSendableDocumentCategory(reportLetterType));

                            DataSet ds = null;
                            string[] paramValues = null;

                            if (reportLetterType == ReportLetterTypes.EOY)
                            {
                                bool accountHasDividend = getDividendCount(session, account.Key, beginDate, endDate);

                                ds = getFiscalYearReportDataSet(session, account, beginDate, endDate);
                                paramValues = new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
                                                             true.ToString(), concerning, description, 
                                                             reportLetterYear.ToString(), accountHasDividend.ToString(), true.ToString() };
                            }
                            else
                            {
                                ds = getQuarterReportDataSet(session, portfolioDevelopment, (portfolioOverview || portfolioSummary),
                                                             transactionOverview, moneyMutations, account, beginDate, endDate);
                                paramValues = new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
                                                             portfolioDevelopment.ToString(), portfolioOverview.ToString(), portfolioSummary.ToString(),  
                                                             transactionOverview.ToString(), moneyMutations.ToString(), 
                                                             chartCover.ToString(), concerning, description, true.ToString() };
                            }

                            wrapper.Run(ds, pdfOnlineFullPath, paramValues);

                            if (needsSendByPost)
                            {
                                if (showLogo)
                                    File.Copy(pdfOnlineFullPath, pdfPostFullPath, true);
                                else
                                {
                                    // "ShowLogo" is the last parameter (for now)
                                    paramValues[paramValues.Length - 1] = showLogo.ToString();
                                    wrapper.Run(ds, pdfPostFullPath, paramValues);
                                }
                            }

                            session.Close();

                            logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.PrintSuccess, "",
                                      pdfFileName, pdfOnlineReportsFolder, needsSendByPost);

                            printedCount++;
                        }
                        else
                            logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.NoValuations, "",
                                      null, null, null);
                    }
                    catch (Exception ex)
                    {
                        logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.Error, ex.Message,
                                  null, null, null);
                    }
                    finally
                    {
                        if (session.IsOpen)
                            session.Close();
                    }
                }
            }
            else
                throw new ApplicationException("No accounts were specified for printing reports.");

            return printedCount;
        }


        public static bool getEndtermvaluesEnddateCount(DateTime endDate)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList endtermvalues = EndTermValueMapper.GetEndValues(session, endDate);
            session.Close();
            return (endtermvalues.Count > 0);
        }


        private static bool getDividendCount(IDalSession session, int accountId, DateTime beginDate, DateTime endDate)
        {
            IList<IGeneralOperationsBooking> dividends = GeneralOperationsBookingMapper.GetBookings(session, GeneralOperationsBookingReturnClass.CashDividend, accountId, beginDate, endDate, false);
            return (dividends.Count > 0);
        }

        private static bool getShowLogoByDefault(int managementCompanyId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
                return managementCompany.ShowLogoByDefault;
            }
            finally
            {
                session.Close();
            }
        }

        #region Fiscal Year Reports

        public static byte[] ViewFiscalYearReports(int accountId, DateTime beginDate, DateTime endDate,
             string concerning, string description, int managementCompanyId, int reportLetterYear, string reportLetterTypeName)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
                if (managementCompanyId == 0 && account != null) managementCompanyId = account.AccountOwner.Key;
                bool accountHasDividend = getDividendCount(session, account.Key, beginDate, endDate);

                byte[] reportContent;

                if (account != null)
                {
                    ReportLetterTypes reportLetterType = ReportLetterMapper.GetReportLetterType(reportLetterTypeName);
                    ReportExecutionWrapper wrapper = getReportWrapper(beginDate, endDate, managementCompanyId, reportLetterType);

                    reportContent = wrapper.GetReportContent(
                        getFiscalYearReportDataSet(session, account, beginDate, endDate),
                        new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), true.ToString(), concerning, description, 
                                       reportLetterYear.ToString(), accountHasDividend.ToString(), true.ToString() });
                }
                else
                    throw new ApplicationException("The specified account does not exist.");

                return reportContent;
            }
            finally
            {
                session.Close();
            }
        }

        // Makes a dataset for Fiscal Year Overview used by DumpDataSets.aspx.
        public static DataSet GetFiscalYearReportDataSet(int accountId, DateTime beginDate, DateTime endDate, 
            string concerning, string description, int managementCompanyId, int reportLetterYear)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
                return getFiscalYearReportDataSet(session, account, beginDate, endDate);
            }
            finally
            {
                session.Close();
            }
        }

        // Makes a dataset for Fiscal Year Overview
        private static DataSet getFiscalYearReportDataSet(IDalSession session, ICustomerAccount account, DateTime beginDate, DateTime endDate)
        {
            DateTime[] monthDates = EndTermValueMapper.GetEndDates(EndTermType.FullYear, endDate.Year);

            DataSet ds = new DataSet();
            ds.Tables.Add(getHeaderTable(account));

            // Begin and End Values
            DataRow drMinTotalPortfolioValues;
            IList portfolioEndValues = EndTermValueMapper.GetEndValues(session, account, EndTermType.FullYear, endDate.Year, true);

            DataTable dtTotalPortfolioValues = new DataTable("TotalPortfolioValues");
            dtTotalPortfolioValues.Columns.Add("Date", typeof(DateTime));
            dtTotalPortfolioValues.Columns.Add("TotalValue_Quantity", typeof(Decimal));
            dtTotalPortfolioValues.Columns.Add("GreenValue_Quantity", typeof(Decimal));
            dtTotalPortfolioValues.Columns.Add("CultureValue_Quantity", typeof(Decimal));

            drMinTotalPortfolioValues = dtTotalPortfolioValues.NewRow();
            drMinTotalPortfolioValues["Date"] = monthDates[0];
            drMinTotalPortfolioValues["TotalValue_Quantity"] = ((IEndTermValue)portfolioEndValues[0]).ClosingValue.Quantity;
            drMinTotalPortfolioValues["GreenValue_Quantity"] = ((IEndTermValue)portfolioEndValues[0]).GreenFundValue.Quantity;
            drMinTotalPortfolioValues["CultureValue_Quantity"] = ((IEndTermValue)portfolioEndValues[0]).CultureFundValue.Quantity;
            dtTotalPortfolioValues.Rows.Add(drMinTotalPortfolioValues);

            //Check if the minimum value is present. If not add a zero value
            drMinTotalPortfolioValues = dtTotalPortfolioValues.NewRow();
            drMinTotalPortfolioValues["Date"] = monthDates[1];
            drMinTotalPortfolioValues["TotalValue_Quantity"] = ((IEndTermValue)portfolioEndValues[1]).ClosingValue.Quantity;
            drMinTotalPortfolioValues["GreenValue_Quantity"] = ((IEndTermValue)portfolioEndValues[1]).GreenFundValue.Quantity;
            drMinTotalPortfolioValues["CultureValue_Quantity"] = ((IEndTermValue)portfolioEndValues[1]).CultureFundValue.Quantity;
            dtTotalPortfolioValues.Rows.Add(drMinTotalPortfolioValues);

            ds.Tables.Add(dtTotalPortfolioValues);

            // CashDividend Transacties
//            DataTable dtCashDivTransactions = DataSetBuilder.CreateDataTableFromBusinessObjectList(
//                ObsoleteTransactionMapper.GetTransactions(session, ObsoleteTransactionReturnClass.CashDividend, account, beginDate, endDate, false),
//                @"TransactionDate, CashGeneratingInstrument.Name, CashGeneratingInstrument.Country.Iso2,
//                    Units.Quantity, DividendDetails.UnitPrice.Quantity, ValueSize.UnderlyingShortName, ValueSize.Quantity, TaxQuantity, ExchangeRate, TotalBaseAmountQuantity, ValueSizeQuantity",
//            "CashDivTransactions");

            DataTable dtCashDivTransactions = GeneralOperationsBookingMapper.GetBookings(
                session, GeneralOperationsBookingReturnClass.CashDividend, account.Key, beginDate, endDate, false)
                .Cast<ICashDividend>()
                .Select(c => new
                {
                    TransactionDate =
                        c.GeneralOpsJournalEntry.TransactionDate,
                    CashGeneratingInstrument_Name =
                        c.DividendDetails.Instrument.Name,
                    CashGeneratingInstrument_Country_Iso2 =
                        c.DividendDetails.Instrument.Country.Iso2,
                    Units_Quantity =
                        c.UnitsInPossession.Quantity,
                    DividendDetails_UnitPrice_Quantity =
                        c.DividendDetails.UnitPrice.Quantity,
                    ValueSize_UnderlyingShortName =
                        c.NettAmount.UnderlyingShortName, 
                    ValueSize_Quantity =
                        c.NettAmount.Quantity,
                    TaxQuantity =
                        c.TaxAmount.Quantity,
                    ExchangeRate =
                        c.GeneralOpsJournalEntry.ExchangeRate,
                    TotalBaseAmountQuantity =
                        c.Components.TotalBaseAmount.Quantity,
                    ValueSizeQuantity =
                        c.NettAmount.Quantity
                })
                .ToDataTable("CashDivTransactions");


            ds.Tables.Add(dtCashDivTransactions);

            return ds;
        }
        
        #endregion

        #region Quarter Reports

        public static byte[] ViewQuarterReport(int accountId, DateTime beginDate, DateTime endDate, 
            bool portfolioDevelopment, bool portfolioOverview, bool portfolioSummary, bool transactionOverview, bool moneyMutations, bool chartCover,
            string concerning, string description, int managementCompanyId, string reportLetterTypeName)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
                if (managementCompanyId == 0 && account != null) managementCompanyId = account.AccountOwner.Key;
                byte[] reportContent;

                if (account != null)
                {
                    ReportLetterTypes reportLetterType = ReportLetterMapper.GetReportLetterType(reportLetterTypeName);
                    ReportExecutionWrapper wrapper = getReportWrapper(beginDate, endDate, managementCompanyId, reportLetterType);

                    reportContent = wrapper.GetReportContent(
                        getQuarterReportDataSet(session, portfolioDevelopment, (portfolioOverview || portfolioSummary),
                                                transactionOverview, moneyMutations, account, beginDate, endDate),
                        new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
                                       portfolioDevelopment.ToString(), portfolioOverview.ToString(), portfolioSummary.ToString(),  
                                       transactionOverview.ToString(), moneyMutations.ToString(), 
                                       chartCover.ToString(), concerning, description, true.ToString() });
                }
                else
                    throw new ApplicationException("The specified account does not exist.");

                return reportContent;
            }
            finally
            {
                session.Close();
            }
        }

        // Makes a dataset with multiple datatables for all reports
        public static DataSet GetQuarterReportDataSet(bool portfolioDevelopment, bool portfolioOverview, bool transactionOverview,
            bool moneyMutations, int accountId, DateTime beginDate, DateTime endDate)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
                return getQuarterReportDataSet(session, portfolioDevelopment, portfolioOverview,
                                               transactionOverview, moneyMutations,
                                               account, beginDate, endDate);
            }
            finally
            {
                session.Close();
            }
        }

        // Makes a dataset with multiple datatables for all reports
        private static DataSet getQuarterReportDataSet(IDalSession session, bool portfolioDevelopment, bool portfolioOverview, bool transactionOverview,
            bool moneyMutations, ICustomerAccount account, DateTime beginDate, DateTime endDate)
        {
            int months = Util.DateDiff(DateInterval.Month, beginDate, endDate) + 1;
            DateTime[] monthDates = new DateTime[months];
            IInstrumentsCategories defaultInstrumentCategory = null;

            //Makes a datearray for 1 year for header graph
            for (int i = 0; i < months; i++)
            {
                monthDates[i] = Util.GetLastDayOfMonth(beginDate.AddMonths(i));
            }

            IList<IValuation> valuationsEndDate = ValuationMapper.GetValuations(session, account.Key, new DateTime[] { endDate });

            DataSet ds = new DataSet();
            ds.Tables.Add(getHeaderTable(account));

            // Model at the end of the period
            IModelHistory histModel = AccountMapper.GetHistoricalModel(session, account, endDate);
            DataTable dtModel = new IModelHistory[] { histModel }
                .Select(c => new
                {
                    ModelPortfolio_ModelName =
                        c.ModelPortfolio.ModelName
                })
                .ToDataTable("Model");
            ds.Tables.Add(dtModel);

            // Used for Plot Graph -> Portfolio Development
            DataTable dtTotalPortfolioValues = ValuationMapper.GetValuationsTotalPortfolio(session, account.Key, monthDates)
                .Select(c => new
                {
                    c.Date,
                    TotalValue_Quantity =
                        c.TotalValue.Quantity
                })
                .ToDataTable("TotalPortfolioValues");
            ds.Tables.Add(dtTotalPortfolioValues);

            // Used for Pie Chart -> Portfolio Asset Breakup
            PortfolioBreakUp breakUp = new PortfolioBreakUp(valuationsEndDate, endDate);
            PortfolioBreakUpDetail[] breakUpDetails = new PortfolioBreakUpDetail[breakUp.BreakUpDetails.Count];
            breakUp.BreakUpDetails.CopyTo(breakUpDetails, 0);
            DataTable dtPortfolioAssetClassBreakUp = breakUpDetails
                .Select(c => new
                {
                    AssetClass_AssetName = 
                        c.AssetClass.AssetName,
                    BreakUpValue_Quantity =
                        c.BreakUpValue.Quantity,
                    c.BreakUpPercentage
                })
                .ToDataTable("PortfolioAssetClassBreakUp");
            ds.Tables.Add(dtPortfolioAssetClassBreakUp);

            if (portfolioDevelopment)
            {
                PortfolioDevelopment portfolioDev = ValuationMapper.GetPortfolioDevelopment(session, account, beginDate, endDate, histModel.ModelPortfolioKey);
                DataTable dtPortfolioDev = new PortfolioDevelopment[] { portfolioDev }
                    .Select(c => new
                    {
                        TotalValueBegin_Quantity = 
                            c.TotalValueBegin.Quantity,
                        TotalValueEnd_Quantity = 
                            c.TotalValueEnd.Quantity,
                        TotalValueDifference_Quantity = 
                            c.TotalValueDifference.Quantity,
                        c.InvestmentReturnPercentage,
                        RealisedAmount_Quantity = 
                            c.RealisedAmount.Quantity,
                        UnRealisedAmount_Quantity = 
                            c.UnRealisedAmount.Quantity,
                        UnRealisedAmountPreviousPeriod_Quantity =
                            c.UnRealisedAmountPreviousPeriod.Quantity,
                        Withdrawals_Quantity = 
                            c.Withdrawals.Quantity,
                        Deposits_Quantity = 
                            c.Deposits.Quantity,
                        c.BenchMarkPerformance,
                        c.BenchMarkValue,
                        c.IBoxxTarget,
                        c.MSCIWorldTarget,
                        c.CompositeTarget
                    })
                    .ToDataTable("PortfolioDevelopment");
                ds.Tables.Add(dtPortfolioDev);

                PortfolioDevelopmentCash[] portCash = new PortfolioDevelopmentCash[portfolioDev.CashMovements.Count];
                portfolioDev.CashMovements.CopyTo(portCash, 0);
                DataTable dtPortfolioCashDev = portCash
                    .Select(c => new
                    {
                        c.ValuationCashTypeDescription,
                        Amount_Quantity = 
                            c.Amount.Quantity,
                        c.IsIncome
                    })
                    .ToDataTable("PortfolioCashDevelopment");
                ds.Tables.Add(dtPortfolioCashDev);
            }

            if (portfolioOverview)
            {
                // get the default InstrumentCategory
                getDefaultInstrumentCategory(session, ref defaultInstrumentCategory);
                ValuationMutation.defaultInstrumentCategory = defaultInstrumentCategory;

                DataTable dtPortfolioOverview = ValuationMapper.GetValuationsPortfolioOverview(valuationsEndDate)
                    .Select(c => new
                    {
                        Instrument_IsCash = 
                            c.Instrument.IsCash,
                        ValuationMutation_Position_AssetClass_Key = 
                            c.AssetClass.Key,
                        ValuationMutation_Position_AssetClass_AssetName = 
                            c.AssetClass.AssetName,
                        c.DisplayInstrumentsCategory,
                        Instrument_Name = 
                            c.Instrument.Name,
                        CurrencyNominal_Symbol = 
                            c.CurrencyNominal.Symbol,
                        CurrencyNominal_BaseCurrency_Symbol = 
                            c.CurrencyNominal.BaseCurrency.Symbol,
                        Size_Quantity = 
                            c.Size.Quantity,
                        CostPrice_DisplayQuantity = 
                            c.CostPrice.DisplayQuantity,
                        BookValue_Quantity = 
                            c.BookValue.Quantity,
                        MarketPrice_DisplayQuantity = 
                            c.MarketPrice.DisplayQuantity,
                        BaseMarketValue_Quantity = 
                            c.BaseMarketValue.Quantity,
                        UnRealisedAmountToDate_Quantity = 
                            c.UnRealisedAmountToDate.Quantity
                    })
                    .ToDataTable("PortfolioOverview");
                ds.Tables.Add(dtPortfolioOverview);
                ValuationMutation.defaultInstrumentCategory = null;

                // get historical exrates
                ArrayList currencyList = new ArrayList();
                foreach (IValuation valuation in valuationsEndDate)
                {
                    if (!currencyList.Contains(valuation.CostPrice.Underlying))
                        currencyList.Add(valuation.CostPrice.Underlying);
                }
                ICurrency[] currencies = new ICurrency[currencyList.Count];
                currencyList.CopyTo(currencies, 0);
                DataTable dtHistoricalExchangeRates = HistoricalExRateMapper.GetHistoricalExRates(session, currencies, endDate)
                    .Select(c => new
                    {
                        Currency_Symbol = 
                            c.Currency.Symbol,
                        c.Rate
                    })
                    .ToDataTable("HistoricalExchangeRates");
                ds.Tables.Add(dtHistoricalExchangeRates);
            }

            if (transactionOverview)
            {
                DataTable dtTransactionOverview = ValuationMapper.GetSecurityValuationMutations(session, account.Key, beginDate, endDate)
                    .Select(c => new
                    {
                        Position_AssetClass_Key = 
                            c.Position.AssetClass.Key,
                        Position_AssetClass_AssetName = 
                            c.Position.AssetClass.AssetName,
                        Instrument_Name = 
                            c.Instrument.Name,
                        SizeChange_Quantity = 
                            c.SizeChange.Quantity,
                        c.Date,
                        BookChange_Quantity = 
                            c.BookChange.Quantity,
                        TotalBaseTradeAmount_Quantity = 
                            c.TotalBaseTradeAmount.Quantity,
                        BaseRealisedAmount_Quantity = 
                            c.BaseRealisedAmount.Quantity,
                        BaseCommission_Quantity = 
                            c.BaseCommission.Quantity
                    })
                    .ToDataTable("TransactionOverview");
                ds.Tables.Add(dtTransactionOverview);
            }

            if (moneyMutations)
            {
                // cache the TransactionType descriptions
                ValuationMutation.defaultInstrumentCategory = defaultInstrumentCategory;

                //    AccountMapper.GetCashPositionTransactions(session, account, beginDate, endDate, false),
                //    "TransactionTypeID, ParentTransaction.TransactionTypeDescription, Description, TransactionDate, Amount.Quantity, Amount.Underlying.ToCurrency.AltSymbol, SideValue, " +
                //    "SizeQuantity, TradedInstrument.Name, PriceQuantity, PriceCurrency.AltSymbol, CommissionQuantity, CommissionCurrency.AltSymbol, ServiceChargeQuantity, ServiceChargeCurrency.AltSymbol, " +
                //    "ForeignCashMoneyOrderValueQuantity, ForeignCashMoneyOrderBaseValueQuantity, ForeignCashMoneyOrderCurrency.AltSymbol, ForeignCashMoneyOrderExRate, CashGeneratingInstrument.Name, TaxQuantity, FullDescription",
                List<ICashMutationView> lines = AccountMapper.GetCashPositionTransactions(session, account, beginDate, endDate, false);
                DataTable dtMoneyMutations = lines.Select(c => new
                {
                    Key =
                        c.Key,
                    c.Account.Number,
                    CashMutationViewTypeID =
                        (int)c.CashMutationViewType,
                    c.TypeID,
                    c.TypeDescription,
                    Description =
                        c.FullDescription,
                    c.TransactionDate,
                    BaseAmount = 
                        c.Amount.BaseAmount.Quantity
                })
                .ToDataTable("MoneyMutations");
                ds.Tables.Add(dtMoneyMutations);
                
                
                // Get Cash Position startdate -1 (day before) & end date
                ICurrency baseCur = account.AccountOwner.StichtingDetails.BaseCurrency;
                IList moneyPositionStartAndEndDate = ValuationMapper.GetValuations(session, account.Key, baseCur.Key, new DateTime[] { beginDate.AddDays(-1), endDate }, true);

                DataTable dtMoneyPositionStartAndEndDate = new DataTable("MoneyPositionStartAndEndDate");
                dtMoneyPositionStartAndEndDate.Columns.Add("Date", typeof(DateTime));
                dtMoneyPositionStartAndEndDate.Columns.Add("Size_Quantity", typeof(decimal));
                decimal valStartDate = 0M;
                decimal valEndDate = 0M;
                foreach (IValuation val in moneyPositionStartAndEndDate)
                {
                    if (val.Date.Equals(beginDate.AddDays(-1)))
                        valStartDate = val.Size.Quantity;
                    else if (val.Date.Equals(endDate))
                        valEndDate = val.Size.Quantity;
                }
                Util.AddNewRowToDataTableWhenRecordNotExists(dtMoneyPositionStartAndEndDate, beginDate.AddDays(-1), valStartDate);
                Util.AddNewRowToDataTableWhenRecordNotExists(dtMoneyPositionStartAndEndDate, endDate, valEndDate);
                ds.Tables.Add(dtMoneyPositionStartAndEndDate);
            }

            return ds;
        }

        #endregion

        #region Helper methods

        private static DataTable getHeaderTable(ICustomerAccount account)
        {
            account.Formatter.AssertAddressIsComplete();

            return new ICustomerAccount[] { account }
                .Select(a => new
                {
                    a.Number,
                    a.ModelPortfolioName,
                    BaseCurrency_Symbol =
                        a.BaseCurrency.Symbol,
                    BaseCurrency_AltSymbol =
                        a.BaseCurrency.AltSymbol,
                    ContactAddress_DearSirForm =
                        a.Formatter.DearSirForm,
                    ContactAddress_AddressFirstLine =
                        a.Formatter.AddressFirstLine,
                    ContactAddress_AddressSecondLine =
                        a.Formatter.AddressSecondLine,
                    ContactAddress_ContactStreet =
                        a.Formatter.Address.StreetAddressLine,
                    ContactAddress_ContactCity =
                        a.Formatter.Address.CityAddressLine
                })
                .ToDataTable("Header");
        }

        private static int getLatestReportLetterId(int managementCompanyId, int reportLetterYear, ReportLetterTypes reportLetterType)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IReportLetter reportLetter = ReportLetterMapper.GetLatestReportLetter(session, managementCompanyId, reportLetterType, reportLetterYear);
                if (reportLetter != null)
                    return reportLetter.Key;
                else
                    throw new ApplicationException(string.Format("Could not find {0} report letter for year {1} and company ID {2}.",
                                                                 reportLetterType, reportLetterYear, managementCompanyId));
            }
            finally
            {
                session.Close();
            }
        }

        private static int[] getAccountIdsByReportStatus(int reportLetterYear, ReportLetterTypes reportLetterType, int[] accountIds, 
                                                         ReportStatuses reportStatus)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                List<IReport> reports = ReportMapper.GetReports(session, reportLetterYear, reportLetterType, accountIds, reportStatus);
                return reports.ConvertAll<int>(report => report.Account.Key).ToArray();
            }
            finally
            {
                session.Close();
            }
        }

        private static string getPdfReportsFolder(IManagementCompany managementCompany, string purposeSubfolder, 
                                                  string periodName, string modelPortolioName)
        {
            if (managementCompany.PdfReportsFolder != null && managementCompany.PdfReportsFolder != string.Empty)
            {
                string pdfReportsFolder = string.Format(@"{0}\Reports\{1}\{2}\{3}",
                                                        managementCompany.PdfReportsFolder, purposeSubfolder, periodName, modelPortolioName);
                if (!Directory.Exists(pdfReportsFolder))
                    Directory.CreateDirectory(pdfReportsFolder);
                return pdfReportsFolder;
            }
            else
                throw new ApplicationException(string.Format("PDF report-generation folder not set for management company '{0}'.",
                                                             managementCompany.CompanyName));
        }

        private static ReportExecutionWrapper getReportWrapper(DateTime beginDate, DateTime endDate, int managementCompanyId,
                                                               ReportLetterTypes reportLetterType)
        {
            string reportTemplateName;
            string[] parameterNames;
            if (reportLetterType == ReportLetterTypes.EOY)
            {
                reportTemplateName = ReportReturnClass.FiscalYearReport.ToString();
                parameterNames = new string[] { "BeginDate", "EndDate", "FiscalYearDividend", "Betreft", "Omschrijving", "SelectedFiscalYear", 
                                                "accountHasDividend", "ShowLogo" };
            }
            else
            {
                reportTemplateName = ReportReturnClass.QuarterReport.ToString();
                parameterNames = new string[] { "BeginDate", "EndDate", "PortfolioDevelopment", "PortfolioOverview", "PortfolioSummary", 
                                                "TransactionOverview", "MoneyMutations", "Chartcover", "Betreft", "Omschrijving", "ShowLogo" };
            }

            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
                IReportTemplate reportTemplate = ReportTemplateMapper.GetReportTemplate(session, managementCompany, reportTemplateName, true);

                ReportExecutionWrapper wrapper = new ReportExecutionWrapper();
                wrapper.SetReportName(reportTemplate.ReportTemplateName);
                wrapper.AddParameters(parameterNames);
                return wrapper;
            }
            finally
            {
                session.Close();
            }
        }

        private static void getDefaultInstrumentCategory(IDalSession session, ref IInstrumentsCategories defaultInstrumentCategory)
        {
            if (defaultInstrumentCategory == null)
                defaultInstrumentCategory = ClassificationMapper.GetDefaultInstrumentsCategory(session);
        }

        private static int[] getDifference(int[] firstTerm, int[] minusTerm)
        {
            List<int> firstList = new List<int>(firstTerm);
            List<int> minusList = new List<int>(minusTerm);
            firstList.RemoveAll(i => minusList.Contains(i));
            return firstList.ToArray();
        }

        private static void logReport(int oldAccountId, int oldReportLetterYear, ReportLetterTypes oldReportLetterType,
                                      int newReportLetterId, ReportStatuses newReportStatus, string newErrorMessage,
                                      string pdfFileName, string pdfFilePath, bool? sentByPost)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IReportLetter reportLetter = ReportLetterMapper.GetReportLetter(session, newReportLetterId);
                ICustomerAccount account = AccountMapper.GetAccount(session, oldAccountId) as ICustomerAccount;
                
                if (newErrorMessage.Length > 200)
                    newErrorMessage = newErrorMessage.Substring(0, 200);

                IReport report = ReportMapper.GetReport(session, oldAccountId, oldReportLetterYear, oldReportLetterType);
                if (report == null)
                    report = Report.CreateReport(account, reportLetter, newReportStatus, newErrorMessage);
                else
                {
                    report.ModelPortfolio = account.ModelPortfolio;
                    report.SetContactsNAW(account);
                    report.ReportLetter = reportLetter;
                    report.ReportStatusId = (int)newReportStatus;
                    report.ErrorMessage = newErrorMessage;
                    report.CreationDate = DateTime.Now;
                }

                if (pdfFileName != null)
                {
                    if (report.Document == null)
                        report.Document = new FinancialReportDocument();
                    report.Document.FileName = pdfFileName;
                    report.Document.FilePath = pdfFilePath;
                    report.Document.SentByPost = (bool)sentByPost;
                    session.InsertOrUpdate(report.Document);
                }
                else
                    report.Document = null;
                
                session.InsertOrUpdate(report);
            }
            finally
            {
                session.Close();
            }
        }

        #endregion
    }
}

