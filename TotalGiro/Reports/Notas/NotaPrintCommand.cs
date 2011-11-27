using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Reports.Documents;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Reports.Notas
{
    public abstract class NotaPrintCommand
    {
        public NotaPrintCommand()
        {
            // Default values for properties
            ReportGrouping = new NotaGroupingOnePerReport();
            NotasPerPage = 0;       // zero means no page grouping
        }

        public static void PrintNotas(BatchExecutionResults results, NotaReturnClass notaType, int managementCompanyId)
        {
            NotaPrintCommand.CreateNew(notaType).PrintNotas(results, managementCompanyId);
        }

        public static NotaPrintCommand CreateNew(NotaReturnClass notaType)
        {
            switch (notaType)
            {
                case NotaReturnClass.NotaTransaction:
                    return new NotaTransactionPrintCommand();
                case NotaReturnClass.NotaDeposit:
                    return new NotaDepositPrintCommand();
                case NotaReturnClass.NotaDividend:
                    return new NotaDividendPrintCommand();
                case NotaReturnClass.NotaTransfer:
                    return new NotaTransferPrintCommand();
                case NotaReturnClass.NotaFees:
                    return new NotaFeesPrintCommand();
                case NotaReturnClass.NotaInstrumentConversion:
                    return new NotaCorporateActionPrintCommand();
                default:
                    throw new ApplicationException(string.Format("NotaPrintCommand of type {0} cannot be created.", notaType));
            }
        }

        public NotaReturnClass NotaType
        {
            get { return notaType; }
            protected set { notaType = value; }
        }

        public object GetHeaderFields(INota n)
        {
            n.Formatter.AssertAddressIsComplete();

            return new
            {
                n.PrintCount,
                n.Formatter.AddressFirstLine,
                n.Formatter.AddressSecondLine,
                n.Formatter.Address.StreetAddressLine,
                n.Formatter.Address.CityAddressLine,
                n.Formatter.Address.CountryAddressLine
            };
        }

        public abstract object GetNotaFields(INota n);

        protected NotaGrouping ReportGrouping
        {
            get { return reportGrouping; }
            set { reportGrouping = value; }
        }

        protected int NotasPerPage
        {
            get { return notasPerPage; }
            set { notasPerPage = value; }
        }

        protected abstract string GetFileSuffix(INota[] notaGroup);

        protected virtual void GenerateParamNames(List<string> paramNames)
        {
            paramNames.Add("ShowLogo");
        }

        protected virtual void GenerateParamValues(List<string> paramValues, bool showLogo, INota[] notaGroup)
        {
            paramValues.Add(showLogo.ToString());
        }

        public virtual string HeaderDataTableName { get { return "ContactHeaderInfo"; } }
        public virtual string NotaDataTableName { get { return NotaType.ToString(); } }
        public virtual string ReportName { get { return NotaType.ToString(); } }

        protected virtual void BeforeDataSetBuild(IDalSession session, INota[] notaGroup) { }
        protected virtual void AfterDataSetBuild(IDalSession session, INota[] notaGroup, DataSet ds) { }

        public void PrintNotas(BatchExecutionResults results, int[] managementCompanyIds)
        {
            foreach (int managementCompanyId in managementCompanyIds)
                PrintNotas(results, managementCompanyId);
        }

        public void PrintNotas(BatchExecutionResults results, int managementCompanyId)
        {
            string companyDesc = managementCompanyId.ToString();

            try
            {
                ReportExecutionWrapper reportExecutionWrapper = new ReportExecutionWrapper();
                string pdfOnlineReportsFolder = null, pdfPostReportsFolder = null;
                bool showLogo;

                using (IDalSession session1 = NHSessionFactory.CreateSession())
                {
                    IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session1, managementCompanyId);
                    if (managementCompany == null)
                    {
                        results.MarkError(
                            new ApplicationException(string.Format("Management Company with ID '{0}' could not be found.", managementCompanyId)));
                        return;
                    }

                    companyDesc = managementCompany.CompanyName;

                    IReportTemplate reportTemplate = ReportTemplateMapper.GetReportTemplate(session1, managementCompany, ReportName, true);
                    reportExecutionWrapper.SetReportName(reportTemplate.ReportTemplateName);

                    List<string> paramNames = new List<string>();
                    GenerateParamNames(paramNames);
                    reportExecutionWrapper.AddParameters(paramNames.ToArray());

                    showLogo = managementCompany.ShowLogoByDefault;

                    pdfOnlineReportsFolder = getPdfReportsFolder(managementCompany, "Online", null);
                    pdfPostReportsFolder = getPdfReportsFolder(managementCompany, "Post", showLogo);
                }

                int[] accountIdsWithUnprintedNotas = null;
                
                using (IDalSession session2 = NHSessionFactory.CreateSession())
                {
                    accountIdsWithUnprintedNotas = NotaMapper.GetAccountsIdsWithUnprintedNotas(session2, managementCompanyId, NotaType);
                }

                foreach (int accountid in accountIdsWithUnprintedNotas)
                    printNotasForAccount(results, accountid, reportExecutionWrapper, pdfOnlineReportsFolder, pdfPostReportsFolder, showLogo);
            }
            catch (Exception ex)
            {
                results.MarkError(
                    new ApplicationException(string.Format("Error while preparing printing of nota's ({0}) for company '{1}'.",
                                             NotaType.ToString(), companyDesc), ex));
            }
        }

        private void printNotasForAccount(BatchExecutionResults results, int accountId, ReportExecutionWrapper reportExecutionWrapper,
                                          string pdfOnlineReportsFolder, string pdfPostReportsFolder, bool showLogo)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                try
                {
                    List<INota> notas = NotaMapper.GetUnprintedNotasByAccount(session, accountId, NotaType);
                    if (notas.Count > 0)
                    {
                        ICustomerAccount account = notas[0].Account;
                        foreach (INota[] notaGroup in ReportGrouping.GetGroups(notas))
                        {
                            try
                            {
                                DataSet ds = buildDataSet(session, notaGroup);

                                string pdfFileName = string.Format(@"{0}_{1:d4}_{2}_{3}.pdf",
                                                                   account.Number,
                                                                   account.AccountHolders.PrimaryAccountHolder.Contact.Key,
                                                                   notaGroup[0].NotaNumber,
                                                                   GetFileSuffix(notaGroup));
                                string pdfOnlineFullPath = string.Format(@"{0}\{1}", pdfOnlineReportsFolder, pdfFileName),
                                       pdfPostFullPath = string.Format(@"{0}\{1}", pdfPostReportsFolder, pdfFileName);

                                List<string> paramValues = new List<string>();
                                GenerateParamValues(paramValues, showLogo, notaGroup);

                                List<string> paramValuesWithShowLogo = copyParamValuesWithShowLogo(paramValues, reportExecutionWrapper.ExtraParamNames);

                                reportExecutionWrapper.Run(ds, pdfOnlineFullPath, paramValuesWithShowLogo.ToArray());

                                bool needsSendByPost = account.NeedsSendByPost(SendableDocumentCategories.NotasAndQuarterlyReports);
                                if (needsSendByPost)
                                {
                                    if (showLogo)
                                        File.Copy(pdfOnlineFullPath, pdfPostFullPath, true);
                                    else
                                        reportExecutionWrapper.Run(ds, pdfPostFullPath, paramValues.ToArray());
                                }

                                registerNotaPrinting(session, notaGroup, pdfFileName, pdfOnlineReportsFolder, needsSendByPost);
                                results.MarkSuccess(notaGroup.Length);
                            }
                            catch (Exception ex)
                            {
                                results.MarkError(new ApplicationException(
                                    string.Format("Error printing nota's ({0}) for account {1} ({2}), in PDF file containing nota number {3}.",
                                                  NotaType.ToString(), account.DisplayNumberWithName, accountId,
                                                  notaGroup[0].NotaNumber), ex));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ICustomerAccount errAccount = AccountMapper.GetAccount(session, accountId) as ICustomerAccount;
                    string accountNumberWithName = (errAccount != null ? errAccount.DisplayNumberWithName : "");
                    results.MarkError(new ApplicationException(string.Format("Error retrieving nota's ({0}) for account {1} ({2}).",
                                                                             NotaType.ToString(), accountNumberWithName, accountId), ex));
                }
            }
        }

        // Used for dumping data into an XML file, for testing with the Report Designer
        public static DataSet BuildTestDataSet(IDalSession session, int notaId)
        {
            INota nota = NotaMapper.GetNota(session, notaId);
            if (nota != null)
                return NotaPrintCommand.CreateNew(NotaPrintCommand.getNotaReturnClass(nota)).buildDataSet(session, new INota[] { nota });
            else
                throw new ArgumentException(string.Format("No Nota found with ID {0}.", notaId));
        }

        // Used for dumping data into an XML file, for testing with the Report Designer
        public static DataSet BuildTestDataSet(IDalSession session, int accountId, NotaReturnClass notaType)
        {
            return NotaPrintCommand.CreateNew(notaType).buildTestDataSet(session, accountId);
        }

        private DataSet buildTestDataSet(IDalSession session, int accountId)
        {
            IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);
            if (account != null)
            {
                IList notas = NotaMapper.GetUnprintedNotasByAccount(session, accountId, NotaType);
                if (notas.Count > 0)
                    // for testing, dump only the first group
                    return buildDataSet(session, ReportGrouping.GetGroups(notas)[0]);
                else
                    throw new ArgumentException(string.Format("No unprinted notas found for AccountID {0}.", accountId));
            }
            else
                throw new ArgumentException(string.Format("{0} is not a valid AccountID.", accountId));
        }

        private DataSet buildDataSet(IDalSession session, INota[] notaGroup)
        {
            BeforeDataSetBuild(session, notaGroup);

            DataSet ds = new DataSet();
            
            DataTable dt = notaGroup.Take(1)
                                    .Select<INota, object>(GetHeaderFields)
                                    .ToDataTable(HeaderDataTableName);
            ds.Tables.Add(dt);

            dt = notaGroup.Select<INota, object>(GetNotaFields)
                          .ToDataTable(NotaDataTableName);
            createPageGrouping(dt);
            ds.Tables.Add(dt);

            AfterDataSetBuild(session, notaGroup, ds);

            return ds;
        }

        private List<string> copyParamValuesWithShowLogo(List<string> paramValues, List<string> paramNames)
        {
            List<string> paramValuesWithShowLogo = paramValues.Select(p => p).ToList();
            int showLogoParamIndex = paramNames.FindIndex(name => name == "ShowLogo");
            paramValuesWithShowLogo[showLogoParamIndex] = true.ToString();
            
            return paramValuesWithShowLogo;
        }

        private string getPdfReportsFolder(IManagementCompany managementCompany, string purposeSubfolder, bool? showLogo)
        {
            if (managementCompany.PdfReportsFolder != null && managementCompany.PdfReportsFolder != string.Empty)
            {
                string pdfReportsFolder = string.Format(@"{0}\Notas\{1}\{2:yyyy.MM.dd}{3}",
                                                        managementCompany.PdfReportsFolder, purposeSubfolder, DateTime.Today,
                                                        showLogo == null ? "" : (bool)showLogo ? @"\WithLogo" : @"\NoLogo");
                if (!Directory.Exists(pdfReportsFolder))
                    Directory.CreateDirectory(pdfReportsFolder);
                return pdfReportsFolder;
            }
            else
                throw new ApplicationException(string.Format("PDF report-generation folder not set for management company '{0}'.",
                                                             managementCompany.CompanyName));
        }

        protected string[] ConcatenateArrays(string[] a, string[] b)
        {
            string[] result = new string[a.Length + b.Length];
            a.CopyTo(result, 0);
            b.CopyTo(result, a.Length);
            return result;
        }

        private void createPageGrouping(DataTable dt)
        {
            if (NotasPerPage > 0)
            {
                dt.Columns.Add("PageNumber", typeof(int));
                int rowNumber = 0;
                foreach (DataRow row in dt.Rows)
                    row["PageNumber"] = (rowNumber++ / NotasPerPage) + 1;
            }
        }

        private void registerNotaPrinting(IDalSession session, INota[] notas, string pdfFileName, string pdfOnlineReportsFolder, bool sentByPost)
        {
            // TODO: create a document object only when PrintCount is zero (in case this is a reprint)
            INotaDocument document = new NotaDocument(pdfFileName, pdfOnlineReportsFolder, sentByPost);
            DocumentMapper.Update(session, document);

            foreach (INota nota in notas)
            {
                nota.Document = document;
                nota.IncrementPrintCount();
            }

            NotaMapper.Update(session, notas);
        }

        private static NotaReturnClass getNotaReturnClass(INota nota)
        {
            if (nota is INotaTransaction)
                return NotaReturnClass.NotaTransaction;
            else if (nota is INotaDeposit)
                return NotaReturnClass.NotaDeposit;
            else if (nota is INotaDividend)
                return NotaReturnClass.NotaDividend;
            else if (nota is INotaTransfer)
                return NotaReturnClass.NotaTransfer;
            else if (nota is INotaFees)
                return NotaReturnClass.NotaFees;
            else if (nota is INotaInstrumentConversion)
                return NotaReturnClass.NotaInstrumentConversion;
            else
                return NotaReturnClass.Nota;
        }

        private static string getPathFromConfigFile(string configFileEntry)
        {
            string path = ConfigurationManager.AppSettings.Get(configFileEntry);
            if (path != null)
            {
                if (Directory.Exists(path))
                    return path;
                else
                    throw new DirectoryNotFoundException(
                        string.Format("Could not find folder {0}. Please create this folder or change entry '{1}' in the config file to point to a valid folder.",
                                      path, configFileEntry));
            }
            else
                throw new ConfigurationErrorsException(string.Format("Could not find entry '{0}' in config file.", configFileEntry));

        }

        private NotaReturnClass notaType;
        private NotaGrouping reportGrouping;
        private int notasPerPage;
    }
}
