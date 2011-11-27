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
using B4F.TotalGiro.ApplicationLayer.Portfolio;

public partial class Portfolio_ClosedPositions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Closed Positions";

            showAccountDetails();
            showCashGridView();

            gvClosedCash.Sort("Instrument_DisplayName", SortDirection.Ascending);
            gvClosedPositions.Sort("InstrumentName", SortDirection.Ascending);
        }
    }

    private void showAccountDetails()
    {
        if (Session["SelectedAccountId"] != null)
        {
            int accountId = (int)Session["SelectedAccountId"];
            lblAccount.Text = ClosedPositionsAdapter.GetAccountDescription(accountId);
        }
    }

    private void showCashGridView()
    {
        bool gridViewVisible = false;

        if (Session["SelectedAccountId"] != null)
        {
            int accountId = (int)Session["SelectedAccountId"];
            //if (accountId > 0)
            //    gridViewVisible = (ClosedPositionsAdapter.GetClosedCashPositions(accountId).Tables[0].Rows.Count > 0);
        }

        pnlCash.Visible = gridViewVisible;
    }

    protected void gvClosedPositions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex == 0)
        {
            int positionsCount = ((DataRowView)e.Row.DataItem).Row.Table.Rows.Count;
            gvClosedPositions.Caption = string.Format("Closed Positions ({0})", positionsCount);
        }
    }

    protected void gvClosedPositions_DataBound(object sender, EventArgs e)
    {
        if (gvClosedPositions.Rows.Count == 0)
            gvClosedPositions.Caption = "Closed Positions";
    }

    protected void gridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.ToUpper() == "SELECT")
        {
            int rowIndex = int.Parse((string)e.CommandArgument);
            int positionId = (int)((GridView)e.CommandSource).DataKeys[rowIndex].Value;

            Session["SelectedPositionId"] = positionId;
            Response.Redirect("PositionTransactions.aspx");
        }
    }
}
