using System;

namespace B4F.TotalGiro.Client.Web.Help
{
    public partial class PortfolioHelp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((Help)Master).HeaderText = "Handleiding over uw portefeuille overzicht";
            }
        }
    } 
}
