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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

public partial class RuleEdit : System.Web.UI.Page
{
    public int ID
    {
        get
        {
            object b = ViewState["ID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["ID"] = value;
        }
    }

    public int ModelID
    {
        get
        {
            object b = ViewState["ModelID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["ModelID"] = value;
        }
    }
    
    public int AccountID
    {
        get
        {
            object b = ViewState["AccountID"];
            return ((b == null) ? 0 : (int)b);
        }
        set
        {
            ViewState["AccountID"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = "";
            if (!IsPostBack)
            {
                try
                {
                    initStaticControls();

                    ID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "id");
                    ModelID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "ModelID");
                    AccountID = QueryStringModule.GetValueFromQueryString(Request.RawUrl, "AccountID");
                    if (ID != 0)
                    {
                        loadRecord(ID);
                        hdnIdValue.Value = ID.ToString();
                    }
                    else if (ModelID != 0)
                    {
                        ddlModelPortfolio.SelectedValue = ModelID.ToString();
                    }
                }
                catch (ApplicationException ex)
                {
                    lblErrorMessage.Text = "Error loading rule: " + ex.Message;
                }
            }
            DatePickerStartDate.Expanded += new EventHandler(datePicker_Expanded);
            DatePickerEndDate.Expanded += new EventHandler(datePicker_Expanded);

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Util.GetMessageFromException(ex);
        }
    }

    protected void datePicker_Expanded(object sender, EventArgs e)
    {
        if (DatePickerStartDate.IsExpanded || DatePickerEndDate.IsExpanded)
        {

            ddlModelPortfolio.Visible = false;
            ddlExchange.Visible = false;
            ddlAccount.Visible = false;
            btnFilterAccount.Visible = false;
            ddlInstrument.Visible = false;
            btnFilterInstrument.Visible = false;
        }
        else
        {
            ddlModelPortfolio.Visible = true;
            ddlExchange.Visible = true;
            ddlAccount.Visible = true;
            btnFilterAccount.Visible = true;
            ddlInstrument.Visible = true;
            btnFilterInstrument.Visible = true;
        }
    }

    private void initStaticControls()
    {
        ddlCommissionRuleType.DataSource = RuleEditAdapter.GetCommRuleTypes();
        ddlCommissionRuleType.DataTextField = "Description";
        ddlCommissionRuleType.DataValueField = "Key";
        ddlCommissionRuleType.DataBind();

        ddlBuySell.DataSource = RuleEditAdapter.GetBuySellOptions();
        ddlBuySell.DataTextField = "Description";
        ddlBuySell.DataValueField = "Key";
        ddlBuySell.DataBind();

        ddlOpenClose.DataSource = RuleEditAdapter.GetOpenCloseOptions();
        ddlOpenClose.DataTextField = "Description";
        ddlOpenClose.DataValueField = "Key";
        ddlOpenClose.DataBind();

        ddlOriginalOrderType.DataSource = RuleEditAdapter.GetBaseOrderTypes();
        ddlOriginalOrderType.DataTextField = "Description";
        ddlOriginalOrderType.DataValueField = "Key";
        ddlOriginalOrderType.DataBind();

        lbOrderActionType.DataSource = RuleEditAdapter.GetOrderActionTypeOptions();
        lbOrderActionType.DataTextField = "Description";
        lbOrderActionType.DataValueField = "Key";
        lbOrderActionType.DataBind();

        reqCommissionCalculation.InitialValue = int.MinValue.ToString();
        ddlAccount.SelectedValue = int.MinValue.ToString();
        ddlInstrument.SelectedValue = int.MinValue.ToString();
        ddlOriginalOrderType.SelectedValue = ((int)BaseOrderTypes.Both).ToString();
    }
    
    private void loadRecord(int id)
    {
        if (id != 0)
        {
            ((EG)this.Master).setHeaderText = "Edit Commission Rule";

            CommRuleDetails ruleDetails = RuleEditAdapter.GetCommRuleDetails(id);

            txtCommissionRuleName.Text = ruleDetails.CommRuleName;

            if (Util.IsNotNullDate(ruleDetails.StartDate))
                DatePickerStartDate.SelectedDate = ruleDetails.StartDate;

            if (Util.IsNotNullDate(ruleDetails.EndDate))
                DatePickerEndDate.SelectedDate = ruleDetails.EndDate;

            ddlCommissionCalculation.SelectedValue = ruleDetails.CommCalculation.ToString();

            ddlAdditionalCalculation.SelectedValue = ruleDetails.AdditionalCalculation.ToString();

            if (ruleDetails.CommRuleType != null)
            {
                if (this.ddlCommissionRuleType.Items == null || this.ddlCommissionRuleType.Items.Count <= 1)
                    this.ddlCommissionRuleType.DataBind();
                if (this.ddlCommissionRuleType.Items.FindByValue(ruleDetails.CommRuleType.ToString()) != null)
                    ddlCommissionRuleType.SelectedValue = ruleDetails.CommRuleType.ToString();
            }

            if (ruleDetails.Account != null)
            {
                if (this.ddlAccount.Items == null || this.ddlAccount.Items.Count <= 1)
                    this.ddlAccount.DataBind();
                if (this.ddlAccount.Items.FindByValue(ruleDetails.Account.ToString()) != null)
                    this.ddlAccount.SelectedValue = ruleDetails.Account.ToString();
            }

            if (ruleDetails.Model != null)
            {
                if (this.ddlModelPortfolio.Items == null || this.ddlModelPortfolio.Items.Count <= 1)
                    this.ddlModelPortfolio.DataBind();
                if (this.ddlModelPortfolio.Items.FindByValue(ruleDetails.Model.ToString()) != null)
                    this.ddlModelPortfolio.SelectedValue = ruleDetails.Model.ToString();
            }

            if (ruleDetails.Exchange != null)
            {
                if (this.ddlExchange.Items == null || this.ddlExchange.Items.Count <= 1)
                    this.ddlExchange.DataBind();
                if (this.ddlExchange.Items.FindByValue(ruleDetails.Exchange.ToString()) != null)
                    this.ddlExchange.SelectedValue = ruleDetails.Exchange.ToString();
            }

            if (ruleDetails.Instrument != null)
            {
                if (this.ddlInstrument.Items == null || this.ddlInstrument.Items.Count <= 1)
                    this.ddlInstrument.DataBind();
                if (this.ddlInstrument.Items.FindByValue(ruleDetails.Instrument.ToString()) != null)
                    this.ddlInstrument.SelectedValue = ruleDetails.Instrument.ToString();
            }

            if (ruleDetails.RuleSecCategory != null)
            {
                if (this.ddlSecCategory.Items == null || this.ddlSecCategory.Items.Count <= 1)
                    this.ddlSecCategory.DataBind();
                if (this.ddlSecCategory.Items.FindByValue(ruleDetails.RuleSecCategory.ToString()) != null)
                    this.ddlSecCategory.SelectedValue = ruleDetails.RuleSecCategory.ToString();
            }

            if (ruleDetails.BuySell != null)
            {
                if (this.ddlBuySell.Items == null || this.ddlBuySell.Items.Count <= 1)
                    this.ddlBuySell.DataBind();
                if (this.ddlBuySell.Items.FindByValue(ruleDetails.BuySell.ToString()) != null)
                    ddlBuySell.SelectedValue = ruleDetails.BuySell.ToString();
            }

            if (ruleDetails.OpenClose != null)
            {
                if (this.ddlOpenClose.Items == null || this.ddlOpenClose.Items.Count <= 1)
                    this.ddlOpenClose.DataBind();
                if (this.ddlOpenClose.Items.FindByValue(ruleDetails.OpenClose.ToString()) != null)
                    ddlOpenClose.SelectedValue = ruleDetails.OpenClose.ToString();
            }

            if (ruleDetails.OriginalOrderType != null)
            {
                if (this.ddlOriginalOrderType.Items == null || this.ddlOriginalOrderType.Items.Count <= 1)
                    this.ddlOriginalOrderType.DataBind();
                if (this.ddlOriginalOrderType.Items.FindByValue(ruleDetails.OriginalOrderType.ToString()) != null)
                    ddlOriginalOrderType.SelectedValue = ruleDetails.OriginalOrderType.ToString();
            }

            chkApplyToAllAccounts.Checked = ruleDetails.ApplyToAll;
            chkHasEmployerRelation.Checked = ruleDetails.HasEmployerRelation;

            int orderActionType;
            if (ruleDetails.OrderActionType != null && int.TryParse(ruleDetails.OrderActionType.ToString(), out orderActionType))
            {
                foreach (ListItem item in lbOrderActionType.Items)
                {
                    if (Util.IsEnumInValue(orderActionType, Convert.ToInt32(item.Value)))
                        item.Selected = true;
                }
            }
        }
        else
        {
            ((EG)this.Master).setHeaderText = "Add Commission Rule";
            //ddlCommissionRuleType.SelectedValue = ((int)RuleDependencyType.Generic).ToString();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (serverSideValidationOk())
            {
                int id = 0;
                int commCalcId = 0;
                int.TryParse(hdnIdValue.Value, out id);
                int.TryParse(ddlCommissionCalculation.SelectedValue, out commCalcId);

                saveSpecificRule(id, txtCommissionRuleName.Text, commCalcId);
                btnCancel_Click(sender, e);
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "* " + ex.Message;
        }
    }

    private void saveSpecificRule(int id, string commissionRuleName, int commissionCalculationId)
    {
        CommRuleDetails ruleDetails = new CommRuleDetails();
        
        ruleDetails.CommRuleName = commissionRuleName;
        ruleDetails.CommCalculation = commissionCalculationId;

        if (int.Parse(ddlAdditionalCalculation.SelectedValue) != int.MinValue)
            ruleDetails.AdditionalCalculation = int.Parse(ddlAdditionalCalculation.SelectedValue);

        ruleDetails.ApplyToAll = chkApplyToAllAccounts.Checked;
        ruleDetails.HasEmployerRelation = chkHasEmployerRelation.Checked;
        if (Util.IsNotNullDate(DatePickerStartDate.SelectedDate))
            ruleDetails.StartDate = DatePickerStartDate.SelectedDate;
        else
            ruleDetails.StartDate = DateTime.MinValue;

        if (Util.IsNotNullDate(DatePickerEndDate.SelectedDate))
            ruleDetails.EndDate = DatePickerEndDate.SelectedDate;
        else
            ruleDetails.EndDate = DateTime.MinValue;

        if (int.Parse(ddlAccount.SelectedValue) != int.MinValue)
            ruleDetails.Account = int.Parse(ddlAccount.SelectedValue);
        if (int.Parse(ddlModelPortfolio.SelectedValue) != int.MinValue)
            ruleDetails.Model = int.Parse(ddlModelPortfolio.SelectedValue);
        if (int.Parse(ddlExchange.SelectedValue) != int.MinValue)
            ruleDetails.Exchange = int.Parse(ddlExchange.SelectedValue);
        if (int.Parse(ddlInstrument.SelectedValue) != int.MinValue)
            ruleDetails.Instrument = int.Parse(ddlInstrument.SelectedValue);
        if (int.Parse(ddlSecCategory.SelectedValue) != int.MinValue)
            ruleDetails.RuleSecCategory = int.Parse(ddlSecCategory.SelectedValue);
        if (int.Parse(ddlBuySell.SelectedValue) != int.MinValue)
            ruleDetails.BuySell = int.Parse(ddlBuySell.SelectedValue);
        if (int.Parse(ddlOpenClose.SelectedValue) != int.MinValue)
            ruleDetails.OpenClose = int.Parse(ddlOpenClose.SelectedValue);
        if (int.Parse(ddlOriginalOrderType.SelectedValue) != 0)
            ruleDetails.OriginalOrderType = int.Parse(ddlOriginalOrderType.SelectedValue);
        int orderActionType = 0;
        foreach (ListItem item in lbOrderActionType.Items)
        {
            if (item.Selected)
                orderActionType += Convert.ToInt32(item.Value);
        }
        ruleDetails.OrderActionType = orderActionType;

        ruleDetails.CommRuleType = int.Parse(ddlCommissionRuleType.SelectedValue);

        RuleEditAdapter.SaveCommissionRule(id, ruleDetails);
    }

    private bool serverSideValidationOk()
    {
        // At least one of the fields must be filled in 
        bool result = true;

        if (DatePickerStartDate.IsEmpty)
            throw new ApplicationException("Start date cannot be empty");
        if (ddlAccount.SelectedValue == int.MinValue.ToString() && ddlExchange.SelectedValue == int.MinValue.ToString() &&
            ddlInstrument.SelectedValue == int.MinValue.ToString() && ddlSecCategory.SelectedValue == int.MinValue.ToString() &&
            ddlBuySell.SelectedValue == int.MinValue.ToString())
            throw new ApplicationException("One of the following fields have to be filled: Account, Instrument, Exchange, Security category or Buy/Sell.");
        return result;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (ModelID != 0)
        {
            string qStr = QueryStringModule.Encrypt(string.Format("ModelID={0}", ModelID));
            Response.Redirect(string.Format("~/DataMaintenance/Models/ModelMaintenance.aspx{0}", qStr));
        }
        if (AccountID != 0)
        {
            Session["accountnrid"] = AccountID;
            Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
        }
        else
            Response.Redirect("RuleOverview.aspx");
    }

    protected void btnFilterAccount_Click(object sender, EventArgs e)
    {
        pnlAccountFinder.Visible = !pnlAccountFinder.Visible;
        btnFilterAccount.Text = getFilterButtonLabel(pnlAccountFinder.Visible);
    }

    protected void btnFilterInstrument_Click(object sender, EventArgs e)
    {
        pnlInstrumentFinder.Visible = !pnlInstrumentFinder.Visible;
        btnFilterInstrument.Text = getFilterButtonLabel(pnlInstrumentFinder.Visible);
    }

    private string getFilterButtonLabel(bool findControlVisible)
    {
        return "Filter  " + (findControlVisible ? "<<" : ">>");
    }

    protected void ddlAccount_DataBound(object sender, EventArgs e)
    {
        if (ddlAccount.Items.Count == 2)
            ddlAccount.SelectedIndex = 1;
    }

}
