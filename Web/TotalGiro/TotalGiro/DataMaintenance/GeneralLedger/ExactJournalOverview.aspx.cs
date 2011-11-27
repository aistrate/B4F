using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ExactJournalOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Exact Journal Overview";
            //this.gvGLJournals.Sort("JournalNumber", SortDirection.Ascending);
        }
    }


    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["glJournalID"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/DataMaintenance/GeneralLedger/JournalDetails.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}

