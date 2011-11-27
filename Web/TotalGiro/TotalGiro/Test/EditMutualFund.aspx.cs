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
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Dal;
using NHibernate.Collection;
using System.Collections.Generic;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.StaticData;

public partial class DataMaintenance_MutualFund : System.Web.UI.Page
{
    //IDalSession session;
    ITradeableInstrument tradInstr;
    ListItem firstLstItem = new ListItem("--Choose--", "-1"); 

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        //session = NHSessionFactory.CreateSession();
        //// Used in ApplicationLayer
        //Session["MutualFundIDalSession"] = session;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ClientScript.IsClientScriptBlockRegistered("clientScript"))
        {
            String scriptString = "<script language=JavaScript> var _oldColor;";
            scriptString += "function SetNewColor(source) { ";
            scriptString += "_oldColor = source.style.backgroundColor;";
            scriptString += "source.style.backgroundColor = '#AAB9C2'; } ";
            scriptString += "function SetOldColor(source) { ";
            scriptString += "source.style.backgroundColor = _oldColor; }";
            scriptString += "</";
            scriptString += "script>";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "clientScript", scriptString);

        }

        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Maintenance Mutual Fund";

            Session["TradeableInstrumentID"] = 33;
            // From page TradeableInstrument
            if (Session["TradeableInstrumentID"] != null)
            {
                pnlExchanges.Visible = true;

                IInstrumentExchange exchange;

                // odsExchanges parameter
                hfInstrumentID.Value = Session["TradeableInstrumentID"].ToString();
                IDalSession session = NHSessionFactory.CreateSession();
                tradInstr = InstrumentMapper.GetTradeableInstrument(session,
                        (int)Session["TradeableInstrumentID"]);
                if (tradInstr != null)
                {
                    this.InitializeInstrumentDetails();

                    exchange = (IInstrumentExchange)tradInstr.DefaultExchange;
                    if (exchange != null && exchange.Key > 0)
                    {
                        pnlExchangeDetails.Visible = true;
                        this.InitializeExchangeDetails(exchange);
                    }
                    else
                    {
                        pnlExchangeDetails.Visible = false;
                    }
                }
                session.Close();
            }
            else
            {
                pnlExchangeDetails.Visible = false;
                pnlExchanges.Visible = false;
            }

        }
    }

    protected void Page_Unload()
    {
        //session.Close();
        Session["MutualFundIDalSession"] = null;
    }

    protected void gvExchanges_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        IList<ICurrency> currencies;
        ListItem lstItem;
        IList countries;
        string strAbbr;

        pnlExchangeDetails.Visible = true;
        Int32 dataKey = (Int32)gvExchanges.DataKeys[e.NewSelectedIndex].Value;

        IDalSession session = NHSessionFactory.CreateSession();
        tradInstr = InstrumentMapper.GetTradeableInstrument(session,
                                (int)Session["TradeableInstrumentID"]);

        if (tradInstr != null)
        {
            IInstrumentExchangeCollection collExchanges = tradInstr.InstrumentExchanges;
            IInstrumentExchange exchange = collExchanges.GetItemByExchange(dataKey);

            tbExchangeName.Text = exchange.Exchange.ExchangeName;
            tbNumberOfDecimals.Text = exchange.NumberOfDecimals.ToString();

            currencies = InstrumentMapper.GetCurrencies(session);
            ddDefaultExchangeCurrency.Items.Clear();
            ddDefaultExchangeCurrency.Items.Add(firstLstItem);

            foreach (IInstrument curr in currencies)
            {
                lstItem = new ListItem(curr.Name.ToString(), curr.Key.ToString());
                if (exchange.Exchange.DefaultCurrency != null &&
                    (exchange.Exchange.DefaultCurrency.Key == curr.Key))
                {
                    lstItem.Selected = true;
                }
                ddDefaultExchangeCurrency.Items.Add(lstItem);
            }
            ddDefaultExchangeCurrency.DataBind();

            countries = CountryMapper.GetCountries(session);
            ddDefaultExchangeCountry.Items.Clear();
            ddDefaultExchangeCountry.Items.Add(firstLstItem);
            foreach (ICountry country in countries)
            {
                if (country.CountryName.ToString().Length > 18)
                {
                    strAbbr = country.CountryName.ToString().Substring(0, 18);
                }
                else
                {
                    strAbbr = country.CountryName.ToString();
                }
                lstItem = new ListItem(strAbbr, country.Key.ToString());
                if (exchange.Exchange.DefaultCurrency != null &&
                    ((ICountry)exchange.Exchange.DefaultCountry).Key == country.Key)
                {
                    lstItem.Selected = true;
                }
                ddDefaultExchangeCountry.Items.Add(lstItem);
            }
            ddDefaultExchangeCountry.DataBind();
        }
        session.Close();
    }

    protected void gvExchanges_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        const int LINK_BUTTON_CONTROL = 0;
        const int LINK_BUTTON_COLUMN = 0;
        LinkButton linkBtn = null;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onMouseOver", "SetNewColor(this); this.style.cursor='hand'");
            e.Row.Attributes.Add("onMouseOut", "SetOldColor(this); this.style.cursor='text'");
            linkBtn = (LinkButton)e.Row.Cells[LINK_BUTTON_COLUMN].Controls[LINK_BUTTON_CONTROL];
            e.Row.Attributes.Add("onDblClick", ClientScript.GetPostBackClientHyperlink(linkBtn, ""));
        }
    }

    private void InitializeExchangeDetails(IInstrumentExchange exchange)
    {
        IList<ICurrency> currencies;
        ListItem lstItem;
        IList countries;
        string strAbbr;

        tbExchangeName.Text = exchange.Exchange.ExchangeName;
        tbNumberOfDecimals.Text = exchange.NumberOfDecimals.ToString();

        IDalSession session = NHSessionFactory.CreateSession();
        currencies = InstrumentMapper.GetCurrencies(session);
        lstItem = new ListItem("--Choose--", "-1");
        ddDefaultExchangeCurrency.Items.Add(lstItem);

        foreach (IInstrument curr in currencies)
        {
            lstItem = new ListItem(curr.Name.ToString(), curr.Key.ToString());
            if (exchange.Exchange.DefaultCurrency != null &&
                (exchange.Exchange.DefaultCurrency.Key == curr.Key))
            {
                lstItem.Selected = true;
            }
            ddDefaultExchangeCurrency.Items.Add(lstItem);
        }
        ddDefaultExchangeCurrency.DataBind();

        countries = CountryMapper.GetCountries(session);
        lstItem = new ListItem("--Choose--", "-1");
        ddDefaultExchangeCountry.Items.Add(lstItem);
        foreach (ICountry country in countries)
        {
            if (country.CountryName.ToString().Length > 18)
            {
                strAbbr = country.CountryName.ToString().Substring(0, 18);
            }
            else
            {
                strAbbr = country.CountryName.ToString();
            }
            lstItem = new ListItem(strAbbr, country.Key.ToString());
            if (exchange.Exchange.DefaultCurrency != null &&
                ((ICountry)exchange.Exchange.DefaultCountry).Key == country.Key)
            {
                lstItem.Selected = true;
            }
            ddDefaultExchangeCountry.Items.Add(lstItem);
        }
        ddDefaultExchangeCountry.DataBind();
        session.Close();
    }

    private void InitializeInstrumentDetails()
    {
        ListItem lstItem;
        IList<ICurrency> currencies;
        tbName.Text = tradInstr.Name;
        tbCompanyName.Text = tradInstr.CompanyName;
        cbIsCash.Checked = tradInstr.IsTradeable;

        IDalSession session = NHSessionFactory.CreateSession();
        currencies = InstrumentMapper.GetCurrencies(session);
        ddCurrencyNominal.Items.Add(firstLstItem);
        foreach (IInstrument curr in currencies)
        {
            lstItem = new ListItem(curr.Name.ToString(), curr.Key.ToString());
            if (tradInstr.CurrencyNominal != null &&
                ((IInstrument)tradInstr.CurrencyNominal).Key == curr.Key)
            {
                lstItem.Selected = true;
            }
            ddCurrencyNominal.Items.Add(lstItem);
        }
        ddCurrencyNominal.DataBind();

        cbIsTradeable.Checked = tradInstr.IsTradeable;
        tbIsin.Text = tradInstr.Isin;

        ddDefaultExchange.Items.Add(firstLstItem);
        if (tradInstr != null)
        {
            IInstrumentExchangeCollection collExchanges = tradInstr.InstrumentExchanges;
            foreach (IInstrumentExchange exchange in collExchanges)
            {
                lstItem = new ListItem(exchange.Instrument.Name.ToString(), exchange.Key.ToString());
                if (tradInstr.DefaultExchange != null &&
                    ((IInstrumentExchange)tradInstr.DefaultExchange).Key == exchange.Key)
                {
                    lstItem.Selected = true;
                }
                ddDefaultExchange.Items.Add(lstItem);
            }
            ddDefaultExchange.DataBind();
        }
        session.Close();
    }
}
