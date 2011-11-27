using System;

public partial class SettingsHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((Help)Master).HeaderText = "Handleiding voor de instellingen";
        }
    }
}
