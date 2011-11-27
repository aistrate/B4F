using System;

namespace B4F.TotalGiro.Client.Web.Help
{
    public partial class Help : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string HeaderText
        {
            get { return ((TotalGiroClient)Master).HeaderText; }
            set { ((TotalGiroClient)Master).HeaderText = value; }
        }
    } 
}
