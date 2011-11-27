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
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;

public partial class MemorialBookings : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlJournalEntryFinder.Search += new EventHandler(ctlJournalEntryFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Memorial Bookings";

            ctlJournalEntryFinder.JournalType = JournalTypes.Memorial;

            int journalId = (Session["journalId"] != null ? (int)Session["journalId"] : int.MinValue);
            ctlJournalEntryFinder.JournalId = journalId;

            DateTime lastTransactionDate = BankStatementsAdapter.GetLastTransactionDate(journalId);
            if (lastTransactionDate != DateTime.MinValue)
                ctlJournalEntryFinder.TransactionDateFrom = lastTransactionDate.AddDays(-7);

            ctlJournalEntryFinder.Statuses = JournalEntryStati.New | JournalEntryStati.Booked | JournalEntryStati.Open;

            gvBookings.Sort("JournalEntryNumber", SortDirection.Ascending);
            gvBookings.DataBind();

            hdfOpenBookings.Value = MemorialBookingsAdapter.OpenMemorialBookingsCount(ctlJournalEntryFinder.JournalId).ToString();
        }

        lblErrorMessage.Text = "";
    }

    protected void ctlJournalEntryFinder_Search(object sender, EventArgs e)
    {
        gvBookings.DataBind();
    }
    
    protected void lbtLines_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["journalEntryId"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/GeneralLedger/MemorialBookingLines.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNewBooking_Click(object sender, EventArgs e)
    {
        try
        {
            if (ctlJournalEntryFinder.JournalId != 0)
            {
                Session["journalEntryId"] = MemorialBookingsAdapter.CreateMemorialBooking(ctlJournalEntryFinder.JournalId);
                Response.Redirect("~/BackOffice/GeneralLedger/MemorialBookingLines.aspx");
            }
            else
                lblErrorMessage.Text = "Please select a Journal in the drop-down list at the top of the page, press Search, then try again.";
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
