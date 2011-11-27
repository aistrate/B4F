using System;

namespace B4F.TotalGiro.Client.Web
{
    public partial class Privacy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Privacy Reglement";
            }
        }
    } 
}
