using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BonusDistributionOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((EG)this.Master).setHeaderText = "Bonus Distribution Overview";
        this.hdnStartDate.Value = DateTime.Now.AddYears(-3).ToString();
        this.hdnEndDate.Value = DateTime.Now.ToString();


    }


    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["instrumentHistoryID"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/CorporateActions/BonusDistributionDetails.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNewDistribution_Click(object sender, EventArgs e)
    {
        try
        {
            //if (checkFields())
            //{
                //DividendAdapter.DividendHistoryDetails newValue = getNewDividendDetails();
                //Session["instrumentHistoryID"] = DividendAdapter.CreateOrSaveDividendHistory(newValue);
                //Response.Redirect("~/BackOffice/CorporateActions/DividendDetails.aspx");
            //}

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    
}
