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
using B4F.TotalGiro.ApplicationLayer.UC;
using System.ComponentModel;

public partial class AccountFinder : System.Web.UI.UserControl
{
    // NOTE: pages that use this control and need to assign its properties (e.g. AssetManagerId, AccountNumber, AccountName)
    // should do it AFTER the page is loaded, otherwise the assignments may not take effect (call DataBind() first, if necessary)
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ShowSearchButton)
        {
            Utility.SetDefaultButton(this.Page, ddlAssetManager, btnSearch);
            if (this.ShowAccountName)
                Utility.SetDefaultButton(this.Page, txtAccountName, btnSearch);
            if (this.ShowAccountNumber)
                Utility.SetDefaultButton(this.Page, txtAccountNumber, btnSearch);
            if (this.ShowRemisier)
                Utility.SetDefaultButton(this.Page, ddlRemisier, btnSearch);
            if (this.ShowRemisierEmployee)
                Utility.SetDefaultButton(this.Page, ddlRemisierEmployee, btnSearch);
            if (this.ShowLifecycle)
                Utility.SetDefaultButton(this.Page, ddlLifecycle, btnSearch);
            if (this.ShowModelPortfolio)
                Utility.SetDefaultButton(this.Page, ddlModelPortfolio, btnSearch);
            if (this.ShowYear)
                Utility.SetDefaultButton(this.Page, ddlYear, btnSearch);
            if (this.ShowTegenrekening)
                Utility.SetDefaultButton(this.Page, txtTegenrekening, btnSearch);
            if (this.ShowContactActiveCbl)
                Utility.SetDefaultButton(this.Page, ddlContactActive, btnSearch);
            if (this.ShowAccountTradeabilityDdl)
                Utility.SetDefaultButton(this.Page, ddlTradeability, btnSearch);
        }

        hasChanged = false;
        if (!IsDataBound)
            DataBind();
    }

    protected bool IsDataBound
    {
        get
        {
            object b = ViewState["IsDataBound"];
            return ((b == null) ? false : (bool)b);
        }
        set { ViewState["IsDataBound"] = value; }
    }

    public override void DataBind()
    {
        if (!IsLoggedInAsAssetManager)
        {
            mvwAssetManager.ActiveViewIndex = 1;
            ddlAssetManager.DataBind();

        }
        else
        {
            mvwAssetManager.ActiveViewIndex = 0;
            lblAssetManager.Text = AccountFinderAdapter.GetCurrentManagmentCompanyName();
        }

        if (ShowRemisier)
            ddlRemisier.DataBind();

        if (ShowRemisierEmployee)
            ddlRemisierEmployee.DataBind();

        if (ShowLifecycle)
            ddlLifecycle.DataBind();

        if (ShowModelPortfolio)
            ddlModelPortfolio.DataBind();

        if (ShowYear)
            ddlYearDataBind();

        if (ShowAccountTradeabilityDdl)
        {
            ddlTradeability.DataBind();
            ddlTradeability.SelectedValue = ((int)AccountFinderAdapter.AccountGuiTradeability.Tradeable).ToString();
        }

        IsDataBound = true;

        base.DataBind();
    }

    private void ddlYearDataBind()
    {
        if (ddlYear.Items.Count == 0)
        {
            ArrayList years = new ArrayList();
            for (int year = 2005; year <= DateTime.Today.Year; year++)
                years.Add(year);

            ddlYear.DataSource = years;
            ddlYear.DataBind();

            ddlYear.SelectedIndex = ddlYear.Items.Count - 1;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected void ddlContactActive_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlLifecycle.SelectedIndex != -1)
        {
            string oldLifecycleId = ddlLifecycle.SelectedValue;
            ddlLifecycle.SelectedIndex = -1;
            ddlLifecycle.DataBind();
            if (ddlLifecycle.Items.FindByValue(oldLifecycleId) != null)
                ddlLifecycle.SelectedValue = oldLifecycleId;
        }
        if (ddlModelPortfolio.SelectedIndex != -1)
        {
            string oldModelId = ddlModelPortfolio.SelectedValue;
            ddlModelPortfolio.SelectedIndex = -1;
            ddlModelPortfolio.DataBind();
            if (ddlModelPortfolio.Items.FindByValue(oldModelId) != null)
                ddlModelPortfolio.SelectedValue = oldModelId;
        }
    }

    public bool IsLoggedInAsAssetManager
    {
        get
        {
            object b = ViewState["IsLoggedInAsAssetManager"];
            return ((b == null) ? AccountFinderAdapter.IsLoggedInAsAssetManager() : (bool)b);
        }
    }

    public void Reset()
    {
        DataBind();
        AccountNumber = string.Empty;
        AccountName = string.Empty;
        Tegenrekening = string.Empty;
        BsN_KvK = string.Empty;
        ddlContactActive.SelectedValue = ((int)AccountFinderAdapter.AccountGuiStatus.Active).ToString();
        ddlYear.SelectedIndex = ddlYear.Items.Count - 1;
        ddlTradeability.SelectedValue = ((int)AccountFinderAdapter.AccountGuiTradeability.Tradeable).ToString();
    }

    public bool Enabled
    {
        get { return ddlModelPortfolio.Enabled; }
        set
        {
            ddlAssetManager.Enabled = value;
            ddlRemisier.Enabled = value;
            ddlRemisierEmployee.Enabled = value;
            ddlLifecycle.Enabled = value;
            ddlModelPortfolio.Enabled = value;
            txtAccountNumber.Enabled = value;
            txtAccountName.Enabled = value;
            txtBsN_KvK.Enabled = value;
            txtTegenrekening.Enabled = value;
            ddlContactActive.Enabled = value;
            ddlYear.Enabled = value;
            btnSearch.Enabled = value;
            ddlTradeability.Enabled = value;
        }
    }

    [DefaultValue(0)]
    [Description("TabIndex on the page containing the AccountFinder will be increased by 8.")]
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

            ddlAssetManager.TabIndex = value++;
            ddlRemisier.TabIndex = value++;
            ddlRemisierEmployee.TabIndex = value++;
            ddlLifecycle.TabIndex = value++;
            ddlModelPortfolio.TabIndex = value++;
            txtAccountNumber.TabIndex = value++;
            txtAccountName.TabIndex = value++;
            txtTegenrekening.TabIndex = value++;
            txtBsN_KvK.TabIndex = value++;
            ddlContactActive.TabIndex = value++;
            ddlTradeability.TabIndex = value++;
            ddlYear.TabIndex = value++;
            btnSearch.TabIndex = value++;
        }
    }

    [Browsable(false)]
    public bool HasChanged
    {
        get { return hasChanged; }
    }

    [Browsable(false)]
    public int AssetManagerId
    {
        get
        {
            object i = ViewState["AssetManagerId"];
            return ((i == null) ? (IsLoggedInAsAssetManager ? AccountFinderAdapter.GetCurrentManagmentCompanyId() : 0) : (int)i);
        }
        set
        {
            if (!IsLoggedInAsAssetManager)
            {
                int oldAssetManagerId = AssetManagerId;
                ViewState["AssetManagerId"] = (value != int.MinValue ? value : 0);
                if (AssetManagerId != oldAssetManagerId)
                    hasChanged = true;

                ListItem listItem = ddlAssetManager.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlAssetManager.SelectedValue = listItem.Value;
                else if (ddlAssetManager.Items.Count > 0)
                    ddlAssetManager.SelectedIndex = 0;
            }
        }
    }

    [Browsable(false)]
    public int RemisierId
    {
        get
        {
            object i = ViewState["RemisierId"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            int oldRemisierId = RemisierId;
            ViewState["RemisierId"] = (value != int.MinValue ? value : 0);
            if (RemisierId != oldRemisierId)
                hasChanged = true;

            if (ShowRemisier)
            {
                ListItem listItem = ddlRemisier.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlRemisier.SelectedValue = listItem.Value;
                else if (ddlRemisier.Items.Count > 0)
                    ddlRemisier.SelectedIndex = 0;
            }
        }
    }

    [Browsable(false)]
    public int RemisierEmployeeId
    {
        get
        {
            object i = ViewState["RemisierEmployeeId"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            int oldRemisierEmployeeId = RemisierEmployeeId;
            ViewState["RemisierEmployeeId"] = (value != int.MinValue ? value : 0);
            if (RemisierEmployeeId != oldRemisierEmployeeId)
                hasChanged = true;

            if (ShowRemisierEmployee)
            {
                ListItem listItem = ddlRemisierEmployee.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlRemisierEmployee.SelectedValue = listItem.Value;
                else if (ddlRemisierEmployee.Items.Count > 0)
                    ddlRemisierEmployee.SelectedIndex = 0;
            }
        }
    }

    [Browsable(false)]
    public int LifecycleId
    {
        get
        {
            object i = ViewState["LifecycleId"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            int oldLifecycleId = LifecycleId;
            ViewState["LifecycleId"] = (value != int.MinValue ? value : 0);
            if (LifecycleId != oldLifecycleId)
                hasChanged = true;

            if (ShowLifecycle)
            {
                ListItem listItem = ddlLifecycle.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlLifecycle.SelectedValue = listItem.Value;
                else if (ddlLifecycle.Items.Count > 0)
                    ddlLifecycle.SelectedIndex = 0;
            }
        }
    }

    [Browsable(false)]
    public int ModelPortfolioId
    {
        get
        {
            object i = ViewState["ModelPortfolioId"];
            return ((i == null) ? 0 : (int)i);
        }
        set
        {
            int oldModelPortfolioId = ModelPortfolioId;
            ViewState["ModelPortfolioId"] = (value != int.MinValue ? value : 0);
            if (ModelPortfolioId != oldModelPortfolioId)
                hasChanged = true;

            if (ShowModelPortfolio)
            {
                ListItem listItem = ddlModelPortfolio.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlModelPortfolio.SelectedValue = listItem.Value;
                else if (ddlModelPortfolio.Items.Count > 0)
                    ddlModelPortfolio.SelectedIndex = 0;
            }
        }
    }

    [Browsable(false)]
    public string AccountNumber
    {
        get
        {
            object s = ViewState["AccountNumber"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            string oldAccountNumber = AccountNumber;
            ViewState["AccountNumber"] = value;
            if (AccountNumber != oldAccountNumber)
                hasChanged = true;

            if (ShowAccountNumber)
                txtAccountNumber.Text = value;
        }
    }

    [Browsable(false)]
    public string AccountName
    {
        get
        {
            object s = ViewState["AccountName"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            string oldAccountName = AccountName;
            ViewState["AccountName"] = value;
            if (AccountName != oldAccountName)
                hasChanged = true;

            if (ShowAccountName)
                txtAccountName.Text = value;
        }
    }

    [Browsable(false)]
    public string BsN_KvK
    {
        get
        {
            object s = ViewState["BsN_KvK"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            ViewState["BsN_KvK"] = value;
            if (ShowBsN_KvK)
                txtBsN_KvK.Text = value;
        }
    }


    [Browsable(false)]
    public string Tegenrekening
    {
        get
        {
            object s = ViewState["Tegenrekening"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            string oldTegenrekening = Tegenrekening;
            ViewState["Tegenrekening"] = value;
            if (Tegenrekening != oldTegenrekening)
                hasChanged = true;

            if (ShowTegenrekening)
                txtTegenrekening.Text = value;
        }
    }

    [Browsable(false)]
    public bool ContactActive
    {
        get
        {
            object b = ViewState["ContactActive"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            bool oldContactActive = ContactActive;
            ViewState["ContactActive"] = value;
            if (ContactActive != oldContactActive)
                hasChanged = true;

            if (ShowContactActiveCbl && value && (ddlContactActive.SelectedValue != ((int)AccountFinderAdapter.AccountGuiStatus.All).ToString()))
                ddlContactActive.SelectedValue = ((int)AccountFinderAdapter.AccountGuiStatus.Active).ToString();

        }
    }

    [Browsable(false)]
    public bool ContactInactive
    {
        get
        {
            object b = ViewState["ContactInactive"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            bool oldContactInactive = ContactInactive;
            ViewState["ContactInactive"] = value;
            if (ContactInactive != oldContactInactive)
                hasChanged = true;

            if (ShowContactActiveCbl && value && (ddlContactActive.SelectedValue != ((int)AccountFinderAdapter.AccountGuiStatus.All).ToString()))
                ddlContactActive.SelectedValue = ((int)AccountFinderAdapter.AccountGuiStatus.Inactive).ToString();

        }
    }

    [Browsable(false)]
    public bool ContactActiveAll
    {
        get
        {
            object b = ViewState["ContactActiveAll"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            bool oldContactActiveAll = ContactActiveAll;
            ViewState["ContactActiveAll"] = value;
            if (ContactActiveAll != oldContactActiveAll)
                hasChanged = true;

            if (ShowContactActiveCbl && value)
                ddlContactActive.SelectedValue = ((int)AccountFinderAdapter.AccountGuiStatus.All).ToString();
        }
    }

    [Browsable(false)]
    public bool AccountTradeable
    {
        get
        {
            object b = ViewState["AccountTradeable"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            bool oldAccountTradeable = AccountTradeable;
            ViewState["AccountTradeable"] = value;
            if (AccountTradeable != oldAccountTradeable)
                hasChanged = true;

            if (ShowAccountTradeabilityDdl && value && (ddlTradeability.SelectedValue != ((int)AccountFinderAdapter.AccountGuiTradeability.All).ToString()))
                ddlTradeability.SelectedValue = ((int)AccountFinderAdapter.AccountGuiTradeability.Tradeable).ToString();

        }
    }

    [Browsable(false)]
    public bool AccountNonTradeable
    {
        get
        {
            object b = ViewState["AccountNonTradeable"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            bool oldAccountNonTradeable = AccountNonTradeable;
            ViewState["AccountNonTradeable"] = value;
            if (AccountNonTradeable != oldAccountNonTradeable)
                hasChanged = true;

            if (ShowAccountTradeabilityDdl && value && (ddlTradeability.SelectedValue != ((int)AccountFinderAdapter.AccountGuiTradeability.All).ToString()))
                ddlTradeability.SelectedValue = ((int)AccountFinderAdapter.AccountGuiTradeability.NonTradeable).ToString();
        }
    }

    [Browsable(false)]
    public bool AccountTradeableAll
    {
        get
        {
            object b = ViewState["AccountTradeableAll"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            bool oldAccountTradeableAll = AccountTradeableAll;
            ViewState["AccountTradeableAll"] = value;
            if (AccountTradeableAll != oldAccountTradeableAll)
                hasChanged = true;

            if (ShowAccountTradeabilityDdl && value)
                ddlTradeability.SelectedValue = ((int)AccountFinderAdapter.AccountGuiTradeability.All).ToString();
        }
    }

    [Browsable(false)]
    public int Year
    {
        get
        {
            object i = ViewState["Year"];
            return ((i == null) ? DateTime.Today.Year : (int)i);
        }
        set
        {
            int oldYear = Year;
            ViewState["Year"] = value;
            if (Year != oldYear)
                hasChanged = true;

            if (ShowYear)
            {
                ListItem listItem = ddlYear.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlYear.SelectedValue = listItem.Value;
                else if (ddlYear.Items.Count > 0)
                    ddlYear.SelectedIndex = ddlYear.Items.Count - 1;
            }
        }
    }

    public string AccountNameLabel
    {
        get { return lblAccountName.Text; }
        set { lblAccountName.Text = value; }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Remisier should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Remisier should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowRemisier
    {
        get
        {
            object b = ViewState["ShowRemisier"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowRemisier"] = value;
            pnlRemisier.Visible = value;
            ddlAssetManager.AutoPostBack = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Remisier Employee should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Remisier Employee should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowRemisierEmployee
    {
        get
        {
            object b = ViewState["ShowRemisierEmployee"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowRemisierEmployee"] = value;
            pnlRemisierEmployee.Visible = value;
            ddlRemisier.AutoPostBack = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Lifecycle should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Lifecycle should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowLifecycle
    {
        get
        {
            object b = ViewState["ShowLifecycle"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowLifecycle"] = value;
            pnlLifecycle.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Model Portofolio should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Model Portofolio should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowModelPortfolio
    {
        get
        {
            object b = ViewState["ShowModelPortfolio"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowModelPortfolio"] = value;
            pnlModelPortfolio.Visible = value;
            ddlAssetManager.AutoPostBack = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion AccountNumber should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion AccountNumber should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowAccountNumber
    {
        get
        {
            object b = ViewState["ShowAccountNumber"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ShowAccountNumber"] = value;
            pnlAccountNumber.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion AccountName should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion AccountName should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowAccountName
    {
        get
        {
            object b = ViewState["ShowAccountName"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ShowAccountName"] = value;
            pnlAccountName.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Tegenrekening should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Tegenrekening should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowBsN_KvK
    {
        get
        {
            object b = ViewState["ShowBsN_KvK"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowBsN_KvK"] = value;
            pnlBsN_KvK.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Tegenrekening should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Tegenrekening should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowTegenrekening
    {
        get
        {
            object b = ViewState["ShowTegenrekening"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowTegenrekening"] = value;
            pnlTegenrekening.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion CblContactActive should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion CblContactActive should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowContactActiveCbl
    {
        get
        {
            object b = ViewState["ShowContactActiveCbl"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowContactActiveCbl"] = value;
            pnlCblContactActive.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion CblContactActive should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion ddlTradeability should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowAccountTradeabilityDdl
    {
        get
        {
            object b = ViewState["ShowAccountTradeabilityDdl"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowAccountTradeabilityDdl"] = value;
            pnlddlTradeability.Visible = value;
        }
    }

    [Description("Gets a value indicating whether search criterion Year should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowYear
    {
        get
        {
            object b = ViewState["ShowYear"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowYear"] = value;
            pnlYear.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether button Search should be visible.
    /// </summary>
    [Description("Gets a value indicating whether button Search should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowSearchButton
    {
        get
        {
            object b = ViewState["ShowSearchButton"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ShowSearchButton"] = value;
            btnSearch.Visible = value;
        }
    }

    /// <summary>
    /// Performs the action of button Search: assigns the values in TextBoxes to their corresponding properties.
    /// </summary>
    public void DoSearch()
    {
        if (!IsLoggedInAsAssetManager)
        {
            if (!string.IsNullOrEmpty(ddlAssetManager.SelectedValue))
                AssetManagerId = int.Parse(ddlAssetManager.SelectedValue);
        }

        if (ShowRemisier)
            RemisierId = int.Parse(ddlRemisier.SelectedValue);

        if (ShowRemisierEmployee)
        {
            if (ddlRemisierEmployee.Items.Count > 0)
                RemisierEmployeeId = int.Parse(ddlRemisierEmployee.SelectedValue);
            else
                RemisierEmployeeId = int.MinValue;
        }

        if (ShowLifecycle)
            LifecycleId = Utility.GetKeyFromDropDownList(ddlLifecycle);

        if (ShowModelPortfolio)
            ModelPortfolioId = Utility.GetKeyFromDropDownList(ddlModelPortfolio);

        if (ShowAccountName)
            AccountName = txtAccountName.Text.Trim();

        if (ShowAccountNumber)
            AccountNumber = txtAccountNumber.Text.Trim();

        if (ShowBsN_KvK)
            BsN_KvK = txtBsN_KvK.Text.Trim();

        if (ShowTegenrekening)
            Tegenrekening = txtTegenrekening.Text.Trim();

        if (ShowContactActiveCbl)
        {
            ContactActive = ((ddlContactActive.SelectedValue == ((int)AccountFinderAdapter.AccountGuiStatus.Active).ToString()) ||
                (ddlContactActive.SelectedValue == ((int)AccountFinderAdapter.AccountGuiStatus.All).ToString()));
            ContactInactive = ((ddlContactActive.SelectedValue == ((int)AccountFinderAdapter.AccountGuiStatus.Inactive).ToString()) ||
                (ddlContactActive.SelectedValue == ((int)AccountFinderAdapter.AccountGuiStatus.All).ToString()));
            ContactActiveAll = (ddlContactActive.SelectedValue == ((int)AccountFinderAdapter.AccountGuiStatus.All).ToString());
        }

        if (ShowAccountTradeabilityDdl)
        {
            AccountTradeable = ((ddlTradeability.SelectedValue == ((int)AccountFinderAdapter.AccountGuiTradeability.Tradeable).ToString()) ||
                (ddlTradeability.SelectedValue == ((int)AccountFinderAdapter.AccountGuiTradeability.All).ToString()));
            AccountNonTradeable = ((ddlTradeability.SelectedValue == ((int)AccountFinderAdapter.AccountGuiTradeability.NonTradeable).ToString()) ||
                (ddlTradeability.SelectedValue == ((int)AccountFinderAdapter.AccountGuiTradeability.All).ToString()));
            AccountTradeableAll = (ddlTradeability.SelectedValue == ((int)AccountFinderAdapter.AccountGuiTradeability.All).ToString());
        }

        if (ShowYear)
            Year = int.Parse(ddlYear.SelectedValue);

        OnSearch();
    }

    protected void OnSearch()
    {
        if (Search != null)
            Search(this, EventArgs.Empty);
    }

    [Description("Event triggered when button Search is clicked."), Category("Behavior")]
    public EventHandler Search;

    private bool hasChanged = false;
}
