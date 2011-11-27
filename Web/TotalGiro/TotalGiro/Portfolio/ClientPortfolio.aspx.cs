using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using B4F.TotalGiro.ApplicationLayer;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using System.Collections.Specialized;
using System.Drawing;
using B4F.TotalGiro.Notifications;
using B4F.TotalGiro.Security;

public partial class ClientPortfolio : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Client Portfolio";

            AllowClosePositions = SecurityManager.IsCurrentUserInRole("Portfolio: Client Portfolio Allow Execute");
            AllowRebalance = AllowClosePositions;

            readUrlParameters();

            //gvCash.Sort("PositionCurrency_DisplayName", SortDirection.Ascending);
            gvPositions.Sort("InstrumentName", SortDirection.Ascending);
        }

        mvwButtons.ActiveViewIndex = 0;
        lblWarningMessages.Text = "";

        ErrorMessage = "";
    }

    protected bool AllowClosePositions
    {
        get { return btnClosePositions.Visible; }
        set
        {
            btnClosePositions.Visible = value;
            gvPositions.MultipleSelection = value;
        }
    }

    protected bool AllowRebalance
    {
        get { return btnRebalance.Visible; }
        set
        {
            btnRebalance.Visible = value;
            btnLiquidate.Visible = value;
            chkNoCharges.Visible = value;
        }
    }

    protected string ErrorMessage
    {
        get
        {
            return (pnlErrorMessage.Visible ? lblErrorMessage.Text : string.Empty);
        }
        set
        {
            if (value.Trim() != string.Empty)
            {
                pnlErrorMessage.Visible = true;
                lblErrorMessage.Text = value;
                lblErrorMessage.ForeColor = Color.Red;
            }
            else
            {
                pnlErrorMessage.Visible = false;
                lblErrorMessage.Text = string.Empty;
            }
        }
    }

    private void readUrlParameters()
    {
        NameValueCollection pageParams = QueryStringModule.ParseQueryString(HttpContext.Current.Request.RawUrl);
        bool active = true, inactive = true, refresh = false;
        if (pageParams != null && pageParams.Count > 0)
        {
            foreach (string param in pageParams.AllKeys)
            {
                switch (param.ToLower())
                {
                    case "accountnumber":
                        ctlAccountFinder.AccountNumber = HttpUtility.UrlDecode(pageParams[param]);
                        refresh = true;
                        break;
                    case "active":
                        if (bool.TryParse(HttpUtility.UrlDecode(pageParams[param]), out active))
                            refresh = true;
                        break;
                    case "inactive":
                        if (bool.TryParse(HttpUtility.UrlDecode(pageParams[param]), out inactive))
                            refresh = true;
                        break;
                    default:
                        continue;
                }
            }
        }
        if (active && inactive)
            ctlAccountFinder.ContactActiveAll = true;
        else if (active)
            ctlAccountFinder.ContactActive = active;
        else if (inactive)
            ctlAccountFinder.ContactInactive = inactive;
        
        if (refresh)
            doSearch();
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        doSearch();
    }

    private void doSearch()
    {
        pnlSelectedAccount.Visible = true;
        ddlSelectedAccount.DataBind();

        if (ddlSelectedAccount.Items.Count != 2)
        {
            ddlSelectedAccount.SelectedIndex = 0;
            pnlClientPortfolio.Visible = false;
        }
        else
        {
            ddlSelectedAccount.SelectedIndex = 1;
            ddlSelectedAccount_SelectedIndexChanged(ddlSelectedAccount, EventArgs.Empty);
        }
    }

    protected void ddlSelectedAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlSelectedAccount.SelectedIndex != 0)
            {
                pnlClientPortfolio.Visible = true;

                showAccountDetails();

                showCashGridView();

                if (gvPositions.MultipleSelection)
                    gvPositions.ClearSelection();
                gvPositions.DataBind();
            }
            else
                pnlClientPortfolio.Visible = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void chkBuyingPowerFilter_CheckedChanged(object sender, EventArgs e)
    {
        gvBuyingPower.DataBind();
    }

    private void showAccountDetails()
    {
        lblAccountName.Text = string.Empty;
        lblContactStreet.Text = string.Empty;
        lblContactCity.Text = string.Empty;
        lblModelName.Text = string.Empty;
        ctlAccountLabel.Clear();
        lblStatus.Text = string.Empty;
        lblStatus.ForeColor = Color.Black;
        btnClosePositions.Enabled = true;
        btnRebalance.Enabled = true;
        btnLiquidate.Enabled = true;
        chkNoCharges.Enabled = true;
        lblTotalCash.Text = string.Empty;
        lblTotalPositions.Text = string.Empty;
        lblTotal.Text = string.Empty;
        lblLastRebalance.Text = string.Empty;
        lblCurrentRebalance.Text = string.Empty;
        lblRemisier.Text = string.Empty;
        lblRemisierEmployee.Text = string.Empty;
        lblFutureWithdrawals.Text = string.Empty;

        if (ddlSelectedAccount.SelectedValue != string.Empty)
        {
            int accountId = int.Parse(ddlSelectedAccount.SelectedValue);
            if (accountId != 0)
            {
                AccountDetailsView accountDetailsView  = ClientPortfolioAdapter.GetAccountDetails(accountId);

                lblAccountName.Text = accountDetailsView.AccountName;
                lblContactStreet.Text = accountDetailsView.StreetAddressLine;
                lblContactCity.Text = accountDetailsView.CityAddressLine;
                lblModelName.Text = accountDetailsView.ModelName;
                ctlAccountLabel.AccountID = accountId;
                ctlAccountLabel.AccountNumber = accountDetailsView.AccountNumber;
                ctlAccountLabel.Notification = accountDetailsView.Notification;
                ctlAccountLabel.NotificationType = accountDetailsView.NotificationType;
                ctlAccountLabel.AccountIsActive = (accountDetailsView.Status == AccountStati.Active);
                ctlAccountLabel.AccountIsDeparting = accountDetailsView.IsDeparting;
                ctlAccountLabel.AccountIsUnderRebalance = accountDetailsView.IsUnderRebalance;
                ctlAccountLabel.AccountActiveOrderCount = accountDetailsView.ActiveOrderCount;

                lblStatus.Text = accountDetailsView.Status.ToString();
                lblRemisier.Text = accountDetailsView.Remisier;
                lblRemisierEmployee.Text = accountDetailsView.RemisierEmployee;
                if (!accountDetailsView.StatusIsOpen)
                {
                    lblStatus.ForeColor = Color.Red;
                    btnClosePositions.Enabled = false;
                    btnRebalance.Enabled = false;
                    btnLiquidate.Enabled = false;
                    chkNoCharges.Enabled = false;
                }
                lblTotalPositions.Text = accountDetailsView.TotalPositions;
                lblTotalCash.Text = accountDetailsView.TotalCash;
                lblTotal.Text = accountDetailsView.TotalAll;
                lblLastRebalance.Text = (accountDetailsView.LastRebalanceDate == DateTime.MinValue ? string.Empty : accountDetailsView.LastRebalanceDate.ToString("dd-MM-yyyy"));
                lblCurrentRebalance.Text = (accountDetailsView.CurrentRebalanceDate == DateTime.MinValue ? string.Empty : accountDetailsView.CurrentRebalanceDate.ToString("dd-MM-yyyy"));
                if (accountDetailsView.FutureWithdrawalAmount != 0M)
                {
                    lblFutureWithdrawalsLabel.Visible = true;
                    lblFutureWithdrawals.Text = accountDetailsView.DisplayFutureWithdrawalAmount;
                }
                else
                    lblFutureWithdrawalsLabel.Visible = false;

                AllowClosePositions =
                    (accountDetailsView.IsTradeable && SecurityManager.IsCurrentUserInRole("Portfolio: Client Portfolio Allow Execute")) ||
                    (accountDetailsView.IsCrumbleAccount && SecurityManager.IsCurrentUserInRole("Portfolio: Client Portfolio Allow POS"));
                
                AllowRebalance = (accountDetailsView.IsTradeable && SecurityManager.IsCurrentUserInRole("Portfolio: Client Portfolio Allow Execute"));
            }
        }
    }

    private void showCashGridView()
    {
        bool gridViewVisible = false;

        if (ddlSelectedAccount.SelectedValue != "")
        {
            int accountId = int.Parse(ddlSelectedAccount.SelectedValue);
            if (accountId != 0)
                //gridViewVisible = (ClientPortfolioAdapter.GetCash(accountId).Tables[0].Rows.Count > 0);
                gridViewVisible = ClientPortfolioAdapter.HasCashPosition(accountId);
        }

        //pnlCash.Visible = gridViewVisible;
        pnlCash.Visible = true;
    }

    protected void gvBuyingPower_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView dataRowView = (DataRowView)e.Row.DataItem;
            if ((bool)dataRowView["IsSubTotalLine"])
                e.Row.BackColor = System.Drawing.Color.Silver;
            else
                if ((bool)dataRowView["IsSummaryLine"])
                    e.Row.BackColor = System.Drawing.Color.FromArgb(0x99CCFF);
        }
    }

    protected void gvBuyingPower_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.ToUpper() == "VIEWCASHDETAILS")
            {
                int subposid = int.Parse((string)e.CommandArgument);
                if (subposid != 0)
                {
                    Session["SelectedSubPositionId"] = subposid;
                    Response.Redirect("CashPositionTransactions.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnClosePositions_Click(object sender, EventArgs e)
    {
        try
        {
            int[] positionIds = gvPositions.GetSelectedIds();
            
            if (positionIds.Length > 0)
            {
                string[] warningMessages = ClientPortfolioAdapter.ClosePositions(positionIds, false, chkNoCharges.Checked);
                if (warningMessages.Length > 0)
                {
                    ViewState["positionIdsWithWarnings"] = positionIds;
                    mvwButtons.ActiveViewIndex = 1;
                    lblWarningMessages.Text = formatWarningMessages(warningMessages);
                }
                else
                    ErrorMessage = formatSuccessMessage(positionIds.Length);
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            string errorMessage;
            if (SecurityManager.IsCurrentUserInRole("Portfolio: Client Portfolio Allow POS"))
                ClientPortfolioAdapter.AggregatePOSOrders(out errorMessage);
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnRebalance_Click(object sender, EventArgs e)
    {
        try
        {
            string message;
            int accountId = int.Parse(ddlSelectedAccount.SelectedValue);

            if (accountId != 0)
                if (ClientPortfolioAdapter.RebalanceAccount(accountId, chkNoCharges.Checked, out message))
                {
                    ErrorMessage = message;
                    lblErrorMessage.ForeColor = Color.Black;
                }
                else
                    throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            showAccountDetails();
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnLiquidate_Click(object sender, EventArgs e)
    {
        try
        {
            string message;
            int accountId = int.Parse(ddlSelectedAccount.SelectedValue);

            if (accountId != 0)
                if (ClientPortfolioAdapter.LiquidateAccount(accountId, chkNoCharges.Checked, out message))
                {
                    ErrorMessage = message;
                    lblErrorMessage.ForeColor = Color.Black;
                }
                else
                    throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            showAccountDetails();
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
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
        return string.Format("Successfully closed {0} position{1}.", positionsCount, (positionsCount != 1 ? "s" : ""));
    }

    protected void btnAccountDetails_Click(object sender, EventArgs e)
    {
        try
        {
            int accountId = int.Parse(ddlSelectedAccount.SelectedValue);
            if (accountId != 0)
            {
                Session["accountnrid"] = accountId;
                Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnShowClosedPositions_Click(object sender, EventArgs e)
    {
        try
        {
            int accountId = int.Parse(ddlSelectedAccount.SelectedValue);
            if (accountId != 0)
            {
                Session["SelectedAccountId"] = accountId;
                Response.Redirect("ClosedPositions.aspx");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {
            int[] positionIdsWithWarnings = (int[])ViewState["positionIdsWithWarnings"];
            ClientPortfolioAdapter.ClosePositions(positionIdsWithWarnings, true, chkNoCharges.Checked);
            ErrorMessage = formatSuccessMessage(positionIdsWithWarnings.Length);
        }
        catch (Exception ex)
        {
            ErrorMessage = Utility.GetCompleteExceptionMessage(ex);
        }
        finally
        {
            ViewState["positionIdsWithWarnings"] = new int[0];
        }
    }

    protected void gvPositions_DataBinding(object sender, EventArgs e)
    {
        accruedInterestVisible = false;
    }

    protected void gvPositions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex == 0)
        {
            int positionsCount = ((DataRowView)e.Row.DataItem).Row.Table.Rows.Count;
            gvPositions.Caption = string.Format("Positions ({0})", positionsCount);

        }

        if (e.Row.RowType == DataControlRowType.DataRow && (bool)((DataRowView)e.Row.DataItem)["ShowAccruedInterest"])
            accruedInterestVisible = true;
    }

    protected void gvPositions_DataBound(object sender, EventArgs e)
    {
        if (gvPositions.Rows.Count == 0)
            gvPositions.Caption = "Positions";

        gvPositions.Columns
            .Cast<DataControlField>()
            .Where(x => x.HeaderText == "Accr.Int.")
            .ToList()
            .ForEach(x => x.Visible = accruedInterestVisible);
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

    private bool accruedInterestVisible = false;
}
