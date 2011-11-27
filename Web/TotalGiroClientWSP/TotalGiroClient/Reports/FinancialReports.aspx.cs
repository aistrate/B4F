using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.Stichting.Login;

public partial class FinancialReports : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Rapportages";
            ((TotalGiroClient)Master).HelpUrl = "~/Help/FinancialReportsHelp.aspx";

            ctlPortfolioNavigationBar.Visible = CommonAdapter.GetCurrentLoginType() != LoginTypes.Customer;

            gvFinancialReportDocuments.Sort("CreationDate", SortDirection.Descending);
            
            ddlAccount.DataBind();
            gvFinancialReportDocuments.DataBind();
            ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);
        }
    }

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvFinancialReportDocuments.Caption = "Rapportages - " + ddlAccount.SelectedItem.Text;
    }
}
