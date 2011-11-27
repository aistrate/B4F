using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Instruments;

public partial class DataMaintenance_Instruments_BondTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            dtpSettlementDate.SelectedDate = DateTime.Today;
            lblCurrency.Text = "€";
            ddlOrderType.SelectedIndex = 1;
        }
    }

    protected void ddlOrderType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bool isAmountBased = (ddlOrderType.SelectedValue == ((int)BaseOrderTypes.AmountBased).ToString());
            dbAmount.Enabled = isAmountBased;
            reqAmount.Enabled = isAmountBased;

            dbSize.Enabled = !isAmountBased;
            reqSize.Enabled = !isAmountBased;
            if (isAmountBased)
                dbSize.Clear();
            else
            {
                dbAmount.Clear();
                dbPrice.Clear();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
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

    protected void btnTest_Click(object sender, EventArgs e)
    {
        lblResult.Text = BondTestAdapter.CalculateAccruedInterest(
            Utility.GetKeyFromDropDownList(ddlInstrument),
            dtpSettlementDate.SelectedDate,
            (BaseOrderTypes)Utility.GetKeyFromDropDownList(ddlOrderType),
            dbAmount.Value, dbSize.Value, dbPrice.Value,
            Utility.GetKeyFromDropDownList(ddlExchange));
    }
}
