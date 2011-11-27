using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.VirtualFunds;

public partial class VirtualFundNavOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Virtual Fund NAV Calculations Overview";

        }

        //lblErrorMessage.Text = "";
    }

    protected void lbtNew_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["CalculationID"] = VirtualFundNavOverviewAdapter.CreateNavCalculation(int.Parse((string)e.CommandArgument));
            Response.Redirect("~/VirtualFund/NavCalculationDetails.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtCalculations_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["fundID"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/VirtualFund/NavCalculationsOverview.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
