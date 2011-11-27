using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;

namespace B4F.TotalGiro.Client.Web.Authenticate
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CommonAdapter.GetUserName() != string.Empty)
                Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Inloggen";
                ((TotalGiroClient)Master).HelpUrl = "~/Help/LoginHelp.aspx";

                pnlAnnouncement.Visible = (DateTime.Now.Date <= new DateTime(2009, 4, 15));

                ctlLogin.Focus();
            }
        }

        protected void ctlLogin_LoggingIn(object sender, LoginCancelEventArgs e)
        {
            Session["LastLoginDate"] = CommonAdapter.GetLastLoginDate(ctlLogin.UserName.Trim());
        }

        protected void ctlLogin_LoginError(object sender, EventArgs e)
        {
            Session["LastLoginDate"] = null;
        }
    }
}
