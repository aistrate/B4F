using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.VirtualFunds;

public partial class NavCalculationsOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "NAV Calculations Overview";

            int fundID = (Session["fundID"] != null ? (int)Session["fundID"] : int.MinValue);
            this.hdnFundID.Value = fundID.ToString();

            DateTime lastValuationDate = NavCalculationsOverviewAdapter.GetLastValuationDate(fundID);
            if (lastValuationDate != DateTime.MinValue)
                this.dpNavDateFrom.SelectedDate = lastValuationDate.AddDays(-7);
            this.dpNavDateTo.SelectedDate = DateTime.MinValue;

            this.gvCalculations.Sort("ValuationDate", SortDirection.Ascending);
            gvCalculations.DataBind();
            loadFundDetails();

        }
    }

    private void loadFundDetails()
    {
        B4F.TotalGiro.ApplicationLayer.VirtualFunds.NavCalculationDetailsAdapter.FundDetails fundDetails = B4F.TotalGiro.ApplicationLayer.VirtualFunds.NavCalculationDetailsAdapter.GetVirtualFundDetails(int.Parse(this.hdnFundID.Value));
        if (fundDetails != null)
        {
            this.txtFundName.Text = fundDetails.FundName;
        }
    }

    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["calculationID"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/VirtualFund/NavCalculationDetails.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
