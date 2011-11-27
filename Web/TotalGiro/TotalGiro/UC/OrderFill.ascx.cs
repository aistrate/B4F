using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design.WebControls;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.Globalization;
using B4F.TotalGiro.Utils;
using B4F.Web.WebControls;

public partial class OrderFill : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Visible)
        {
            dpTransactionDate.SelectionChanged += new EventHandler(dpTransactionDate_SelectionChanged);
            dpTransactionDate.Expanded += new EventHandler(dpTransactionDate_Expanded);
            if (isAccruedInterestVisible)
                dpSettlementDate.SelectionChanged += new EventHandler(dpSettlementDate_SelectionChanged);
            dpSettlementDate.Expanded += new EventHandler(dpSettlementDate_Expanded);
            dbPrice.ValueChanged += new EventHandler(dbPrice_ValueChanged);
            dbSize.ValueChanged += new EventHandler(dbSize_ValueChanged);
            dbAmount.ValueChanged += new EventHandler(dbAmount_ValueChanged);
            dbServiceChargePercentage.ValueChanged += new EventHandler(dbServiceChargePercentage_ValueChanged);
            dbServiceChargeAmount.ValueChanged += new EventHandler(dbServiceChargeAmount_ValueChanged);
        }

        DisplayError("");
    }

    [Description("The control ID of an IDataSource that will be used as the data source."), DefaultValue(""), Category("Data"),
        IDReferenceProperty(typeof(DataSourceControl)), TypeConverter(typeof(DataSourceIDConverter))]
    public string DataSourceID
    {
        get { return dvOrderFill.DataSourceID; }
        set { dvOrderFill.DataSourceID = value; }
    }

    public int DataItemCount
    {
        get { return dvOrderFill.DataItemCount; }
    }

    public bool IsServiceChargeVisible
    {
        get
        {
            object b = ViewState["IsServiceChargeVisible"];
            return ((b == null) ? true : (bool)b);
        }
        set { ViewState["IsServiceChargeVisible"] = value; }
    }

    protected bool isAccruedInterestVisible
    {
        get { return lblAccruedInterest.Parent.Parent.Visible; }
    }

    public bool IsTransactionTimeVisible
    {
        get
        {
            object b = ViewState["IsTransactionTimeVisible"];
            return ((b == null) ? true : (bool)b);
        }
        set { ViewState["IsTransactionTimeVisible"] = value; }
    }

    public bool IsSettlementDateVisible
    {
        get
        {
            object b = ViewState["IsSettlementDateVisible"];
            return ((b == null) ? true : (bool)b);
        }
        set { ViewState["IsSettlementDateVisible"] = value; }
    }

    public bool IsCounterpartyVisible
    {
        get
        {
            object b = ViewState["IsCounterpartyVisible"];
            return ((b == null) ? true : (bool)b);
        }
        set { ViewState["IsCounterpartyVisible"] = value; }
    }

    public bool IsExchangeVisible
    {
        get
        {
            object b = ViewState["IsExchangeVisible"];
            return ((b == null) ? true : (bool)b);
        }
        set 
        { 
            ViewState["IsExchangeVisible"] = value;
            this.rfvExchange.Enabled = value;
        }
    }

    protected bool IsCheckDone
    {
        get
        {
            object b = ViewState["IsCheckDone"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["IsCheckDone"] = value;
            if (!value)
            {
                CompleteFillText = "";
                IsCompleteFill = false;
            }
        }
    }

    protected void DisplayError(Exception ex)
    {
        doDisplayError(Utility.GetCompleteExceptionMessage(ex));
    }

    protected void DisplayError(string message)
    {
        if (message != string.Empty)
            doDisplayError(message + "<br/>");
        else
        {
            elbErrorMessage.Text = "";
        }
    }

    // do not use this directly
    private void doDisplayError(string message)
    {
        elbErrorMessage.Text = message.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>") + "<br/>";
    }

    protected void dvOrderFill_DataBound(object sender, EventArgs e)
    {
        try
        {
            // This will hide the entire table row (<tr>)
            lblTransactionTime.Parent.Parent.Visible = IsTransactionTimeVisible;
            lblSettlementDate.Parent.Parent.Visible = IsSettlementDateVisible;
            lblServiceCharge.Parent.Parent.Visible = IsServiceChargeVisible;
            lblCounterparty.Parent.Parent.Visible = IsCounterpartyVisible;
            lblExchange.Parent.Parent.Visible = IsExchangeVisible;

            if (dvOrderFill.DataItemCount > 0)
            {
                int exchangeID = ((OrderFillView)dvOrderFill.DataItem).ExchangeId;
                if (exchangeID != 0)
                {
                    DropDownList ddlExchange = (DropDownList)dvOrderFill.FindControl("ddlExchange");
                    ddlExchange.SelectedValue = exchangeID.ToString();
                }

                int defaultCounterpartyID = ((OrderFillView)dvOrderFill.DataItem).CounterpartyAccountId;
                if (defaultCounterpartyID != 0)
                {
                    DropDownList ddlCounterpartyAccount = (DropDownList)dvOrderFill.FindControl("ddlCounterpartyAccount");
                    ddlCounterpartyAccount.SelectedValue = defaultCounterpartyID.ToString();
                }

                bool doesSupportServiceCharge = ((OrderFillView)dvOrderFill.DataItem).DoesSupportServiceCharge;
                lblServiceCharge.Parent.Parent.Visible = doesSupportServiceCharge;

                TickSize = ((OrderFillView)dvOrderFill.DataItem).TickSize;

                bool showExRate = ((OrderFillView)dvOrderFill.DataItem).ShowExRate;
                lblExRate.Parent.Parent.Visible = showExRate;

                bool showAccruedInterest = ((OrderFillView)dvOrderFill.DataItem).ShowAccruedInterest;
                lblAccruedInterest.Parent.Parent.Visible = showAccruedInterest;
            }
        }
        catch (Exception ex)
        {
            doDisplayError(Utility.GetCompleteExceptionMessage(ex));
        }
    }
    
    protected void dvOrderFill_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        try
        {
            if (!IsCheckDone)
            {
                OrderFillView orderFillView = new OrderFillView(OrderId, Size, Amount, Price, ExRate, IsSizeBased, ServiceChargeAmount, AccruedInterest, ExchangeId);
                orderFillView.TickSize = TickSize;
                PrefillCheckReturnValues prefillCheck = OrderFillAdapter.CheckCompleteFill(orderFillView);
                if (prefillCheck != PrefillCheckReturnValues.OK)
                {
                    IsCheckDone = true;
                    if ((prefillCheck & PrefillCheckReturnValues.Warning) == PrefillCheckReturnValues.Warning)
                        DisplayError(orderFillView.Warning);
                    if ((prefillCheck & PrefillCheckReturnValues.AskCompleteFill) == PrefillCheckReturnValues.AskCompleteFill)
                        CompleteFillText = orderFillView.DisplayFillPercentage;
                    e.Cancel = true;
                    return;
                }
            }

            e.NewValues["Price"] = Price;
            e.NewValues["Size"] = Size;
            e.NewValues["Amount"] = Amount;
            e.NewValues["ExchangeRate"] = ExRate;

            if (IsServiceChargeVisible)
            {
                e.NewValues["ServiceChargePercentage"] = ServiceChargePercentage;
                e.NewValues["ServiceChargeAmount"] = ServiceChargeAmount;
            }
            if (isAccruedInterestVisible)
            {
                e.NewValues["AccruedInterestAmount"] = AccruedInterest;
            }
            e.NewValues["TransactionDate"] = TransactionDate;
            if (IsTransactionTimeVisible)
                e.NewValues["TransactionTime"] = TransactionTime;
            if (IsSettlementDateVisible)
                e.NewValues["SettlementDate"] = SettlementDate;
            if (IsCounterpartyVisible)
                e.NewValues["CounterpartyAccountId"] = CounterpartyAccountId;
            if (IsExchangeVisible)
                e.NewValues["ExchangeId"] = ExchangeId;
            e.NewValues["IsCompleteFill"] = IsCompleteFill;
        }
        catch (Exception ex)
        {
            DisplayError(ex);
            e.Cancel = true;
        }
    }

    protected void dvOrderFill_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
    {
        if (e.Exception == null)
            OnFilled();
        else
        {
            DisplayError(e.Exception);
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
    }

    protected void dvOrderFill_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        if (e.CommandName.ToUpper() == "CANCEL")
            OnCancelled();
    }

    protected void dpTransactionDate_SelectionChanged(object sender, EventArgs e)
    {
        try
        {
            if (IsTransactionDateValid)
            {
                OrderFillView orderFillView = 
                    new OrderFillView(OrderId, Size, Amount, Price, ExRate, IsSizeBased, TransactionDate, CounterpartyAccountId, ExchangeId);

                if (IsSettlementDateVisible)
                {
                    OnTransactionDateChanged(orderFillView);
                    SettlementDate = orderFillView.SettlementDate;
                }

                if (!dbPrice.IsEmpty && IsPriceValid)
                {
                    OnPriceChanged(orderFillView);
                    DisplayError(orderFillView.Warning);
                }

                CalculateAccruedInterestAmount();
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void dpTransactionDate_Expanded(object sender, EventArgs e)
    {
        if (IsSettlementDateVisible)
        {
            if (dpTransactionDate.IsExpanded)
                dpSettlementDate.IsExpanded = false;
        }

        if (IsCounterpartyVisible)
            ddlCounterpartyAccount.Visible = !dpTransactionDate.IsExpanded;
    }

    protected void dpSettlementDate_SelectionChanged(object sender, EventArgs e)
    {
        try
        {
            CalculateAccruedInterestAmount();
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void dpSettlementDate_Expanded(object sender, EventArgs e)
    {
        if (IsCounterpartyVisible)
            ddlCounterpartyAccount.Visible = !dpSettlementDate.IsExpanded;
    }

    protected bool IsTransactionDateValid
    {
        get
        {
            rfvTransactionDate.Validate();
            rvTransactionDate.Validate();
            return rfvTransactionDate.IsValid && rvTransactionDate.IsValid;
        }
    }

    protected bool IsTransactionTimeValid
    {
        get
        {
            cvTransactionTime.Validate();
            return cvTransactionTime.IsValid;
        }
    }

    protected bool IsPriceValid
    {
        get
        {
            rfvPrice.Validate();
            return rfvPrice.IsValid;
        }
    }

    protected bool IsSizeValid
    {
        get
        {
            rfvSize.Validate();
            return rfvSize.IsValid;
        }
    }

    protected bool IsAmountValid
    {
        get
        {
            rfvAmount.Validate();
            return rfvAmount.IsValid;
        }
    }

    protected bool IsServiceChargePercentageValid
    {
        get
        {
            //rfvServiceChargePercentage.Validate();
            //return rfvServiceChargePercentage.IsValid;
            return true;
        }
    }

    protected void dbPrice_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            if (IsPriceValid)
            {
                OrderFillView orderFillView = new OrderFillView(OrderId, Size, Amount, Price, ExRate, IsSizeBased);
                orderFillView.TransactionDate = (IsTransactionDateValid ? TransactionDate : DateTime.Today);
                OnPriceChanged(orderFillView);

                DisplayError(orderFillView.Warning);

                Price = orderFillView.Price;
                Size = orderFillView.Size;
                Amount = orderFillView.Amount;

                if (IsServiceChargeVisible && IsSizeBased)
                {
                    ServiceChargePercentage = InitialServiceChargePercentage;
                    CalculateServiceChargeAmount();
                }

                IsCheckDone = false;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void dbSize_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            if (IsSizeValid)
            {
                IsCheckDone = false;
                CalculateAccruedInterestAmount();
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void dbAmount_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            if (IsAmountValid)
            {
                if (IsServiceChargeVisible)
                {
                    ServiceChargePercentage = InitialServiceChargePercentage;
                    CalculateServiceChargeAmount();
                }

                IsCheckDone = false;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void dbServiceChargePercentage_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            if (IsServiceChargePercentageValid)
            {
                CalculateServiceChargeAmount();
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void CalculateServiceChargeAmount()
    {
        if (Amount > 0 && ServiceChargePercentage > 0)
        {
            ServiceChargeAmount = OrderFillAdapter.CalculateServiceChargeAmount(Amount, ServiceChargePercentage);
            IsCheckDone = false;
        }
    }

    protected void dbServiceChargeAmount_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            if (Amount > 0 && ServiceChargeAmount > 0)
            {
                ServiceChargePercentage = OrderFillAdapter.CalculateServiceChargePercentage(Amount, ServiceChargeAmount);
                IsCheckDone = false;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
    }

    protected void CalculateAccruedInterestAmount()
    {
        if (isAccruedInterestVisible && Size > 0 && Util.IsNotNullDate(SettlementDate))
        {
            AccruedInterest = OrderFillAdapter.CalculateAccruedInterestAmount(OrderId, Size, SettlementDate);
            IsCheckDone = false;
        }
    }

    protected int OrderId { get { return (int)dvOrderFill.DataKey.Value; } }
    protected bool IsSizeBased { get { return getBooleanValue(hdnIsSizeBased.Value); } }
    protected DateTime TransactionDate
    {
        get { return dpTransactionDate.SelectedDate; }
        set { dpTransactionDate.SelectedDate = value; }
    }
    protected DateTime TransactionTime
    {
        get { return tpTime.GetTime(TransactionDate); }
        set 
        {
            tpTime.SelectedHour = value.Hour;
            tpTime.SelectedMinute = value.Minute;
        }
    }
    protected DateTime SettlementDate
    {
        get { return dpSettlementDate.SelectedDate; }
        set { dpSettlementDate.SelectedDate = value; }
    }
    protected decimal Price
    {
        get { return dbPrice.Value; }
        set { dbPrice.Value = value; }
    }

    protected decimal Size
    {
        get { return dbSize.Value; }
        set { dbSize.Value = value; }
    }
    protected int AmountDecimals { get { return dbAmount.DecimalPlaces; } }

    public decimal TickSize
    {
        get
        {
            object b = ViewState["TickSize"];
            return ((b == null) ? 0M : Convert.ToDecimal(b));
        }
        set { ViewState["TickSize"] = value; }
    }

    protected decimal Amount
    {
        get { return dbAmount.Value; }
        set { dbAmount.Value = value; }
    }

    protected decimal AccruedInterest
    {
        get { return dbAccruedInterest.Value; }
        set { dbAccruedInterest.Value = value; }
    }

    protected decimal ExRate
    {
        get 
        {
            if (lblExRate.Parent.Parent.Visible)
                return dbExRate.Value;
            else
                return 1M;
        }
        set { dbExRate.Value = value; }
    }

    protected decimal InitialServiceChargePercentage { get { return getDecimalValue(hdnInitialServiceChargePercentage.Value) / 100m; } }
    protected decimal ServiceChargePercentage
    {
        get { return dbServiceChargePercentage.Value / 100m; }
        set { dbServiceChargePercentage.Value = (value * 100m); }
    }
    protected decimal ServiceChargeAmount
    {
        get { return dbServiceChargeAmount.Value; }
        set { dbServiceChargeAmount.Text = (ServiceChargePercentage != 0 ? Util.FormatDecimal(value, AmountDecimals) : ""); }
    }
    protected int CounterpartyAccountId
    {
        get { return (ddlCounterpartyAccount.Visible ? int.Parse(ddlCounterpartyAccount.SelectedValue) : 0); }
        set { ddlCounterpartyAccount.SelectedValue = value.ToString(); }
    }
    protected bool UseNostro
    {
        get { return chkUseNostro.Checked; }
        set { chkUseNostro.Checked = value; }
    }
    protected int ExchangeId
    {
        get { return (ddlExchange.Visible ? int.Parse(ddlExchange.SelectedValue) : 0); }
        set { ddlExchange.SelectedValue = value.ToString(); }
    }
    protected bool IsCompleteFill
    {
        get { return chkIsCompleteFill.Checked; }
        set { chkIsCompleteFill.Checked = value; }
    }
    protected string CompleteFillText
    {
        get { return chkIsCompleteFill.Text; }
        set { chkIsCompleteFill.Text = value; chkIsCompleteFill.Enabled = (value != string.Empty); }
    }

    private bool getBooleanValue(string value)
    {
        bool b;
        return (value != string.Empty && bool.TryParse(value, out b) ? bool.Parse(value) : false);
    }

    private decimal getDecimalValue(string value)
    {
        NumberStyles numberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        decimal result = 0m;
        if (value != string.Empty)
            decimal.TryParse(value, numberStyles, null, out result);
        return result;
    }

    protected HiddenField hdnIsSizeBased { get { return (HiddenField)findControl("hdnIsSizeBased"); } }
    protected DatePicker dpTransactionDate { get { return (DatePicker)findControl("dpTransactionDate"); } }
    protected RequiredFieldValidator rfvTransactionDate { get { return (RequiredFieldValidator)findControl("rfvTransactionDate"); } }
    protected RangeValidator rvTransactionDate { get { return (RangeValidator)findControl("rvTransactionDate"); } }
    protected Label lblTransactionTime { get { return (Label)findControl("lblTransactionTime"); } }
    protected TimePicker tpTime { get { return (TimePicker)findControl("tpTime"); } }
    protected CustomValidator cvTransactionTime { get { return (CustomValidator)findControl("cvTransactionTime"); } }
    protected Label lblSettlementDate { get { return (Label)findControl("lblSettlementDate"); } }
    protected DatePicker dpSettlementDate { get { return (DatePicker)findControl("dpSettlementDate"); } }
    protected RequiredFieldValidator rfvSettlementDate { get { return (RequiredFieldValidator)findControl("rfvSettlementDate"); } }
    protected DecimalBox dbPrice { get { return (DecimalBox)findControl("dbPrice"); } }
    protected RequiredFieldValidator rfvPrice { get { return (RequiredFieldValidator)findControl("rfvPrice"); } }
    protected DecimalBox dbSize { get { return (DecimalBox)findControl("dbSize"); } }
    protected RequiredFieldValidator rfvSize { get { return (RequiredFieldValidator)findControl("rfvSize"); } }
    protected DecimalBox dbAmount { get { return (DecimalBox)findControl("dbAmount"); } }
    protected RequiredFieldValidator rfvAmount { get { return (RequiredFieldValidator)findControl("rfvAmount"); } }
    protected Label lblServiceCharge { get { return (Label)findControl("lblServiceCharge"); } }
    protected HiddenField hdnInitialServiceChargePercentage { get { return (HiddenField)findControl("hdnInitialServiceChargePercentage"); } }
    protected DecimalBox dbServiceChargePercentage { get { return (DecimalBox)findControl("dbServiceChargePercentage"); } }
    //protected RequiredFieldValidator rfvServiceChargePercentage { get { return (RequiredFieldValidator)findControl("rfvServiceChargePercentage"); } }
    protected DecimalBox dbServiceChargeAmount { get { return (DecimalBox)findControl("dbServiceChargeAmount"); } }
    protected Label lblCounterparty { get { return (Label)findControl("lblCounterparty"); } }
    protected DropDownList ddlCounterpartyAccount { get { return (DropDownList)findControl("ddlCounterpartyAccount"); } }
    protected Label lblExchange { get { return (Label)findControl("lblExchange"); } }
    protected DropDownList ddlExchange { get { return (DropDownList)findControl("ddlExchange"); } }
    protected RequiredFieldValidator rfvExchange { get { return (RequiredFieldValidator)findControl("rfvExchange"); } }
    protected CheckBox chkUseNostro { get { return (CheckBox)findControl("chkUseNostro"); } }
    protected CheckBox chkIsCompleteFill { get { return (CheckBox)findControl("chkIsCompleteFill"); } }
    protected Label lblExRate { get { return (Label)findControl("lblExRate"); } }
    protected DecimalBox dbExRate { get { return (DecimalBox)findControl("dbExRate"); } }
    protected RequiredFieldValidator rfvExRate { get { return (RequiredFieldValidator)findControl("rfvExRate"); } }
    protected Label lblAccruedInterest { get { return (Label)findControl("lblAccruedInterest"); } }
    protected DecimalBox dbAccruedInterest { get { return (DecimalBox)findControl("dbAccruedInterest"); } }

    private Control findControl(string controlId)
    {
        Control control = dvOrderFill.FindControl(controlId);

        if (control != null)
            return control;
        else
            throw new ArgumentException(string.Format("Could not find control with id '{0}'.", controlId));
    }

    protected void OnTransactionDateChanged(OrderFillView orderFillView)
    {
        if (TransactionDateChanged != null)
            TransactionDateChanged(this, new OrderFillEventArgs(orderFillView));
    }

    protected void OnPriceChanged(OrderFillView orderFillView)
    {
        if (PriceChanged != null)
            PriceChanged(this, new OrderFillEventArgs(orderFillView));
    }

    protected void OnFilled()
    {
        if (Filled != null)
            Filled(this, EventArgs.Empty);
    }

    protected void OnCancelled()
    {
        if (Cancelled != null)
            Cancelled(this, EventArgs.Empty);
    }

    public OrderFillEventHandler TransactionDateChanged;
    public OrderFillEventHandler PriceChanged;
    public EventHandler Filled;
    public EventHandler Cancelled;
}
