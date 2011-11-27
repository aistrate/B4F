using System;

public partial class ChartsHelp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((Help)Master).HeaderText = "Handleiding voor de grafieken";
    }
}
