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
using B4F.TotalGiro.ApplicationLayer;

public partial class EG : System.Web.UI.MasterPage
{
    public string CustomBGColor = ConfigurationManager.AppSettings["CustomBGColor"];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (CommonAdapter.RunningInDebugMode())
                lblDatabase.Text = CommonAdapter.GetDatabaseName();
            else
                lblDatabaseLabel.Visible = false;

            string userName = CommonAdapter.GetUserName();
            if (userName != string.Empty)
                lblUser.Text = userName;
            else
                lblUserLabel.Visible = false;
        }

        // When the background colour is not present, display red colour.
        if (CustomBGColor == "" || CustomBGColor == null)
        {
            CustomBGColor = "Red";
        }
    }

	public String setHeaderText
	{
		set 
        { 
            this.headerText.Text = value; 
        }
	}

    protected void menMain_MenuItemDataBound(object sender, MenuEventArgs e)
    {
        SiteMapNode node = e.Item.DataItem as SiteMapNode;

        // check for the visible attribute and if false
        // remove the node from the parent
        // this allows nodes to appear in the SiteMapPath but not show on the menu

        if (!string.IsNullOrEmpty(node["checkManagementCompany"]))
        {
            bool isVisible = CommonAdapter.GetManagementCompanyProperty(node["checkManagementCompany"]);
            if (!isVisible)
                e.Item.Parent.ChildItems.Remove(e.Item);
        }
    }


}
