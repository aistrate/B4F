using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions;

public partial class BonusDistributionDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //((EG)this.Master).setHeaderText = "Bonus Distribution Details";
            ////this.hdnInstrumentHistoryID.Value = ((int)Session["instrumentHistoryID"]).ToString();
            ////this.ravExDividendDate.MinimumValue = getMinimumDate().ToString();
            ////this.ravExDividendDate.MaximumValue = DateTime.Today.AddDays(-1).ToString();
            //this.DataBind();
            //displayDistributionDetails();
            //this.gvBonusDistributionDetails.Sort("Account", SortDirection.Ascending);
        }
    }



    protected int InstrumentHistoryID
    {
        get
        {
            object i = Session["instrumentHistoryID"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            Session["instrumentHistoryID"] = value;
            this.hdnInstrumentHistoryID.Value = value.ToString();
        }
    }

    protected void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //this.btnNewDividend.Enabled = this.ddlInstrumentOfPosition.SelectedIndex != 0;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    private void displayDistributionDetails()
    {
        if (this.InstrumentHistoryID != 0)
        {
            //BonusDistributionAdapter.BonusDistributionDetails details = BonusDistributionAdapter.GetBonusDistributionDetails(this.InstrumentHistoryID);
            //this.ddlInstrument.SelectedValue = details.FundID.ToString();
            //this.dpBonusDate.SelectedDate = details.ChangeDate;
            //if (details.Accountid != 0) this.ddlAccounts.SelectedValue = details.Accountid.ToString();
            //this.txtTotalCustomerHoldings.Text = details.TotalHoldingsAtDate.ToString();
            //this.txtSizeToDistibute.Text = details.SizeToDistribute.ToString();
            //this.dpPaymentDate.SelectedDate = details.SettlementDate;
            //this.dbPriceQuantity.Value = details.UnitPrice;
            //this.txtExternalDescription.Text = details.ExtDescription;
            //this.ddlDividendTaxStyle.SelectedValue = (details.DividendTaxStyle).ToString();
            //this.dbTaxPercentage.Value = (details.TaxPercentage != null) ? details.TaxPercentage : 0m;
            //this.IsInitialised = details.IsInitialised;

            //enableControls(this.IsInitialised, details.IsExecuted);
            //setUpTax(this.IsInitialised, ((DividendTaxStyle)(int.Parse(ddlDividendTaxStyle.SelectedValue))));
        }
    }
}
