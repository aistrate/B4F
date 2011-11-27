using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Valuations.ReportedData;

public partial class EndYearManagement : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "End Year Management";
            this.CurrentBookYear = EndYearManagementAdapter.GetCurrentBookYear();
            setupPage();
        }

        lblErrorMessage.Text = "";
    }

    protected void btnCloseCurrentYear_Click(object sender, EventArgs e)
    {
        IPeriodicReporting lastReport = getLastReport();
        EndYearManagementAdapter.StoreEndTermValues(lastReport.ReportingPeriod.getNextReportingPeriodDetail());

    }

    protected IGLBookYear CurrentBookYear
    {
        get
        {
            object b = ViewState["CurrentBookYear"];
            return ((b == null) ? null : (IGLBookYear)b);
        }
        set { ViewState["CurrentBookYear"] = value; }
    }

    private IPeriodicReporting getLastReport()
    {
        return EndYearManagementAdapter.GetLastReportingPeriod();
    }

    private void setupPage()
    {
        DateTime currentDate = DateTime.Now;

        IPeriodicReporting lastReport = getLastReport();
        if (lastReport != null)
        {
            this.txtLastEndTermDescription.Text = lastReport.ToString();
            
        }
        this.txtEmdTermDate.Text = lastReport.EndTermDate.ToShortDateString();
        this.txtCurrentDate.Text = currentDate.ToShortDateString();
        ReportingPeriodDetail nextPeriod = lastReport.ReportingPeriod.getNextReportingPeriodDetail();
        this.txtNextPossiblePeriod.Text = nextPeriod.ToString();

        if (currentDate > nextPeriod.GetEndDate())
        {
            this.btnCloseCurrentYear.Text = string.Format(@"Close {0}", nextPeriod.ToString());
            this.btnCloseCurrentYear.Enabled = true;
            this.lblInformation.Text = string.Format(@"Next period is earlier than current Date : {0}", currentDate.ToLongDateString());
        }
        else
        {
            this.btnCloseCurrentYear.Text = string.Format(@"Close {0}", nextPeriod.ToString());
            this.btnCloseCurrentYear.Enabled = false;
            this.lblInformation.Text = "Cannot close next period as Current Date is less than End Date";
        }
    }

}
