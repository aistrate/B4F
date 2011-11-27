using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using System.Drawing;

public partial class DecimalBox : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (ViewState["DecimalPlaces"] == null)
            DecimalPlaces = 2;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (hdfDecimalSeparator.Value == null || hdfDecimalSeparator.Value == string.Empty)
            hdfDecimalSeparator.Value = Utility.DecimalSeparator();
        if (hdfNumberGroupSeparator.Value == null || hdfNumberGroupSeparator.Value == string.Empty)
            hdfNumberGroupSeparator.Value = Utility.NumberGroupSeparator();
        if (hdfDecimalPlaces.Value == null || hdfDecimalPlaces.Value == string.Empty)
            hdfDecimalPlaces.Value = DecimalPlaces.ToString();

        ClientScriptManager csm = Page.ClientScript;
        if (!csm.IsClientScriptIncludeRegistered(typeof(DecimalBox), "decimalBoxClientScript"))
            csm.RegisterClientScriptInclude(typeof(DecimalBox), "decimalBoxClientScript", ResolveClientUrl("DecimalBox.js"));

    }

    /// <summary>
    /// The value of the decimal box.
    /// </summary>
    [Description("The value of the decimal box"), Category("Behavior")]
    public decimal Value
    {
        get 
        {
            decimal retVal;
            decimal.TryParse(tbDecimal.Text, out retVal);
            return decimal.Round(retVal, DecimalPlaces); 
        }
        set 
        {
            decimal v = decimal.Round(value, DecimalPlaces);
            tbDecimal.Text = v.ToString(); 
        }
    }

    ///// <summary>
    ///// The value.ToString()of the decimal box
    ///// </summary>
    //[Description("The value.ToString()of the decimal box"), Category("Behavior")]
    public string Text
    {
        get { return tbDecimal.Text; }
        set 
        {
            decimal v;
            if (decimal.TryParse(value, out v))
                tbDecimal.Text = decimal.Round(v, DecimalPlaces).ToString();
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Model Portofolio should be visible.
    /// </summary>
    [Description("Is the Negative Sign allowed."), DefaultValue(false), Category("Behavior")]
    public bool AllowNegativeSign
    {
        get
        {
            object b = ViewState["AllowNegativeSign"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["AllowNegativeSign"] = value;
            setValidChars();
        }
    }

    /// <summary>
    /// get/set decimals of number
    /// </summary>
    [Description("Number of decimal places allowed."), DefaultValue(2), Category("Behavior")]
    public int DecimalPlaces
    {
        get
        {
            int decimalPlaces = 0;
            object b = ViewState["DecimalPlaces"];
            if (b != null)
                int.TryParse(b.ToString(), out decimalPlaces);
            return decimalPlaces;
        }
        set
        {
            if (value < 0) value = 0;
            ViewState["DecimalPlaces"] = value;
            setValidChars();
            hdfDecimalPlaces.Value = value.ToString();
        }
    }

    /// <summary>
    /// get/set decimals of number
    /// </summary>
    [Description("The Maximum value allowed."), DefaultValue(0), Category("Behavior")]
    public decimal MaximumValue
    {
        get
        {
            int maximumValue = 0;
            object b = ViewState["MaximumValue"];
            if (b != null)
                int.TryParse(b.ToString(), out maximumValue);
            return maximumValue;
        }
        set
        {
            ViewState["MaximumValue"] = value;
            if (value != 0)
            {
                rvDecimal.MaximumValue = value.ToString();
            }
            rvDecimal.Enabled = (value != 0);
        }
    }

    private void setValidChars()
    {
        if (DecimalPlaces == 0)
            this.ftbeDecimal.ValidChars = "";
        else
            this.ftbeDecimal.ValidChars = CONST_ValidChars;

        if (AllowNegativeSign)
            this.ftbeDecimal.ValidChars += "-";
    }

    [Description("Width of the control."), Category("Behavior")]
    public Unit Width
    {
        get {  return this.tbDecimal.Width; }
        set { this.tbDecimal.Width = value; }
    }

    [Description("Is the control aligned to the left."), DefaultValue(false), Category("Behavior")]
    public bool AlignLeft
    {
        get { return (this.tbDecimal.Style["text-align"] == "left"); }
        set { this.tbDecimal.Style["text-align"] = (value ? "left" : "right"); }
    }

    [Description("ToolTip of the control."), DefaultValue(false), Category("Behavior")]
    public string ToolTip
    {
        get { return this.tbDecimal.ToolTip; }
        set { this.tbDecimal.ToolTip = value; }
    }

    [Description("autocomplete setting of the control."), DefaultValue("off"), Category("Behavior")]
    public string autocomplete
    {
        get { return this.tbDecimal.Attributes["autocomplete"].ToString(); }
        set { this.tbDecimal.Attributes.Add("autocomplete", value); }
    }

    public void Clear()
    {
        tbDecimal.Text = "";
    }

    public bool IsEmpty
    {
        get { return (tbDecimal.Text == ""); }
    }

    /// <summary>
    /// Enable/Disable the control.
    /// </summary>
    [Description("Enable/Disable the control."), DefaultValue(true), Category("Misc")]
    public bool Enabled
    {
        get { return tbDecimal.Enabled; }
        set { tbDecimal.Enabled = value; }
    }

    /// <summary>
    /// Visualize the control.
    /// </summary>
    [Description("Visualize the control."), DefaultValue(true), Category("Misc")]
    public bool Visible
    {
        get { return tbDecimal.Visible; }
        set { tbDecimal.Visible = value; }
    }

    /// <summary>
    /// ReadOnly property of the control.
    /// </summary>
    [Description("ReadOnly property of the control."), DefaultValue(true), Category("Misc")]
    public bool ReadOnly
    {
        get { return tbDecimal.ReadOnly; }
        set { tbDecimal.ReadOnly = value; }
    }

    /// <summary>
    /// The BackColor of the control.
    /// </summary>
    [Description("The BackColor of the control."), DefaultValue(true), Category("Misc")]
    public Color BackColor
    {
        get { return tbDecimal.BackColor; }
        set { tbDecimal.BackColor = value; }
    }

    /// <summary>
    /// The AutoPostBack property of the control.
    /// </summary>
    [Description("The AutoPostBack property of the control."), DefaultValue(false), Category("Misc")]
    public bool AutoPostBack
    {
        get { return tbDecimal.AutoPostBack; }
        set { tbDecimal.AutoPostBack = value; }
    }

    protected void tbDecimal_TextChanged(object sender, EventArgs e)
    {
        if (ValueChanged != null)
        {
            bool isValid = true;
            if (rvDecimal.Enabled)
            {
                rvDecimal.Validate();
                isValid = rvDecimal.IsValid;
            }
            if (isValid)
                ValueChanged(this, EventArgs.Empty);
        }
    }

    public EventHandler ValueChanged;
    const string CONST_ValidChars = ".,";

}
