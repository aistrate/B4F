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
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class BankStatementLines : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMessageMain.Text = "";
        lblErrorMessageDV.Text = "";
        
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Bank Statement Lines";

            JournalEntryId = (Session["journalEntryId"] != null ? (int)Session["journalEntryId"] : 0);
            ctlJournalEntryLines.JournalEntryId = JournalEntryId;

            displayStatementDetails();
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
        displayStatementDetails();
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

    #region Header: Bank Statement Details

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

    private void displayStatementDetails()
    {
        DataSet ds = BankStatementLinesAdapter.GetBankStatementDetails(JournalEntryId);
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dataRow = ds.Tables[0].Rows[0];

            JournalManagementCompanyId = (int)dataRow["Journal_ManagementCompany_Key"];
            ctlJournalEntryLines.JournalManagementCompanyId = JournalManagementCompanyId;

            lblJournalEntryNumber.Text = (string)dataRow["JournalEntryNumber"];
            hdnJournalEntryId.Value = ((int)dataRow["Key"]).ToString();
            lblStatus.Text = (string)dataRow["DisplayStatus"];
            lblJournal.Text = (string)dataRow["Journal_FullDescription"];
            Money openAmount = (Money)dataRow["OpenAmount"];
            bool hasClosingBalance = (bool)dataRow["HasClosingBalance"];
            lblOpenAmount.Text = (hasClosingBalance ? openAmount.ToString("#,##0.00") : "");

            ctlJournalEntryLines.ShowExRate = !(bool)dataRow["Journal_Currency_IsBase"] || (bool)dataRow["ContainsForeignCashLines"];
            ctlJournalEntryLines.ShowManualAllowedGLAccountsOnly = (bool)dataRow["ShowManualAllowedGLAccountsOnly"];

            DateTime startingBalanceDate = (DateTime)dataRow["StartingBalanceDate"];
            ctlJournalEntryLines.HisXRateDate = startingBalanceDate;
            lblStartingBalanceDate.Text = (startingBalanceDate != DateTime.MinValue ? startingBalanceDate.ToString("dd-MM-yyyy") : "");
            lblStartingBalance.Text = (startingBalanceDate != DateTime.MinValue ? ((Money)dataRow["StartingBalance"]).ToString("#,##0.00") : "");
            DateTime transactionDate = (DateTime)dataRow["TransactionDate"];
            lblTransactionDate.Text = (transactionDate != DateTime.MinValue ? transactionDate.ToString("dd-MM-yyyy") : "");
            lblClosingBalance.Text = (hasClosingBalance ? ((Money)dataRow["ClosingBalance"]).ToString("#,##0.00") : "");

            JournalEntryStati status = (JournalEntryStati)dataRow["Status"];
            int lineCount = (int)dataRow["Lines_Count"];
            btnImport.Enabled = (status == JournalEntryStati.New && (!hasClosingBalance || lineCount == 0) && transactionDate != DateTime.MinValue);
            btnEditStatement.Enabled = (status == JournalEntryStati.New);
            btnBook.Enabled = (status != JournalEntryStati.Booked && (!hasClosingBalance || (hasClosingBalance && openAmount.IsZero)));
        }
        else
            lblErrorMessageMain.Text = string.Format("No Bank Statement with ID '{0}' was found.", JournalEntryId);

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
            mvwBankStatement.ActiveViewIndex = (value == ScreenModeState.EditLine ? 0 : (value == ScreenModeState.AddTransferFee ? 3 : (int)value));
            if (value == ScreenModeState.EditJournalEntry)
                dvBankStatement.DataBind();
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

    protected void btnStatements_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds = BankStatementLinesAdapter.GetBankStatementDetails(JournalEntryId);
            if (ds.Tables[0].Rows.Count > 0)
                Session["journalId"] = ds.Tables[0].Rows[0]["Journal_Key"];

            Response.Redirect("~/BackOffice/GeneralLedger/BankStatements.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessageMain.Text = BankStatementLinesAdapter.ImportBankStatementLines(JournalEntryId);

            displayStatementDetails();
            ctlJournalEntryLines.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessageMain.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnEditStatement_Click(object sender, EventArgs e)
    {
        try
        {
            Booking = false;
            ScreenMode = ScreenModeState.EditJournalEntry;
        }
        catch (Exception ex)
        {
            lblErrorMessageDV.Text = Utility.GetCompleteExceptionMessage(ex);
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

    protected void btnYes_Click(object sender, EventArgs e)
    {
        bookBankStatement(true);
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        Booking = false;
        ScreenMode = ScreenModeState.Main;
    }

    private void bookBankStatement(bool forceIfUnchanged)
    {
        try
        {
            BookingResults bookingResults = BankStatementLinesAdapter.BookBankStatement(JournalEntryId, forceIfUnchanged);

            switch (bookingResults)
            {
                case BookingResults.OK:
                    Booking = false;
                    ScreenMode = ScreenModeState.Main;
                    displayStatementDetails();
                    ctlJournalEntryLines.DataBind();
                    lblErrorMessageMain.Text = "Bank Statement booked succesfully.";
                    break;
                case BookingResults.ClosingBalanceNeeded:
                    Booking = true;
                    ScreenMode = ScreenModeState.EditJournalEntry;
                    break;
                case BookingResults.UnchangedBalance:
                    Booking = true;
                    ScreenMode = ScreenModeState.Question;
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


    #region DetailsView : Bank Statement

    protected void dvBankStatement_DataBound(object sender, EventArgs e)
    {
        dpDVTransactionDate.Parent.Parent.Visible = CanChangeTransactionDate;
        lbtDVOk.Text = (Booking ? "Book" : "OK");
    }
    
    protected void dvBankStatement_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        try
        {
            e.NewValues["CanChangeTransactionDate"] = CanChangeTransactionDate;
            if (CanChangeTransactionDate)
                e.NewValues["TransactionDate"] = TransactionDate;
            e.NewValues["ClosingBalanceQuantity"] = ClosingBalanceQuantity;
        }
        catch (Exception ex)
        {
            lblErrorMessageDV.Text = Utility.GetCompleteExceptionMessage(ex);
            e.Cancel = true;
        }
    }

    protected void dvBankStatement_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
    {
        if (e.Exception == null)
        {
            if (Booking)
                bookBankStatement(false);
            else
                ScreenMode = ScreenModeState.Main;
            
            displayStatementDetails();
        }
        else
        {
            lblErrorMessageDV.Text = Utility.GetCompleteExceptionMessage(e.Exception);
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
    }

    protected void dvBankStatement_ItemCommand(object sender, DetailsViewCommandEventArgs e)
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
                displayStatementDetails();
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

    protected decimal ClosingBalanceQuantity
    {
        get { return getDecimalValue(txtDVClosingBalanceQuantity.Text); }
        set { txtDVClosingBalanceQuantity.Text = value.ToString("###0.00"); }
    }

    protected bool CanChangeTransactionDate { get { return getBooleanValue(hdnDVCanChangeTransactionDate.Value); } }

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

    protected DatePicker dpDVTransactionDate { get { return (DatePicker)Utility.FindControl(dvBankStatement, "dpDVTransactionDate"); } }
    protected TextBox txtDVClosingBalanceQuantity { get { return (TextBox)Utility.FindControl(dvBankStatement, "txtDVClosingBalanceQuantity"); } }
    protected HiddenField hdnDVCanChangeTransactionDate { get { return (HiddenField)Utility.FindControl(dvBankStatement, "hdnDVCanChangeTransactionDate"); } }
    protected LinkButton lbtDVOk { get { return (LinkButton)Utility.FindControl(dvBankStatement, "lbtDVOk"); } }
    
    #endregion
}
