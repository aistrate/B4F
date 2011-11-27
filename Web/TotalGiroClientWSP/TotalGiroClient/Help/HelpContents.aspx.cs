using System;

public partial class HelpContents : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((TotalGiroClient)Master).HeaderText = "Overzichten gebruikershandleidingen";
    }
}
