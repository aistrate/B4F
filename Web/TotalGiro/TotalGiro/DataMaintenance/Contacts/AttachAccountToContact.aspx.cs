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
using B4F.TotalGiro.Accounts;

public partial class DataMaintenance_AttachAccountToContact : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
         base.OnInit(e);
         ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        try
        {
            pnlAccounts.Visible = true;

            if (AccountFinderAdapter.IsLoggedInAsStichting())
            {
                btnAddNewAccount.Enabled = false;
            }

            gvAccounts.EditIndex = -1;
            gvAccounts.DataBind();
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
                //string accountNumber;
                ((EG)this.Master).setHeaderText = "Attach an account to a contact";

                //accountNumber = Request.QueryString["accountnumber"];
                //if (accountNumber != null)
                //    ctlAccountFinder.AccountNumber = accountNumber;

                Utility.CreatePrevPageSession();

                int contactID = 0;

                if (Session["contactid"] != null)
                    contactID = (int)Session["contactid"];

                lblContact.Text = AttachAccountToContactEditAdapter.GetName(contactID);
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
    {
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
                            AttachAccountToContactEditAdapter.AddAccountHolder(int.Parse(Session["contactid"].ToString()), accountID);
                            Utility.NavigateToPrevPageSessionIfAny();
                            break;
                    }

                }
            }
            gvAccounts.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnAddNewAccount_Click(object sender, EventArgs e)
    {
        pnlCreateNewAccounts.Visible = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (IsNewAccountValid())
            {
                int familyID = int.Parse(ddlAccountFamily.SelectedValue);
                string accountShortName = tbShortName.Text;
                ICustomerAccount newBie = AccountOverviewAdapter.CreateMinimalAccount(familyID, accountShortName);

                AttachAccountToContactEditAdapter.AddAccountHolder(int.Parse(Session["contactid"].ToString()), newBie.Key);

                Session["accountnrid"] = newBie.Key;
                Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private bool IsNewAccountValid()
    {
        ValidatorCollection validators = null;
        bool isValid = true;
        validators = Page.Validators;
        foreach (IValidator validator in validators)
        {
            validator.Validate();
            if (!validator.IsValid)
                isValid = false;

        }
        return isValid;
    }
}
