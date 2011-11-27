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
using System.Globalization;
using System.Text;
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;
using B4F.TotalGiro.Orders;

public partial class MidOffice : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Mid Office";

            gvUnapprovedTrades.Sort("Key", SortDirection.Ascending);
            gvApprovedOrders.Sort("Key", SortDirection.Ascending);
            gvUnapprovedTrades.SelectedIndex = -1;
        }
    }

	protected void gvUnapprovedTrades_RowCommand(Object sender, GridViewCommandEventArgs e)
	{
        gvApprovedOrders.Visible = false;

        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvUnapprovedTrades.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int tradeID = (int)gvUnapprovedTrades.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "CANCELTRADE":
                        MidOfficeAdapter.CancelTrade(tradeID);
                        gvUnapprovedTrades.DataBind();
                        break;
                    case "VIEWORDER":
                        gvApprovedOrders.DataBind();
                        gvApprovedOrders.Visible = true;
                        return;
                }
            }
        }

        gvUnapprovedTrades.SelectedIndex = -1;
	}

	// Approve action collects the unapproved trades and approves them.
	protected void btnApprove_Click(object sender, EventArgs e)
	{
        try
        {
            lblErrorMessage.Text = "";
            int[] tradeIds = gvUnapprovedTrades.GetSelectedIds();

            if (tradeIds == null || tradeIds.Length == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            if (tradeIds.Length > 150)
                throw new ApplicationException("Too many orders selected. Fund Settle only allows a maximum of 150 orders.");
            if (tradeIds.Length > 0)
            {
                string errorMessage;
                MidOfficeAdapter.ApproveTrades(tradeIds, out errorMessage);

                lblErrorMessage.Text = errorMessage;

                gvUnapprovedTrades.ClearSelection();
                gvUnapprovedTrades.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "Error during trade approval: " + ex.Message;
        }
	}
}
