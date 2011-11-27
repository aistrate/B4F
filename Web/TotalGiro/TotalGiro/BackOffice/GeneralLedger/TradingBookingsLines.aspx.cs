using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using B4F.TotalGiro.GeneralLedger.Journal;

public partial class TradingBookingsLines : System.Web.UI.Page
{
    enum ViewOption
    {
        Lines = 0,
        Summary = 1
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMessageMain.Text = "";

        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Trading Booking Lines";

            JournalEntryId = (Session["journalEntryId"] != null ? (int)Session["journalEntryId"] : 0);
            this.gvSummary.Sort("giroacct_Number, account_FullDescription", SortDirection.Ascending);

            displayBookingDetails();
            SetupViews();
        }
    }

    protected void gvLines_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["GiroAccount_Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    public int JournalEntryId
    {
        get { return getIntegerValue(hdnJournalEntryId.Value); }
        set { hdnJournalEntryId.Value = value.ToString(); }
    }

    private int getIntegerValue(string value)
    {
        NumberStyles numberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                                    NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        int result = 0;
        if (value != string.Empty)
        {
            if (!int.TryParse(value, numberStyles, null, out result))
                throw new ArgumentException(string.Format("Invalid numeric format: '{0}'.", value));
        }

        return result;
    }

    private void displayBookingDetails()
    {
        DataSet ds = TradingBookingsLinesAdapter.GetTradingBookingsDetails(JournalEntryId);
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dataRow = ds.Tables[0].Rows[0];


            lblJournalEntryNumber.Text = (string)dataRow["JournalEntryNumber"];
            hdnJournalEntryId.Value = ((int)dataRow["Key"]).ToString();
            lblStatus.Text = (string)dataRow["DisplayStatus"];
            lblJournal.Text = (string)dataRow["Journal_FullDescription"];

            DateTime transactionDate = (DateTime)dataRow["TransactionDate"];
            lblTransactionDate.Text = (transactionDate != DateTime.MinValue ? transactionDate.ToString("dd-MM-yyyy") : "");

            lblDescription.Text = (dataRow["TradedInstrument"] != DBNull.Value ? (string)dataRow["TradedInstrument"] : "");
            lblVolume.Text = (dataRow["TradeSizeDisplay"] != DBNull.Value ? (string)dataRow["TradeSizeDisplay"] : "");
            lblCounterParty.Text = (dataRow["CounterParty"] != DBNull.Value ? (string)dataRow["CounterParty"] : "");

            lblPrice.Text = (dataRow["TradePrice_DisplayString"] != DBNull.Value ? (string)dataRow["TradePrice_DisplayString"] : "");
            JournalEntryStati status = (JournalEntryStati)dataRow["Status"];

            lblExchangeRate.Text = ((decimal)dataRow["ExchangeRate"]).ToString();
            pnlExchangeRate.Visible = !(bool)dataRow["Journal_Currency_IsBase"];
        }
        else
            lblErrorMessageMain.Text = string.Format("No Memorial Booking with ID '{0}' was found.", JournalEntryId);


    }

    protected int ViewOptionChoice
    {
        get
        {
            object i = ViewState["ViewOptionChoice"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["ViewOptionChoice"] = value; }
    }

    protected void btnToggleLines_Click(object sender, EventArgs e)
    {
        ViewOptionChoice = Math.Abs(ViewOptionChoice - 1);
        SetupViews();
    }

    private void SetupViews()
    {
        mveLines.ActiveViewIndex = ViewOptionChoice;
        btnToggleLines.Text = (ViewOptionChoice == (int)ViewOption.Lines ? "View Summary" : "View Lines");
    }
}
