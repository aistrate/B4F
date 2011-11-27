using System;

namespace B4F.TotalGiro.Client.Web.Help
{
    public partial class ChartsHelp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((Help)Master).HeaderText = "Handleiding voor de grafieken";
        }
    } 
}
