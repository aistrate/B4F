using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design.WebControls;
using B4F.TotalGiro.ApplicationLayer.UC;
using System.Globalization;
using B4F.TotalGiro.Utils;
using B4F.Web.WebControls;

[ValidationProperty("SelectedDate")]
public partial class CalendarPlusNavigation : System.Web.UI.UserControl
{
    #region Constructors and Ininitialization

    public event EventHandler DateChanged;

    protected void Page_Init(object sender, EventArgs e)
    {
        cldDate.DateChanged += new EventHandler(cldDate_DateChanged);
    }

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!IsPostBack)
    //    {
    //    }
    //}

    #endregion

    /// <summary>
    /// The currently selected date.
    /// </summary>
    [PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)]
    [DefaultValue(typeof(DateTime), "1/1/0001")]
    [Description("The currently selected date."), Category("Behavior")]
    [Bindable(true, BindingDirection.TwoWay)]
    public DateTime SelectedDate
    {
        get { return cldDate.SelectedDate; } 
        set { cldDate.SelectedDate = value; }
    }

    /// <summary>
    /// Specifies whether the Delete button (which empties the TextBox of the control) is visible.
    /// </summary>
    [Description("Specifies whether the Delete button (which empties the TextBox of the control) is visible."), DefaultValue(true), Category("Misc")]
    public bool IsButtonDeleteVisible
    {
        get { return cldDate.IsButtonDeleteVisible; }
        set { cldDate.IsButtonDeleteVisible = value; }
    }

    /// <summary>
    /// Specifies the default view of the control.
    /// </summary>
    [Description("Specifies the default view of the control."), DefaultValue(AjaxControlToolkit.CalendarDefaultView.Days), Category("Misc")]
    public AjaxControlToolkit.CalendarDefaultView DefaultView
    {
        get { return cldDate.DefaultView; }
        set { cldDate.DefaultView = value; }
    }

    [Description("The currently used date format."), Category("Behaviour")]
    public string Format
    {
        get { return cldDate.Format; }
        set { cldDate.Format = value; }
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
            btnPrev.Enabled = value;
            btnNext.Enabled = value;
            cldDate.Enabled = value; 
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
            btnPrev.Visible = value;
            btnNext.Visible = value;
            cldDate.Visible = value;
        }
    }

    //[Description("Width of the control."), DefaultValue(70), Category("Behavior")]
    //public Unit Width
    //{
    //    get 
    //    {
    //        double width = ViewState["Width"] != null ? (double)ViewState["Width"] : 70;
    //        return new Unit(width);
    //    }
    //    set
    //    {
    //        ViewState["Width"] = value.Value;
    //        this.txtCalendar.Width = value; 
    //    }
    //}

    [Description("ToolTip of the control."), DefaultValue(false), Category("Behavior")]
    public string ToolTip
    {
        get { return cldDate.ToolTip; }
        set { cldDate.ToolTip = value; }
    }


    #region Methods

    public void Clear()
    {
        cldDate.Clear();
    }

    public override void Focus()
    {
        cldDate.Focus();
    }

    protected void OnDateChanged(EventArgs e)
    {
        if (DateChanged != null)
        {
            DateChanged(this, e);
        }
    }

    #endregion

    #region Local Controls

    protected void cldDate_DateChanged(object sender, EventArgs e)
    {
        OnDateChanged(e);
    }

    protected void btnPrev_Click(object sender, ImageClickEventArgs e)
    {
        navigateDate(-1);
        OnDateChanged(e);
    }

    protected void btnNext_Click(object sender, ImageClickEventArgs e)
    {
        navigateDate(1);
        OnDateChanged(e);
    }

    protected void navigateDate(int day)
    {
        if (!cldDate.IsEmpty)
            cldDate.SelectedDate = cldDate.SelectedDate.AddDays(day);
        else
            cldDate.SelectedDate = DateTime.Today;
    }


    #endregion
}
