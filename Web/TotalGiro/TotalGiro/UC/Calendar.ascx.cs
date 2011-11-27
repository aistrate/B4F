using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Globalization;
using B4F.TotalGiro.Utils;

[ValidationProperty("SelectedDate")]
public partial class UC_Calendar : UserControl
{
    #region Constructors and Ininitialization

    public event EventHandler DateChanged;

    protected void Page_Load(object sender, EventArgs e)
    {
        rvCalendar.MinimumValue = MinimumDate.ToShortDateString();
        rvCalendar.MaximumValue = MaximumDate.ToShortDateString(); ;

        if (!IsEditable)
        {
            txtCalendar.Attributes.Add("readonly", "readonly");
            txtCalendar.ForeColor = System.Drawing.Color.Gray;
        }

        ClientScriptManager csm = Page.ClientScript;
        if (!csm.IsClientScriptIncludeRegistered(typeof(UC_Calendar), "calendarClientScript"))
            csm.RegisterClientScriptInclude(typeof(UC_Calendar), "calendarClientScript", ResolveClientUrl("Calendar.js"));

        imgDelete.Attributes.Add("onclick", "document.getElementById('" + txtCalendar.ClientID + "').value=''; document.getElementById('" + txtCalendar.ClientID + "').fireEvent('onchange');");
        if (!IsPostBack)
        {
            cldxCalender.OnClientShowing = "showStuff";
            if (!string.IsNullOrEmpty(Format))
                txtCalendar.ToolTip = string.Format("Format {0}", Format);
        }

        //if (IsPostBack)
        //{
        //    txtCalendar.Text = Request.Form[txtCalendar.UniqueID];
        //}
    }

    public override void RenderControl(HtmlTextWriter writer)
    {
        string divCssClass = "";
        string divPositionRelative = ""; 

        if (CssClass != string.Empty)
            divCssClass = string.Format("class='{0}' ", CssClass);

        if (IsEditable)
            divPositionRelative = "; position: relative;";

        writer.WriteLine("<div {0}style='display: inline;{1}'>", divCssClass, divPositionRelative);
        base.RenderControl(writer);
        writer.WriteLine("</div>");
    }

    #endregion

    public DateTime SelectedDate
    {
        get { return parseDate(txtCalendar.Text); } 
        set 
        {
            if (Util.IsNotNullDate(value))
            {
                //cldxCalender.SelectedDate = value;
                txtCalendar.Text = value.ToString(Format);
            }
            else
                cldxCalender.SelectedDate = null;
        }
    }

    /// <summary>
    /// Specifies whether the Delete button (which empties the TextBox of the control) is visible.
    /// </summary>
    [Description("Specifies whether the Delete button (which empties the TextBox of the control) is visible."), DefaultValue(true), Category("Misc")]
    public bool IsButtonDeleteVisible
    {
        get { return ViewState["IsButtonDeleteVisible"] != null ? (bool)ViewState["IsButtonDeleteVisible"] : true; }
        set
        {
            ViewState["IsButtonDeleteVisible"] = value;
            tblDate.Rows[0].Cells[2].Visible = value;
            setDeleteButtonVisible();
        }
    }

    /// <summary>
    /// Specifies the default view of the control.
    /// </summary>
    [Description("Specifies the default view of the control."), DefaultValue(AjaxControlToolkit.CalendarDefaultView.Days), Category("Misc")]
    public AjaxControlToolkit.CalendarDefaultView DefaultView
    {
        get { return cldxCalender.DefaultView != null ? cldxCalender.DefaultView : AjaxControlToolkit.CalendarDefaultView.Days; }
        set { cldxCalender.DefaultView = value; }
    }

    [Description("The currently used css class."), Category("Behaviour")]
    public string CssClass
    {
        get { return ViewState["CssClass"] != null ? (string)ViewState["CssClass"] : ""; }
        set { ViewState["CssClass"] = value; }
    }

    [Description("The currently used ValidationGroup."), Category("Behaviour")]
    public string ValidationGroup
    {
        get { return ViewState["ValidationGroup"] != null ? (string)ViewState["ValidationGroup"] : ""; }
        set 
        { 
            ViewState["ValidationGroup"] = value;
            cvValidDate.ValidationGroup = value;
            rvCalendar.ValidationGroup = value;
        }
    }

    [Description("The MinimumDate."), Category("Behaviour")]
    public DateTime MinimumDate
    {
        get { return ViewState["MinimumDate"] != null ? (DateTime)ViewState["MinimumDate"] : DateTime.MinValue; }
        set { ViewState["MinimumDate"] = value; }
    }

    [Description("The MaximumDate."), Category("Behaviour")]
    public DateTime MaximumDate
    {
        get { return ViewState["MaximumDate"] != null ? (DateTime)ViewState["MaximumDate"] : DateTime.MaxValue; }
        set { ViewState["MaximumDate"] = value; }
    }

    [Description("The currently used date format."), Category("Behaviour")]
    public string Format
    {
        get 
        {
            if (IsEditable)
                return "dd/MM/yyyy";
            else
                return cldxCalender.Format; 
        }
        set 
        {
            if (cldxCalender != null && !IsEditable)
                cldxCalender.Format = value;
        }
    }

    [Description("Is the control editable."), DefaultValue(false), Category("Behaviour")]
    public bool IsEditable
    {
        get { return ViewState["IsEditable"] != null ? (bool)ViewState["IsEditable"] : false; }
        set
        {
            ViewState["IsEditable"] = value;
            txtCalendar.AutoPostBack = value;
            cvValidDate.Enabled = value;
            cvValidDateCalloutExtender.Enabled = value;
            rvCalendar.Enabled = value;
            rvCalendar_ValidatorCalloutExtender.Enabled = value;
            meeMaskEditExtender.Enabled = value;
            setDeleteButtonVisible();
        }
    }

    [Description("Is the control AutoPostBackable."), Category("Behaviour")]
    public bool AutoPostBack
    {
        get { return txtCalendar.AutoPostBack; }
        set 
        { 
            txtCalendar.AutoPostBack = value;
            setDeleteButtonVisible();
        }
    }

    [Browsable(false)]
    public bool IsEmpty
    {
        get { return Util.IsNullDate(SelectedDate); }
    }

    /// <summary>
    /// Enable/Disable the control.
    /// </summary>
    [Description("Enable/Disable the control."), DefaultValue(true), Category("Misc")]
    public bool Enabled
    {
        get { return cldxCalender.Enabled; }
        set { cldxCalender.Enabled = value; }
    }

    /// <summary>
    /// Visualize the control.
    /// </summary>
    [Description("Visualize the control."), DefaultValue(true), Category("Misc")]
    public bool Visible
    {
        get { return txtCalendar.Visible; }
        set
        {
            txtCalendar.Visible = value;
            imgCalender.Visible = value;
            setDeleteButtonVisible();
        }
    }

    [Description("Width of the control."), DefaultValue(70), Category("Behavior")]
    public Unit Width
    {
        get 
        {
            double width = ViewState["Width"] != null ? (double)ViewState["Width"] : 70;
            return new Unit(width);
        }
        set
        {
            ViewState["Width"] = value.Value;
            this.txtCalendar.Width = value; 
        }
    }

    [Description("ToolTip of the control."), DefaultValue(false), Category("Behavior")]
    public string ToolTip
    {
        get { return this.txtCalendar.ToolTip; }
        set { this.txtCalendar.ToolTip = value; }
    }


    #region Methods

    protected void setDeleteButtonVisible()
    {
        imgDelete.Visible = false;
        imbDelete.Visible = false;

        if (Visible && IsButtonDeleteVisible)
        {
            if (IsEditable)
                imbDelete.Visible = true;
            else
                imgDelete.Visible = true;
        }
    }


    protected void imbDelete_Click(object sender, ImageClickEventArgs e)
    {
        Clear();
        OnDateChanged(e);
    }

    public void Clear()
    {
        txtCalendar.Text = "";
        cldxCalender.SelectedDate = null;
    }

    public override void Focus()
    {
        imgCalender.Focus();
    }

    protected void OnDateChanged(EventArgs e)
    {
        if (DateChanged != null)
        {
            DateChanged(this, e);
        }
    }

    protected void cldxCalender_ClientDateSelectionChanged(object sender, EventArgs e)
    {
        OnDateChanged(e);
    }

    private DateTime parseDate(string dateValue)
    {
        DateTime date = DateTime.MinValue;
        if (!string.IsNullOrEmpty(dateValue))
            DateTime.TryParse(dateValue, out date);
        return date;
    }

    #endregion
}
