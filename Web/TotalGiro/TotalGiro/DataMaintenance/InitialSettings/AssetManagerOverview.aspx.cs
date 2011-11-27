using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AssetManagerOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Asset Manager Overview";
            //this.gvNTMTransfers.Sort("TransferDate", SortDirection.Ascending);
        }
    }

    protected void btnNewAssetManger_Click(object sender, EventArgs e)
    {
        try
        {
            Session["assetManagerID"] = 0;
            Response.Redirect("~/DataMaintenance/InitialSettings/AssetManagerDetails.aspx");
            
        }
        catch (Exception ex)
        {
            //pnlErrorMess.Visible = true;
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["assetManagerID"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/DataMaintenance/InitialSettings/AssetManagerDetails.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
