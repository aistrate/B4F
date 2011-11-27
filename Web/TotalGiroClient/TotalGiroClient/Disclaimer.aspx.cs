using System;

namespace B4F.TotalGiro.Client.Web
{
    public partial class Disclaimer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Disclaimer";
            }
        }
    } 
}
