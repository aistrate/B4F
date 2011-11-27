using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using B4F.TotalGiro.GeneralLedger.Journal;

public partial class TradingBookings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Trading Bookings";

            ctlJournalEntryFinder.JournalType = JournalTypes.ClientTransaction;

            int journalId = (Session["journalId"] != null ? (int)Session["journalId"] : int.MinValue);
            ctlJournalEntryFinder.JournalId = journalId;

            DateTime lastTransactionDate = BankStatementsAdapter.GetLastTransactionDate(journalId);
            if (lastTransactionDate != DateTime.MinValue)
                ctlJournalEntryFinder.TransactionDateFrom = lastTransactionDate.AddDays(-7);

            ctlJournalEntryFinder.Statuses = JournalEntryStati.New | JournalEntryStati.Booked | JournalEntryStati.Open;

            gvBookings.Sort("JournalEntryNumber", SortDirection.Ascending);
            gvBookings.DataBind();
        }

        lblErrorMessage.Text = "";

    }

    protected void lbtLines_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["journalEntryId"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/GeneralLedger/TradingBookingsLines.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

   
}
