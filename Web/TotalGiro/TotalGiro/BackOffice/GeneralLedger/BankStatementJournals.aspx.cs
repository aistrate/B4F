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
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;

public partial class BankStatementJournals : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Bank Statement Journals";
            gvJournals.Sort("JournalNumber", SortDirection.Ascending);
        }
    }

    protected void lbtNew_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["journalEntryId"] = BankStatementsAdapter.CreateBankStatement(int.Parse((string)e.CommandArgument));
            Response.Redirect("~/BackOffice/GeneralLedger/BankStatementLines.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtStatements_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["journalId"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/GeneralLedger/BankStatements.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
