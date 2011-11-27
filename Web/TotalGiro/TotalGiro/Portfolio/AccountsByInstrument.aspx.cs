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
using B4F.TotalGiro.Instruments;
using System.Text;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class Portfolio_AccountsByInstrument : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Accounts by Instrument";
            
            if (isLoggedInAsStichting)
            {
                gvPositions.MultipleSelection = false;
                btnClosePositions.Visible = false;
                chkNoCharges.Visible = false;
            }

            gvPositions.Sort("Account_ShortName", SortDirection.Ascending);
        }

        mvwButtons.ActiveViewIndex = 0;
        lblWarningMessages.Text = "";

        lblErrorMessage.Text = "";
    }

    private bool isLoggedInAsStichting = AccountFinderAdapter.IsLoggedInAsStichting();

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        pnlSelectedInstrument.Visible = true;
        ddlSelectedInstrument.DataBind();

        if (ddlSelectedInstrument.Items.Count != 2)
        {
            ddlSelectedInstrument.SelectedIndex = 0;
            pnlAccounts.Visible = false;
        }
        else
        {
            ddlSelectedInstrument.SelectedIndex = 1;
            ddlSelectedInstrument_SelectedIndexChanged(ddlSelectedInstrument, EventArgs.Empty);
        }
    }

    protected void ddlSelectedInstrument_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            pnlAccounts.Visible = true;
            showInstrumentDetails();
            
            if (!isLoggedInAsStichting)
                gvPositions.ClearSelection();
            gvPositions.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("{0}<br /><br />", ex.Message);
        }
    }

    private void showInstrumentDetails()
    {
        lblTotalSize.Text = "";
        lblTotalValue.Text = "";
        lblPrice.Text = "";
        lblExchangeRate.Text = "";

        if (ddlSelectedInstrument.SelectedValue != "")
        {
            int instrumentId = int.Parse(ddlSelectedInstrument.SelectedValue);
            if (instrumentId > 0)
            {
                decimal totalSize, exchangeRate;
                Money totalValue;
                Price price;

                AccountsByInstrumentAdapter.GetInstrumentDetails(instrumentId, out totalSize, out totalValue, out price, out exchangeRate);

                lblTotalSize.Text = string.Format("{0:###,##0.000000}", totalSize);
                lblTotalValue.Text = totalValue.DisplayString;
                lblPrice.Text = (price != null ? price.ShortDisplayString : "");
                lblExchangeRate.Text = (exchangeRate != 1m ? string.Format("{0:###,##0.0000}", exchangeRate) : "");
            }
        }
    }

    protected void btnClosePositions_Click(object sender, EventArgs e)
    {
        try
        {
            int[] positionIds = gvPositions.GetSelectedIds();
            string[] warningMessages = ClientPortfolioAdapter.ClosePositions(positionIds, false, chkNoCharges.Checked);
            if (warningMessages.Length > 0)
            {
                ViewState["positionIdsWithWarnings"] = positionIds;
                mvwButtons.ActiveViewIndex = 1;
                lblWarningMessages.Text = formatWarningMessages(warningMessages);
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("{0}<br /><br />", ex.Message);
        }
    }

    private string formatWarningMessages(string[] messages)
    {
        StringBuilder result = new StringBuilder();
        result.AppendFormat("WARNING{0}:<br />\n", (messages.Length == 1 ? "" : "S"));

        foreach (string message in messages)
            result.AppendFormat("{0}<br />\n", message);

        result.AppendFormat("Do you still want to place the order{0}?<br /><br />\n", (messages.Length == 1 ? "" : "s"));

        return result.ToString();
    }

    private string formatSuccessMessage(int positionsCount)
    {
        return string.Format("Successfully closed {0} position{1}.<br /><br />", positionsCount, (positionsCount > 1 ? "s" : ""));
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {
            int[] positionIdsWithWarnings = (int[])ViewState["positionIdsWithWarnings"];
            ClientPortfolioAdapter.ClosePositions(positionIdsWithWarnings, true, chkNoCharges.Checked);
            lblErrorMessage.Text = formatSuccessMessage(positionIdsWithWarnings.Length);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("{0}<br /><br />", ex.Message);
        }
        finally
        {
            ViewState["positionIdsWithWarnings"] = new int[0];
        }
    }

    protected void gvPositions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex == 0)
        {
            int positionsCount = ((DataRowView)e.Row.DataItem).Row.Table.Rows.Count;
            gvPositions.Caption = string.Format("Positions ({0})", positionsCount);
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["AccountId"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void gvPositions_DataBound(object sender, EventArgs e)
    {
        if (gvPositions.Rows.Count == 0)
            gvPositions.Caption = "Positions";
    }

    protected void gvPositions_RowCommand(object sender, GridViewCommandEventArgs e)
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
