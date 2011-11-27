using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;

public partial class TotalGiroClient : System.Web.UI.MasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        HeaderText = WebsiteName;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string userName = CommonAdapter.GetUserName();
        
        createUserLogEntry(userName);
        
        if (!IsPostBack)
        {
            if (CommonAdapter.RunningInDebugMode())
            {
                pnlDatabase.Visible = true;
                lblDatabase.Text = CommonAdapter.GetDatabaseName();
            }

            if (userName != string.Empty)
                lblUser.Text = userName + "&nbsp;";
            else
                lblUserLabel.Visible = false;

            lblWebsiteAlternativeTitle.Text = WebsiteAlternativeTitle;
            pnlWebsiteAlternativeTitle.Visible = (lblWebsiteAlternativeTitle.Text != "");

            smdsMenu.SiteMapProvider = userName == string.Empty ||
                                       CommonAdapter.IsCurrentUserInRole("Client: Basic") ||
                                       CommonAdapter.IsCurrentUserInRole("Client: Basic with Disabled Settings") ||
                                       CommonAdapter.IsCurrentUserInRole("Client: Restricted") ?
                                            "CustomerSiteMapProvider" :
                                       CommonAdapter.IsCurrentUserInRole("Remisier Employee: Basic") ?
                                            "RemisierEmployeeSiteMapProvider" :
                                            "InternalEmployeeSiteMapProvider";
        }

        Page.Title = HeaderText;
    }

    protected void lsLogin_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        SafeSession.Current.Remove("LastUnhandledError");
    }

    private void createUserLogEntry(string userName)
    {
        if (userName != string.Empty && userName != OldUserName)
            CommonAdapter.CreateUserLogEntry(userName);
        OldUserName = userName;
    }

    protected string OldUserName
    {
        get
        {
            object s = Session["OldUserName"];
            return ((s == null) ? "" : (string)s);
        }
        set { Session["OldUserName"] = value; }
    }

    public string MasterBackgroundColor
    {
        get
        {
            object s = ConfigurationManager.AppSettings["MasterBackgroundColor"];
            return ((s == null || s == "") ? "White" : (string)s);
        }
    }

    public string WebsiteName
    {
        get
        {
            object s = ConfigurationManager.AppSettings["WebsiteName"];
            return ((s == null) ? "" : (string)s);
        }
    }

    public string WebsiteAlternativeTitle
    {
        get
        {
            object s = ConfigurationManager.AppSettings["WebsiteAlternativeTitle"];
            return ((s == null) ? "" : (string)s);
        }
    }

    public string HeaderText
    {
        get { return lblHeaderText.Text; }
        set { lblHeaderText.Text = value; }
    }

    public string HelpUrl
    {
        get { return hlkHelp.NavigateUrl; }
        set
        {
            hlkHelp.NavigateUrl = value;
            hlkHelp.Text = (value != null && value.Trim() != string.Empty ? "Uitleg van deze pagina" : "Gebruikershandleiding");
        }
    }

    public void ScrollToPageBottom()
    {
        hlkDisclaimer.Focus();
    }
}
