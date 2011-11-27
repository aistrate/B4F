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

public partial class InstrumentCurrentPrices : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Instrument Current Prices";
            //gvInstrumentPrices.Sort("DisplayName", SortDirection.Ascending);
            this.rblDataSourceChoice.SelectedIndex = (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.InstrumentsTradeable;
        }
    }


    protected void rblDataSourceChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.mlvPricesView.ActiveViewIndex = this.rblDataSourceChoice.SelectedIndex;
        //switch (this.rblDataSourceChoice.SelectedIndex)
        //{
        //   case (int)B4F.TotalGiro.ApplicationLayer.DataMaintenance.InstrumentCurrentPricesAdapter.DataSourceChoices.Currencies:
        //       //SetupCurrencies();

        //       break;
        //    default:
        //        break;  
        //}
    }

    //private void SetupCurrencies()
    //{

    //    this.gvInstrumentPrices.DataSourceID = "odsCurrencyRates";
    //    this.gvInstrumentPrices.Columns.Clear();

    //    BoundField bc = new BoundField();
    //    bc.DataField = "Symbol";
    //    bc.HeaderText = "Symbol";
    //    bc.SortExpression = "Symbol";
    //    bc.ItemStyle.Wrap = false;
    //    this.gvInstrumentPrices.Columns.Add(bc);

    //    BoundField bc1 = new BoundField();
    //    bc1.DataField = "CountryOfOrigin_CountryName";
    //    bc1.HeaderText = "Country";
    //    bc1.SortExpression = "CountryOfOrigin_CountryName";
    //    bc1.ItemStyle.Wrap = false;
    //    this.gvInstrumentPrices.Columns.Add(bc1);

    //    BoundField bc2 = new BoundField();
    //    bc2.DataField = "AltSymbol";
    //    bc2.HeaderText = "AltSymbol";
    //    bc2.SortExpression = "AltSymbol";
    //    bc2.ItemStyle.Wrap = false;
    //    this.gvInstrumentPrices.Columns.Add(bc2);

    //    BoundField bc3 = new BoundField();
    //    bc3.DataField = "ExchangeRate_Rate";
    //    bc3.HeaderText = "Rate";
    //    bc3.SortExpression = "ExchangeRate_Rate";
    //    bc3.ItemStyle.Wrap = false;
    //    this.gvInstrumentPrices.Columns.Add(bc3);

    //    BoundField bc4 = new BoundField();
    //    bc4.DataField = "ExchangeRate_RateDate";
    //    bc4.HeaderText = "RateDate";
    //    bc4.SortExpression = "ExchangeRate_RateDate";
    //    bc4.ItemStyle.Wrap = false;
    //    bc4.DataFormatString = "{0:d-MMMM-yyyy}";
    //    this.gvInstrumentPrices.Columns.Add(bc4);


    //    this.gvInstrumentPrices.DataBind();

    //}
}
