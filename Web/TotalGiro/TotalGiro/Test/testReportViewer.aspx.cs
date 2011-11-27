using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.ApplicationLayer.Reports;
using B4F.TotalGiro.Utils;
using System.Drawing;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.Reports.Financial;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class Reports_testReportViewer : System.Web.UI.Page
{
    public enum ReportAction
    {
        Print = 0,
        Preview = 1,
        ViewResult = 2
    }

    private string reportLetterType;
    private DateTime beginDate, endDate;

    private bool portfolioDevelopment;
    private bool portfolioOverview;
    private bool portfolioSummary;
    private bool moneyMutations;
    private bool transactionOverview;
    private bool chartcover;
    private bool fiscalYearOverview;
    private bool reportNeedToReprint;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }
    protected void btnShowReport_Click(object sender, EventArgs e)
    {
        byte[] reportContent;

        int year = 2007;
        string txtBetreft;
        string txtOmschrijving;
        string reportLetterType = "";
        string lblErrorMessage = "";
        int currentManagementCompanyId = 10;
        int accountId = Int32.Parse("681");

        txtBetreft = "Test ReportVier " + year.ToString();
        txtOmschrijving = "Hallo Wolrd";

        DateTime beginDate = new DateTime(2006, 12, 31);          //DateTime BeginDateOfYear;           // = new DateTime(now.Year, 1, 1);
        DateTime endDate = new DateTime(2007, 12, 31);

        try
        {
            // Print a Fiscal report in preview-mode.
            ReportViewer1.ServerReport.ReportPath = "/TotalGiro/Reports/TestByMlim";
            //reportContent = FinancialReportAdapter.ViewFiscalYearReports(accountId, beginDate, endDate,
            //txtBetreft, txtOmschrijving, currentManagementCompanyId, year, reportLetterType);

        }
        catch (Exception ex)
        {
            lblErrorMessage = "Error: " + ex.Message;
        }



    }
}
