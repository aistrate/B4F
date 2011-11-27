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

public partial class MemorialJournals : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Memorial Journals";
            gvJournals.Sort("JournalNumber", SortDirection.Ascending);
        }
    }

    protected void lbtNew_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int journalId = int.Parse((string)e.CommandArgument);
            Session["journalEntryId"] = MemorialBookingsAdapter.CreateMemorialBooking(journalId);
            Response.Redirect("~/BackOffice/GeneralLedger/MemorialBookingLines.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtBookings_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["journalId"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/GeneralLedger/MemorialBookings.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
