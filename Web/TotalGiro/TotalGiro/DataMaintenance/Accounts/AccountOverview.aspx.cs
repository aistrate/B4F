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
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.Security;
using System.Drawing;
using B4F.TotalGiro.Accounts;

public partial class AccountOverview : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Accounts Overview";
            gvAccounts.Sort("Number", SortDirection.Ascending);
            gvImportedAccounts.Sort("CreationDate", SortDirection.Descending);
            btnAddAccountsFromEffectenGiro.Visible = AccountOverviewAdapter.HasEffectenGiroRights();
            ctlAccountFinder.ShowLifecycle = AccountOverviewAdapter.ShowLifecycle();
        }
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        mvwAccounts.ActiveViewIndex = 0;
        pnlAccounts.Visible = true;
        gvAccounts.EditIndex = -1;
        gvAccounts.DataBind();
        //btnAddNewAccount.Visible = !AccountFinderAdapter.IsLoggedInAsStichting();
    }

    protected void gvAccounts_DataBinding(object sender, EventArgs e)
    {
        UserHasEditRights = SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit");
    }
    
    protected void gvAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();

            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null && !(bool)(((DataRowView)e.Row.DataItem)["IsOpen"]))
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Font.Bold = true;
            }
        }
    }

    protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string commName = e.CommandName.ToUpper();
        if (commName.Equals("EDITACCOUNT"))
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int giroaccountkey = (int)gvAccounts.SelectedDataKey.Value;
                    Session["accountnrid"] = giroaccountkey;
                    Response.Redirect("Account.aspx");
                }
            }
        }
    }
    protected void gvImportedAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null && !(bool)(((DataRowView)e.Row.DataItem)["IsOpen"]))
            {
                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Font.Bold = true;
            }
        }
    }

    protected void gvImportedAccounts_DataBinding(object sender, EventArgs e)
    {
        UserHasEditRights = SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit");
    }

    protected void gvImportedAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string commName = e.CommandName.ToUpper();
        if (commName.Equals("EDITACCOUNT"))
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvImportedAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int giroaccountkey = (int)gvImportedAccounts.SelectedDataKey.Value;
                    Session["accountnrid"] = giroaccountkey;
                    Response.Redirect("Account.aspx");
                }
            }
        }
    }


    protected bool UserHasEditRights = false;

    protected void btnAddAccountsFromEffectenGiro_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds = AccountOverviewAdapter.ImportAccountsFromEffectenGiro();

            gvAccounts.DataSource = ds;

            this.mvwAccounts.ActiveViewIndex = 1;
            pnlImportedAccounts.Visible = true;
            gvImportedAccounts.EditIndex = -1;
            gvImportedAccounts.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }





}
