using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.Collections;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Security;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Prices;

public partial class DataMaintenance_Security : System.Web.UI.Page
{
    public int InstrumentID
    {
        get
        {
            object b = ViewState["InstrumentID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["InstrumentID"] = value;
        }
    }

    public SecCategories SecCategory
    {
        get
        {
            object b = ViewState["SecCategory"];
            return ((b == null) ? SecCategories.Undefined : (SecCategories)b);
        }
        set
        {
            ViewState["SecCategory"] = value;
        }
    }

    public int InstrumentToConvertID
    {
        get
        {
            object b = ViewState["InstrumentToConvertID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["InstrumentToConvertID"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            elbErrorMessage.Text = "";
            if (!IsPostBack)
            {
                Utility.DisablePageCaching();
                Utility.AlertSaveMessage();
                ((EG)this.Master).setHeaderText = "Edit Instrument";

                if (SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                    bntSave.Enabled = true;

                if (QueryStringModule.NameValueCollectionContains(Request.RawUrl, "InstrumentToConvertID"))
                {
                    InstrumentToConvertID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "InstrumentToConvertID");
                    if (Session["instrumentid"] != null)
                    {
                        InstrumentID = (int)Session["instrumentid"];
                        hdnInstrumentId.Value = InstrumentID.ToString();
                        Session["instrumentid"] = null;
                        loadRecord(InstrumentID);
                    }
                    else
                    {
                        loadRecord(InstrumentToConvertID);
                        tbInstrumentName.Text = "New " + tbInstrumentName.Text;
                        tbISIN.Text = "";
                        ucIssueDate.SelectedDate = DateTime.Today;
                    }
                }
                else
                {
                    SecCategory = (SecCategories)QueryStringModule.GetValueFromQueryString(Request.RawUrl, "SecCategory");
                    switch (SecCategory)
                    {
                        case SecCategories.Bond:
                            pnlBond.Visible = true;
                            dbTickSize.Clear();
                            dbTickSize.Enabled = false;
                            break;
                        case SecCategories.VirtualFund:
                            pnlVirtualFund.Visible = true;
                            break;
                    }

                    if (Session["instrumentid"] != null)
                    {
                        InstrumentID = (int)Session["instrumentid"];
                        hdnInstrumentId.Value = InstrumentID.ToString();
                        Session["instrumentid"] = null;
                        loadRecord(InstrumentID);
                    }
                }

                if (InstrumentID == 0)
                {
                    ((EG)this.Master).setHeaderText = "Create New " + SecCategory.ToString();
                    if (InstrumentToConvertID == 0)
                        setDefault();
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    private void setDefault()
    {
        ddlCountry.SelectedValue = "21";
        ddlDefaultRoute.SelectedValue = "1";
        dbDecimalPlaces.Value = 6M;
        ddCurrencyNominal.SelectedValue = "600";
        rbPricingType.SelectedValue = ((int)B4F.TotalGiro.Instruments.PricingTypes.Direct).ToString();
        chkAllowNetting.Checked = true;

        dbDefaultSettlementPeriod.Value = 3M;
        dbNumberOfDecimals.Value = 4M;
        chkDoesSupportAmountBasedBuy.Checked = true;
        chkDoesSupportAmountBasedSell.Checked = true;

        switch (SecCategory)
        {
            case SecCategories.Bond:
                rbPricingType.SelectedValue = ((int)PricingTypes.Percentage).ToString();
                break;
            case SecCategories.VirtualFund:
                dbInitailNAVperUnit.Value = 100;
                dbExactJournalNumber.Value = 4;
                break;
        }
    }

    private void loadRecord(int id)
    {
        btnAddCouponRateHistory.Enabled = false;
        if (id > 0)
        {
            SecurityDetails sec = InstrumentEditAdapter.GetSecurityDetails(id);
            if (sec != null)
            {
                btnAddCouponRateHistory.Enabled = true;
                btnViewPrices.Visible = true;
                SecCategory = sec.SecCategory;
                tbInstrumentName.Text = sec.InstrumentName;
                tbISIN.Text = sec.ISIN;
                tbCompanyName.Text = sec.CompanyName;
                ucIssueDate.SelectedDate = sec.IssueDate;
                ddlHomeExchange.SelectedValue = sec.HomeExchangeID.ToString();
                ddlDefaultExchange.SelectedValue = sec.DefaultExchangeID.ToString();
                ddlCountry.SelectedValue = sec.CountryID.ToString();
                ddlDefaultRoute.SelectedValue = sec.RouteID.ToString();
                dbDecimalPlaces.Value = sec.DecimalPlaces;
                chkActiveCurrenciesOnly.Checked = sec.IsCurrencyActive;
                if (!sec.IsCurrencyActive)
                    chkActiveCurrenciesOnly_CheckedChanged(null, null);
                if (sec.NominalCurrencyID != int.MinValue)
                    ddCurrencyNominal.SelectedValue = sec.NominalCurrencyID.ToString();
                rbPricingType.SelectedValue = ((int)sec.PriceType).ToString();
                chkAllowNetting.Checked = sec.AllowNetting;
                chkGreenFund.Checked = sec.IsGreenFund;
                chkCultureFund.Checked = sec.IsCultureFund;

                switch (sec.SecCategory)
                {
                    case SecCategories.Bond:
                        pnlBond.Visible = true;
                        ddlAccruedInterestCalcType.SelectedValue = ((int)sec.AccruedInterestCalcType).ToString();
                        ddlAccruedInterestCalcType_SelectedIndexChanged(null, null);
                        dbNominalValue.Value = sec.NominalValue;

                        if (this.ddlCouponFreq.Items == null || this.ddlCouponFreq.Items.Count <= 1)
                            this.ddlCouponFreq.DataBind();
                        if (this.ddlCouponFreq.Items.FindByValue(((int)sec.CouponFreq).ToString()) != null)
                            ddlCouponFreq.SelectedValue = ((int)sec.CouponFreq).ToString();

                        dbCouponRate.Value = sec.CouponRate;
                        dpFirstCouponPaymntDate.SelectedDate = sec.FirstCouponPaymntDate;
                        chkUltimoDating.Checked = sec.UltimoDating;
                        dpMaturityDate.SelectedDate = sec.MaturityDate;

                        if (sec.RedemptionAmount != 0)
                            dbRedemptionAmount.Value = sec.RedemptionAmount;
                        chkIsPerpetual.Checked = sec.IsPerpetual;
                        setPerpetual(sec.IsPerpetual);
                        chkFixedCouponRate.Checked = sec.IsFixedCouponRate;
                        setFixedCouponRate(sec.IsFixedCouponRate);
                        break;
                    case SecCategories.VirtualFund:
                        pnlVirtualFund.Visible = true;
                        txtTradingAccountNumber.Text = sec.TradingAccountNumber;
                        txtHoldingAccountNumber.Text = sec.HoldingAccountNumber;
                        dbInitailNAVperUnit.Value = sec.InitialNavPerUnit;
                        dbExactJournalNumber.Text = sec.ExactJournalNumber;
                        dbJournalNumber.Text = sec.JournalNumber;
                        txtJournalDescription.Text = sec.JournalDescription;
                        break;
                }
                
                // activity
                pnlActivity.Visible = true;
                chkIsActive.Checked = sec.IsActive;
                cldInActiveDate.Enabled = !chkIsActive.Checked;
                if (!sec.IsActive)
                    cldInActiveDate.SelectedDate = sec.InActiveDate;

                if (!string.IsNullOrEmpty(sec.ParentInstrumentName))
                {
                    pnlParentInstrument.Visible = true;
                    lblParentInstrument.Text = sec.ParentInstrumentName;
                }

                ddlDefaultCounterparty.SelectedValue = sec.DefaultCounterpartyID.ToString();
                dbDefaultSettlementPeriod.Value = sec.DefaultSettlementPeriod;
                dbNumberOfDecimals.Value = sec.NumberOfDecimals;
                if (sec.SecCategory == SecCategories.Bond)
                {
                    dbTickSize.Enabled = false;
                    dbTickSize.Clear();
                }
                else
                    dbTickSize.Value = sec.TickSize;
                chkDoesSupportAmountBasedBuy.Checked = sec.DoesSupportAmountBasedBuy;
                chkDoesSupportAmountBasedSell.Checked = sec.DoesSupportAmountBasedSell;
                chkDoesSupportServiceCharge.Checked = sec.DoesSupportServiceCharge;
                dbServiceChargePercentageBuy.Value = sec.ServiceChargePercentageBuy;
                dbServiceChargePercentageSell.Value = sec.ServiceChargePercentageSell;
                tbRegisteredInNameof.Text = sec.RegisteredInNameof;
                tbDividendPolicy.Text = sec.DividendPolicy;
                tbCommissionRecipientName.Text = sec.CommissionRecipientName;
                chkCertificationRequired.Checked = sec.CertificationRequired;
            }
        }
    }

    protected void chkIsActive_CheckedChanged(object sender, EventArgs e)
    {
        cldInActiveDate.Enabled = !chkIsActive.Checked;
        if (chkIsActive.Checked)
            cldInActiveDate.Clear();
        else
            cldInActiveDate.SelectedDate = DateTime.Today;
    }

    protected void chkActiveCurrenciesOnly_CheckedChanged(object sender, EventArgs e)
    {
        string strCurId = ddCurrencyNominal.SelectedValue;
        ddCurrencyNominal.ClearSelection();
        ddCurrencyNominal.DataBind();
        if (ddCurrencyNominal.Items.Count > 0)
        {
            if (ddCurrencyNominal.Items.FindByValue(strCurId) != null)
                ddCurrencyNominal.SelectedValue = strCurId;
            else
                ddCurrencyNominal.SelectedValue = XEConverterAdapter.GetBaseCurrencyId().ToString();
        }
    }

    protected bool IsCouponRateLineInsert
    {
        get 
        {
            bool b;
            return (hdnIsCouponRateLineInsert.Value != string.Empty && bool.TryParse(hdnIsCouponRateLineInsert.Value, out b) ? bool.Parse(hdnIsCouponRateLineInsert.Value) : false);
        }
        set
        {
            hdnIsCouponRateLineInsert.Value = value.ToString();
            btnAddCouponRateHistory.Enabled = !value;
        }
    }

    private void setPerpetual(bool isPerpetual)
    {
        dpMaturityDate.Enabled = !isPerpetual;
        rfvMaturityDate.Enabled = !isPerpetual;
        if (isPerpetual)
            dpMaturityDate.Clear();
    }

    protected void setFixedCouponRate(bool isFixedCouponRate)
    {
        dbCouponRate.Enabled = isFixedCouponRate;
        if (!isFixedCouponRate)
            dbCouponRate.Clear();
        pnlCouponRates.Visible = !isFixedCouponRate;
        rfvCouponRate.Enabled = isFixedCouponRate;
    }

    protected void btnViewPrices_Click(object sender, EventArgs e)
    {
        try
        {
            Session["instrumentid"] = InstrumentID;
            Response.Redirect("~/DataMaintenance/Prices/InstrumentPriceUpdate.aspx");
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void bntSave_Click(object sender, EventArgs e)
    {
        try
        {
            Page.Validate();
            bool valid = Page.IsValid;
            int instrumentID = InstrumentID;
            bool blnSaveSuccess = false;

            if (valid)
            {
                SecurityDetails secDetails = new SecurityDetails();

                secDetails.SecCategory = SecCategory;
                secDetails.InstrumentName = tbInstrumentName.Text;
                secDetails.ISIN = tbISIN.Text;
                secDetails.CompanyName = tbCompanyName.Text;
                secDetails.IssueDate = ucIssueDate.SelectedDate;
                secDetails.HomeExchangeID = Utility.GetKeyFromDropDownList(ddlHomeExchange);
                secDetails.DefaultExchangeID = Utility.GetKeyFromDropDownList(ddlDefaultExchange);
                secDetails.CountryID = Utility.GetKeyFromDropDownList(ddlCountry);
                secDetails.RouteID = Utility.GetKeyFromDropDownList(ddlDefaultRoute);
                secDetails.DecimalPlaces = Convert.ToInt32(dbDecimalPlaces.Value);
                secDetails.NominalCurrencyID = Utility.GetKeyFromDropDownList(ddCurrencyNominal);
                secDetails.PriceType = (PricingTypes)Convert.ToInt32(rbPricingType.SelectedValue);
                secDetails.AllowNetting = chkAllowNetting.Checked;
                secDetails.IsGreenFund = chkGreenFund.Checked;
                secDetails.IsCultureFund = chkCultureFund.Checked;
                if (pnlActivity.Visible)
                {
                    secDetails.IsActive = chkIsActive.Checked;
                    secDetails.InActiveDate = cldInActiveDate.SelectedDate;
                }

                switch (SecCategory)
                {
                    case SecCategories.Bond:
                        secDetails.AccruedInterestCalcType = (AccruedInterestCalcTypes)Utility.GetKeyFromDropDownList(ddlAccruedInterestCalcType);
                        secDetails.NominalValue = dbNominalValue.Value;
                        secDetails.CouponFreq = (Regularities)Utility.GetKeyFromDropDownList(ddlCouponFreq);
                        secDetails.CouponRate = dbCouponRate.Value;
                        secDetails.FirstCouponPaymntDate = dpFirstCouponPaymntDate.SelectedDate;
                        secDetails.UltimoDating = chkUltimoDating.Checked;
                        secDetails.MaturityDate = dpMaturityDate.SelectedDate;
                        secDetails.RedemptionAmount = dbRedemptionAmount.Value;
                        secDetails.IsPerpetual = chkIsPerpetual.Checked;
                        secDetails.IsFixedCouponRate = chkFixedCouponRate.Checked;

                        if (instrumentID != 0 && !secDetails.IsFixedCouponRate && gvCouponRates.Rows.Count == 0)
                            throw new ApplicationException("When the bond is not a fixed rate bond fill in some coupon rates.");
                        break;
                    case SecCategories.VirtualFund:
                        secDetails.TradingAccountNumber = txtTradingAccountNumber.Text;
                        secDetails.HoldingAccountNumber = txtHoldingAccountNumber.Text;
                        secDetails.InitialNavPerUnit = dbInitailNAVperUnit.Value;
                        secDetails.ExactJournalNumber = dbExactJournalNumber.Text;
                        secDetails.JournalNumber = dbJournalNumber.Text;
                        secDetails.JournalDescription = txtJournalDescription.Text;
                        break;
                }

                secDetails.DefaultCounterpartyID = Utility.GetKeyFromDropDownList(ddlDefaultCounterparty);
                secDetails.DefaultSettlementPeriod = Convert.ToInt16(dbDefaultSettlementPeriod.Value);
                secDetails.NumberOfDecimals = Convert.ToByte(dbNumberOfDecimals.Value);
                secDetails.TickSize = dbTickSize.Value;
                secDetails.DoesSupportAmountBasedBuy = chkDoesSupportAmountBasedBuy.Checked;
                secDetails.DoesSupportAmountBasedSell = chkDoesSupportAmountBasedSell.Checked;
                secDetails.DoesSupportServiceCharge = chkDoesSupportServiceCharge.Checked;
                secDetails.ServiceChargePercentageBuy = dbServiceChargePercentageBuy.Value;
                secDetails.ServiceChargePercentageSell = dbServiceChargePercentageSell.Value;
                secDetails.RegisteredInNameof = tbRegisteredInNameof.Text;
                secDetails.DividendPolicy = tbDividendPolicy.Text;
                secDetails.CommissionRecipientName = tbCommissionRecipientName.Text;
                secDetails.CertificationRequired = chkCertificationRequired.Checked;

                InstrumentEditAdapter.SaveSecurity(ref instrumentID, ref blnSaveSuccess, secDetails);

                if (InstrumentToConvertID != 0)
                {
                    string qStr = QueryStringModule.Encrypt(string.Format("InstrumentToConvertID={0}&NewInstrumentID={1}&Edit={2}", 
                        InstrumentToConvertID, 
                        instrumentID,
                        false));
                    Response.Redirect(string.Format("~/BackOffice/CorporateActions/InstrumentConversionDetails.aspx{0}", qStr));
                }
                else
                {
                    Session["instrumentid"] = instrumentID;
                    Response.Redirect("Security.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex) + "<br />";
        }
    }

    protected void ddlAccruedInterestCalcType_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool enable = Utility.GetKeyFromDropDownList(ddlAccruedInterestCalcType) > 1;
        rfvCouponFreq.Enabled = enable;
        rfvCouponRate.Enabled = enable;
        rfvFirstCouponPaymntDate.Enabled = enable;

    }

    protected void chkIsPerpetual_CheckedChanged(object sender, EventArgs e)
    {
        setPerpetual(chkIsPerpetual.Checked);
    }

    protected void chkFixedCouponRate_CheckedChanged(object sender, EventArgs e)
    {
        setFixedCouponRate(chkFixedCouponRate.Checked);
    }

    protected void btnAddCouponRateHistory_Click(object sender, EventArgs e)
    {
        IsCouponRateLineInsert = true;
        gvCouponRates.DataBind();

        if (gvCouponRates.PageIndex != gvCouponRates.PageCount - 1 && (gvCouponRates.PageCount - 1) >= 0)
        {
            gvCouponRates.PageIndex = gvCouponRates.PageCount - 1;
            gvCouponRates.DataBind();
        }
        gvCouponRates.EditIndex = gvCouponRates.Rows.Count - 1;
    }

    protected void gvCouponRates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView dataRowView = (DataRowView)e.Row.DataItem;

                if (gvCouponRates.EditIndex >= 0 && e.Row.RowIndex == gvCouponRates.EditIndex)
                {
                    editingRow = e.Row;

                    lbtDelete.Visible = false;
                    lbtEdit.Visible = false;
                    lbtUpdate.Visible = true;
                    lbtCancel.Visible = true;
                    btnAddCouponRateHistory.Enabled = false;

                    //    //if ((decimal)dataRowView["DebitQuantity"] != 0m)
                    //    //    DebitQuantity = (decimal)dataRowView["DebitQuantity"];

                    //    //if ((decimal)dataRowView["CreditQuantity"] != 0m)
                    //    //    CreditQuantity = (decimal)dataRowView["CreditQuantity"];

                    //    //if (dataRowView["GiroAccount_Number"] != DBNull.Value)
                    //    //    GiroAccountNumber = (string)dataRowView["GiroAccount_Number"];

                    //    //if (dataRowView["Description"] != DBNull.Value)
                    //    //    Description = (string)dataRowView["Description"];

                    //    //ddlGLAccount.Focus();
                    //}
                    //else
                    //{
                    //    //gvCouponRates.EditIndex = -1;
                    //    //throw new ApplicationException(
                    //    //    string.Format("Journal Entry Line number '{0}' is not editable.", (int)dataRowView["LineNumber"]));
                    //}
                }
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCouponRates_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName.ToUpper())
            {
                case "":
                    gvCouponRates.EditIndex = -1;
                    TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;
                    gvCouponRates.SelectedIndex = ((GridViewRow)tableRow).RowIndex;
                    break;
                case "CANCEL":
                    IsCouponRateLineInsert = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCouponRates_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            e.NewValues["CouponRate"] = decimal.Parse(e.NewValues["CouponRate"].ToString());
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvCouponRates_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        if (e.Exception != null)
        {
            e.ExceptionHandled = true;
            e.KeepInEditMode = true;
        }
        else
        {
            IsCouponRateLineInsert = false;
        }
    }


    protected void lbtEdit_Command(object sender, CommandEventArgs e)
    {
        try
        {
            IsCouponRateLineInsert = false;

            int couponRateHistoryId = int.Parse((string)e.CommandArgument);
            gvCouponRates.EditIndex = findRowIndex(gvCouponRates, couponRateHistoryId);
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void lbtDelete_Command(object sender, CommandEventArgs e)
    {
        try
        {
            int couponRateHistoryId = int.Parse((string)e.CommandArgument);
            InstrumentEditAdapter.DeleteBondCouponRateLine(couponRateHistoryId, InstrumentID);

            IsCouponRateLineInsert = false;
            gvCouponRates.EditIndex = -1;
            gvCouponRates.DataBind();
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void odsCouponRates_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        try
        {
            if (e.Exception != null)
            {
                string errMessage = "";
                if (e.Exception.InnerException != null)
                    errMessage = Utility.GetCompleteExceptionMessage(e.Exception.InnerException);
                else
                    errMessage = Utility.GetCompleteExceptionMessage(e.Exception);

                if (errMessage != "")
                {
                    elbErrorMessage.Text = errMessage;
                }
                e.ExceptionHandled = true;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private int findRowIndex(GridView gridView, int key)
    {
        int rowIndex = -1;
        for (int i = 0; i < gridView.DataKeys.Count; i++)
            if ((int)gridView.DataKeys[i].Value == key)
                rowIndex = i;

        return rowIndex;
    }

    protected GridViewRow EditingRow
    {
        get
        {
            if (editingRow == null)
                editingRow = (gvCouponRates.EditIndex >= 0 ? gvCouponRates.Rows[gvCouponRates.EditIndex] : null);

            return editingRow;
        }
    }


    protected LinkButton lbtDelete { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtDelete"); } }
    protected LinkButton lbtEdit { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtEdit"); } }
    protected LinkButton lbtUpdate { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtUpdate"); } }
    protected LinkButton lbtCancel { get { return (LinkButton)Utility.FindControl(EditingRow, "lbtCancel"); } }

    private GridViewRow editingRow;
}