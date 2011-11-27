using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Reports.Financial;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public class ReportResultsAdapter
    {
        public static byte[] ViewPrintedReports(int managementCompanyId, int year, string reportLetterTypeName)
        {
            DataSet dsPrintedReports = GetPrintedReportsDataSet(managementCompanyId, year, reportLetterTypeName);

            ReportExecutionWrapper wrapper = new ReportExecutionWrapper();
            wrapper.SetReportName(getReportTemplateName(managementCompanyId, ReportReturnClass.ResultOfPrintedReport.ToString()));
            wrapper.AddParameters(new string[] { "SelectedReportYear" });
            
            return wrapper.GetReportContent(dsPrintedReports, new string[] { year.ToString() });
        }

        // Also used by DumpDataSets.aspx
        public static DataSet GetPrintedReportsDataSet(int managementCompanyId, int year, string reportLetterTypeName)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                ReportLetterTypes reportLetterType = ReportLetterMapper.GetReportLetterType(reportLetterTypeName);

                DataSet dsPrintedReports = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            ReportMapper.GetReports(session, year, reportLetterType, managementCompanyId),
                            @"Key, Account.Key, ReportLetter.Key, ReportStatusId, ReportLetter.ReportLetterYear, ReportLetter.ReportLetterTypeId, 
                              ModelPortfolio.Key, ModelPortfolio.ModelName");

                return getPrintedReportsSummary(dsPrintedReports);
            }
            finally
            {
                session.Close();
            }
        }

        private static string getReportTemplateName(int managementCompanyId, string reportName)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IManagementCompany managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
                IReportTemplate reportTemplate = ReportTemplateMapper.GetReportTemplate(session, managementCompany, reportName, true);
                return reportTemplate.ReportTemplateName;
            }
            finally
            {
                session.Close();
            }
        }

        private static DataSet getPrintedReportsSummary(DataSet dsPrintedReports)
        {
            DataTable dtSummary = new DataTable("TotPrintedReportPerModel");
            dtSummary.Columns.Add("ModelId", typeof(int));
            dtSummary.Columns.Add("ModelName", typeof(string));
            dtSummary.Columns.Add("Year", typeof(int));
            dtSummary.Columns.Add("ReportLetterType", typeof(string));

            foreach (ReportStatuses reportStatus in Report.ReportStatusList)
            {
                dtSummary.Columns.Add(reportStatus.ToString(), typeof(int));
                dtSummary.Columns[reportStatus.ToString()].DefaultValue = 0;
            }

            foreach (DataRow reportStatusRow in dsPrintedReports.Tables[0].Rows)
            {
                string search = string.Format("ModelId = {0}", reportStatusRow["ModelPortfolio_Key"]);
                DataRow[] filteredSummaryRows = dtSummary.Select(search);

                DataRow summaryRow = null;
                if (filteredSummaryRows.Length > 0)
                    summaryRow = filteredSummaryRows[0];
                else
                {
                    summaryRow = dtSummary.NewRow();
                    summaryRow["ModelId"] = (int)reportStatusRow["ModelPortfolio_Key"];
                    summaryRow["ModelName"] = reportStatusRow["ModelPortfolio_ModelName"];
                    summaryRow["Year"] = (int)reportStatusRow["ReportLetter_ReportLetterYear"];
                    summaryRow["ReportLetterType"] = reportStatusRow["ReportLetter_ReportLetterTypeId"];
                    dtSummary.Rows.Add(summaryRow);
                }

                string countColumnName = ((ReportStatuses)reportStatusRow["ReportStatusId"]).ToString();
                summaryRow[countColumnName] = (int)summaryRow[countColumnName] + 1;
            }

            DataSet dsSummary = new DataSet();
            dsSummary.Tables.Add(dtSummary);
            return dsSummary;
        }
    }
}
