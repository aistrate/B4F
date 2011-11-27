using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.ClientApplicationLayer.UC;
using B4F.TotalGiro.Stichting.Login;

public partial class AccountFinder : System.Web.UI.UserControl
{
    // NOTE: pages that use this control and need to assign its properties (e.g. AssetManagerId, AccountNumber, AccountName)
    // should do it AFTER the page is loaded, otherwise the assignments may not take effect (call DataBind() first, if necessary)
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CurrentLoginType == LoginTypes.Customer)
            throw new SecurityLayerException();

        WebControl[] controls = new WebControl[] {
            ddlAssetManager, ddlRemisier, ddlRemisierEmployee, ddlModelPortfolio, txtAccountNumber, txtAccountName, 
            cbAccountStatusActive, cbAccountStatusInactive, cbEmailStatusYes, cbEmailStatusNo, ddlLoginStatus };

        if (!IsDataBound)
            DataBind();
        
        if (ShowSearchButton)
            foreach (WebControl control in controls)
                Utility.SetDefaultButton(Page, control, btnSearch);

        if (!IsPostBack)
            controls.First(c => c.Visible && c.Enabled).Focus();
    }

    protected void ddlAssetManager_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ShowRemisier && RemisierDisplayType == DisplayType.SelectionList)
            ddlRemisier.DataBind();
        if (ShowRemisierEmployee && RemisierEmployeeDisplayType == DisplayType.SelectionList)
            ddlRemisierEmployee.DataBind();
        if (ShowModelPortfolio)
            ddlModelPortfolio.DataBind();

        ddlAssetManager.Focus();
    }

    protected void ddlRemisier_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ShowRemisierEmployee && RemisierEmployeeDisplayType == DisplayType.SelectionList)
            ddlRemisierEmployee.DataBind();

        ddlRemisier.Focus();
    }

    protected bool IsDataBound
    {
        get { return viewStateBool("IsDataBound", false); }
        set { ViewState["IsDataBound"] = value; }
    }

    public override void DataBind()
    {
        if (AssetManagerDisplayType == DisplayType.SelectionList)
        {
            mvwAssetManager.ActiveViewIndex = 1;
            ddlAssetManager.DataBind();
        }
        else
        {
            mvwAssetManager.ActiveViewIndex = 0;
            lblAssetManagerName.Text = AccountFinderAdapter.GetCurrentAssetManagerInfo().Value;
        }

        if (ShowRemisier)
        {
            if (RemisierDisplayType == DisplayType.SelectionList)
            {
                mvwRemisier.ActiveViewIndex = 1;
                ddlRemisier.DataBind();
            }
            else
            {
                mvwRemisier.ActiveViewIndex = 0;
                lblRemisierName.Text = AccountFinderAdapter.GetCurrentRemisierInfo().Value;
            }
        }

        if (ShowRemisierEmployee)
        {
            if (RemisierEmployeeDisplayType == DisplayType.SelectionList)
            {
                mvwRemisierEmployee.ActiveViewIndex = 1;
                ddlRemisierEmployee.DataBind();
                if (CurrentLoginType == LoginTypes.RemisierEmployee && ViewState["RemisierEmployeeId"] == null)
                    RemisierEmployeeId = AccountFinderAdapter.GetCurrentRemisierEmployeeInfo().Key;
            }
            else
            {
                mvwRemisierEmployee.ActiveViewIndex = 0;
                lblRemisierEmployeeName.Text = AccountFinderAdapter.GetCurrentRemisierEmployeeInfo().Value;
            }
        }

        if (ShowModelPortfolio)
            ddlModelPortfolio.DataBind();

        IsDataBound = true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
        btnSearch.Focus();
    }

    protected enum DisplayType
    {
        SelectionList,
        Label
    }

    protected DisplayType AssetManagerDisplayType
    {
        get { return CurrentLoginTypeIsOneOf(LoginTypes.StichtingEmployee, LoginTypes.ComplianceEmployee) ?
                            DisplayType.SelectionList :
                            DisplayType.Label;
        }
    }

    protected DisplayType RemisierDisplayType
    {
        get { return CurrentLoginTypeIsOneOf(LoginTypes.StichtingEmployee, LoginTypes.ComplianceEmployee, LoginTypes.AssetManagerEmployee) ?
                            DisplayType.SelectionList :
                            DisplayType.Label;
        }
    }

    protected DisplayType RemisierEmployeeDisplayType
    {
        get { return CurrentLoginType != LoginTypes.Customer ?
                            DisplayType.SelectionList :
                            DisplayType.Label;
        }
    }

    protected bool CurrentLoginTypeIsOneOf(params LoginTypes[] loginTypes)
    {
        if (loginTypes != null && loginTypes.Length > 0)
            return loginTypes.Any(loginType => (loginType & CurrentLoginType) == loginType);
        else
            return false;
    }

    protected LoginTypes CurrentLoginType
    {
        get
        {
            if (currentLoginType == null)
                currentLoginType = CommonAdapter.GetCurrentLoginType();
            return (LoginTypes)currentLoginType;
        }
    }
    private LoginTypes? currentLoginType = null;

    public AccountFinderCriteria GetCriteria()
    {
        AccountFinderCriteria criteria = new AccountFinderCriteria();

        criteria.AssetManagerId = AssetManagerId;
        criteria.RemisierId = RemisierId;
        criteria.RemisierEmployeeId = RemisierEmployeeId;
        criteria.ModelPortfolioId = ModelPortfolioId;
        criteria.AccountNumber = AccountNumber;
        criteria.AccountName = AccountName;
        criteria.AccountStatusActive = AccountStatusActive;
        criteria.AccountStatusInactive = AccountStatusInactive;
        criteria.EmailStatusYes = EmailStatusYes;
        criteria.EmailStatusNo = EmailStatusNo;
        criteria.LoginStatus = LoginStatus;

        return criteria;
    }

    public void SetCriteria(AccountFinderCriteria criteria)
    {
        if (AssetManagerDisplayType == DisplayType.SelectionList)
            ddlAssetManager.DataBind();
        AssetManagerId = criteria.AssetManagerId;

        if (ShowRemisier && RemisierDisplayType == DisplayType.SelectionList)
            ddlRemisier.DataBind();
        RemisierId = criteria.RemisierId;
        
        if (ShowRemisierEmployee && RemisierEmployeeDisplayType == DisplayType.SelectionList)
            ddlRemisierEmployee.DataBind();
        RemisierEmployeeId = criteria.RemisierEmployeeId;

        if (ShowModelPortfolio)
            ddlModelPortfolio.DataBind();
        ModelPortfolioId = criteria.ModelPortfolioId;
        
        AccountNumber = criteria.AccountNumber;
        AccountName = criteria.AccountName;
        AccountStatusActive = criteria.AccountStatusActive;
        AccountStatusInactive = criteria.AccountStatusInactive;
        EmailStatusYes = criteria.EmailStatusYes;
        EmailStatusNo = criteria.EmailStatusNo;
        LoginStatus = criteria.LoginStatus;
    }

    public void Reset()
    {
        DataBind();
        AccountNumber = string.Empty;
        AccountName = string.Empty;
        AccountStatusActive = true;
        AccountStatusInactive = false;
        EmailStatusYes = true;
        EmailStatusNo = true;
        ddlLoginStatus.SelectedIndex = 0;
    }

    public bool Enabled
    {
        get { return ddlModelPortfolio.Enabled; }
        set
        {
            ddlAssetManager.Enabled = value;
            ddlModelPortfolio.Enabled = value;
            txtAccountNumber.Enabled = value;
            txtAccountName.Enabled = value;
            cbAccountStatusActive.Enabled = value;
            cbAccountStatusInactive.Enabled = value;
            cbEmailStatusYes.Enabled = value;
            cbEmailStatusNo.Enabled = value;
            ddlLoginStatus.Enabled = value;
            btnSearch.Enabled = value;
        }
    }

    private int viewStateInt(string key)
    {
        object i = ViewState[key];
        return i == null ? 0 : (int)i;
    }

    private bool viewStateBool(string key, bool defaultValue)
    {
        object b = ViewState[key];
        return ((b == null) ? defaultValue : (bool)b);
    }

    private string viewStateString(string key)
    {
        return viewStateString(key, string.Empty);
    }

    private string viewStateString(string key, string defaultValue)
    {
        object s = ViewState[key];
        return ((s == null) ? defaultValue : (string)s);
    }

    private void selectValueOrFirst(DropDownList dropDownList, int value)
    {
        ListItem listItem = dropDownList.Items.FindByValue(value.ToString());
        if (listItem != null)
            dropDownList.SelectedValue = listItem.Value;
        else if (dropDownList.Items.Count > 0)
            dropDownList.SelectedIndex = 0;
    }

    [Browsable(false)]
    public int AssetManagerId
    {
        get
        {
            if (AssetManagerDisplayType == DisplayType.SelectionList)
                return viewStateInt("AssetManagerId");
            else
                return AccountFinderAdapter.GetCurrentAssetManagerInfo().Key;
        }
        set
        {
            if (AssetManagerDisplayType == DisplayType.SelectionList)
            {
                ViewState["AssetManagerId"] = (value != int.MinValue ? value : 0);
                selectValueOrFirst(ddlAssetManager, value);
            }
        }
    }

    [Browsable(false)]
    public int RemisierId
    {
        get
        {
            if (RemisierDisplayType == DisplayType.SelectionList)
                return viewStateInt("RemisierId");
            else
                return AccountFinderAdapter.GetCurrentRemisierInfo().Key;
        }
        set
        {
            if (RemisierDisplayType == DisplayType.SelectionList)
            {
                ViewState["RemisierId"] = (value != int.MinValue ? value : 0);
                if (ShowRemisier)
                    selectValueOrFirst(ddlRemisier, value);
            }
        }
    }

    [Browsable(false)]
    public int RemisierEmployeeId
    {
        get
        {
            if (RemisierEmployeeDisplayType == DisplayType.SelectionList)
                return viewStateInt("RemisierEmployeeId");
            else
                return AccountFinderAdapter.GetCurrentRemisierEmployeeInfo().Key;
        }
        set
        {
            if (RemisierEmployeeDisplayType == DisplayType.SelectionList)
            {
                ViewState["RemisierEmployeeId"] = (value != int.MinValue ? value : 0);
                if (ShowRemisierEmployee)
                    selectValueOrFirst(ddlRemisierEmployee, value);
            }
        }
    }

    [Browsable(false)]
    public int ModelPortfolioId
    {
        get { return viewStateInt("ModelPortfolioId"); }
        set
        {
            ViewState["ModelPortfolioId"] = (value != int.MinValue ? value : 0);
            if (ShowModelPortfolio)
                selectValueOrFirst(ddlModelPortfolio, value);
        }
    }

    [Browsable(false)]
    public string AccountNumber
    {
        get { return viewStateString("AccountNumber"); }
        set
        {
            ViewState["AccountNumber"] = value;
            if (ShowAccountNumber)
                txtAccountNumber.Text = value;
        }
    }

    [Browsable(false)]
    public string AccountName
    {
        get { return viewStateString("AccountName"); }
        set
        {
            ViewState["AccountName"] = value;
            if (ShowAccountName)
                txtAccountName.Text = value;
        }
    }
    
    [Browsable(false)]
    public bool AccountStatusActive
    {
        get { return viewStateBool("AccountStatusActive", true); }
        set
        {
            ViewState["AccountStatusActive"] = value;
            if (ShowAccountStatus)
                cbAccountStatusActive.Checked = value;
        }
    }

    [Browsable(false)]
    public bool AccountStatusInactive
    {
        get { return viewStateBool("AccountStatusInactive", false); }
        set
        {
            ViewState["AccountStatusInactive"] = value;
            if (ShowAccountStatus)
                cbAccountStatusInactive.Checked = value;
        }
    }

    [Browsable(false)]
    public bool EmailStatusYes
    {
        get { return viewStateBool("EmailStatusYes", true); }
        set
        {
            ViewState["EmailStatusYes"] = value;
            if (ShowEmailStatus)
                cbEmailStatusYes.Checked = value;
        }
    }

    [Browsable(false)]
    public bool EmailStatusNo
    {
        get { return viewStateBool("EmailStatusNo", true); }
        set
        {
            ViewState["EmailStatusNo"] = value;
            if (ShowEmailStatus)
                cbEmailStatusNo.Checked = value;
        }
    }

    [Browsable(false)]
    public string LoginStatus
    {
        get { return viewStateString("LoginStatus", "Any"); }
        set
        {
            ViewState["LoginStatus"] = null;
            if (ShowLoginStatus)
            {
                ListItem listItem = ddlLoginStatus.Items.FindByValue(value);
                if (listItem != null)
                {
                    ddlLoginStatus.SelectedValue = listItem.Value;
                    ViewState["LoginStatus"] = listItem.Value;
                }
                else
                    ddlLoginStatus.SelectedIndex = 0;
            }
        }
    }

    [Browsable(false)]
    public bool? HasLogin
    {
        get
        {
            switch (LoginStatus)
            {
                case "NoLogin":
                    return false;
                case "LoginSentNoPassword":
                case "PasswordSentActive":
                case "PasswordSentInactive":
                    return true;
                default:
                    return (bool?)null;
            }
        }
    }

    [Browsable(false)]
    public bool? PasswordSent
    {
        get
        {
            switch (LoginStatus)
            {
                case "LoginSentNoPassword":
                    return false;
                case "PasswordSentActive":
                case "PasswordSentInactive":
                    return true;
                default:
                    return (bool?)null;
            }
        }
    }

    [Browsable(false)]
    public bool? IsLoginActive
    {
        get
        {
            switch (LoginStatus)
            {
                case "PasswordSentActive":
                    return true;
                case "PasswordSentInactive":
                    return false;
                default:
                    return (bool?)null;
            }
        }
    }

    public string AccountNameLabel
    {
        get { return lblAccountName.Text; }
        set { lblAccountName.Text = value; }
    }

    public string AccountStatusLabel
    {
        get { return lblAccountStatus.Text; }
        set { lblAccountStatus.Text = value; }
    }
    
    [DefaultValue(false), Category("Behavior")]
    public bool ShowRemisier
    {
        get { return viewStateBool("ShowRemisier", false); }
        set
        {
            bool val = ShowRemisierEmployee ? true : value;

            ViewState["ShowRemisier"] = val;
            pnlRemisier.Visible = val;
            setAssetManagerAutoPostBack();
        }
    }
    
    [DefaultValue(false), Category("Behavior")]
    public bool ShowRemisierEmployee
    {
        get { return viewStateBool("ShowRemisierEmployee", false); }
        set
        {
            ViewState["ShowRemisierEmployee"] = value;
            pnlRemisierEmployee.Visible = value;
            ddlRemisier.AutoPostBack = value;
            
            if (value) ShowRemisier = true;
        }
    }

    [DefaultValue(false), Category("Behavior")]
    public bool ShowModelPortfolio
    {
        get { return viewStateBool("ShowModelPortfolio", false); }
        set
        {
            ViewState["ShowModelPortfolio"] = value;
            pnlModelPortfolio.Visible = value;
            setAssetManagerAutoPostBack();
        }
    }

    private void setAssetManagerAutoPostBack()
    {
        ddlAssetManager.AutoPostBack = ShowRemisier || ShowModelPortfolio;
    }

    [DefaultValue(true), Category("Behavior")]
    public bool ShowAccountNumber
    {
        get { return viewStateBool("ShowAccountNumber", true); }
        set
        {
            ViewState["ShowAccountNumber"] = value;
            pnlAccountNumber.Visible = value;
        }
    }

    [DefaultValue(true), Category("Behavior")]
    public bool ShowAccountName
    {
        get { return viewStateBool("ShowAccountName", true); }
        set
        {
            ViewState["ShowAccountName"] = value;
            pnlAccountName.Visible = value;
        }
    }

    [DefaultValue(false), Category("Behavior")]
    public bool ShowAccountStatus
    {
        get { return viewStateBool("ShowAccountStatus", false); }
        set
        {
            ViewState["ShowAccountStatus"] = value;
            pnlAccountStatus.Visible = value;
        }
    }

    [DefaultValue(false), Category("Behavior")]
    public bool ShowEmailStatus
    {
        get { return viewStateBool("ShowEmailStatus", false); }
        set
        {
            ViewState["ShowEmailStatus"] = value;
            pnlEmailStatus.Visible = value;
        }
    }

    [DefaultValue(false), Category("Behavior")]
    public bool ShowLoginStatus
    {
        get { return viewStateBool("ShowLoginStatus", false); }
        set
        {
            ViewState["ShowLoginStatus"] = value;
            pnlLoginStatus.Visible = value;
        }
    }

    [DefaultValue(true), Category("Behavior")]
    public bool ShowSearchButton
    {
        get { return viewStateBool("ShowSearchButton", true); }
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
        if (AssetManagerDisplayType == DisplayType.SelectionList)
            AssetManagerId = int.Parse(ddlAssetManager.SelectedValue);

        if (ShowRemisier && RemisierDisplayType == DisplayType.SelectionList)
            RemisierId = int.Parse(ddlRemisier.SelectedValue);

        if (ShowRemisierEmployee && RemisierEmployeeDisplayType == DisplayType.SelectionList)
            RemisierEmployeeId = int.Parse(ddlRemisierEmployee.SelectedValue);

        if (ShowModelPortfolio)
            ModelPortfolioId = int.Parse(ddlModelPortfolio.SelectedValue);

        if (ShowAccountName)
            AccountName = txtAccountName.Text.Trim();

        if (ShowAccountNumber)
            AccountNumber = txtAccountNumber.Text.Trim();

        if (ShowAccountStatus)
        {
            if (!cbAccountStatusActive.Checked && !cbAccountStatusInactive.Checked)
            {
                cbAccountStatusActive.Checked = true;
                cbAccountStatusInactive.Checked = true;
            }
            AccountStatusActive = cbAccountStatusActive.Checked;
            AccountStatusInactive = cbAccountStatusInactive.Checked;
        }

        if (ShowEmailStatus)
        {
            if (!cbEmailStatusYes.Checked && !cbEmailStatusNo.Checked)
            {
                cbEmailStatusYes.Checked = true;
                cbEmailStatusNo.Checked = true;
            }
            EmailStatusYes = cbEmailStatusYes.Checked;
            EmailStatusNo = cbEmailStatusNo.Checked;
        }

        if (ShowLoginStatus)
            LoginStatus = ddlLoginStatus.SelectedValue;
        
        OnSearch();
    }

    protected void OnSearch()
    {
        if (Search != null)
            Search(this, EventArgs.Empty);
    }

    [Description("Event triggered when button Search is clicked."), Category("Behavior")]
    public EventHandler Search;
}
