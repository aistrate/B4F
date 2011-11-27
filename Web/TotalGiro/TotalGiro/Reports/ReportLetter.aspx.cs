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
using System.Drawing;
//using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.ApplicationLayer.Reports;

public partial class Reports_ReportLetter : System.Web.UI.Page
{
    private int currentManagementCompanyId;
    private string report;
    private int year;
    private Boolean blnSaveSuccess;
    private string[] reports;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            ((EG)this.Master).setHeaderText = "Add Financial Reports letter";

            reports = new string[] { "Q1", "Q2", "Q3", "Q4", "EOY"};
            ddlReport.DataSource = reports;
            ddlReport.DataBind();

            if (ddlYear.Items.Count == 0)
            {
                ArrayList years = new ArrayList();
                for (int year = 2005; year <= DateTime.Today.Year; year++)
                    years.Add(year);

                ddlYear.DataSource = years;
                ddlYear.DataBind();

                ddlYear.SelectedIndex = ddlYear.Items.Count - 1;
            }

            lblBeginText.ForeColor = Color.DarkBlue;
            lblEndText.ForeColor = Color.DarkBlue;
        }
    }

    protected void btnTest_Click(object sender, EventArgs e)
    {
        report = ddlReport.SelectedValue.ToString();
        year = int.Parse(ddlYear.SelectedValue);

        currentManagementCompanyId = ReportLetterEditAdapter.GetCurrentManagementCompanyId();
        blnSaveSuccess =  ReportLetterEditAdapter.AddReportLetter(report, year, currentManagementCompanyId, txtConcerning.Text, txtDescription.Text);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        report = ddlReport.SelectedValue.ToString();
        year = int.Parse(ddlYear.SelectedValue);

        currentManagementCompanyId = ReportLetterEditAdapter.GetCurrentManagementCompanyId();
        blnSaveSuccess = ReportLetterEditAdapter.AddReportLetter(report, year, currentManagementCompanyId, txtConcerning.Text, txtDescription.Text);

        if (blnSaveSuccess)
        {
            txtConcerning.Enabled = false;
            txtDescription.Enabled = false;

            txtConcerning.BackColor = Color.Ivory;
            txtDescription.BackColor = Color.Ivory;
            lblStatus.Text = "Your text is saved.";
            btnPreview.Enabled = true;
        }
        else
        {
            txtConcerning.BackColor = Color.Red;
            txtDescription.BackColor = Color.Red;
            lblStatus.Text = "Your text is not saved.";
        }

        btnSave.Enabled = false;
    }
    protected void ddlReport_SelectedIndexChanged(object sender, EventArgs e)
    {
        refreshReportLetterDetails();
    }
    protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        refreshReportLetterDetails();
    }

    protected void refreshReportLetterDetails()
    {
        report = ddlReport.SelectedValue.ToString();
        year = int.Parse(ddlYear.SelectedValue);
        currentManagementCompanyId = ReportLetterEditAdapter.GetCurrentManagementCompanyId();
        lblStatus.Visible = false;
        
        string concerning, description;
        bool found = ReportLetterEditAdapter.GetLatestReportLetterDetails(currentManagementCompanyId, year, report, 
                                                                          out concerning, out description);

        if (found)
        {
            txtConcerning.Text = concerning;
            txtDescription.Text = description;

            txtConcerning.Enabled = false;
            txtDescription.Enabled = false;

            txtConcerning.BackColor = Color.Ivory;
            txtDescription.BackColor = Color.Ivory;
        }
        else
        {
            txtConcerning.Text = "There are no text availble for the report. Please enter your text.";
            txtDescription.Text = "There are no text availble for the report. Please enter your text.";
            txtConcerning.BackColor = Color.LightGray;
            txtDescription.BackColor = Color.LightGray;
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        txtConcerning.Text = "";
        txtDescription.Text = "";

        txtConcerning.Enabled = true;
        txtDescription.Enabled = true;

        txtConcerning.BackColor = Color.White;
        txtDescription.BackColor = Color.White;
        lblStatus.ForeColor = Color.DarkBlue;
        lblStatus.Text = "Please enter the text for your report.";
        lblStatus.Visible = true;
        btnSave.Enabled = true;
        btnPreview.Enabled = false;
    }

    protected void btnPreview_Click(object sender, EventArgs e)
    {
        byte[] reportContent;
        bool portfolioDevelopment = false;
        bool portfolioOverview = false;
        bool portfolioSummary = false;
        bool moneyMutations = false;
        bool transactionOverview = false;
        bool chartcover = true;
        string concerning = (txtConcerning.Text != "" ? txtConcerning.Text : String.Empty);
        string description = (txtDescription.Text !=""  ? txtDescription.Text : String.Empty);

        int AssetManagerId = ctlAccountFinder.AssetManagerId;
        int accountId = 47;

        DateTime[] monthDates  = new DateTime[2];
        monthDates = ReportLetterEditAdapter.GetBeginAndEndDatesForReporting(ddlReport.SelectedValue.ToString(), int.Parse(ddlYear.SelectedValue));
        string reportLetterType = ddlReport.SelectedValue.ToString();

        if (reportLetterType == "EOY" )
        {
            // Print a Fiscal report in preview-mode.
            reportContent = FinancialReportAdapter.ViewFiscalYearReports(accountId, monthDates[0], monthDates[1],
            concerning, description, AssetManagerId, int.Parse(ddlYear.SelectedValue), reportLetterType);
        }
        else
        {
            // Print a Quarter report in preview-mode.
            reportContent = FinancialReportAdapter.ViewQuarterReport(accountId, monthDates[0], monthDates[1],
            portfolioDevelopment, portfolioOverview, portfolioSummary, transactionOverview, moneyMutations,
            chartcover, concerning, description , AssetManagerId, reportLetterType);
        }

        Session["report"] = reportContent;
        Response.Redirect("~/Reports/ReportViewer.aspx");

    }
}
