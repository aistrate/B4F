using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Clients;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.UC;
using B4F.TotalGiro.Utils.Tuple;

public partial class ClientPortfolios : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        gvClients.SelectedIndex = -1;

        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Client Portfolios";

            if (LoadPersisted && SearchCriteria != null && NavigationSettings != null)
            {
                ctlAccountFinder.SetCriteria(SearchCriteria);
                gvClients.SetNavigationSettings(NavigationSettings);
                ctlAccountFinder.DoSearch();

                gvClients.SetSelectedKey(ContactId);
            }
            else
                gvClients.Sort("ShortName", SortDirection.Ascending);

            ctlAccountFinder.Focus();
        }

        elbErrorMessage.Text = "";
    }

    protected bool ShowFlagsPerAccount { get { return true; } }
    protected bool ShowFlagsPerContact { get { return false; } }
    
    protected bool LoadPersisted
    {
        get { return Utility.GetQueryParameters().GetBoolValue("loadpersisted", false); }
    }

    protected AccountFinderCriteria SearchCriteria
    {
        get { return Session["ClientPortfolios_SearchCriteria"] as AccountFinderCriteria; }
        set { Session["ClientPortfolios_SearchCriteria"] = value; }
    }

    protected GridViewNavigationSettings NavigationSettings
    {
        get { return Session["ClientPortfolios_NavigationSettings"] as GridViewNavigationSettings; }
        set { Session["ClientPortfolios_NavigationSettings"] = value; }
    }

    protected int ContactId
    {
        get
        {
            object i = Session["ContactId"];
            return (i == null ? 0 : (int)i);
        }
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        pnlClients.Visible = true;
        gvClients.DataBind();
    }

    //private DateTime startTime = DateTime.Now;

    protected void gvClients_DataBound(object sender, EventArgs e)
    {
        gvClients.Caption = string.Format("Clients ({0})", gvClients.DataRowCount);
        
        //elbErrorMessage.Text = DateTime.Now.Subtract(startTime).ToString();
    }

    protected void linkButtonField_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton linkButton = (LinkButton)sender;

            Session["ContactId"] = int.Parse(linkButton.CommandArgument);
            Session["AccountId"] = null;
            Session["ActivePortfolioViewIndex"] = null;

            SearchCriteria = ctlAccountFinder.GetCriteria();
            NavigationSettings = gvClients.GetNavigationSettings();

            switch (linkButton.CommandName.ToUpper())
            {
                case "PORTFOLIO":
                    Response.Redirect("~/Portfolio/PortfolioPositions.aspx");
                    break;
                case "CHARTS":
                    Response.Redirect("~/Charts/Charts.aspx");
                    break;
                case "PLANNER":
                    Response.Redirect("~/Planning/FinancialPlanner.aspx?initialize=true");
                    break;
                case "NOTAS":
                    Response.Redirect("~/Reports/Notas.aspx");
                    break;
                case "FINANCIALREPORTS":
                    Response.Redirect("~/Reports/FinancialReports.aspx");
                    break;
            }
        }
        catch (Exception ex)
        {
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected string FormatAccountNumbersByColor(IEnumerable<Tuple<string, Color>> accountNumbers)
    {
        return Utility.FormatAccountNumbers<Tuple<string, Color>>(accountNumbers,
                                                                  t => FormatTextByColor(t.Item1, t.Item2));
    }

    protected string FormatTextByColor(string text, Color color)
    {
        if (CommonAdapter.TrafficLightColors.Contains(color))
            return string.Format(@"{0} <img src='../Images/TrafficLight/Set5/{1}-light.png' title='{2}'
                                            width='12px' height='12px'
                                            style='vertical-align: base; margin-bottom: -1px; margin-left: -2px' />",
                                 text, color.Name.ToLower(), Utility.GetTrafficLightColorDescription(color));
        else
            return text;
    }

    protected Tuple<string, Color>[] GetActiveAccountNumbers(int contactId)
    {
        return ClientPortfoliosAdapter.GetActiveAccountNumbers(contactId, ShowFlagsPerAccount, accountColorCache);
    }

    protected Color GetContactColor(int contactId)
    {
        return ClientPortfoliosAdapter.GetContactColor(contactId, accountColorCache);
    }

    Dictionary<int, Color> accountColorCache = new Dictionary<int, Color>();
}
