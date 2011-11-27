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

public partial class CounterAccountFinder : System.Web.UI.UserControl
{
    // NOTE: pages that use this control and need to assign its properties (e.g. AssetManagerId, AccountNumber, ContactName)
    // should do it AFTER the page is loaded, otherwise the assignments may not take effect (call DataBind() first, if necessary)
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ShowSearchButton)
        {
            Utility.SetDefaultButton(this.Page, ddlAssetManager, btnSearch);
            if (this.ShowCounterAccountName)
                Utility.SetDefaultButton(this.Page, txtCounterAccountName, btnSearch);
            if (this.ShowCounterAccountNumber)
                Utility.SetDefaultButton(this.Page, txtCounterAccountNumber, btnSearch);
            if (this.ShowAccountNumber)
                Utility.SetDefaultButton(this.Page, txtAccountNumber, btnSearch);
            if (this.ShowContactName)
                Utility.SetDefaultButton(this.Page, txtContactName, btnSearch);
            if (this.ShowIsPublic)
                Utility.SetDefaultButton(this.Page, chkIsPublic, btnSearch);
            if (this.ShowContactActiveCbl)
                Utility.SetDefaultButton(this.Page, cblContactActive, btnSearch);
        }

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

        IsDataBound = true;

        base.DataBind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
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
        CounterAccountNumber = string.Empty;
        CounterAccountName = string.Empty;
        AccountNumber = string.Empty;
        ContactName = string.Empty;
        ContactActive = true;
        ContactInactive = false;
        IsPublic = false;
    }

    public bool Enabled
    {
        get { return ddlAssetManager.Enabled; }
        set
        {
            ddlAssetManager.Enabled = value;
            txtCounterAccountNumber.Enabled = value;
            txtCounterAccountName.Enabled = value;
            txtAccountNumber.Enabled = value;
            txtContactName.Enabled = value;
            cblContactActive.Enabled = value;
            chkIsPublic.Enabled = value;
            btnSearch.Enabled = value;
        }
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
                ViewState["AssetManagerId"] = (value != int.MinValue ? value : 0);
                ListItem listItem = ddlAssetManager.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlAssetManager.SelectedValue = listItem.Value;
                else if (ddlAssetManager.Items.Count > 0)
                    ddlAssetManager.SelectedIndex = 0;
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
            ViewState["AccountNumber"] = value;
            if (ShowAccountNumber)
                txtAccountNumber.Text = value;
        }
    }

    [Browsable(false)]
    public string ContactName
    {
        get
        {
            object s = ViewState["ContactName"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            ViewState["ContactName"] = value;
            if (ShowContactName)
                txtContactName.Text = value;
        }
    }
    
    [Browsable(false)]
    public string CounterAccountNumber
    {
        get
        {
            object s = ViewState["CounterAccountNumber"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            ViewState["CounterAccountNumber"] = value;
            if (ShowCounterAccountNumber)
                txtCounterAccountNumber.Text = value;
        }
    }

    [Browsable(false)]
    public string CounterAccountName
    {
        get
        {
            object s = ViewState["CounterAccountName"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            ViewState["CounterAccountName"] = value;
            if (ShowCounterAccountName)
                txtCounterAccountName.Text = value;
        }
    }

    [Browsable(false)]
    public bool ContactActive
    {
        get
        {
            object b = ViewState["ContactActive"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ContactActive"] = value;
            if (ShowContactActiveCbl)
                cblContactActive.Items.FindByValue("ACTIVE").Selected = value;
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
            ViewState["ContactInactive"] = value;
            if (ShowContactActiveCbl)
                cblContactActive.Items.FindByValue("INACTIVE").Selected = value;
        }
    }

    [Browsable(false)]
    public bool IsPublic
    {
        get
        {
            object b = ViewState["IsPublic"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["IsPublic"] = value;
            if (ShowIsPublic)
                chkIsPublic.Checked = value;
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
    /// Gets a value indicating whether search criterion ContactName should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion ContactName should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowContactName
    {
        get
        {
            object b = ViewState["ShowContactName"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ShowContactName"] = value;
            pnlContactName.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion CounterAccountNumber should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion CounterAccountNumber should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowCounterAccountNumber
    {
        get
        {
            object b = ViewState["ShowCounterAccountNumber"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowCounterAccountNumber"] = value;
            pnlCounterAccountNumber.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion CounterAccountName should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion CounterAccountName should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowCounterAccountName
    {
        get
        {
            object b = ViewState["ShowCounterAccountName"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowCounterAccountName"] = value;
            pnlCounterAccountName.Visible = value;
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
    /// Gets a value indicating whether search criterion IsPublic should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion IsPublic should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowIsPublic
    {
        get
        {
            object b = ViewState["ShowIsPublic"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowIsPublic"] = value;
            pnlIsPublic.Visible = value;
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
            AssetManagerId = int.Parse(ddlAssetManager.SelectedValue);

        if (ShowCounterAccountNumber)
            CounterAccountNumber = txtCounterAccountNumber.Text.Trim();

        if (ShowCounterAccountName)
            CounterAccountName = txtCounterAccountName.Text.Trim();

        if (ShowContactName)
            ContactName = txtContactName.Text.Trim();

        if (ShowAccountNumber)
            AccountNumber = txtAccountNumber.Text.Trim();

        if (ShowContactActiveCbl)
        {
            ContactActive = cblContactActive.Items.FindByValue("ACTIVE").Selected;
            ContactInactive = cblContactActive.Items.FindByValue("INACTIVE").Selected;
        }

        if (ShowIsPublic)
            IsPublic = chkIsPublic.Checked;

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
