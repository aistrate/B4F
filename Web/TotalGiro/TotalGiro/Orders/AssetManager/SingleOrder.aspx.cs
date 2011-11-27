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
using B4F.TotalGiro.ApplicationLayer.Orders.AssetManager;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Security;

public partial class Orders_AssetManager_SingleOrder : System.Web.UI.Page
{
    private bool isRblIgnoreWarningVisible = false;
    private bool isBypassValidation = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = Page.Title;
            ViewState["IsRblIgnoreWarningVisible"] = false;
            setFilterVisible(pnlAccountFinder, btnFilterAccount, true);
            setFilterVisible(pnlInstrumentFinder, btnFilterInstrument, true);
            ddlAccount.DataBind();
            ddlInstrument.DataBind();
            ddlAccount.Focus();

            addOnClickScript();
        }

        btnPlaceOrder.Enabled = true;

        elbErrorMessage.Text = "";
        isBypassValidation = cbBypassValidation.Visible;
        rblIgnoreWarning.Visible = false;
        cbBypassValidation.Visible = false;

        isRblIgnoreWarningVisible = (bool)ViewState["IsRblIgnoreWarningVisible"];
        ViewState["IsRblIgnoreWarningVisible"] = false;
    }

    private void addOnClickScript()
    {
        string jsScript = string.Format(
            @"  if (Page_ClientValidate('{0}') == false)
                    return true;
                var errMsg = document.getElementById('{1}');
                if (errMsg != null)
                    errMsg.innerHTML = 'Please wait...';
                else
                    document.getElementById('waitMessage').innerHTML = 'Please wait...';
                this.disabled = true;
                {2};
                return true;",
            btnPlaceOrder.ValidationGroup,
            elbErrorMessage.ClientID,
            Page.ClientScript.GetPostBackEventReference(btnPlaceOrder, null));

        btnPlaceOrder.Attributes.Add("onclick", jsScript);
    }

    protected void btnFilterAccount_Click(object sender, EventArgs e)
    {
        setFilterVisible(pnlAccountFinder, btnFilterAccount, !pnlAccountFinder.Visible);
        btnFilterAccount.Focus();
    }

    protected void btnFilterInstrument_Click(object sender, EventArgs e)
    {
        setFilterVisible(pnlInstrumentFinder, btnFilterInstrument, !pnlInstrumentFinder.Visible);
        btnFilterInstrument.Focus();
    }

    private void setFilterVisible(Panel filterPanel, Button filterButton, bool visible)
    {
        filterPanel.Visible = visible;
        filterButton.Text = "Filter  " + (visible ? "<<" : ">>");
    }

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        int accountId = int.Parse(ddlAccount.SelectedValue);
        string cashPositionString = "", moneyFundPositionString = "", openOrderCashString = "";

        if (accountId > 0)
        {
            try
            {
                SingleOrderAdapter.GetTotalCashAmount(accountId, out cashPositionString, out moneyFundPositionString, out openOrderCashString);
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }
        
        lblCashPosition.Text = cashPositionString;
        lblMoneyFund.Text = moneyFundPositionString;
        lblOpenOrders.Text = openOrderCashString;
        ddlAccount.Focus();
    }

    protected void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlInstrument.Focus();
    }

    protected void rbType_CheckedChanged(object sender, EventArgs e)
    {
        showSizeAmountControls(((RadioButton)sender).Text);
    }

    protected void rbType_PreRender(object sender, EventArgs e)
    {
        // this is necessary because event CheckedChanged stops working properly
        // after pressing "Place Order" (full postback)
        RadioButton radioButton = (RadioButton)sender;
        radioButton.InputAttributes.Add("onclick",
            string.Format("javascript:setTimeout('{0};', 0)",
                          Page.ClientScript.GetPostBackEventReference(radioButton, "").Replace("'", @"\'")));
    }

    protected void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        if (!(isRblIgnoreWarningVisible && rblIgnoreWarning.SelectedValue == "No") && validate())
        {
            try
            {
                Side side = (rblSide.SelectedValue == "Buy" ? Side.Buy : Side.Sell);
                decimal size = (rbTypeSize.Checked ? dbSize.Value : 0m);
                decimal amount = (rbTypeAmount.Checked ? dbAmount.Value : 0m);
                bool noCharges = (rbTypeAmount.Checked ? rblCommission.SelectedValue == "No commission" : chkNoCommission.Checked);

                OrderValidationResult validationResult = SingleOrderAdapter.PlaceOrder(
                    int.Parse(ddlAccount.SelectedValue), int.Parse(ddlInstrument.SelectedValue), side, rbTypeAmount.Checked,
                    size, amount, int.Parse(ddlCurrency.SelectedValue), rblCommission.SelectedValue == "Inclusive",
                    noCharges, ignoreWarnings(), bypassValidation());

                updateLabels(validationResult);

                if (validationResult.MainType == OrderValidationType.Success)
                {
                    if (cbClearScreen.Checked)
                        clearScreen();

                    ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);

                    elbErrorMessage.Text = "Order was successfully placed.";
                    return;
                }
            }
            catch (Exception ex)
            {
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }
        
        btnPlaceOrder.Focus();
    }

    private bool validate()
    {
        bool result = false;

        try
        {
            if (!rbTypeSize.Checked && !rbTypeAmount.Checked)
                elbErrorMessage.Text = "Please choose an order type (Size or Amount).";
            else
            {
                decimal val = (rbTypeSize.Checked ? dbSize.Value : dbAmount.Value);
                result = true;
            }
        }
        catch (FormatException)
        {
            elbErrorMessage.Text = (rbTypeSize.Checked ? "Size" : "Amount") + " not in numerical format.";
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }

        return result;
    }

    private bool ignoreWarnings()
    {
        return (isRblIgnoreWarningVisible && rblIgnoreWarning.SelectedValue == "Yes");
    }

    private bool bypassValidation()
    {
        return (isBypassValidation && cbBypassValidation.Checked);
    }

    private void updateLabels(OrderValidationResult validationResult)
    {
        cbBypassValidation.Visible = false;
        cbBypassValidation.Checked = false;
        switch (validationResult.MainType)
        {
            case OrderValidationType.Warning:
                elbErrorMessage.Text = validationResult.Message + "<br/>Do you want to enter this order?";
                rblIgnoreWarning.Visible = true;
                rblIgnoreWarning.SelectedValue = "No";
                ViewState["IsRblIgnoreWarningVisible"] = true;
                break;
            case OrderValidationType.Invalid:
                elbErrorMessage.Text = validationResult.Message;
                cbBypassValidation.Visible = SecurityManager.IsCurrentUserInRole("Asset Manager: Manual Order Bypass Validation");
                break;
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearScreen();
    }

    private void showSizeAmountControls(string what)
    {
        mvwLabels.Visible = (what != "");
        mvwValid.Visible = (what != "");
        mvwSizeAmount.Visible = (what != "");
        
        if (what != "")
        {
            int activeViewIndex = (what == "Size" ? 0 : 1);
            mvwLabels.ActiveViewIndex = activeViewIndex;
            mvwValid.ActiveViewIndex = activeViewIndex;
            mvwSizeAmount.ActiveViewIndex = activeViewIndex;

            switch (what)
            {
                case "Size":
                    dbSize.Clear();
                    dbSize.Focus();
                    chkNoCommission.Checked = false;
                    break;
                case "Amount":
                    dbAmount.Clear();
                    ddlCurrency.DataBind();
                    rblCommission.SelectedValue = null;
                    dbAmount.Focus();
                    break;
            }
        }
    }

    private void clearScreen()
    {
        ddlAccount.SelectedValue = int.MinValue.ToString();
        ddlInstrument.SelectedValue = int.MinValue.ToString();
        ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);

        rblSide.SelectedValue = null;
        rbTypeSize.Checked = false;
        rbTypeAmount.Checked = false;
        showSizeAmountControls("");
        cbBypassValidation.Visible = false;
        cbBypassValidation.Checked = false;
        ddlAccount.Focus();
    }

    protected void ddlAccount_DataBound(object sender, EventArgs e)
    {
        if (ddlAccount.Items.Count == 2)
            ddlAccount.SelectedIndex = 1;
        ddlCurrency.DataBind();
        ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);
    }

    protected void ddlInstrument_DataBound(object sender, EventArgs e)
    {
        if (ddlInstrument.Items.Count == 2)
            ddlInstrument.SelectedIndex = 1;
        ddlCurrency.DataBind();
        ddlInstrument.Focus();
    }

    protected void ddlCurrency_DataBound(object sender, EventArgs e)
    {
        if (ddlCurrency.Items.Count == 2)
            ddlCurrency.SelectedIndex = 1;
    }
}
