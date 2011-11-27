using System;
using System.Data;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Orders.AssetManager;
using B4F.TotalGiro.Orders;

public partial class Orders_AssetManager_NewPortfolios : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Manage Cash Deposits";
            this.rblNewCashChoice.SelectedIndex = 0;
            gvNewCustomers.Sort("Number", SortDirection.Ascending);
            gvNewCustomers.SelectedIndex = -1;
            gvAccountsNewCash.Sort("Number", SortDirection.Ascending);
            gvAccountsNewCash.SelectedIndex = -1;
            ViewState["EditMode"] = false;
        }

        lblError.Text = String.Empty;
        lblResult.Text = String.Empty;
    }

    protected void rblNewCashChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.mlvNewCashView.ActiveViewIndex = this.rblNewCashChoice.SelectedIndex;
        gvNewCustomers.SelectedIndex = -1;
        gvAccountsNewCash.SelectedIndex = -1;
        gvCashTransfers.Visible = false;
        btnHideCashTransfers.Visible = false;
        EditMode = false;
    }

    protected void mlvNewCashView_ActiveViewChanged(object sender, EventArgs e)
    {
        switch (mlvNewCashView.ActiveViewIndex)
        {
            case 0:
                gvNewCustomers.DataBind();
                break;
            case 1:
                gvAccountsNewCash.DataBind();
                break;
        }
    }

    protected void gvNewCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    lblError.Text = string.Empty;
                    gvNewCustomers.EditIndex = -1;

                    // Select row
                    gvNewCustomers.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int key = (int)gvNewCustomers.SelectedDataKey.Value;
                    hdnAccountID.Value = key.ToString();
                    hdnAction.Value = e.CommandName.ToUpper();

                    switch (e.CommandName.ToUpper())
                    {
                        case "CREATEPORTFOLIO":
                            if (!EditMode)
                            {
                                btnOK.Text = ((LinkButton)e.CommandSource).Text;
                                chkNoCommissionCharged.Checked = false;
                                EditMode = true;
                            }
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    protected void gvNewCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!((B4F.TotalGiro.Accounts.Tradeability)((DataRowView)e.Row.DataItem)["TradeableStatus"] == B4F.TotalGiro.Accounts.Tradeability.Tradeable &&
                (B4F.TotalGiro.Accounts.AccountStati)((DataRowView)e.Row.DataItem)["Status"] == B4F.TotalGiro.Accounts.AccountStati.Active))
            {
                e.Row.FindControl("lbtCreatePortfolio").Visible = false;
            }

            int accountId = (int)((DataRowView)e.Row.DataItem)["Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void gvAccountsNewCash_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        gvCashTransfers.Visible = false;
        btnHideCashTransfers.Visible = false;

        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    lblError.Text = string.Empty;
                    gvAccountsNewCash.EditIndex = -1;

                    // Select row
                    gvAccountsNewCash.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int key = (int)gvAccountsNewCash.SelectedDataKey.Value;
                    hdnAccountID.Value = key.ToString();
                    hdnAction.Value = e.CommandName;

                    switch (e.CommandName.ToUpper())
                    {
                        case "REBALANCE":
                        case "BUYMODEL":
                            if (!EditMode)
                            {
                                btnOK.Text = ((LinkButton)e.CommandSource).Text;
                                chkNoCommissionCharged.Checked = false;
                                decimal diff = 0M;
                                if (e.CommandArgument != null)
                                    decimal.TryParse((string)e.CommandArgument, out diff);
                                if (diff > 0M)
                                {
                                    pnlIncludeAllCash.Visible = true;
                                    chkIncludeAllCash.Checked = true;
                                }
                                else
                                    pnlIncludeAllCash.Visible = false;
                                hdnDepositCashPositionDiff.Value = diff.ToString();
                                EditMode = true;
                            }
                            break;
                        case "VIEWTRANSACTIONS":
                            gvCashTransfers.DataBind();
                            gvCashTransfers.Caption = string.Format("Cash Transfers from {0} {1}", tableRow.Cells[1].Text, tableRow.Cells[2].Text);
                            gvCashTransfers.Visible = true;
                            btnHideCashTransfers.Visible = true;
                            return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    protected void gvAccountsNewCash_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((bool)((DataRowView)e.Row.DataItem)["IsExecOnlyCustomer"])
            {
                e.Row.FindControl("lbtRebalance").Visible = false;
            }

            if (!((B4F.TotalGiro.Accounts.Tradeability)((DataRowView)e.Row.DataItem)["TradeableStatus"] == B4F.TotalGiro.Accounts.Tradeability.Tradeable &&
                (B4F.TotalGiro.Accounts.AccountStati)((DataRowView)e.Row.DataItem)["Status"] == B4F.TotalGiro.Accounts.AccountStati.Active))
            {
                e.Row.FindControl("lbtRebalance").Visible = false;
                e.Row.FindControl("lbtBuyModel").Visible = false;
            }

            int accountId = (int)((DataRowView)e.Row.DataItem)["Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void gvCashTransfers_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        lblError.Text = String.Empty;
        try
        {
            int key = (int)gvCashTransfers.DataKeys[e.NewSelectedIndex].Value;
            bool success = NewPortfoliosAdapter.SkipTransferForRebalance(key);
            if (success)
            {
                gvAccountsNewCash.DataBind();
                gvCashTransfers.DataBind();

                lblResult.Text = string.Format("Successfully skipped transaction {0}.", key.ToString());
            }
        }
        catch (ApplicationException ex)
        {
            lblError.Text = ex.Message;
        }
    }

    private bool EditMode
    {
        get { return (bool)ViewState["EditMode"]; }
        set
        {
            GridView grdView = null;
            string orderAction = "1";
            switch (mlvNewCashView.ActiveViewIndex)
            {
                case 0:
                    grdView = gvNewCustomers;
                    break;
                case 1:
                    grdView = gvAccountsNewCash;
                    orderAction = "16";
                    ddlOrderActionTypes.Enabled = true;
                    break;
            }
            if (grdView != null)
            {
                grdView.Enabled = !value;
                if (!value)
                {
                    gvNewCustomers.SelectedIndex = -1;
                    hdnDepositCashPositionDiff.Value = "";
                }
            }
            pnlDoAction.Visible = value;
            ddlOrderActionTypes.SelectedValue = orderAction;
            ViewState["EditMode"] = value;
        }
    }

    protected void btnOK_Click(object sender, EventArgs e)
    {
        lblError.Text = String.Empty;
        try
        {
            bool success = false;
            GridView grdView = null;
            string verb = string.Empty;
            int key = Convert.ToInt32(hdnAccountID.Value);
            string action = hdnAction.Value;

            OrderActionTypes orderActionType = OrderActionTypes.Deposit;
            if (ddlOrderActionTypes.SelectedValue != string.Empty)
                orderActionType = (OrderActionTypes)Convert.ToInt32(ddlOrderActionTypes.SelectedValue);

            switch (action.ToUpper())
            {
                case "CREATEPORTFOLIO":
                    success = NewPortfoliosAdapter.BuyModel(key, orderActionType, chkNoCommissionCharged.Checked, 0M, false);
                    grdView = gvNewCustomers;
                    verb = "created new portfolio for";
                    break;
                case "BUYMODEL":
                    decimal diff = 0M;
                    if (!string.IsNullOrEmpty(hdnDepositCashPositionDiff.Value))
                        decimal.TryParse(hdnDepositCashPositionDiff.Value, out diff);
                    bool includePrevCash = (chkIncludeAllCash.Visible && chkIncludeAllCash.Checked) || diff < 0M;
                    success = NewPortfoliosAdapter.BuyModel(key, orderActionType, chkNoCommissionCharged.Checked, diff, includePrevCash);
                    grdView = gvAccountsNewCash;
                    verb = "bought the model for";
                    break;
                case "REBALANCE":
                    success = NewPortfoliosAdapter.Rebalance(key, orderActionType, chkNoCommissionCharged.Checked);
                    grdView = gvAccountsNewCash;
                    verb = "rebalanced";
                    break;
            }

            if (success)
            {
                EditMode = false;
                if (grdView != null)
                {
                    grdView.SelectedIndex = -1;
                    grdView.DataBind();
                }

                string number;
                string shortName;
                if (NewPortfoliosAdapter.GetAccountDetails(key, out number, out shortName))
                    lblResult.Text = string.Format("Successfully {0} account {1} - {2}.", verb, number, shortName);
            }
        }
        catch (ApplicationException ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        EditMode = false;
    }

    protected void btnHideCashTransfers_Click(object sender, EventArgs e)
    {
        try
        {
            gvCashTransfers.Visible = false;
            gvCashTransfers.SelectedIndex = -1;
            btnHideCashTransfers.Visible = false;
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
}
