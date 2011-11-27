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

public partial class TelephoneDetails : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    ///// <summary>
    ///// Gets a value indicating whether search criterion AccountName should be visible.
    ///// </summary>
    //[Description("Telephone Number Country Code"), DefaultValue(""), Category("Behavior")]
    public string CountryCode
    {
        get { return txtCountryID.Text; }
        set { txtCountryID.Text = value; }
    }


    public string CityCode
    {
        get { return txtCityID.Text; }
        set { txtCityID.Text = value; }
    }

  
    public string TelephoneNumber
    {
        get { return txtNumberID.Text; }
        set { txtNumberID.Text = value; }
    }

    protected void btnEditDetails_Click(object sender, EventArgs e)
    {
        pnlTelephoneEdit.Visible = true;
    }
}
