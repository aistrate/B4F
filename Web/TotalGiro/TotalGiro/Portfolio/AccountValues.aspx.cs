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
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class AccountValues : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Account Values";
            gvAccounts.Sort("ShortName", SortDirection.Ascending);
            ctlAccountFinder.ShowLifecycle = AccountOverviewAdapter.ShowLifecycle();
        }
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        gvAccounts.DataBind();

        pnlAccounts.Visible = true;

        int count;
        Money totalAmount;
        AccountValuesAdapter.GetCountTotals(ctlAccountFinder.AssetManagerId, 
            ctlAccountFinder.RemisierId, ctlAccountFinder.RemisierEmployeeId,
            ctlAccountFinder.LifecycleId, ctlAccountFinder.ModelPortfolioId,
            ctlAccountFinder.AccountNumber, ctlAccountFinder.AccountName,
            ctlAccountFinder.ContactActive, ctlAccountFinder.ContactInactive,
            ctlAccountFinder.AccountTradeable, ctlAccountFinder.AccountNonTradeable,
            out count, out totalAmount);
        lblCount.Text = count.ToString();
        lblTotal.Text = totalAmount.DisplayString;
    }

    protected void gvAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }
}
