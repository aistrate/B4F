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
using System.ComponentModel;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Utils;

public partial class InstrumentFinder : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pageLoadStarted = true;

        if (this.ShowSearchButton)
        {
            Utility.SetDefaultButton(this.Page, txtInstrumentName, btnSearch);
            if (this.IsinEnabled)
                Utility.SetDefaultButton(this.Page, txtIsin, btnSearch);
            if (this.ShowSecCategory)
                Utility.SetDefaultButton(this.Page, ddlSecCategory, btnSearch);
            if (this.ShowActivityFilter)
                Utility.SetDefaultButton(this.Page, ddlActivityFilter, btnSearch);
            if (this.CurrencyNominalEnabled)
                Utility.SetDefaultButton(this.Page, ddlCurrencyNominal, btnSearch);
            if (this.ShowExchange)
                Utility.SetDefaultButton(this.Page, ddlExchange, btnSearch);
        }

        if (!IsPostBack)
        {
            if (ShowExchange)
                pnlExchange.Visible = true;
            if (ShowSecCategory)
                pnlSecCategory.Visible = true;
            if (ShowActivityFilter)
                pnlActivityFilter.Visible = true;

            if (!ShowSearchButton)
                btnSearch.Visible = false;

            txtIsin.Enabled = this.IsinEnabled;
            lblIsin.Enabled = this.IsinEnabled;
            ddlCurrencyNominal.Enabled = this.CurrencyNominalEnabled;
            lblCurrencyNominal.Enabled = this.CurrencyNominalEnabled;

            DataSet dsSC = InstrumentFinderAdapter.GetSecCategories(SecCategoryFilter);
            ddlSecCategory.DataSource = dsSC;
            ddlSecCategory.DataTextField = "Description";
            ddlSecCategory.DataValueField = "Key";
            ddlSecCategory.DataBind();

            displayFields();
        }
        else
        {
            isin = (string)ViewState["isin"];
            instrumentName = (string)ViewState["instrumentName"];
            secCategoryId = (int)ViewState["secCategoryId"];
            exchangeId = (int)ViewState["exchangeId"];
            currencyNominalId = (int)ViewState["currencyNominalId"];
            if (ViewState["activityFilter"] != null)
            activityFilter = (int)ViewState["activityFilter"];
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (ViewState["ShowExchange"] == null)
            ShowExchange = true;
        if (ViewState["ShowSecCategory"] == null)
            ShowSecCategory = true;
        if (ViewState["ShowActivityFilter"] == null)
            ShowActivityFilter = false;
        if (ViewState["ShowSearchButton"] == null)
            ShowSearchButton = true;
        if (ViewState["IsinEnabled"] == null)
            IsinEnabled = true;
        if (ViewState["CurrencyNominalEnabled"] == null)
            CurrencyNominalEnabled = true;
        //if (ViewState["ISINLabel"] == null)
        //    ISINLabel = lblIsin.Text;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    private void displayFields()
    {
        displayIsin();
        displayInstrumentName();
        displaySecCategoryId();
        displayExchangeId();
        displayCurrencyNominalId();
        displayActivityFilter();
    }

    private void displayIsin()
    {
        if (pageLoadStarted)
        {
            txtIsin.Text = Isin;
            ViewState["isin"] = Isin;
        }
    }

    private void displayInstrumentName()
    {
        if (pageLoadStarted)
        {
            txtInstrumentName.Text = InstrumentName;
            ViewState["instrumentName"] = InstrumentName;
        }
    }

    private void displaySecCategoryId()
    {
        if (pageLoadStarted)
        {
            if (ShowSecCategory)
            {
                if (ddlSecCategory.Items == null || ddlSecCategory.Items.Count == 0)
                    ddlSecCategory.DataBind();
                ListItem listItem = ddlSecCategory.Items.FindByValue(secCategoryId.ToString());
                if (listItem != null)
                    ddlSecCategory.SelectedValue = listItem.Value;
                else
                    ddlSecCategory.SelectedValue = int.MinValue.ToString();      // the first (empty) line
            }
            ViewState["secCategoryId"] = secCategoryId;
        }
    }

    private void displayExchangeId()
    {
        if (pageLoadStarted)
        {
            if (ShowExchange)
            {
                if (ddlExchange.Items == null || ddlExchange.Items.Count == 0)
                    ddlExchange.DataBind();
                ListItem listItem = ddlExchange.Items.FindByValue(exchangeId.ToString());
                if (listItem != null)
                    ddlExchange.SelectedValue = listItem.Value;
                else
                    ddlExchange.SelectedValue = int.MinValue.ToString();      // the first (empty) line
            }
            ViewState["exchangeId"] = exchangeId;
        }
    }

    private void displayCurrencyNominalId()
    {
        if (pageLoadStarted)
        {
            if (ddlCurrencyNominal.Items == null || ddlCurrencyNominal.Items.Count == 0)
                ddlCurrencyNominal.DataBind();
            ListItem listItem = ddlCurrencyNominal.Items.FindByValue(currencyNominalId.ToString());
            if (listItem != null)
                ddlCurrencyNominal.SelectedValue = listItem.Value;
            else
                ddlCurrencyNominal.SelectedValue = int.MinValue.ToString();      // the first (empty) line

            ViewState["currencyNominalId"] = currencyNominalId;
        }
    }

    private void displayActivityFilter()
    {
        if (pageLoadStarted)
        {
            if (ShowActivityFilter)
            {
                if (ddlActivityFilter.Items == null || ddlActivityFilter.Items.Count == 0)
                    ddlActivityFilter.DataBind();
                ListItem listItem = ddlActivityFilter.Items.FindByValue(activityFilter.ToString());
                if (listItem != null)
                    ddlActivityFilter.SelectedValue = listItem.Value;
                else
                    ddlActivityFilter.SelectedValue = ((int)ActivityReturnFilter.Active).ToString();
            }
            ViewState["activityFilter"] = activityFilter;
        }
    }

    public bool Enabled
    {
        get { return txtIsin.Enabled; }
        set
        {
            txtIsin.Enabled = value;
            txtInstrumentName.Enabled = value;
            ddlSecCategory.Enabled = value;
            ddlExchange.Enabled = value;
            ddlCurrencyNominal.Enabled = value;
            btnSearch.Enabled = value;
        }
    }

    [DefaultValue(0)]
    [Description("TabIndex on the page containing the InstrumentFinder will be increased by 6.")]
    public short TabIndex
    {
        get
        {
            object i = ViewState["TabIndex"];
            return ((i == null) ? (short)0 : (short)i);
        }
        set
        {
            ViewState["TabIndex"] = value;

            txtIsin.TabIndex = value++;
            txtInstrumentName.TabIndex = value++;
            ddlSecCategory.TabIndex = value++;
            ddlExchange.TabIndex = value++;
            ddlCurrencyNominal.TabIndex = value++;
            btnSearch.TabIndex = value++;
        }
    }

    /// <summary>
    /// Retrieve/set the asset manager key.
    /// </summary>
    [Description("Retrieve/set the SecCategory Filter.")]
    public SecCategoryFilterOptions SecCategoryFilter
    {
        get
        {
            object b = ViewState["SecCategoryFilter"];
            return ((b == null) ? SecCategoryFilterOptions.All : (SecCategoryFilterOptions)b);
        }
        set
        {
            ViewState["SecCategoryFilter"] = value;
        }
    }

    [Browsable(false)]
    public string Isin
    {
        get { return isin; }
        set { isin = value; displayIsin(); }
    }

    [Browsable(false)]
    public string InstrumentName
    {
        get { return instrumentName; }
        set { instrumentName = value; displayInstrumentName(); }
    }

    [Browsable(false)]
    public int SecCategoryId
    {
        get { return secCategoryId; }
        set { secCategoryId = value; displaySecCategoryId(); }
    }

    [Browsable(false)]
    public int ExchangeId
    {
        get { return exchangeId; }
        set { exchangeId = value; displayExchangeId(); }
    }

    [Browsable(false)]
    public int CurrencyNominalId
    {
        get { return currencyNominalId; }
        set { currencyNominalId = value; displayCurrencyNominalId(); }
    }

    [Browsable(true)]
    public ActivityReturnFilter ActivityFilter
    {
        get { return (ActivityReturnFilter)activityFilter; }
        set { activityFilter = (int)value; displayActivityFilter(); }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Exchange should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Exchange should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowExchange
    {
        get { return (bool)ViewState["ShowExchange"]; }
        set { ViewState["ShowExchange"] = value; }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion SecCategory should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion SecCategory should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowSecCategory
    {
        get { return (bool)ViewState["ShowSecCategory"]; }
        set { ViewState["ShowSecCategory"] = value; }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion ActivityFilter should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion ActivityFilter should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowActivityFilter
    {
        get { return (bool)ViewState["ShowActivityFilter"]; }
        set { ViewState["ShowActivityFilter"] = value; }
    }


    /// <summary>
    /// Gets a value indicating whether button Search should be visible.
    /// </summary>
    [Description("Gets a value indicating whether button Search should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowSearchButton
    {
        get { return (bool)ViewState["ShowSearchButton"]; }
        set { ViewState["ShowSearchButton"] = value; }
    }

    /// <summary>
    /// Gets a value indicating whether Isin textbox is Enabled.
    /// </summary>
    [Description("Gets a value indicating whether Isin textbox is Enabled."), DefaultValue(true), Category("Behavior")]
    public bool IsinEnabled
    {
        get { return (bool)ViewState["IsinEnabled"]; }
        set { ViewState["IsinEnabled"] = value; }
    }

    /// <summary>
    /// Gets a value indicating whether CurrencyNominal textbox is Enabled.
    /// </summary>
    [Description("Gets a value indicating whether CurrencyNominal textbox is Enabled."), DefaultValue(true), Category("Behavior")]
    public bool CurrencyNominalEnabled
    {
        get { return (bool)ViewState["CurrencyNominalEnabled"]; }
        set { ViewState["CurrencyNominalEnabled"] = value; }
    }

    /// <summary>
    /// Gets a value for the ISIN Label.
    /// </summary>
    [Description("Gets a value for the ISIN Label."), Category("Behavior")]
    public string ISINLabel
    {
        get { return (string)ViewState["ISINLabel"]; }
        set 
        { 
            ViewState["ISINLabel"] = value;
            if (!string.IsNullOrEmpty(value))
                lblIsin.Text = value;
        }
    }

    /// <summary>
    /// Performs the action of button Search: assigns the values in TextBoxes to their corresponding properties.
    /// </summary>
    public void DoSearch()
    {
        isin = txtIsin.Text.Trim();
        instrumentName = txtInstrumentName.Text.Trim();

        if (ShowSecCategory)
        {
            secCategoryId = int.Parse(ddlSecCategory.SelectedValue);
            secCategoryId = (secCategoryId == int.MinValue ? 0 : secCategoryId);
        }

        if (ShowExchange)
        {
            exchangeId = int.Parse(ddlExchange.SelectedValue);
            exchangeId = (exchangeId == int.MinValue ? 0 : exchangeId);
        }

        if (ShowActivityFilter)
        {
            activityFilter = Utility.GetKeyFromDropDownList(ddlActivityFilter);
        }

        currencyNominalId = int.Parse(ddlCurrencyNominal.SelectedValue);
        currencyNominalId = (currencyNominalId == int.MinValue ? 0 : currencyNominalId);

        displayFields();

        OnSearch();
    }

    protected void OnSearch()
    {
        if (Search != null)
            Search(this, EventArgs.Empty);
    }
    
    public EventHandler Search;
    private bool pageLoadStarted;
    private string isin, instrumentName;
    private int secCategoryId, exchangeId = 0, currencyNominalId;
    private int activityFilter = (int)ActivityReturnFilter.Active;
}
