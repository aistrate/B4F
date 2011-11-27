using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions;

public partial class DividendOverview : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
        cldDateFrom.DateChanged += new EventHandler(cldDate_DateChanged);
        cldDateTo.DateChanged += new EventHandler(cldDate_DateChanged);
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        ((EG)this.Master).setHeaderText = "Dividend Overview";

    }

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        pnlSelectedInstrument.Visible = true;
        if (Util.IsNullDate(cldDateFrom.SelectedDate))
            cldDateFrom.SelectedDate = new DateTime(DateTime.Today.Year, 1, 1);
        ddlSelectedInstrument.DataBind();

        if (ddlSelectedInstrument.Items.Count != 2)
        {
            ddlSelectedInstrument.SelectedIndex = 0;
        }
        else
        {
            ddlSelectedInstrument.SelectedIndex = 1;
            ddlSelectedInstrument_SelectedIndexChanged(ddlSelectedInstrument, EventArgs.Empty);
        }
        databind();
    }

    protected void cldDate_DateChanged(object sender, EventArgs e)
    {
        databind();
    }

    protected void ddlSelectedInstrument_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            databind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void databind()
    {
        gvDividends.Visible = true;
        gvDividends.DataBind();
    }

    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("instrumentHistoryID={0}&Edit={1}", (string)e.CommandArgument, true));
            Response.Redirect(string.Format("~/BackOffice/CorporateActions/DividendDetails.aspx{0}", qStr));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNewDividend_Click(object sender, EventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("Edit={0}", false));
            Response.Redirect(string.Format("~/BackOffice/CorporateActions/DividendDetails.aspx{0}", qStr));
            //if (checkFields())
            //{
            //    DividendAdapter.DividendHistoryDetails newValue = getNewDividendDetails();
            //    Session["instrumentHistoryID"] = DividendAdapter.CreateOrSaveDividendHistory(newValue);
            //}

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    //private DividendAdapter.DividendHistoryDetails getNewDividendDetails()
    //{
    //    DividendAdapter.DividendHistoryDetails returnValue = new DividendAdapter.DividendHistoryDetails();
    //    returnValue.ExDividendDate = dpExDividendDate.SelectedDate;
    //    returnValue.SettlementDate = dpPaymentDate.SelectedDate;
    //    returnValue.FundID = int.Parse(ddlInstrumentOfPosition.SelectedValue);
    //    returnValue.UnitPrice = this.dbPriceQuantity.Value;
    //    returnValue.ExtDescription = this.txtExternalDescription.Text;
    //    return returnValue;
    //}

    //protected void ddlInstrumentOfPosition_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        this.btnNewDividend.Enabled = this.ddlInstrumentOfPosition.SelectedIndex != 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
    //    }
    //}

    //private DateTime getMinimumDate()
    //{
    //    DateTime now = DateTime.Now;
    //    return new DateTime(now.Year, 1, 1);
    //}

    //private bool checkFields()
    //{
    //    ValidatorCollection validators = null;
    //    bool isValid = true;
    //    validators = Page.Validators;
    //    foreach (IValidator validator in validators)
    //    {
    //        if ((validator is RequiredFieldValidator) || (validator is RangeValidator))
    //        {
    //            validator.Validate();
    //            if (!validator.IsValid)
    //                isValid = false;
    //        }
    //    }
    //    return isValid;
    //}

}
