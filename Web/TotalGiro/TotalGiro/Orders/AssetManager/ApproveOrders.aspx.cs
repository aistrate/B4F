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
using B4F.TotalGiro.ApplicationLayer.Orders.AssetManager;

public partial class ApproveOrders : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        try
        {
            ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Approve Orders";
                gvUnapprovedOrders.Sort("AccountName", SortDirection.Ascending);
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        try
        {
            gvUnapprovedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        try
        {
            ApproveOrdersAdapter.ApproveOrdersPerAccount(gvUnapprovedOrders.GetSelectedIds());
            gvUnapprovedOrders.DataBind();

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
    
    protected void gvUnapprovedOrders_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.ToUpper() == "SELECT")
            {
                int rowIndex = int.Parse((string)e.CommandArgument);
                int accountId = (int)gvUnapprovedOrders.DataKeys[rowIndex].Value;

                Session["ApproveOrdersSelectedID"] = accountId;
                Response.Redirect("ApproveOrdersChildren.aspx");
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvUnapprovedOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["AccountId"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

}
