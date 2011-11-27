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

public partial class Reports_ReportsVB : System.Web.UI.Page
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

    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Financial Reports";

            DateTime beginDate;
            DateTime endDate;
            string quarter;
            getNearestQuarterlyDates(out beginDate, out endDate, out quarter);

            string[] quarters = new string[] { "Q1", "Q2", "Q3", "Q4" };
            ddlQuarter.DataSource = quarters;
            ddlQuarter.DataBind();
            ddlQuarter.SelectedValue = quarter;

            ctlBeginDate.SelectedDate = beginDate;
            ctlEndDate.SelectedDate = endDate;

            cbUsePreselectedDates.Checked = true;
        }
        elbErrorMessage.Text = "";
    }

    protected void gvAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    private static void getNearestQuarterlyDates(out DateTime beginDate, out DateTime endDate, out string quarter)
    {
        DateTime now = DateTime.Now.Date;
        int months = (now.Month % 3) * -1;
        if (months == 0)
            months = -3;
        DateTime tempDate = now.AddMonths(months);
        endDate = Util.GetLastDayOfMonth(tempDate);
        beginDate = Util.GetFirstDayOfMonth(tempDate.AddMonths(-2));
        quarter = string.Format("Q{0}", (beginDate.Month - 1) / 3 + 1);
    }


    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        pnlSelectedAccounts.Visible = true;
        gvAccounts.DataBind();
        refreshReportLetterDetails();
    }

    protected void ddlQuarter_SelectedIndexChanged(object sender, EventArgs e)
    {
        refreshReportLetterDetails();
    }

    protected void setValues()
    {
        portfolioDevelopment = chkPortfolioDevelopment.Checked;
        portfolioOverview = chkPortfolioOverview.Checked;
        portfolioSummary = chkPortfolioSummary.Checked;
        moneyMutations = chkMoneyMutations.Checked;
        transactionOverview = chkTransaction.Checked;
        chartcover = rdoCover2.Checked;
        fiscalYearOverview = chkFiscaalJaaroverzicht.Checked;

        try
        {
            if (fiscalYearOverview)
            {
                beginDate = new DateTime((ctlAccountFinder.Year - 1), 12, 31);
                endDate = new DateTime(ctlAccountFinder.Year, 12, 31);
            }
            else
            {
                if (cbUsePreselectedDates.Checked)
                    Util.GetDatesFromQuarter(ctlAccountFinder.Year, ddlQuarter.SelectedValue, out beginDate, out endDate);
                else
                {
                    beginDate = ctlBeginDate.SelectedDate;
                    endDate = ctlEndDate.SelectedDate;
                }
            }

            if (chkFiscaalJaaroverzicht.Checked == true)
                reportLetterType = "EOY";
            else
                reportLetterType = ddlQuarter.SelectedValue;

        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Util.GetMessageFromException(ex);
        }
    }

    protected void DoReport(ReportAction reportAction)
    {
        byte[] reportContent;

        try
        {
            switch (reportAction)
            {
                case ReportAction.Print:
                    int printedCount = FinancialReportAdapter.PrintReports(gvAccounts.GetSelectedIds(), beginDate, endDate, 
                        portfolioDevelopment, portfolioOverview, portfolioSummary, transactionOverview, moneyMutations, chartcover,
                        txtConcerning.Text, txtDescription.Text, ctlAccountFinder.AssetManagerId, ctlAccountFinder.Year, reportLetterType);

                    if (printedCount > 0)
                        elbErrorMessage.Text = string.Format("{0} report{1} successfully printed.",
                                                             printedCount, (printedCount == 1 ? " was" : "s were"));
                    else
                        elbErrorMessage.Text = "No reports were printed.";
                    break;
                
                case ReportAction.Preview:
                    if (fiscalYearOverview)
                    {
                      // Print a Fiscal report in preview-mode.
                      reportContent = FinancialReportAdapter.ViewFiscalYearReports(gvAccounts.GetSelectedIds()[0], beginDate, endDate,
                      txtConcerning.Text, txtDescription.Text, ctlAccountFinder.AssetManagerId, ctlAccountFinder.Year, reportLetterType);
                    }
                    else
                    {
                      // Print a Quarter report in preview-mode.
                      reportContent = FinancialReportAdapter.ViewQuarterReport(gvAccounts.GetSelectedIds()[0], beginDate, endDate,
                      portfolioDevelopment, portfolioOverview, portfolioSummary, transactionOverview, moneyMutations,
                      chartcover, txtConcerning.Text, txtDescription.Text, ctlAccountFinder.AssetManagerId, reportLetterType);
                    }

                    Session["report"] = reportContent;
                    Response.Redirect("~/Reports/ReportViewer.aspx");
                    break;

                case ReportAction.ViewResult:
                    // Print a total result of the printed reports in preview-mode.
                    reportContent = ReportResultsAdapter.ViewPrintedReports(ctlAccountFinder.AssetManagerId, ctlAccountFinder.Year, reportLetterType);
                    Session["report"] = reportContent;
                    Response.Redirect("~/Reports/ReportViewer.aspx");
                    break;

                default:
                    elbErrorMessage.Text = "No action was selected.";
                    break;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Util.GetMessageFromException(ex);
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (!validateUserInput(true))
        {
            setValues();
            DoReport(ReportAction.Print);
        }
        btnPrint.Focus();
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        if (!validateUserInput(true))
        {
            setValues();
            DoReport(ReportAction.Preview);
        }
        btnView.Focus();
    }

    protected void btnViewResult_Click(object sender, EventArgs e)
    {
        if (!validateUserInput(false))
        {
            setValues();
            DoReport(ReportAction.ViewResult);
        }
        btnViewResult.Focus();
    }

    protected void cbUsePreselectedDates_CheckedChanged(object sender, EventArgs e)
    {
        ddlQuarter.Visible = cbUsePreselectedDates.Checked;
        ctlBeginDate.Visible = !cbUsePreselectedDates.Checked;
        ctlEndDate.Visible = !cbUsePreselectedDates.Checked;
        lblFrom.Visible = !cbUsePreselectedDates.Checked;
        lblTo.Visible = !cbUsePreselectedDates.Checked;
    }

    protected void chkFiscaalJaaroverzicht_CheckedChanged(object sender, EventArgs e)
    {
        if (chkFiscaalJaaroverzicht.Checked)
        {
            chkMoneyMutations.Checked = false;
            chkPortfolioDevelopment.Checked = false;
            chkPortfolioSummary.Checked = false;
            chkPortfolioOverview.Checked = false;
            chkTransaction.Checked = false;
            chkMoneyMutations.Checked = false;

            chkPortfolioDevelopment.Enabled = false;
            chkPortfolioSummary.Enabled = false;
            chkPortfolioOverview.Enabled = false;
            chkTransaction.Enabled = false;
            chkMoneyMutations.Enabled = false;

            ddlQuarter.Enabled = false;

            cbUsePreselectedDates.Enabled = false;

            rdoCover1.Enabled = false;
            rdoCover2.Enabled = false;

            refreshReportLetterDetails();
        }
        else
        {
            chkPortfolioDevelopment.Enabled = true;
            chkPortfolioSummary.Enabled = true;
            chkPortfolioOverview.Enabled = true;
            chkTransaction.Enabled = true;
            chkMoneyMutations.Enabled = true;

            ddlQuarter.Enabled = true;
            rdoCover1.Enabled = true;
            rdoCover2.Enabled = true;
            cbUsePreselectedDates.Enabled = true;
        }
    }

    protected void chkPortfolioDevelopment_CheckedChanged(object sender, EventArgs e)
    {
        if (chkPortfolioDevelopment.Checked && chkFiscaalJaaroverzicht.Enabled == true)
        {
            chkFiscaalJaaroverzicht.Checked = false;
            chkFiscaalJaaroverzicht.Enabled = false;
            refreshReportLetterDetails();
        }

        if (chkMoneyMutations.Checked == false && chkTransaction.Checked == false && chkPortfolioOverview.Checked == false &&
            chkPortfolioSummary.Checked == false && chkPortfolioDevelopment.Checked == false)
        {
            chkFiscaalJaaroverzicht.Enabled = true;
            chkFiscaalJaaroverzicht.Enabled = true;
        }
    }

    protected void chkPortfolioSummary_CheckedChanged(object sender, EventArgs e)
    {
        if (chkPortfolioSummary.Checked && chkFiscaalJaaroverzicht.Enabled == true)
        {
            chkFiscaalJaaroverzicht.Checked = false;
            chkFiscaalJaaroverzicht.Enabled = false;
            refreshReportLetterDetails();
        }

        if (chkMoneyMutations.Checked == false && chkTransaction.Checked == false && chkPortfolioOverview.Checked == false &&
            chkPortfolioSummary.Checked == false && chkPortfolioDevelopment.Checked == false)
        {
            chkFiscaalJaaroverzicht.Enabled = true;
        }
    }

    protected void chkPortfolioOverview_CheckedChanged(object sender, EventArgs e)
    {
        if (chkPortfolioOverview.Checked && chkFiscaalJaaroverzicht.Enabled == true)
        {
            chkFiscaalJaaroverzicht.Checked = false;
            chkFiscaalJaaroverzicht.Enabled = false;
            refreshReportLetterDetails();
        }

        if (chkMoneyMutations.Checked == false && chkTransaction.Checked == false && chkPortfolioOverview.Checked == false &&
            chkPortfolioSummary.Checked == false && chkPortfolioDevelopment.Checked == false)
        {
            chkFiscaalJaaroverzicht.Enabled = true;
        }
    }

    protected void chkTransaction_CheckedChanged(object sender, EventArgs e)
    {
        if (chkTransaction.Checked && chkFiscaalJaaroverzicht.Enabled == true)
        {
            chkFiscaalJaaroverzicht.Checked = false;
            chkFiscaalJaaroverzicht.Enabled = false;
            refreshReportLetterDetails();
        }

        if (chkMoneyMutations.Checked == false && chkTransaction.Checked == false && chkPortfolioOverview.Checked == false &&
            chkPortfolioSummary.Checked == false && chkPortfolioDevelopment.Checked == false)
        {
            chkFiscaalJaaroverzicht.Enabled = true;
        }
    }

    protected void chkMoneyMutations_CheckedChanged(object sender, EventArgs e)
    {
        if (chkMoneyMutations.Checked && chkFiscaalJaaroverzicht.Enabled == true)
        {
            chkFiscaalJaaroverzicht.Checked = false;
            chkFiscaalJaaroverzicht.Enabled = false;
            refreshReportLetterDetails();
        }

        if (chkMoneyMutations.Checked == false && chkTransaction.Checked == false && chkPortfolioOverview.Checked == false &&
            chkPortfolioSummary.Checked == false && chkPortfolioDevelopment.Checked == false) 
        {
            chkFiscaalJaaroverzicht.Enabled =true;
        }
    }

    protected void refreshReportLetterDetails()
    {
        if (chkFiscaalJaaroverzicht.Checked == true)
            reportLetterType = "EOY";
        else
            reportLetterType = ddlQuarter.SelectedValue;

        string concerning, description;
        bool found = ReportLetterEditAdapter.GetLatestReportLetterDetails(ctlAccountFinder.AssetManagerId, ctlAccountFinder.Year, reportLetterType,
                                                                          out concerning, out description);
        if (found)
        {
            txtConcerning.Text = concerning;
            txtDescription.Text = description;
            txtConcerning.BackColor = Color.Ivory;
            txtDescription.BackColor = Color.Ivory;
        }
        else
        {
            txtConcerning.Text = "Geen tekst aanwezig";
            txtDescription.Text = "Geen tekst aanwezig";
            txtConcerning.BackColor = Color.AntiqueWhite;
            txtDescription.BackColor = Color.AntiqueWhite;
        }
    }

    protected bool validateUserInput(bool completeCheck)
    {
        bool result = false;
        DateTime dtmMaxDate = DateTime.Now.Date.AddDays(-1);
        portfolioDevelopment = chkPortfolioDevelopment.Checked;
        portfolioOverview = chkPortfolioOverview.Checked;
        portfolioSummary = chkPortfolioSummary.Checked;
        moneyMutations = chkMoneyMutations.Checked;
        transactionOverview = chkTransaction.Checked;
        chartcover = rdoCover2.Checked;
        fiscalYearOverview = chkFiscaalJaaroverzicht.Checked;
 
        if (completeCheck)
        {
            // Required to select an account.
            if (gvAccounts.GetSelectedIds().Length == 0)
            {
                elbErrorMessage.Text = "Please select one or more accounts first.";
                result = true;
            }

            // Required to select a report.
            if ((!portfolioDevelopment) && (!portfolioOverview) && (!portfolioSummary) && (!moneyMutations) && (!transactionOverview) && (!fiscalYearOverview))
            {
                elbErrorMessage.Text = "Please select one or more reports first.";
                result = true;
            }

            // The endtermvalue should be close first before printing.
            if (cbUsePreselectedDates.Checked)
                Util.GetDatesFromQuarter(ctlAccountFinder.Year, ddlQuarter.SelectedValue, out beginDate, out endDate);
            else
                endDate = ctlEndDate.SelectedDate;

            if (!FinancialReportAdapter.getEndtermvaluesEnddateCount(endDate))
            {
                elbErrorMessage.Text = "The End Term Values for the Selected Period haven’t been closed yet. For printing an report you need to close the endtermvalue first.";
                result = true;
            }

        }

        // Required to select the begin- and end dates.
        if (!cbUsePreselectedDates.Checked)
        {
            if (ctlBeginDate.IsEmpty == true)
            {
                elbErrorMessage.Text = "Select a start date.";
                result = true;
            }
            else if (ctlEndDate.IsEmpty == true)
            {
                elbErrorMessage.Text = "Select an end date.";
                result = true;
            }
            else if (ctlBeginDate.SelectedDate >= ctlEndDate.SelectedDate)
            {
                elbErrorMessage.Text = "Start date cannot be greater than end date.";
                result = true;
            }
            else if (ctlBeginDate.SelectedDate > dtmMaxDate || ctlEndDate.SelectedDate > dtmMaxDate)
            {
                elbErrorMessage.Text = string.Format("Dates can not be greater than {0:dd-MM-yyyy}.", dtmMaxDate);
                result = true;
            }
        }

        return result;
    }
}
