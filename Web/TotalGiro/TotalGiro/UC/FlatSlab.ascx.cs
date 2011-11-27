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

public partial class FlatSlab : System.Web.UI.UserControl
{

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Page.IsPostBack)
		{
			bool boolCustVisible = true;
            if (dbFrom.Value != 0M)
            {
                boolCustVisible = false;
            }
			if (dbPercent.Value != 0M)
			{
				boolCustVisible = false;
			}
			custVal.Visible = boolCustVisible;
		}
	}

    public void checkFields(object source, ServerValidateEventArgs args)
	{
        if (IsAmountBased && dbPercent.IsEmpty)
		{
			args.IsValid = false;
			return;
		}
        else if (!IsAmountBased && dbTariff.IsEmpty)
        {
            args.IsValid = false;
            return;
        }
    }

    /// <summary>
    /// Is this a amount based flat slab?
    /// </summary>
    [Description("Is this a amount based flat slab."), DefaultValue(true), Category("Behavior")]
    public bool IsAmountBased
    {
        get
        {
            object b = ViewState["IsAmountBased"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["IsAmountBased"] = value;
            dbPercent.Visible = value;
            litPercent.Visible = value;
            rangeValPercent.Enabled = value;
            custVal.Enabled = value;
            
            dbTariff.Visible = !value;
            lblTariffCurrency.Visible = !value;
            rvTariff.Enabled = !value;
            cvTariff.Enabled = !value;
        }
    }

    /// <summary>
    /// The commission currency symbol being displayed?
    /// </summary>
    [Description("The commission currency symbol being displayed."), DefaultValue("€"), Category("Behavior")]
    public string CommCurrencySymbol
    {
        get
        {
            object b = ViewState["CommCurrencySymbol"];
            return ((b == null) ? "€" : (string)b);
        }
        set
        {
            ViewState["CommCurrencySymbol"] = value;
            lblTariffCurrency.Text = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether search criterion Model Portofolio should be visible.
    /// </summary>
    [Description("Gets a value indicating whether the static charge should be visible."), DefaultValue(false), Category("Behavior")]
    public bool ShowStaticCharge
    {
        get
        {
            object b = ViewState["ShowStaticCharge"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ViewState["ShowStaticCharge"] = value;
            lblStaticCharge.Visible = value;
            dbStaticCharge.Visible = value;
            tbEurot2.Visible = value;
            rvStaticCharge.Enabled = value;
        }
    }

    public RangeValidator RangeValidatorFrom
    {
        get { return this.FromRangeValidator; }
    }

    public RangeValidator RangeValidatorPercent
    {
        get { return this.rangeValPercent; }
    }

    public RangeValidator RangeValidatorTariff
    {
        get { return this.rvTariff; }
    }

    public DecimalBox dBoxFrom
    {
        get { return dbFrom; }
    }

    public DecimalBox dBoxPercent
    {
        get { return dbPercent; }
    }

    public DecimalBox dBoxTariff
    {
        get { return dbTariff; }
    }

    public DecimalBox dBoxStaticCharge
    {
        get 
        {
            if (dbStaticCharge.Visible)
                return dbStaticCharge;
            else
                return null;
        }
    }

    /// <summary>
    /// The value of the From Range.
    /// </summary>
    [Description("The value of the From Range"), Category("Behavior")]
    public decimal FromRange
    {
        get { return dBoxFrom.Value; }
        set { dBoxFrom.Value = value; }
    }

    /// <summary>
    /// The value of the Percentage.
    /// </summary>
    [Description("The value of the From Range"), Category("Behavior")]
    public decimal Percentage
    {
        get { return dBoxPercent.Value; }
        set { dBoxPercent.Value = value; }
    }

    /// <summary>
    /// The value of the Static Charge.
    /// </summary>
    [Description("The value of the Static Charge"), Category("Behavior")]
    public decimal StaticCharge
    {
        get 
        {
            if (dBoxStaticCharge != null)
                return dBoxStaticCharge.Value;
            else
                return 0M;
        }
        set 
        {
            if (dBoxStaticCharge != null)
                dBoxStaticCharge.Value = value; 
        }
    }

    /// <summary>
    /// The value of the Percentage.
    /// </summary>
    [Description("The label of the Percentage"), Category("Behavior")]
    public string PercentageLabel
    {
        get { return lblUc.Text; }
        set { lblUc.Text = value; }
    }

}
