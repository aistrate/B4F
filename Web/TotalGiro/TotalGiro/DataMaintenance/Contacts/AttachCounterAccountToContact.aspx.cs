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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class DataMaintenance_AttachCounterAccountToContact : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
         base.OnInit(e);
         ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        pnlAccounts.Visible = true;

        if (AccountFinderAdapter.IsLoggedInAsStichting())
        {
            btnAddNewAccount.Enabled = false;
        }

        gvAccounts.EditIndex = -1;
        gvAccounts.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string accountNumber;
            ((EG)this.Master).setHeaderText = "Attach an account to a contact";

            accountNumber = Request.QueryString["accountnumber"];
            if (accountNumber != null)
                ctlAccountFinder.AccountNumber = accountNumber;

            Utility.CreatePrevPageSession();

            int contactID = 0;

            if (Session["contactid"] != null)
            {
                contactID = (int)Session["contactid"];
            }

            lblContact.Text = AttachAccountToContactEditAdapter.GetName(contactID);
        }
    }

    protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        lblError.Text = "";
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    // Select row
                    gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    int accountID = (int)gvAccounts.SelectedDataKey.Value;
                    switch (e.CommandName.ToUpper())
                    {
                        case "ADDACCOUNT":
                            gvAccounts.SelectedIndex = -1;
                            AttachCounterAccountToContactEditAdapter.AddCounterAccount(int.Parse(Session["contactid"].ToString()), accountID);
                            Utility.NavigateToPrevPageSessionIfAny();
                            break;
                    }

                }
            }
            gvAccounts.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnAddNewAccount_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        try
        {
            Session["counteraccountid"] = null;
            Response.Redirect("~/DataMaintenance/Contacts/CounterAccount.aspx");
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
