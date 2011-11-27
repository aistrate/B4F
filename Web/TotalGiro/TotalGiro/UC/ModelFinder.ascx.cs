using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class ModelFinder : System.Web.UI.UserControl
{
    // NOTE: pages that use this control and need to assign its properties (e.g. AssetManagerId, ModelName, AccountName)
    // should do it AFTER the page is loaded, otherwise the assignments may not take effect (call DataBind() first, if necessary)
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ShowSearchButton)
        {
            Utility.SetDefaultButton(this.Page, ddlModelActive, btnSearch);
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

        if (ShowActiveStatus)
            ddlModelActive.DataBind();

        IsDataBound = true;

        base.DataBind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    public void Reset()
    {
        DataBind();
        ModelName = string.Empty;
        ActiveStatus = ((int)AccountFinderAdapter.AccountGuiStatus.Active);
    }

    public bool Enabled
    {
        get { return txtModelName.Enabled; }
        set
        {
            txtModelName.Enabled = value;
            ddlModelActive.Enabled = value;
            btnSearch.Enabled = value;
        }
    }

    [Browsable(false)]
    public int ActiveStatus
    {
        get
        {
            object i = ViewState["ActiveStatus"];
            return ((i == null) ? (int)B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.Active : (int)i);
        }
        set
        {
            if (ShowActiveStatus)
            {
                ListItem listItem = ddlModelActive.Items.FindByValue(value.ToString());
                if (listItem != null)
                    ddlModelActive.SelectedValue = listItem.Value;
                else if (ddlModelActive.Items.Count > 0)
                    ddlModelActive.SelectedIndex = 0;
                ViewState["ActiveStatus"] = Utility.GetKeyFromDropDownList(ddlModelActive);
            }
        }
    }

    [Browsable(false)]
    public string ModelName
    {
        get
        {
            object s = ViewState["ModelName"];
            return ((s == null) ? string.Empty : (string)s);
        }
        set
        {
            ViewState["ModelName"] = value;
            txtModelName.Text = value;
        }
    }


    /// <summary>
    /// Gets a value indicating whether search criterion Remisier should be visible.
    /// </summary>
    [Description("Gets a value indicating whether search criterion Remisier should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowActiveStatus
    {
        get
        {
            object b = ViewState["ShowActiveStatus"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ShowActiveStatus"] = value;
            pnlActivityFilter.Visible = value;
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
        if (ShowActiveStatus)
            ActiveStatus = Utility.GetKeyFromDropDownList(ddlModelActive);

        ModelName = txtModelName.Text.Trim();

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
