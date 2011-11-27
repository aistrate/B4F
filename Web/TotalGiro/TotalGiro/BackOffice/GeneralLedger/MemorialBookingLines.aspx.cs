using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class MemorialBookingLines : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMessageMain.Text = "";
        lblErrorMessageDV.Text = "";
        
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Memorial Booking Lines";

            JournalEntryId = (Session["journalEntryId"] != null ? (int)Session["journalEntryId"] : 0);
            ctlJournalEntryLines.JournalEntryId = JournalEntryId;

            ctlJournalEntryLines.ShowOriginalDescription = false;
            ctlJournalEntryLines.CurrencyEditable = true;

            displayBookingDetails();
        }

        if (ctlJournalEntryLines.Visible)
        {
            ctlJournalEntryLines.Editing += new EventHandler(ctlJournalEntryLines_Editing);
            ctlJournalEntryLines.CanceledEdit += new EventHandler(ctlJournalEntryLines_CanceledEdit);
            ctlJournalEntryLines.Updated += new EventHandler(ctlJournalEntryLines_Updated);
            ctlJournalEntryLines.AddingTranferFee += new CommandEventHandler(ctlJournalEntryLines_AddingTranferFee);
            ctlJournalEntryLines.Error += new ErrorEventHandler(ctlJournalEntryLines_Error);
        }
    }

    protected void ctlJournalEntryLines_Editing(object sender, EventArgs e)
    {
        ScreenMode = ScreenModeState.EditLine;
    }

    protected void ctlJournalEntryLines_CanceledEdit(object sender, EventArgs e)
    {
        ScreenMode = ScreenModeState.Main;
    }

    protected void ctlJournalEntryLines_Updated(object sender, EventArgs e)
    {
        ScreenMode = ScreenModeState.Main;
        displayBookingDetails();
    }

    protected void ctlJournalEntryLines_AddingTranferFee(object sender, CommandEventArgs e)
    {
        ScreenMode = ScreenModeState.AddTransferFee;
        Label lblLineID = (Label)dvAddTransferFee.FindControl("lblLineID");
        if (lblLineID != null)
            lblLineID.Text = e.CommandArgument.ToString();
        TextBox txtDescription = (TextBox)dvAddTransferFee.FindControl("txtDescription");
        if (txtDescription != null)
            txtDescription.Text = "";
        DecimalBox dbBox = (DecimalBox)dvAddTransferFee.FindControl("dbTransferFee");
        if (dbBox != null)
            dbBox.Clear();
    }

    protected void ctlJournalEntryLines_Error(object sender, ErrorEventArgs e)
    {
        lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(e.Exception);
        if (!ctlJournalEntryLines.EditMode)
            ScreenMode = ScreenModeState.Main;
    }

    #region Header: Memorial Booking Details

    protected int JournalEntryId
    {
        get
        {
            object i = ViewState["JournalEntryId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["JournalEntryId"] = value; }
    }

    protected int JournalManagementCompanyId
    {
        get
        {
            object i = ViewState["JournalManagementCompanyId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["JournalManagementCompanyId"] = value; }
    }

    private void displayBookingDetails()
    {
        DataSet ds = MemorialBookingLinesAdapter.GetMemorialBookingDetails(JournalEntryId);
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dataRow = ds.Tables[0].Rows[0];

            JournalManagementCompanyId = (int)dataRow["Journal_ManagementCompany_Key"];
            ctlJournalEntryLines.JournalManagementCompanyId = JournalManagementCompanyId;

            ctlJournalEntryLines.DefaultCurrencyId = (int)dataRow["Journal_Currency_Key"];

            lblJournalEntryNumber.Text = (string)dataRow["JournalEntryNumber"];
            hdnJournalEntryId.Value = ((int)dataRow["Key"]).ToString();
            lblStatus.Text = (string)dataRow["DisplayStatus"];
            lblJournal.Text = (string)dataRow["Journal_FullDescription"];

            ctlJournalEntryLines.ShowExRate = !(bool)dataRow["Journal_Currency_IsBase"] || (bool)dataRow["ContainsForeignCashLines"];
            ctlJournalEntryLines.ShowManualAllowedGLAccountsOnly = (bool)dataRow["ShowManualAllowedGLAccountsOnly"];

            DateTime transactionDate = (DateTime)dataRow["TransactionDate"];
            ctlJournalEntryLines.HisXRateDate = transactionDate;
            lblTransactionDate.Text = (transactionDate != DateTime.MinValue ? transactionDate.ToString("dd-MM-yyyy") : "");

            lblDescription.Text = (dataRow["Description"] != DBNull.Value ? (string)dataRow["Description"] : "");

            JournalEntryStati status = (JournalEntryStati)dataRow["Status"];
            btnEditBooking.Enabled = (status == JournalEntryStati.New);
            btnBook.Enabled = (status != JournalEntryStati.Booked);
        }
        else
            lblErrorMessageMain.Text = string.Format("No Memorial Booking with ID '{0}' was found.", JournalEntryId);

        ctlJournalEntryLines.Visible = (ds.Tables[0].Rows.Count > 0);
    }

    #endregion 


    #region Buttons

    public ScreenModeState ScreenMode
    {
        get
        {
            object e = ViewState["ScreenMode"];
            return ((e == null) ? ScreenModeState.Main : (ScreenModeState)e);
        }
        set
        {
            ViewState["ScreenMode"] = value;
            pnlActionButtons.Enabled = (value == ScreenModeState.Main);
            mvwMemorialBookings.ActiveViewIndex = (value == ScreenModeState.EditLine ? 0 : (value == ScreenModeState.AddTransferFee ? 2 : (int)value));
            if (value == ScreenModeState.EditJournalEntry)
                dvMemorialBooking.DataBind();
        }
    }

    protected bool Booking
    {
        get
        {
            object b = ViewState["Booking"];
            return ((b == null) ? false : (bool)b);
        }
        set { ViewState["Booking"] = value; }
    }

    protected void btnBookings_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds = MemorialBookingLinesAdapter.GetMemorialBookingDetails(JournalEntryId);
            if (ds.Tables[0].Rows.Count > 0)
                Session["journalId"] = ds.Tables[0].Rows[0]["Journal_Key"];

            Response.Redirect("~/BackOffice/GeneralLedger/MemorialBookings.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnEditBooking_Click(object sender, EventArgs e)
    {
        try
        {
            Booking = false;
            ScreenMode = ScreenModeState.EditJournalEntry;
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
            ScreenMode = ScreenModeState.Main;
        }
    }

    protected void btnNewLine_Click(object sender, EventArgs e)
    {
        try
        {
            ctlJournalEntryLines.InsertLine(0);
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnBook_Click(object sender, EventArgs e)
    {
        bookBankStatement(false);
    }

    private void bookBankStatement(bool forceIfUnchanged)
    {
        try
        {
            BookingResults bookingResults = MemorialBookingLinesAdapter.BookMemorialBooking(JournalEntryId, forceIfUnchanged);

            switch (bookingResults)
            {
                case BookingResults.OK:
                    Booking = false;
                    ScreenMode = ScreenModeState.Main;
                    displayBookingDetails();
                    ctlJournalEntryLines.DataBind();
                    lblErrorMessageMain.Text = "Memorial Booking booked succesfully.";
                    break;
                case BookingResults.TransactionDateNeeded:
                    Booking = true;
                    ScreenMode = ScreenModeState.EditJournalEntry;
                    break;
            }
        }
        catch (Exception ex)
        {
            ScreenMode = ScreenModeState.Main;
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
    
    #endregion


    #region DetailsView : Memorial Booking

    protected void dvMemorialBooking_DataBound(object sender, EventArgs e)
    {
        lbtDVOk.Text = (Booking ? "Book" : "OK");
    }

    protected void dvMemorialBooking_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        try
        {
            e.NewValues["TransactionDate"] = TransactionDate;
            e.NewValues["Description"] = Description;
        }
        catch (Exception ex)
        {
            lblErrorMessageDV.Text = Utility.GetCompleteExceptionMessage(ex);
            e.Cancel = true;
        }
    }

    protected void dvMemorialBooking_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
    {
        if (e.Exception == null)
        {
            if (Booking)
                bookBankStatement(false);
            else
                ScreenMode = ScreenModeState.Main;

            displayBookingDetails();
        }
        else
        {
            lblErrorMessageDV.Text = Utility.GetCompleteExceptionMessage(e.Exception);
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
    }

    protected void dvMemorialBooking_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        if (e.CommandName.ToUpper() == "CANCEL")
        {
            Booking = false;
            ScreenMode = ScreenModeState.Main;
        }
    }

    protected void dvAddTransferFee_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        ScreenMode = ScreenModeState.Main;
    }

    protected void dvAddTransferFee_ItemInserting(object sender, DetailsViewInsertEventArgs e)
    {
        try
        {
            int lineID = 0;
            string description = "";
            decimal feeQuantity = 0M;

            Label lblLineID = (Label)dvAddTransferFee.FindControl("lblLineID");
            if (lblLineID != null)
                int.TryParse(lblLineID.Text, out lineID);
            TextBox txtDescription = (TextBox)dvAddTransferFee.FindControl("txtDescription");
            if (txtDescription != null)
                description = txtDescription.Text;
            DecimalBox dbBox = (DecimalBox)dvAddTransferFee.FindControl("dbTransferFee");
            if (dbBox != null)
                feeQuantity = dbBox.Value;

            if (JournalEntryLinesAdapter.AddTransferFee(lineID, feeQuantity, description))
            {
                ScreenMode = ScreenModeState.Main;
                ctlJournalEntryLines.DataBind();
                displayBookingDetails();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected DateTime TransactionDate
    {
        get { return dpDVTransactionDate.SelectedDate; }
        set { dpDVTransactionDate.SelectedDate = value; }
    }
    
    protected string Description
    {
        get { return txtDVDescription.Text; }
        set { txtDVDescription.Text = value; }
    }

    private decimal getDecimalValue(string value)
    {
        return getDecimalValue(value, true);
    }

    private decimal getDecimalValue(string value, bool allowLeadingSign)
    {
        NumberStyles numberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        if (allowLeadingSign)
            numberStyles |= NumberStyles.AllowLeadingSign;
        
        decimal result = 0m;
        if (value != string.Empty)
        {
            if (!decimal.TryParse(value, numberStyles, null, out result))
                throw new ArgumentException(string.Format("Invalid numeric format: '{0}'.", value));
        }

        return result;
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

    private bool getBooleanValue(string value)
    {
        bool b;
        return (value != string.Empty && bool.TryParse(value, out b) ? bool.Parse(value) : false);
    }

    protected DatePicker dpDVTransactionDate { get { return (DatePicker)Utility.FindControl(dvMemorialBooking, "dpDVTransactionDate"); } }
    protected TextBox txtDVDescription { get { return (TextBox)Utility.FindControl(dvMemorialBooking, "txtDVDescription"); } }
    protected LinkButton lbtDVOk { get { return (LinkButton)Utility.FindControl(dvMemorialBooking, "lbtDVOk"); } }
    
    #endregion
}
