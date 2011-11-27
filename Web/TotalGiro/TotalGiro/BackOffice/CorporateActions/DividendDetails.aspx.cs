using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions;
using B4F.TotalGiro.Instruments.CorporateAction;
using System.Data;

public partial class DividendDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Dividend Details";
            bool? isEdit = QueryStringModule.GetBValueFromQueryString(Request.RawUrl, "Edit");
            int id = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "instrumentHistoryID");
            if (isEdit.HasValue && isEdit.Value && id != 0)
            {
                InstrumentHistoryID = id;
                this.DataBind();
                displayDividendHistory();
            }
            else
            {
                enableControls(false, false, false);
                btnSaveDetails.Enabled = false;
                btnInitialise.Enabled = false;
                ddlDividendTaxStyle.SelectedValue = ((int)DividendTaxStyle.Nett).ToString();
            }

        }
    }

    protected int InstrumentHistoryID
    {
        get
        {
            object i = ViewState["InstrumentHistoryID"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            ViewState["InstrumentHistoryID"] = value;
            this.hdnInstrumentHistoryID.Value = value.ToString();
        }
    }

    protected bool IsInitialised
    {
        get
        {
            object i = ViewState["isInitialised"];
            return ((i == null) ? false : (bool)i);
        }
        set
        {
            ViewState["isInitialised"] = value;
        }
    }

    protected void gvDividendDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["AccountID"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void ddlDividendTaxStyle_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            setUpTax(this.IsInitialised, ((DividendTaxStyle)(int.Parse(ddlDividendTaxStyle.SelectedValue))));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void ddlInstrumentOfPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            this.btnSaveDetails.Enabled = this.ddlInstrumentOfPosition.SelectedIndex != 0;
            int id = Utility.GetKeyFromDropDownList(ddlInstrumentOfPosition);
            if (id != 0 && id != int.MinValue)
                txtIsin.Text = DividendAdapter.GetStockDividendIsin(id);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void rblDividendType_SelectedIndexChanged(object sender, EventArgs e)
    {
        setDividendType(rblDividendType.SelectedValue == "1");
    }

    protected void setDividendType(bool isCash)
    {
        rfvPriceQuantity.Enabled = isCash;
        dbScripRatio.Enabled = !isCash;
        rfvScripRatio.Enabled = !isCash;
        if (isCash)
            dbScripRatio.Clear();
    }

    private DateTime getMinimumDate()
    {
        DateTime now = DateTime.Now;
        return new DateTime(now.Year, 1, 1);
    }

    protected void btnSaveDetails_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkFields())
            {
                DividendAdapter.DividendHistoryDetails newValue = getDividendDetails();
                this.InstrumentHistoryID = DividendAdapter.CreateOrSaveDividendHistory(newValue);
                displayDividendHistory();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
    protected void btnInitialise_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkFields())
            {
                int test = DividendAdapter.InitialiseDividend(this.InstrumentHistoryID);
                displayDividendHistory();
                DataBind();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnExecute_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkFields())
            {
                this.InstrumentHistoryID = DividendAdapter.ExecuteDividend(this.InstrumentHistoryID);
                displayDividendHistory();
                DataBind();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnLichten_Click(object sender, EventArgs e)
    {
        try
        {
            this.InstrumentHistoryID = DividendAdapter.LichtenDividend(this.InstrumentHistoryID);
            displayDividendHistory();
            DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private DividendAdapter.DividendHistoryDetails getDividendDetails()
    {
        DividendAdapter.DividendHistoryDetails returnValue = new DividendAdapter.DividendHistoryDetails();
        returnValue.Key = this.InstrumentHistoryID;
        returnValue.DividendType = rblDividendType.SelectedValue == "1" ? DividendTypes.Cash : DividendTypes.Scrip;
        returnValue.ExDividendDate = dpExDividendDate.SelectedDate;
        returnValue.SettlementDate = dpPaymentDate.SelectedDate;
        returnValue.FundID = int.Parse(ddlInstrumentOfPosition.SelectedValue);
        returnValue.StockDivIsin = txtIsin.Text;
        returnValue.TotalDividendDeposited = dbTotalDividendDeposited.Value;
        returnValue.UnitPrice = this.dbPriceQuantity.Value;
        returnValue.ScripRatio = this.dbScripRatio.Value;
        returnValue.ExtDescription = this.txtExternalDescription.Text;
        returnValue.DividendTaxStyle = int.Parse(this.ddlDividendTaxStyle.SelectedValue);
        returnValue.TaxPercentage = this.dbTaxPercentage.Value;
        return returnValue;
    }

    private void displayDividendHistory()
    {
        if (this.InstrumentHistoryID != 0)
        {
            DividendAdapter.DividendHistoryDetails details = DividendAdapter.GetDividendDetails(this.InstrumentHistoryID);
            this.ddlInstrumentOfPosition.SelectedValue = details.FundID.ToString();
            rblDividendType.SelectedValue = ((int)details.DividendType).ToString();
            this.txtIsin.Text = details.StockDivIsin;
            this.dbTotalDividendDeposited.Value = details.TotalDividendDeposited;
            this.dpExDividendDate.SelectedDate = details.ExDividendDate;
            this.dpPaymentDate.SelectedDate = details.SettlementDate;
            this.dbPriceQuantity.Value = details.UnitPrice;
            this.dbScripRatio.Value = details.ScripRatio;
            this.txtExternalDescription.Text = details.ExtDescription;
            this.ddlDividendTaxStyle.SelectedValue = (details.DividendTaxStyle).ToString();
            this.dbTaxPercentage.Value = details.TaxPercentage;
            this.IsInitialised = details.IsInitialised;
            this.lblTotalUnitsInPossession.Text = details.TotalUnitsInPossession;
            this.lblTotalDivAmount.Text = details.TotalDividendAmount;

            this.btnLichten.Visible = details.IsStockDiv;
            enableControls(this.IsInitialised, details.IsExecuted, details.IsGelicht);
            setUpTax(this.IsInitialised, ((DividendTaxStyle)(int.Parse(ddlDividendTaxStyle.SelectedValue))));
        }
        else
        {
            btnSaveDetails.Text = "Create New Dividend";
        }
    }

    private void setUpTax(bool isInitialised, DividendTaxStyle style)
    {
        bool enabled = !isInitialised && style == DividendTaxStyle.Gross;
        this.dbTaxPercentage.Enabled = enabled;
        this.rfvTaxPercentage.Enabled = enabled;
        this.cvTaxPercentage.Enabled = enabled;
    }

    private void enableControls(bool isInitialised, bool isExecuted, bool isGelicht)
    {
        this.ddlInstrumentOfPosition.Enabled = !isInitialised;
        this.txtIsin.Enabled = !isInitialised;
        this.dbTotalDividendDeposited.Enabled = !isInitialised;
        this.dpExDividendDate.Enabled = !isInitialised;
        this.dpPaymentDate.Enabled = !isInitialised;
        this.rblDividendType.Enabled = !isInitialised;
        this.dbPriceQuantity.Enabled = !isInitialised;
        this.dbScripRatio.Enabled = !isInitialised;
        this.txtExternalDescription.Enabled = !isInitialised;
        this.dbTaxPercentage.Enabled = !isInitialised;
        this.ddlDividendTaxStyle.Enabled = !isInitialised;
        this.btnSaveDetails.Enabled = !isInitialised;
        this.btnInitialise.Enabled = !isInitialised;
        this.btnExecute.Enabled = isInitialised && !isExecuted;
        this.btnLichten.Enabled = isInitialised && isExecuted && !isGelicht;
        if (!isInitialised)
            setDividendType(rblDividendType.SelectedValue == "1");
    }

    private bool checkFields()
    {
        ValidatorCollection validators = null;
        bool isValid = true;
        validators = Page.Validators;
        foreach (IValidator validator in validators)
        {
            if ((validator is RequiredFieldValidator) || (validator is RangeValidator) || (validator is CompareValidator))
            {
                validator.Validate();
                if (!validator.IsValid)
                    isValid = false;
            }
        }
        return isValid;
    }
}
