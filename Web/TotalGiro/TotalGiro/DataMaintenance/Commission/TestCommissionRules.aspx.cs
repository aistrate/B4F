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
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Utils;

public partial class Commission_TestCommissionRules : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            dtpDate.SelectedDate = DateTime.Today;
            ddlOrderActionType.SelectedValue = ((int)OrderActionTypes.NoAction).ToString();
            ddlBuySell.SelectedValue = "1";
            lblCurrency.Text = "€";
        }
    }

    protected void ddlOriginalOrderType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bool isAmountBased = (ddlOriginalOrderType.SelectedValue == ((int)BaseOrderTypes.AmountBased).ToString());
            dbAmount.Enabled = isAmountBased;
            reqAmount.Enabled = isAmountBased;

            dbSize.Enabled = !isAmountBased;
            reqSize.Enabled = !isAmountBased;
            if (isAmountBased)
            {
                dbSize.Clear();
                dbPrice.Clear();
            }
            else
                dbAmount.Clear();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    protected void btnTest_Click(object sender, EventArgs e)
    {
        lblCalcFound.Text = TestCommissionRuleAdapter.DoTest(
            Utility.GetKeyFromDropDownList(ddlAccount), 
            Utility.GetKeyFromDropDownList(ddlInstrument),
            dtpDate.SelectedDate,
            (OrderActionTypes)Utility.GetKeyFromDropDownList(ddlOrderActionType),
            dbSize.Value, dbAmount.Value, dbPrice.Value, cbIsValueIncludingCommission.Checked,
            (BaseOrderTypes)Utility.GetKeyFromDropDownList(ddlOriginalOrderType),
            ((CommRuleBuySell)Utility.GetKeyFromDropDownList(ddlBuySell) == CommRuleBuySell.Sell ? Side.Sell : Side.Buy));

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
