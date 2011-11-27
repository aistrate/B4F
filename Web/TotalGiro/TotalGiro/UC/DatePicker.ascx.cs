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
using System.Globalization;

[ValidationProperty("SelectedDate")]
public partial class DatePicker : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ddlMonth.Items.Count == 0 || ddlYear.Items.Count == 0)
        {
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            ddlMonth.Items.Clear();
            for (int i = 0; i < 12; i++)
                ddlMonth.Items.Add(monthNames[i]);

            monthChanged((cldDate.SelectedDate.Year != 1 ? cldDate.SelectedDate : DateTime.Today));
        }
        txtDate.Width = new Unit(80);
        tblDate.Rows[0].Cells[0].Width = new Unit(80);
    }

    protected void imbCalendar_Click(object sender, ImageClickEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

    protected void imbDelete_Click(object sender, ImageClickEventArgs e)
    {
        bool fireEvent = false;
        if (cldDate.SelectedDate != DateTime.MinValue)
            fireEvent = true;
        Clear();
        if (fireEvent)
            OnSelectionChanged();
    }
    
    protected void cldDate_SelectionChanged(object sender, EventArgs e)
    {
        txtDate.Text = cldDate.SelectedDate.ToString("dd-MM-yyyy");
        IsExpanded = false;
        OnSelectionChanged();
    }

    /// <summary>
    /// The currently selected date.
    /// </summary>
    [Description("The currently selected date."), Category("Misc")]
    public DateTime SelectedDate
    {
        get { return cldDate.SelectedDate; }
        set
        {
            if (value != DateTime.MinValue)
            {
                cldDate.SelectedDate = value.Date;
                cldDate.VisibleDate = value.Date;
                txtDate.Text = cldDate.SelectedDate.ToString("dd-MM-yyyy");
            }
            else
                Clear();
        }
    }

    [Browsable(false)]
    public bool IsEmpty
    {
        get { return cldDate.SelectedDate == DateTime.MinValue; }
    }

    [Browsable(false)]
    public bool IsExpanded
    {
        get { return pnlCalendar.Visible; }
        set
        {
            bool oldValue = pnlCalendar.Visible;
            pnlCalendar.Visible = value;
            if (oldValue != value)
                OnExpanded();
        }
    }
    
    /// <summary>
    /// Specifies whether the Delete button (which empties the TextBox of the control) is visible.
    /// </summary>
    [Description("Specifies whether the Delete button (which empties the TextBox of the control) is visible."), DefaultValue(true), Category("Misc")]
    public bool IsButtonDeleteVisible
    {
        get { return imbDelete.Visible; }
        set 
        { 
            imbDelete.Visible = value;
            tblDate.Rows[0].Cells[2].Visible = value;
        }
    }

    [Description("Specifies how many years will be displayed in the DropDownList BEFORE the current year."), DefaultValue(4), Category("Appearance")]
    public int ListYearsBeforeCurrent
    {
        get
        {
            object i = ViewState["ListYearsBeforeCurrent"];
            return ((i == null) ? 4 : (int)i);
        }
        set { ViewState["ListYearsBeforeCurrent"] = value; }
    }

    [Description("Specifies how many years will be displayed in the DropDownList AFTER the current year."), DefaultValue(4), Category("Appearance")]
    public int ListYearsAfterCurrent
    {
        get
        {
            object i = ViewState["ListYearsAfterCurrent"];
            return ((i == null) ? 4 : (int)i);
        }
        set { ViewState["ListYearsAfterCurrent"] = value; }
    }

    public void Clear()
    {
        cldDate.SelectedDate = DateTime.MinValue;
        IsExpanded = false;
        txtDate.Text = "";
    }

    public override void Focus()
    {
        txtDate.Focus();
    }

    /// <summary>
    /// Enable/Disable the control.
    /// </summary>
    [Description("Enable/Disable the control."), DefaultValue(true), Category("Misc")]
    public bool Enabled
    {
        get { return cldDate.Enabled; }
        set 
        {
            cldDate.Enabled = value;
            imbDelete.Enabled = value;
            imbCalendar.Enabled = value;
        }
    }

    /// <summary>
    /// Visualize the control.
    /// </summary>
    [Description("Visualize the control."), DefaultValue(true), Category("Misc")]
    public bool Visible
    {
        get { return cldDate.Visible; }
        set
        {
            txtDate.Visible = value;
            cldDate.Visible = value;
            imbDelete.Visible = value;
            imbCalendar.Visible = value;
        }
    }

    protected void cldDate_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
    {
        monthChanged(e.NewDate);
    }

    protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        setVisibleDate();
    }

    protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        setVisibleDate();
        fillYearsList(int.Parse(ddlYear.SelectedValue));
    }

    private void monthChanged(DateTime newDate)
    {
        ddlMonth.SelectedIndex = newDate.Month - 1;
        fillYearsList(newDate.Year);
    }

    private void fillYearsList(int currentYear)
    {
        ddlYear.Items.Clear();
        for (int i = currentYear - ListYearsBeforeCurrent; i <= currentYear + ListYearsAfterCurrent; i++)
            ddlYear.Items.Add(i.ToString());
        ddlYear.SelectedValue = currentYear.ToString();
    }

    private void setVisibleDate()
    {
        cldDate.VisibleDate = new DateTime(int.Parse(ddlYear.SelectedValue), ddlMonth.SelectedIndex + 1, 1);
    }

    protected void OnSelectionChanged()
    {
        if (SelectionChanged != null)
            SelectionChanged(this, EventArgs.Empty);
    }

    protected void OnExpanded()
    {
        if (Expanded != null)
            Expanded(this, EventArgs.Empty);
    }

    [Description("Event triggered when the selected date is changed."), Category("Behavior")]
    public EventHandler SelectionChanged;
    public EventHandler Expanded;
}
