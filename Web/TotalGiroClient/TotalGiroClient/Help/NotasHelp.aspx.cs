using System;

namespace B4F.TotalGiro.Client.Web.Help
{
    public partial class NotasHelp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((Help)Master).HeaderText = "Handleiding voor de afschriften";
            }
        }
    } 
}
