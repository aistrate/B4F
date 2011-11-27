using System;

namespace B4F.TotalGiro.Client.Web.Help
{
    public partial class HelpContents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((TotalGiroClient)Master).HeaderText = "Overzichten gebruikershandleidingen";
        }
    } 
}
