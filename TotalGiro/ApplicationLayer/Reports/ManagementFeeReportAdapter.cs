using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
//    public static class ManagementFeeReportAdapter
//    {
//        public static int PrintKickBackReports(int assetManagerId, int remisierId, int year, int quarter)
//        {
//            int[] remisierIdsToPrint = null;
//            int printedCount = 0;

//            if (remisierId != 0)
//                remisierIdsToPrint[0] = remisierId;
//            else
//            {
//                string hql = "select R.Key from Remisier R where R.AssetManager.Key = :assetManagerId";
//                Hashtable parameters = new Hashtable(1);
//                parameters.Add("assetManagerId", assetManagerId);
//                IDalSession session = NHSessionFactory.CreateSession();
//                IList<int> remisierKeys = session.GetTypedListByHQL<int>(hql, parameters);
//                remisierIdsToPrint = remisierKeys.ToArray();
//            }

//            foreach (int remisierKey in remisierIdsToPrint)
//            {
//                printedCount += printReportsForRemisier(remisierKey, year, quarter);
//            }

//            return printedCount;
//        }

//        private static int printReportsForRemisier(int remisierId, int year, int quarter)
//        {
//            string periodName = string.Format("{0}{1}", year, quarter);
//            int printedCount = 0;

//            ReportExecutionWrapper wrapper = getReportWrapper(beginDate, endDate, managementCompanyId, reportLetterType);

//            IDalSession session = NHSessionFactory.CreateSession();

//            try
//            {
//                IList totalPortfolio = ValuationMapper.GetValuationsTotalPortfolio(session, accountId, monthDates);
//                if (totalPortfolio.Count > 0)
//                {
//                    IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);

//                    string pdfOnlineReportsFolder = getPdfReportsFolder(account.AccountOwner, "Online", periodName, account.ModelPortfolioName),
//                           pdfPostReportsFolder = getPdfReportsFolder(account.AccountOwner, "Post", periodName, account.ModelPortfolioName);
//                    string pdfFileName = string.Format("{0:d4}_{1}_{2}_{3}_{4}.pdf",
//                                                       account.AccountHolders.PrimaryAccountHolder.Contact.Key,
//                                                       account.Number, reportLetterType.ToString(), reportLetterId, reportLetterYear);
//                    string pdfOnlineFullPath = string.Format(@"{0}\{1}", pdfOnlineReportsFolder, pdfFileName),
//                           pdfPostFullPath = string.Format(@"{0}\{1}", pdfPostReportsFolder, pdfFileName);
//                    bool needsSendByPost = account.NeedsSendByPost(ReportLetterMapper.ToSendableDocumentCategory(reportLetterType));


//                    if (reportLetterType != ReportLetterTypes.EOY)
//                    {
//                        wrapper.Run(getQuarterReportDataSet(session, portfolioDevelopment, (portfolioOverview || portfolioSummary),
//                                                            transactionOverview, moneyMutations, account, beginDate, endDate),
//                                    pdfOnlineFullPath,
//                                    new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
//                                                   portfolioDevelopment.ToString(), portfolioOverview.ToString(), portfolioSummary.ToString(),  
//                                                   transactionOverview.ToString(), moneyMutations.ToString(), 
//                                                   chartCover.ToString(), concerning, description, null });

//                        if (needsSendByPost)
//                        {
//                            bool showLogo = managementCompany.ShowLogoByDefault;
//                            if (showLogo)
//                                File.Copy(pdfOnlineFullPath, pdfPostFullPath, true);
//                            else
//                            {

//                                wrapper.Run(getQuarterReportDataSet(session, portfolioDevelopment, (portfolioOverview || portfolioSummary),
//                                transactionOverview, moneyMutations, account, beginDate, endDate),
//                                pdfPostFullPath,
//                                new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
//                                                   portfolioDevelopment.ToString(), portfolioOverview.ToString(), portfolioSummary.ToString(),  
//                                                   transactionOverview.ToString(), moneyMutations.ToString(), 
//                                                   chartCover.ToString(), concerning, description, showLogo.ToString() });
//                            }

//                        }
//                    }
//                    else
//                    {
//                        // Only generate EOY-report for posting.
//                        IList dividends = TransactionMapper.GetTransactions(session, TransactionReturnClass.CashDividend, account, beginDate, endDate, false);
//                        bool accountHasDividend = (dividends.Count > 0);

//                        wrapper.Run(getFiscalYearReportDataSet(session, account, beginDate, endDate),
//                                    pdfPostFullPath,
//                                    new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
//                                                   true.ToString(), concerning, description, 
//                                                   reportLetterYear.ToString(), accountHasDividend.ToString() });
//                    }

//                    session.Close();

//                    if (reportLetterType != ReportLetterTypes.EOY)
//                        logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.PrintSuccess, "", pdfFileName, pdfOnlineReportsFolder);
//                    else
//                        logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.PrintSuccess, "", null, null);

//                    printedCount++;
//                }
//                else
//                    logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.NoValuations, "", null, null);
//            }
//            catch (Exception ex)
//            {
//                logReport(accountId, reportLetterYear, reportLetterType, reportLetterId, ReportStatuses.Error, ex.Message, null, null);
//            }
//            finally
//            {
//                if (session.IsOpen)
//                    session.Close();
//            }
//            return printedCount;
//        }

//        #region Fiscal Year Reports

//        public static byte[] ViewFiscalYearReports(int accountId, DateTime beginDate, DateTime endDate,
//             string concerning, string description, int managementCompanyId, int reportLetterYear, string reportLetterTypeName)
//        {
//            IDalSession session = NHSessionFactory.CreateSession();
//            try
//            {
//                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);

//                IList dividends = TransactionMapper.GetTransactions(session, TransactionReturnClass.CashDividend, account, beginDate, endDate, false);
//                bool accountHasDividend = (dividends.Count > 0);

//                byte[] reportContent;

//                if (account != null)
//                {
//                    ReportLetterTypes reportLetterType = ReportLetterMapper.GetReportLetterType(reportLetterTypeName);
//                    ReportExecutionWrapper wrapper = getReportWrapper(beginDate, endDate, managementCompanyId, reportLetterType);

//                    reportContent = wrapper.GetReportContent(
//                        getFiscalYearReportDataSet(session, account, beginDate, endDate),
//                        new string[] { beginDate.ToShortDateString(), endDate.ToShortDateString(), 
//                            true.ToString(), concerning, description, reportLetterYear.ToString(), accountHasDividend.ToString() });
//                }
//                else
//                    throw new ApplicationException("The specified account does not exist.");

//                return reportContent;
//            }
//            finally
//            {
//                session.Close();
//            }
//        }

//        // Makes a dataset for Fiscal Year Overview used by DumpDataSets.aspx.
//        public static DataSet GetFiscalYearReportDataSet(int accountId, DateTime beginDate, DateTime endDate,
//            string concerning, string description, int managementCompanyId, int reportLetterYear)
//        {
//            IDalSession session = NHSessionFactory.CreateSession();
//            try
//            {
//                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountId);
//                return getFiscalYearReportDataSet(session, account, beginDate, endDate);
//            }
//            finally
//            {
//                session.Close();
//            }
//        }

//        // Makes a dataset for Fiscal Year Overview
//        private static DataSet getFiscalYearReportDataSet(IDalSession session, IAccountTypeCustomer account, DateTime beginDate, DateTime endDate)
//        {
//            DateTime[] monthDates = EndTermValueMapper.GetEndDates(EndTerms.FullYear, endDate.Year);

//            // Step 01.Retrieve Accounts infomations
//            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(new IAccountTypeCustomer[] { account },
//                "Number, ModelPortfolioName, BaseCurrency.Symbol, BaseCurrency.AltSymbol, ContactAddress.DearSirForm, ContactAddress.AddressFirstLine, ContactAddress.AddressSecondLine, ContactAddress.ContactStreet, ContactAddress.ContactCity",
//                "Header");

//            // Begin and End Values
//            DataRow drMinTotalPortfolioValues;
//            IList portfolioEndValues = EndTermValueMapper.GetEndValues(session, account, EndTerms.FullYear, endDate.Year, true);

//            DataTable dtTotalPortfolioValues = new DataTable("TotalPortfolioValues");
//            dtTotalPortfolioValues.Columns.Add("Date", typeof(DateTime));
//            dtTotalPortfolioValues.Columns.Add("TotalValue_Quantity", typeof(Decimal));

//            drMinTotalPortfolioValues = dtTotalPortfolioValues.NewRow();
//            drMinTotalPortfolioValues["Date"] = monthDates[0];
//            drMinTotalPortfolioValues["TotalValue_Quantity"] = ((IEndTermValue)portfolioEndValues[0]).ClosingValue.Quantity;
//            dtTotalPortfolioValues.Rows.Add(drMinTotalPortfolioValues);

//            //Check if the minimum value is present. If not add a zero value
//            drMinTotalPortfolioValues = dtTotalPortfolioValues.NewRow();
//            drMinTotalPortfolioValues["Date"] = monthDates[1];
//            drMinTotalPortfolioValues["TotalValue_Quantity"] = ((IEndTermValue)portfolioEndValues[1]).ClosingValue.Quantity;
//            dtTotalPortfolioValues.Rows.Add(drMinTotalPortfolioValues);

//            ds.Tables.Add(dtTotalPortfolioValues);

//            // CashDividend Transacties
//            DataTable dtCashDivTransactions = DataSetBuilder.CreateDataTableFromBusinessObjectList(
//                TransactionMapper.GetTransactions(session, TransactionReturnClass.CashDividend, account, beginDate, endDate, false),
//                @"TransactionDate, CashGeneratingInstrument.Name, CashGeneratingInstrument.Country.Iso2,
//                    Units.Quantity, DividendDetails.UnitPrice.Quantity, ValueSize.UnderlyingShortName, ValueSize.Quantity, TaxQuantity, ExchangeRate, TotalBaseAmountQuantity, ValueSizeQuantity",
//            "CashDivTransactions");

//            ds.Tables.Add(dtCashDivTransactions);

//            return ds;
//        }

//        #endregion

//        #region Helper methods

//        private static string getPdfReportsFolder(IManagementCompany managementCompany, string purposeSubfolder,
//                                                  string periodName, string remisierName)
//        {
//            if (managementCompany.PdfReportsFolder != null && managementCompany.PdfReportsFolder != string.Empty)
//            {
//                string pdfReportsFolder = string.Format(@"{0}\Reports\{1}\{2}\{3}",
//                                                        managementCompany.PdfReportsFolder, purposeSubfolder, periodName, remisierName);
//                if (!Directory.Exists(pdfReportsFolder))
//                    Directory.CreateDirectory(pdfReportsFolder);
//                return pdfReportsFolder;
//            }
//            else
//                throw new ApplicationException(string.Format("PDF report-generation folder not set for management company '{0}'.",
//                                                             managementCompany.CompanyName));
//        }

//        private static ReportExecutionWrapper getReportWrapper(int year, int quarter, int managementCompanyId)
//        {
//            string reportTemplateName = "KickBackOverview";
//            string[] parameterNames = new string[] { "BeginDate", "EndDate", "FiscalYearDividend", "Betreft", "Omschrijving", "SelectedFiscalYear", 
//                                                "accountHasDividend" };

//            IDalSession session = NHSessionFactory.CreateSession();
//            try
//            {
//                IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
//                IReportTemplate reportTemplate = ReportTemplateMapper.GetReportTemplate(session, managementCompany, reportTemplateName, true);

//                ReportExecutionWrapper wrapper = new ReportExecutionWrapper();
//                wrapper.SetReportName(reportTemplate.ReportTemplateName);
//                wrapper.AddParameters(parameterNames);
//                return wrapper;
//            }
//            finally
//            {
//                session.Close();
//            }
//        }

//        #endregion
//    }
}
