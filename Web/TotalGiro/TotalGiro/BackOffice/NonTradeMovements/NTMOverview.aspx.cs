using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.ApplicationLayer.BackOffice;
using System.Data;

public partial class NTMOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Non Trade Movement Overview";
                this.gvNTMTransfers.Sort("Key", SortDirection.Descending);
            }
            lblErrorMessage.Text = "";
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["positionTransferID"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/NonTradeMovements/TransferBetweenClients.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNewTransfer_Click(object sender, EventArgs e)
    {
        try
        {
            Session["positionTransferID"] = TransferAdapter.CreateNewPositionTransfer();
            Response.Redirect("~/BackOffice/NonTradeMovements/TransferBetweenClients.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvNTMTransfers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountIdA = (int)((DataRowView)e.Row.DataItem)["AccountA_Key"];
            AccountLabel lblA = (AccountLabel)e.Row.FindControl("ctlAccountLabelA");
            lblA.AccountID = accountIdA;
            lblA.GetData();

            int accountIdB = (int)((DataRowView)e.Row.DataItem)["AccountB_Key"];
            AccountLabel lblB = (AccountLabel)e.Row.FindControl("ctlAccountLabelB");
            lblB.AccountID = accountIdB;
            lblB.GetData();
        }
    }
}
