using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.ComponentModel;
using B4F.TotalGiro.Instruments;

public partial class JournalEntryLines : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (gvLines.Visible)
                gvLines.Sort("LineNumber", SortDirection.Ascending);
            hdnShowManualAllowedGLAccountsOnly.Value = ShowManualAllowedGLAccountsOnly.ToString();
        }
    }

    public int JournalEntryId
    {
        get { return getIntegerValue(hdnJournalEntryId.Value); }
        set { hdnJournalEntryId.Value = value.ToString(); }
    }

    public int JournalManagementCompanyId
    {
        get
        {
            object i = ViewState["JournalManagementCompanyId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["JournalManagementCompanyId"] = value; }
    }

    public DateTime HisXRateDate
    {
        get
        {
            object i = ViewState["HisXRateDate"];
            return ((i == null) ? DateTime.MinValue : (DateTime)i);
        }
        set { ViewState["HisXRateDate"] = value; }
    }

    public int DefaultCurrencyId
    {
        get
        {
            object i = ViewState["DefaultCurrencyId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["DefaultCurrencyId"] = value; }
    }

    public bool ShowManualAllowedGLAccountsOnly
    {
        get
        {
            object i = ViewState["ShowManualAllowedGLAccountsOnly"];
            return ((i == null) ? false : (bool)i);
        }
        set 
        { 
            ViewState["ShowManualAllowedGLAccountsOnly"] = value;
            hdnShowManualAllowedGLAccountsOnly.Value = value.ToString();
        }
    }

    public System.Web.UI.WebControls.Unit GridViewWidth
    {
        get
        {
            return this.gvLines.Width;
        }
        set { this.gvLines.Width = value; }
    }

    public bool EditMode
    {
        get { return (gvLines.EditIndex >= 0); }
    }

    public bool ShowOriginalDescription
    {
        get { return gvLines.Columns[9].Visible; }
        set { gvLines.Columns[9].Visible = value; }
    }

    public bool ShowExRate
    {
        get
        {
            object b = ViewState["ShowExRate"];
            return ((b == null) ? true : (bool)b);
        }
        set 
        { 
            ViewState["ShowExRate"] = value;
            gvLines.Columns[4].Visible = value;
        }
    }

    [Description("Width of the control."), Category("Behavior")]
    public Unit Width
    {
        get { return this.gvLines.Width; }
        set { this.gvLines.Width = value; }
    }

    public bool CurrencyEditable
    {
        get
        {
            object b = ViewState["CurrencyEditable"];
            return ((b == null) ? false : (bool)b);
        }
        set { ViewState["CurrencyEditable"] = value; }
    }

    public int VirtualFundID
    {
        get { return getIntegerValue(hdnVirtualFundID.Value); }
        set { hdnVirtualFundID.Value = value.ToString(); }
    }


    public void InsertLine(int stornoedLineId)
    {
        IsLineInsert = true;
        AllowGiroAccountsDataBind = false;
        StornoedLineId = stornoedLineId;

        // TODO: restore sorting criteria after Insert
        gvLines.Sort("LineNumber", SortDirection.Ascending);

        gvLines.DataBind();
        if (gvLines.PageIndex != gvLines.PageCount - 1)
        {
            gvLines.PageIndex = gvLines.PageCount - 1;
            gvLines.DataBind();
        }
        gvLines.EditIndex = gvLines.Rows.Count - 1;

        OnEditing();
    }

    public void DataBind()
    {
        gvLines.DataBind();
    }

    protected void OnEditing()
    {
        if (Editing != null)
            Editing(this, EventArgs.Empty);
    }

    protected void OnCanceledEdit()
    {
        if (CanceledEdit != null)
            CanceledEdit(this, EventArgs.Empty);
    }

    protected void OnUpdated()
    {
        if (Updated != null)
            Updated(this, EventArgs.Empty);
    }

    protected void OnError(Exception exception)
    {
        if (Error != null)
            Error(this, new ErrorEventArgs(exception));
    }

    protected void OnAddingTranferFee(int journalEntryLineId)
    {
        if (AddingTranferFee != null)
            AddingTranferFee(this, new CommandEventArgs("Key", journalEntryLineId));
    }

    [Description("Event triggered when entering Insert or Update mode."), Category("Action")]
    public EventHandler Editing;
    [Description("Event triggered when canceling Insert or Update mode."), Category("Action")]
    public EventHandler CanceledEdit;
    [Description("Event triggered after a line was inserted, updated, deleted, or stornoed."), Category("Action")]
    public EventHandler Updated;
    [Description("Event triggered when an exception was generated by the control."), Category("Action")]
    public ErrorEventHandler Error;
    [Description("Event triggered when in adding transfer fee mode."), Category("Action")]
    public CommandEventHandler AddingTranferFee;


    protected bool AccountListVisible
    {
        get { return pnlAccountList.Visible; }
        set
        {
            AccountFinderVisible = false;
            pnlAccountList.Visible = value;
        }
    }

    protected bool AccountFinderVisible
    {
        get { return pnlAccountFinder.Visible; }
        set
        {
            pnlAccountFinder.Visible = value;
            btnShowAccountFinder.Text = (pnlAccountFinder.Visible ? "Hide Filter" : "Show Filter");
        }
    }

    protected void odsSelectedAccount_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.Cancel = !AllowGiroAccountsDataBind;
    }

    protected void gvLines_DataBinding(object sender, EventArgs e)
    {
        bool visible = (CurrencyEditable && gvLines.EditIndex >= 0);
        gvLines.Columns[3].Visible = visible;
        if (!visible) visible = CurrencyEditable && ShowExRate;
        gvLines.Columns[4].Visible = visible;
    }

    protected void gvLines_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dataRowView = (DataRowView)e.Row.DataItem;

                int accountId = (int)dataRowView["GiroAccount_Key"];
                AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
                if (lbl != null)
                {
                    lbl.AccountID = accountId;
                    lbl.GetData();
                }

                if (gvLines.EditIndex >= 0 && e.Row.RowIndex == gvLines.EditIndex)
                {
                    if ((int)dataRowView["Key"] == 0 || (bool)dataRowView["IsEditable"])
                    {
                        editingRow = e.Row;

                        lbtDelete.Visible = false;
                        lbtEdit.Visible = false;
                        lbtUpdate.Visible = true;
                        lbtCancel.Visible = true;

                        if ((int)dataRowView["GLAccountId"] != 0)
                            GLAccountId = (int)dataRowView["GLAccountId"];

                        if (CurrencyEditable)
                        {
                            int currencyId = 0;
                            if (int.TryParse(dataRowView["BalanceCurrencyId"].ToString(), out currencyId) && currencyId != 0)
                                CurrencyId = currencyId;
                            else if (DefaultCurrencyId != 0)
                            {
                                CurrencyId = DefaultCurrencyId;
                                ExRate = 1M;
                            }
                        }

                        if ((decimal)dataRowView["DebitQuantity"] != 0m)
                            DebitQuantity = (decimal)dataRowView["DebitQuantity"];

                        if ((decimal)dataRowView["CreditQuantity"] != 0m)
                            CreditQuantity = (decimal)dataRowView["CreditQuantity"];

                        if (dataRowView["GiroAccount_Number"] != DBNull.Value)
                            GiroAccountNumber = (string)dataRowView["GiroAccount_Number"];

                        if (dataRowView["Description"] != DBNull.Value)
                            Description = (string)dataRowView["Description"];

                        txtDescription.Width = Unit.Pixel((ShowOriginalDescription ? 128 : 128 + 217 + 17) -
                                                          (CurrencyEditable ? 50 + 7 : 0));

                        ddlGLAccount.Focus();
                    }
                    else
                    {
                        gvLines.EditIndex = -1;
                        throw new ApplicationException(
                            string.Format("Journal Entry Line number '{0}' is not editable.", (int)dataRowView["LineNumber"]));
                    }
                }
                else if ((bool)dataRowView["IsStornoed"])
                    e.Row.ForeColor = System.Drawing.Color.Gray;
                else
                {
                    if ((bool)dataRowView["GLAccountIsFixed"])
                        e.Row.BackColor = System.Drawing.Color.Silver;

                    Trunc.TruncLabel2 truncLabel = (Trunc.TruncLabel2)e.Row.FindControl("lblDescription");
                    truncLabel.Width = Unit.Pixel((ShowOriginalDescription ? 124 : 124 + 217 + 17) -
                                                  (CurrencyEditable && gvLines.EditIndex >= 0 ? 50 + 7 : 0));
                    truncLabel.MaxLength = (ShowOriginalDescription ? 22 : 57) - (CurrencyEditable && gvLines.EditIndex >= 0 ? 8 : 0);
                    truncLabel.LongText = truncLabel.LongText;  // this re-truncates the value of property Text
                }
            }
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void gvLines_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName.ToUpper())
            {
                case "FINDACCOUNT":
                    AccountListVisible = !AccountListVisible;

                    if (AccountListVisible)
                    {
                        if (!AllowGiroAccountsDataBind)
                        {
                            ctlAccountFinder.AssetManagerId = JournalManagementCompanyId;
                            AllowGiroAccountsDataBind = true;
                            lboGiroAccount.DataBind();
                        }
                        if (GiroAccountNumber != string.Empty)
                        {
                            lboGiroAccount.SelectedValue = string.Empty;
                            foreach (ListItem listItem in lboGiroAccount.Items)
                                if (listItem.Value.ToUpper().StartsWith(GiroAccountNumber.ToUpper()))
                                {
                                    lboGiroAccount.SelectedValue = listItem.Value;
                                    GiroAccountNumber = lboGiroAccount.SelectedValue;
                                }
                        }
                        lboGiroAccount.Focus();
                    }
                    else
                        btnFindAccount.Focus();
                    break;
                case "SEARCHACCOUNTS":
                    lboGiroAccount.DataBind();
                    break;
                case "EDITLINE":
                case "STORNOLINE":
                case "UPDATE":
                case "SHOWACCOUNTFINDER":
                case "ADDTRANSFERFEE":
                    // so that ScreenMode doesn't change to 'Main'
                    break;
                default:
                    // this means Cancel
                    IsLineInsert = false;
                    OnCanceledEdit();
                    break;
            }
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void gvLines_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            e.NewValues["JournalEntryId"] = JournalEntryId;
            e.NewValues["Status"] = JournalEntryLineStati.New;
            e.NewValues["GLAccountId"] = GLAccountId;
            if (CurrencyEditable)
                e.NewValues["CurrencyId"] = CurrencyId;
            e.NewValues["ExchangeRate"] = ExRate;
            e.NewValues["DebitQuantity"] = DebitQuantity;
            e.NewValues["CreditQuantity"] = CreditQuantity;
            e.NewValues["GiroAccountNumber"] = GiroAccountNumber;
            e.NewValues["Description"] = Description;
            e.NewValues["StornoedLineId"] = StornoedLineId;
        }
        catch (Exception ex)
        {
            e.Cancel = true;
            OnError(ex);
        }
    }

    protected void gvLines_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        if (e.Exception != null)
        {
            AccountListVisible = false;
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
            OnError(e.Exception);
        }
        else
        {
            IsLineInsert = false;
            OnUpdated();
        }
    }

    protected void lbtStorno_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int journalEntryLineId = int.Parse((string)e.CommandArgument);
            InsertLine(journalEntryLineId);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void lbtAddTransferFee_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int journalEntryLineId = int.Parse((string)e.CommandArgument);
            OnAddingTranferFee(journalEntryLineId);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void lbtEdit_Command(object sender, CommandEventArgs e)
    {
        try
        {
            IsLineInsert = false;
            AllowGiroAccountsDataBind = false;

            int journalEntryLineId = int.Parse((string)e.CommandArgument);
            gvLines.EditIndex = findRowIndex(gvLines, journalEntryLineId);

            OnEditing();
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void lbtDelete_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int journalEntryLineId = int.Parse((string)e.CommandArgument);
            JournalEntryLinesAdapter.DeleteJournalEntryLine(JournalEntryId, journalEntryLineId);

            IsLineInsert = false;
            gvLines.EditIndex = -1;
            gvLines.DataBind();
            OnUpdated();
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    protected void lboGiroAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        GiroAccountNumber = lboGiroAccount.SelectedValue;
        lboGiroAccount.Focus();
    }

    protected void ddlGLAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ShowExRate || CurrencyEditable)
        {
            CurrencyId = DefaultCurrencyId;
            ddlCurrency.Enabled = true;
            if (GLAccountId != int.MinValue)
            {
                int defaulCurrencyId;
                decimal exRate;
                if (JournalEntryLinesAdapter.GetDefaultCurrencyFromGLAccount(GLAccountId, HisXRateDate, out defaulCurrencyId, out exRate))
                {
                    ddlCurrency.Enabled = false;
                    CurrencyId = defaulCurrencyId;
                    ExRate = exRate;
                }
            }
            checkCurrencyDefault();
        }
    }

    protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ShowExRate || CurrencyEditable)
        {
            if (!(CurrencyId == DefaultCurrencyId || CurrencyId == 0))
                ExRate = JournalEntryLinesAdapter.GetExChangeRate(CurrencyId, HisXRateDate);
            checkCurrencyDefault();
        }
    }

    protected void checkCurrencyDefault()
    {
        dbExRateQuantity.Enabled = true;
        if (CurrencyId == DefaultCurrencyId)
        {
            ExRate = 1M;
            dbExRateQuantity.Enabled = false;
        }
    }


    protected void btnShowAccountFinder_Click(object sender, EventArgs e)
    {
        AccountFinderVisible = !AccountFinderVisible;
        btnShowAccountFinder.Focus();
    }

    private int findRowIndex(GridView gridView, int key)
    {
        int rowIndex = -1;
        for (int i = 0; i < gridView.DataKeys.Count; i++)
            if ((int)gridView.DataKeys[i].Value == key)
                rowIndex = i;

        return rowIndex;
    }

    protected int GLAccountId
    {
        get { return int.Parse(ddlGLAccount.SelectedValue); }
        set { ddlGLAccount.SelectedValue = value.ToString(); }
    }

    protected int CurrencyId
    {
        get { return int.Parse(ddlCurrency.SelectedValue); }
        set { ddlCurrency.SelectedValue = value.ToString(); }
    }

    protected decimal ExRate
    {
        get { return dbExRateQuantity.Value; }
        set { dbExRateQuantity.Text = value.ToString("0.00000"); }
    }

    protected decimal DebitQuantity
    {
        get { return dbDebitQuantity.Value; }
        set { dbDebitQuantity.Value = value; }
    }

    protected decimal CreditQuantity
    {
        get { return dbCreditQuantity.Value; }
        set { dbCreditQuantity.Value = value; }
    }

    protected string GiroAccountNumber
    {
        get { return txtGiroAccount.Text.Trim(); }
        set { txtGiroAccount.Text = value; }
    }

    protected bool AllowGiroAccountsDataBind
    {
        get { return getBooleanValue(hdnAllowGiroAccountsDataBind.Value); }
        set { hdnAllowGiroAccountsDataBind.Value = value.ToString(); }
    }

    protected string Description
    {
        get { return txtDescription.Text; }
        set { txtDescription.Text = value; }
    }

    protected bool IsLineInsert
    {
        get { return getBooleanValue(hdnIsLineInsert.Value); }
        set
        {
            hdnIsLineInsert.Value = value.ToString();
            if (!value) StornoedLineId = 0;
        }
    }

    protected int StornoedLineId
    {
        get { return getIntegerValue(hdnStornoedLineId.Value); }
        set { hdnStornoedLineId.Value = value.ToString(); }
    }

    private decimal getDecimalValue(string value)
    {
        NumberStyles numberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;

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

    protected DropDownList ddlGLAccount { get { return (DropDownList)Utility.FindControl(EditingRow, "ddlGLAccount"); } }
    protected DropDownList ddlCurrency { get { return (DropDownList)Utility.FindControl(EditingRow, "ddlCurrency"); } }
    protected DecimalBox dbExRateQuantity { get { return (DecimalBox)Utility.FindControl(EditingRow, "dbExRateQuantity"); } }
    protected DecimalBox dbDebitQuantity { get { return (DecimalBox)Utility.FindControl(EditingRow, "dbDebitQuantity"); } }
    protected DecimalBox dbCreditQuantity { get { return (DecimalBox)Utility.FindControl(EditingRow, "dbCreditQuantity"); } }
    protected TextBox txtGiroAccount { get { return (TextBox)Utility.FindControl(EditingRow, "txtGiroAccount"); } }
    protected Button btnFindAccount { get { return (Button)Utility.FindControl(EditingRow, "btnFindAccount"); } }
    protected ListBox lboGiroAccount { get { return (ListBox)Utility.FindControl(EditingRow, "lboGiroAccount"); } }
    protected Panel pnlAccountList { get { return (Panel)Utility.FindControl(EditingRow, "pnlAccountList"); } }
    protected Button btnShowAccountFinder { get { return (Button)Utility.FindControl(EditingRow, "btnShowAccountFinder"); } }
    protected Panel pnlAccountFinder { get { return (Panel)Utility.FindControl(EditingRow, "pnlAccountFinder"); } }
    protected AccountFinder ctlAccountFinder { get { return (AccountFinder)Utility.FindControl(EditingRow, "ctlAccountFinder"); } }
    protected TextBox txtDescription { get { return (TextBox)Utility.FindControl(EditingRow, "txtDescription"); } }
    protected LinkButton lbtDelete { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtDelete"); } }
    protected LinkButton lbtEdit { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtEdit"); } }
    protected LinkButton lbtUpdate { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtUpdate"); } }
    protected LinkButton lbtCancel { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtCancel"); } }

    protected GridViewRow EditingRow
    {
        get
        {
            if (editingRow == null)
                editingRow = (gvLines.EditIndex >= 0 ? gvLines.Rows[gvLines.EditIndex] : null);

            return editingRow;
        }
    }

    private GridViewRow editingRow;
}
