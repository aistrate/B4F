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
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;
using B4F.TotalGiro.Orders;

public partial class POSSingleOrder : System.Web.UI.Page
{
    private bool isRblIgnoreWarningVisible = false;
    private bool isBypassValidation = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = Page.Title;
            ViewState["IsRblIgnoreWarningVisible"] = false;
        }

        string script = "if (document.getElementById('hdnOrderPlaced') != null) alert('Order has been placed.');";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "OrderPlacedScript", script, true);

        lblErrorMessage.Text = "";
        isBypassValidation = chkBypassValidation.Visible;
        rblIgnoreWarning.Visible = false;
        chkBypassValidation.Visible = false;

        isRblIgnoreWarningVisible = (bool)ViewState["IsRblIgnoreWarningVisible"];
        ViewState["IsRblIgnoreWarningVisible"] = false;
    }


    protected void btnFilterInstrument_Click(object sender, EventArgs e)
    {
        pnlInstrumentFinder.Visible = !pnlInstrumentFinder.Visible;
        //pnlAccountFinder.Visible = false;
        setFilterButtonLabels();
    }

    private string getFilterButtonLabel(bool findControlVisible)
    {
        return "Filter  " + (findControlVisible ? "<<" : ">>");
    }

    private void setFilterButtonLabels()
    {
        //btnFilterAccount.Text = getFilterButtonLabel(pnlAccountFinder.Visible);
        btnFilterInstrument.Text = getFilterButtonLabel(pnlInstrumentFinder.Visible);
    }

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        int accountId = int.Parse(ddlAccount.SelectedValue);
        string cashPositionString = "", openOrderCashString = "";

        //if (accountId > 0)
        //{
        //    try
        //    {
        //        SingleOrderAdapter.GetTotalCashAmount(accountId, out cashPositionString, out openOrderCashString);
        //    }
        //    catch (Exception ex)
        //    {
        //        lblErrorMessage.Text = "<br/>" + Utility.GetCompleteExceptionMessage(ex);
        //    }
        //}
        
        //lblCashPosition.Text = cashPositionString;
        //lblOpenOrders.Text = openOrderCashString;
    }

    protected void rblType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //checkSizeBuyCase();
        showSizeAmountControls(rblType.SelectedValue);
    }

    protected void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        if (!(isRblIgnoreWarningVisible && rblIgnoreWarning.SelectedValue == "No") && validate())
        {
            try
            {
                string msge;
                Side side = (rblSide.SelectedValue == "Buy" ? Side.Buy : Side.Sell);
                decimal size = (rblType.SelectedValue == "Size" ? dbSize.Value : 0M);
                decimal amount = (rblType.SelectedValue == "Amount" ? dbAmount.Value : 0M);

                OrderValidationResult validationResult = SinglePOSOrderAdapter.PlaceOrder(
                    int.Parse(ddlAccount.SelectedValue), int.Parse(ddlInstrument.SelectedValue), side, rblType.SelectedValue == "Amount",
                    size, amount, int.Parse(ddlCurrency.SelectedValue), true, ignoreWarnings(), bypassValidation());
                
                SinglePOSOrderAdapter.AggregatePOSOrders(out msge);

                updateLabels(validationResult);

                if (validationResult.MainType == OrderValidationType.Success)
                {
                    if (chkClearScreen.Checked)
                        clearScreen();

                    // JavaScript message-box: 'Order has been placed.'
                    Page.ClientScript.RegisterHiddenField("hdnOrderPlaced", "true");
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "<br/>" + Utility.GetCompleteExceptionMessage(ex);
            }
        }
    }
    

    private bool validate()
    {
        bool result = false;

        try
        {
            decimal val = (rblType.SelectedValue == "Size" ? dbSize.Value : dbAmount.Value);
            result = true;
        }
        catch (FormatException)
        {
            lblErrorMessage.Text = "<br/>" + rblType.SelectedValue + " not in numerical format.";
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "<br/>" + Utility.GetCompleteExceptionMessage(ex);
        }

        return result;
    }

    private bool ignoreWarnings()
    {
        return (isRblIgnoreWarningVisible && rblIgnoreWarning.SelectedValue == "Yes");
    }

    private bool bypassValidation()
    {
        return (isBypassValidation && chkBypassValidation.Checked);
    }

    private void updateLabels(OrderValidationResult validationResult)
    {
        chkBypassValidation.Visible = false;
        chkBypassValidation.Checked = false;
        switch (validationResult.MainType)
        {
            case OrderValidationType.Warning:
                lblErrorMessage.Text = "<br/>" + validationResult.Message + "<br/>Do you want to enter this order?";
                rblIgnoreWarning.Visible = true;
                rblIgnoreWarning.SelectedValue = "No";
                ViewState["IsRblIgnoreWarningVisible"] = true;
                break;
            case OrderValidationType.Invalid:
                lblErrorMessage.Text = "<br/>" + validationResult.Message;
                chkBypassValidation.Visible = true;
                break;
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearScreen();
    }

    private void checkSizeBuyCase()
    {
        if (rblType.SelectedValue == "Size" && rblSide.SelectedValue == "Buy")
        {
            lblErrorMessage.Text = "<br/>" + "Combination Buy/Size not possible.";
            rblSide.SelectedValue = "Sell";
        }
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
                    break;
                case "Amount":
                    dbAmount.Clear();
                    ddlCurrency.DataBind();
                    //rblCommission.SelectedValue = null;
                    break;
            }
        }
    }

    private void clearScreen()
    {
        ddlAccount.SelectedValue = int.MinValue.ToString();
        ddlInstrument.SelectedValue = int.MinValue.ToString();
        ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);
        
        //pnlAccountFinder.Visible = false;
        pnlInstrumentFinder.Visible = false;
        setFilterButtonLabels();

        rblType.SelectedValue = null;
        rblSide.SelectedValue = null;
        ddlCurrency.DataBind();
        showSizeAmountControls("");
        chkBypassValidation.Visible = false;
        chkBypassValidation.Checked = false;
    }

    protected void ddlAccountInstrument_DataBound(object sender, EventArgs e)
    {
        ddlCurrency.DataBind();
    }

    protected void ddlCurrency_DataBound(object sender, EventArgs e)
    {
        if (ddlCurrency.Items.Count == 2)
            ddlCurrency.SelectedIndex = 1;
    }
}
