using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Prices;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Tuple;

public partial class DataMaintenance_Prices_XEConverter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "XE Converter";
            BaseCurrencyID = XEConverterAdapter.GetBaseCurrencyId();
            ddlCurrencyTo.SelectedValue = BaseCurrencyID.ToString();
        }
        lblErrorMessage.Text = "";
    }

    protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            DropDownList ddl = (DropDownList)sender;
            DateTime date = XEConverterAdapter.GetExRateDate(Utility.GetKeyFromDropDownList(ddl), dpDate.SelectedDate);
            string strDate = Util.IsNotNullDate(date) ? date.ToString("dd-MM-yyyy") : "";
            if (ddl.ID.Contains("From"))
                lblExRateDateFrom.Text = strDate;
            else
                lblExRateDateTo.Text = strDate;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSwitch_Click(object sender, EventArgs e)
    {
        string idFrom = ddlCurrencyFrom.SelectedValue;
        string idTo = ddlCurrencyTo.SelectedValue;
        ddlCurrencyTo.SelectedValue = idFrom;
        ddlCurrencyFrom.SelectedValue = idTo;
        lblExRateDateFrom.Text = "";
        lblExRateDateTo.Text = "";
        lblConvertedAmount.Text = "";
    }

    protected void btnConvert_Click(object sender, EventArgs e)
    {
        try
        {
            lblConvertedAmount.Text = XEConverterAdapter.ConvertAmount(
                dbAmount.Value,
                Utility.GetKeyFromDropDownList(ddlCurrencyFrom),
                Utility.GetKeyFromDropDownList(ddlCurrencyTo),
                dpDate.SelectedDate);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCurrentBaseAmount_Click(object sender, EventArgs e)
    {
        try
        {
            Tuple<string, DateTime> retval = XEConverterAdapter.ConvertToBaseAmount(
                dbAmount.Value,
                Utility.GetKeyFromDropDownList(ddlCurrencyFrom),
                dpDate.SelectedDate, true);
            lblConvertedAmount.Text = retval.Item1;
            lblExRateDateFrom.Text = retval.Item2.ToString("dd-MM-yyyy");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnBaseAmount_Click(object sender, EventArgs e)
    {
        try
        {
            Tuple<string, DateTime> retval = XEConverterAdapter.ConvertToBaseAmount(
                dbAmount.Value,
                Utility.GetKeyFromDropDownList(ddlCurrencyFrom),
                dpDate.SelectedDate, false);
            lblConvertedAmount.Text = retval.Item1;
            lblExRateDateFrom.Text = retval.Item2.ToString("dd-MM-yyyy");

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected int BaseCurrencyID
    {
        get
        {
            object i = ViewState["BaseCurrencyID"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            ViewState["BaseCurrencyID"] = value;
        }
    }
}
