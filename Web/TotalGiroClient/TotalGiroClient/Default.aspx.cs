using System;
using B4F.TotalGiro.ClientApplicationLayer.Common;

namespace B4F.TotalGiro.Client.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["ContactId"] = null;
                Session["AccountId"] = null;
                Session["ActivePortfolioViewIndex"] = null;
                Session["ClientPortfolio_SearchCriteria"] = null;

                if (CommonAdapter.GetUserName() == string.Empty)
                {
                    mvwWelcome.ActiveViewIndex = 0;
                    litWebsiteName.Text = ((TotalGiroClient)Master).WebsiteName;
                    pnlAnnouncement.Visible = (DateTime.Now < new DateTime(2008, 9, 30));
                }
                else if (CommonAdapter.IsCurrentUserInRole("Client: Basic") ||
                         CommonAdapter.IsCurrentUserInRole("Client: Basic with Disabled Settings") ||
                         CommonAdapter.IsCurrentUserInRole("Client: Restricted"))
                    Response.Redirect("~/Portfolio/PortfolioPositions.aspx");
                else if (CommonAdapter.IsCurrentUserInRole("Clients: Client Portfolios") ||
                         CommonAdapter.IsCurrentUserInRole("Remisier Employee: Basic"))
                    Response.Redirect("~/Clients/ClientPortfolios.aspx");
                else if (CommonAdapter.IsCurrentUserInRole("Administration: Client Logins"))
                    Response.Redirect("~/Logins/ClientLogins.aspx");
                else
                    mvwWelcome.ActiveViewIndex = -1;
            }
        }
    } 
}
