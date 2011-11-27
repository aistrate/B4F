using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.Portfolio;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

public partial class PortfolioPositions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        elbErrorMessage.Text = "";
        
        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Portefeuille";
            ((TotalGiroClient)Master).HelpUrl = "~/Help/PortfolioHelp.aspx";

            gvOpenPositions.Sort("InstrumentName", SortDirection.Ascending);
            gvClosedPositions.Sort("InstrumentName", SortDirection.Ascending);

            ctlPortfolioNavigationBar.Visible = CurrentLoginType != LoginTypes.Customer;

            lnkCashMutations.Visible = CommonAdapter.IsCurrentUserInRole("Client: Basic") ||
                                       CommonAdapter.IsCurrentUserInRole("Client: Basic with Disabled Settings") ||
                                       CommonAdapter.IsCurrentUserInRole("Clients: Client Portfolios") ||
                                       CommonAdapter.IsCurrentUserInRole("Remisier Employee: Basic");

            // initialize from Session state
            ActivePortfolioViewIndex = ActivePortfolioViewIndex;
        }
    }

    protected int AccountId
    {
        get
        {
            object i = Session["AccountId"];
            return (i == null ? 0 : (int)i);
        }
        set { Session["AccountId"] = value; }
    }

    protected void ddlAccount_DataBound(object sender, EventArgs e)
    {
        if (AccountId != 0 && ddlAccount.Items.FindByValue(AccountId.ToString()) != null)
            ddlAccount.SelectedValue = AccountId.ToString();
        else if (ddlAccount.Items.Count > 0)
            ddlAccount.SelectedIndex = 0;
        
        showAllPortfolio();
    }

    protected GridView ActivePortfolioGridView
    {
        get
        {
            switch (ActivePortfolioViewIndex)
            {
                case 0:
                    return gvOpenPositions;
                case 1:
                    return gvPortfolioComponents;
                default:
                    return gvClosedPositions;
            }
        }
    }

    protected int ActivePortfolioViewIndex
    {
        get
        {
            object i = Session["ActivePortfolioViewIndex"];
            return (i == null ? 0 : (int)i);
        }
        set
        {
            int val = (0 <= value && value <= 2) ? value : 0;

            Session["ActivePortfolioViewIndex"] = val;
            mvwPositions.ActiveViewIndex = val;

            var panels = new List<Panel>()
            {
                pnlShowOpenPositions,
                pnlShowPortfolioComponents,
                pnlShowClosedPositions
            };
            
            for (int i = 0; i < 3; i++)
                panels[i].Visible = (i != val);
        }
    }

    protected LoginTypes CurrentLoginType
    {
        get
        {
            if (currentLoginType == null)
                currentLoginType = CommonAdapter.GetCurrentLoginType();
            return (LoginTypes)currentLoginType;
        }
    }
    private LoginTypes? currentLoginType = null;

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        TreeViewNodes = null;
        showAllPortfolio();
    }

    protected void lnkShowActivePortfolioView_Command(object sender, CommandEventArgs e)
    {
        int newActivePortfolioViewIndex = 0;
        if (int.TryParse((string)e.CommandArgument, out newActivePortfolioViewIndex))
        {
            ActivePortfolioViewIndex = newActivePortfolioViewIndex;
            showAllPortfolio();

            ((TotalGiroClient)Master).ScrollToPageBottom();
        }
    }

    protected void lnkInstrument_Command(object sender, CommandEventArgs e)
    {
        int positionId = 0;
        if (int.TryParse((string)e.CommandArgument, out positionId))
        {
            Session["PositionId"] = positionId;
            if (e.CommandName == "PositionTxsSecurities")
                Response.Redirect("~/Portfolio/PositionTxsSecurities.aspx");
        }
    }

    private void showAllPortfolio()
    {
        try
        {
            AccountId = (ddlAccount.SelectedValue != string.Empty ? int.Parse(ddlAccount.SelectedValue) : 0);

            pnlPortfolio.Visible = (AccountId != 0);

            if (pnlPortfolio.Visible)
            {
                showPortfolioGridView();
                showAccountDetails();
            }
        }
        catch (Exception ex)
        {
            pnlPortfolio.Visible = false;
            elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private void showPortfolioGridView()
    {
        ActivePortfolioGridView.PageIndex = 0;
        ActivePortfolioGridView.DataBind();
    }

    private void showAccountDetails()
    {
        AccountDetailsView accountDetailsView = PortfolioPositionsAdapter.GetAccountDetails(AccountId);

        lblAccountNumberWithName.Text = accountDetailsView.AccountNumberWithName;
        lblManagementStartDate.Text = formatDate(accountDetailsView.ManagementStartDate);

        lblAccountHoldersLabel.Text = string.Format("Rekeninghouder{0}:",
                                                    !string.IsNullOrEmpty(accountDetailsView.SecondaryAccountHolderName) ? "s" : "");
        pnlSecondaryAccountHolder.Visible = !string.IsNullOrEmpty(accountDetailsView.SecondaryAccountHolderName);
        lblPrimaryAccountHolder.Text = accountDetailsView.PrimaryAccountHolderName;
        lblSecondaryAccountHolder.Text = accountDetailsView.SecondaryAccountHolderName;

        lblStreetAddressLine.Text = accountDetailsView.StreetAddressLine;
        lblCityAddressLine.Text = accountDetailsView.CityAddressLine;
        pnlCountryAddressLine.Visible = !string.IsNullOrEmpty(accountDetailsView.CountryAddressLine);
        lblCountryAddressLine.Text = accountDetailsView.CountryAddressLine;

        pnlVerpandSoort.Visible = !string.IsNullOrEmpty(accountDetailsView.VerpandSoort);
        lblVerpandSoort.Text = accountDetailsView.VerpandSoort;
        pnlPandhouder.Visible = !string.IsNullOrEmpty(accountDetailsView.Pandhouder);
        lblPandhouder.Text = accountDetailsView.Pandhouder;

        pnlRemisier.Visible = (CurrentLoginType & LoginTypes.InternalEmployee) == LoginTypes.InternalEmployee &&
                              !string.IsNullOrEmpty(accountDetailsView.Remisier);
        lblRemisier.Text = accountDetailsView.Remisier;
        pnlRemisierEmployee.Visible = CurrentLoginType != LoginTypes.Customer &&
                                      !string.IsNullOrEmpty(accountDetailsView.RemisierEmployee);
        lblRemisierEmployee.Text = accountDetailsView.RemisierEmployee;
        
        lblModelName.Text = accountDetailsView.ModelName;
        
        pnlPositionValueDetails.Visible = (accountDetailsView.TotalCashQuantity != 0m);
        lblTotalCash.Text = accountDetailsView.TotalCash;
        lblTotalPositions.Text = accountDetailsView.TotalPositions;
        lblTotal.Text = accountDetailsView.TotalAll;
        
        lblLastRebalance.Text = formatDate(accountDetailsView.LastRebalanceDate);
        pnlCurrentRebalance.Visible = (accountDetailsView.CurrentRebalanceDate != DateTime.MinValue);
        lblCurrentRebalance.Text = formatDate(accountDetailsView.CurrentRebalanceDate);
    }

    private string formatDate(DateTime date)
    {
        return (Util.IsNullDate(date) ? string.Empty : date.ToString("dd-MM-yyyy"));
    }

    protected TreeViewNodeCollection TreeViewNodes
    {
        get { return ViewState["TreeViewNodes"] as TreeViewNodeCollection; }
        set { ViewState["TreeViewNodes"] = value; }
    }

    protected void odsPortfolioComponents_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        DataTable dataTable = ((DataSet)e.ReturnValue).Tables[0];

        if (TreeViewNodes == null)
        {
            TreeViewNodes = (new TreeViewNodeCollection(dataTable.Rows.Cast<DataRow>()
                                                                 .Select(row => new TreeViewNode((int)row["LineNumber"],
                                                                                                 (int)row["ParentLineNumber"]))))
                                .ExpandToDepth(1);
        }

        int[] invisibleNodeKeys = TreeViewNodes.Where(node => !node.Visible)
                                               .Select(node => node.NodeKey).ToArray();
        DataRow[] invisibleRows = dataTable.Rows.Cast<DataRow>()
                                           .Where(row => invisibleNodeKeys.Contains((int)row["LineNumber"])).ToArray();
        foreach (DataRow row in invisibleRows)
            dataTable.Rows.Remove(row);
    }

    protected void ibtExpandCollapse_Command(object sender, CommandEventArgs e)
    {
        int lineNumber = 0;
        if (int.TryParse((string)e.CommandArgument, out lineNumber) && lineNumber > 0)
        {
            TreeViewNodes.ToggleExpanded(lineNumber);
            gvPortfolioComponents.DataBind();
        }
    }
}
