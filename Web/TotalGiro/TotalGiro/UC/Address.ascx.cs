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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Utils;

public partial class UC_Address : System.Web.UI.UserControl
{
    private const string DEFAULT_COUNTRY = "NEDERLAND";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataSet dsCountries = PersonEditAdapter.GetCountries();
            ddCountry.DataSource = dsCountries;
            ddCountry.DataTextField = "CountryName";
            ddCountry.DataValueField = "Key";
            ddCountry.DataBind();
            setDefaultCountry(ddCountry);
            lblResidentialAddress.Text = CaptionResidentialAddress;

            ddPostalCountry.DataSource = dsCountries;
            ddPostalCountry.DataTextField = "CountryName";
            ddPostalCountry.DataValueField = "Key";
            ddPostalCountry.DataBind();
            setDefaultCountry(ddPostalCountry);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (ViewState["CaptionResidentialAddress"] == null)
            CaptionResidentialAddress = "Residential Address";
    }

    private void setDefaultCountry(DropDownList ddlCountries)
    {
        if (ddlCountries.SelectedValue == int.MinValue.ToString())
        {
            int index = int.MinValue;
            ListItemCollection coll = ddlCountries.Items;
            for (int counter = 0; counter < coll.Count; counter++)
            {
                if (coll[counter].Text.ToUpper() == DEFAULT_COUNTRY)
                    index = counter;
            }
            if (index > int.MinValue)
                ddlCountries.SelectedIndex = index;
        }
    }

    public string Street
    {
        set { tbStreet.Text = value; }
        get { return tbStreet.Text; }
    }

    public string HouseNumber
    {
        get { return tbHouseNumber.Text; }
        set { tbHouseNumber.Text = value; }
    }

    public string HouseNumberSuffix
    {
        get { return tbHouseNumberSuffix.Text; }
        set { tbHouseNumberSuffix.Text = value; }
    }

    public string PostCode
    {
        get 
        {
            if (tbPostCode.Text != null)
                return tbPostCode.Text.Replace(" ", "");
            else
                return tbPostCode.Text; 
        }
        set { tbPostCode.Text = value; }
    }

    public string City
    {
        get { return tbCity.Text; }
        set { tbCity.Text = value; }
    }

    public string Country
    {
        get { return ddCountry.SelectedValue; }
        set { ddCountry.SelectedValue = value; }
    }

    public Address MainAddress 
    {
        get
        {
            Address address = null;
            if (!IsMainAddressEmpty)
            {
                address = new Address(
                    Street, HouseNumber, HouseNumberSuffix, PostCode, City, null);
                if (Util.IsNumeric(Country))
                    address.CountryId = Convert.ToInt32(Country);
            }
            return address;
        }
        set
        {
            if (value != null)
            {
                Street = value.Street;
                HouseNumber = value.HouseNumber;
                HouseNumberSuffix = value.HouseNumberSuffix;
                PostCode = value.PostalCode;
                City = value.City;
                Country = value.CountryId.ToString();
            }
        }
    }

    public Address PostAddress 
    {
        get
        {
            Address address = null;
            if (!IsPostAddressEmpty)
            {
                address = new Address(
                    PAStreet, PAHouseNumber, PAHouseNumberSuffix, PAPostCode, PACity, null);
                if (Util.IsNumeric(PACountry))
                    address.CountryId = Convert.ToInt32(PACountry);
            }
            return address;
        }
        set
        {
            if (value != null)
            {
                PAStreet = value.Street;
                PAHouseNumber = value.HouseNumber;
                PAHouseNumberSuffix = value.HouseNumberSuffix;
                PAPostCode = value.PostalCode;
                PACity = value.City;
                PACountry = value.CountryId.ToString();
            }
        }
    }

    public string PAStreet
    {
        set { tbPostalStreet.Text = value; }
        get { return tbPostalStreet.Text; }
    }

    public string PAHouseNumber
    {
        get { return tbPostalHouseNumber.Text; }
        set { tbPostalHouseNumber.Text = value; }
    }

    public string PAHouseNumberSuffix
    {
        get { return tbPostalHouseNumberSuffix.Text; }
        set { tbPostalHouseNumberSuffix.Text = value; }
    }

    public string PAPostCode
    {
       get 
       {
            if (tbPostalPostCode.Text != null)
                return tbPostalPostCode.Text.Replace(" ", "");
            else
                return tbPostalPostCode.Text; 
        }
        set { tbPostalPostCode.Text = value; }
    }

    public string PACity
    {
        get { return tbPostalCity.Text; }
        set { tbPostalCity.Text = value; }
    }

    public string PACountry
    {
        get { return ddPostalCountry.SelectedValue; }
        set { ddPostalCountry.SelectedValue = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the control is enabled.
    /// </summary>
    [Description("A property indicating whether the control is enabled."), DefaultValue(true), Category("Behavior")]
    public bool Enabled
    {
        get
        {
            object b = ViewState["Enabled"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["Enabled"] = value;
            lblCity.Enabled = value;
            lblCountry.Enabled = value;
            lblHouseNumber.Enabled = value;
            lblHouseNumberSuffix.Enabled = value;
            lblPostalAddress.Enabled = value;
            lblPostalCity.Enabled = value;
            lblPostalCountry.Enabled = value;
            lblPostalHouseNumber.Enabled = value;
            lblPostalHouseNumberSuffix.Enabled = value;
            lblPostalPostCode.Enabled = value;
            lblPostalStreet.Enabled = value;
            lblPostCode.Enabled = value;
            lblResidentialAddress.Enabled = value;
            lblStreet.Enabled = value;
            tbCity.Enabled = value;
            tbHouseNumber.Enabled = value;
            tbHouseNumberSuffix.Enabled = value;
            tbPostalCity.Enabled = value;
            tbPostalHouseNumber.Enabled = value;
            tbPostalHouseNumberSuffix.Enabled = value;
            tbPostalPostCode.Enabled = value;
            tbPostalStreet.Enabled = value;
            tbPostCode.Enabled = value;
            tbStreet.Enabled = value;
            ddCountry.Enabled = value;
            ddPostalCountry.Enabled = value;

            DataCheckingEnabled = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether DataChecking is enabled.
    /// </summary>
    [Description("A property indicating whether DataChecking is enabled."), DefaultValue(true), Category("Behavior")]
    public bool DataCheckingEnabled
    {
        get
        {
            object b = ViewState["DataCheckingEnabled"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["DataCheckingEnabled"] = value;
            reqCity.Enabled = value;
            reqHouseNumber.Enabled = value;
            reqPostCodeNumbers.Enabled = value;
            reqStreet.Enabled = value;
            rangeCountry.Enabled = value;
            rangeHouseNumber.Enabled = value;
            rangePostalHouseNumber.Enabled = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the Postal Address area should be visible.
    /// </summary>
    [Description("A property indicating whether the postal address area should be visible."), DefaultValue(true), Category("Behavior")]
    public bool ShowPostalAddress
    {
        get
        {
            object b = ViewState["ShowPostalAddress"];
            return ((b == null) ? true : (bool)b);
        }
        set
        {
            ViewState["ShowPostalAddress"] = value;
            pnlPostalAddress.Visible = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the labels should be visible.
    /// </summary>
    [Description("Sets the caption of the ResidentialAddress label."), DefaultValue("Residential Address"), Category("Behavior")]
    public string CaptionResidentialAddress
    {
        get { return (string)ViewState["CaptionResidentialAddress"]; }
        set { ViewState["CaptionResidentialAddress"] = value; }
    }

    public void Reset()
    {
        DataBind();
        tbCity.Text = string.Empty;
        tbHouseNumber.Text = string.Empty;
        tbHouseNumberSuffix.Text = string.Empty;
        tbPostalCity.Text = string.Empty;
        tbPostalHouseNumber.Text = string.Empty;
        tbPostalHouseNumberSuffix.Text = string.Empty;
        tbPostalPostCode.Text = string.Empty;
        tbPostalStreet.Text = string.Empty;
        tbPostCode.Text = string.Empty;
        tbStreet.Text = string.Empty;
        setDefaultCountry(ddCountry);
        setDefaultCountry(ddPostalCountry);
    }

    /// <summary>
    /// Is there any data filled in the control
    /// </summary>
    [Browsable(false)]
    public bool IsEmpty
    {
        get 
        {
            bool retVal = IsMainAddressEmpty;
            if (retVal && ShowPostalAddress)
                retVal = IsPostAddressEmpty;
            return retVal;
        }
    }

    /// <summary>
    /// Is there any Main Address data filled in the control
    /// </summary>
    [Browsable(false)]
    public bool IsMainAddressEmpty
    {
        get
        {
            bool retVal = true;
            if (tbCity.Text != string.Empty || tbHouseNumber.Text != string.Empty ||
                tbHouseNumberSuffix.Text != string.Empty || tbPostCode.Text != string.Empty ||
                tbStreet.Text != string.Empty ||
                (ddCountry.SelectedValue != int.MinValue.ToString() && ddCountry.SelectedItem.Text.ToUpper() != DEFAULT_COUNTRY))
                retVal = false;
            return retVal;
        }
    }

    /// <summary>
    /// Is there any Post Address data filled in the control
    /// </summary>
    [Browsable(false)]
    public bool IsPostAddressEmpty
    {
        get
        {
            bool retVal = true;
            if (ShowPostalAddress)
            {
                if (tbPostalCity.Text != string.Empty || tbPostalHouseNumber.Text != string.Empty ||
                    tbPostalHouseNumberSuffix.Text != string.Empty || tbPostalPostCode.Text != string.Empty ||
                    tbPostalStreet.Text != string.Empty ||
                    (ddPostalCountry.SelectedValue != int.MinValue.ToString() && ddPostalCountry.SelectedItem.Text.ToUpper() != DEFAULT_COUNTRY))
                    retVal = false;
            }
            return retVal;
        }
    }


    protected void ddCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddCountry.SelectedValue != int.MinValue.ToString())
        {
            bool enabled = false;
            if (ddCountry.SelectedItem.Text.ToUpper() == DEFAULT_COUNTRY)
                enabled = true;
            regexPostcode.Enabled = enabled;
            if (regexPostcode.Enabled)
                regexPostcode.Validate();
        }
    }

    protected void ddPostalCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddPostalCountry.SelectedValue != int.MinValue.ToString())
        {
            bool enabled = false;
            if (ddPostalCountry.SelectedItem.Text.ToUpper() == DEFAULT_COUNTRY)
                enabled = true;
            regexPostalPostCode.Enabled = enabled;
            if (regexPostalPostCode.Enabled)
                regexPostalPostCode.Validate();
        }
    }
}
