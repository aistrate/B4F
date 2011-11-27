using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.Stichting.Login;

public partial class Notas : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((TotalGiroClient)Master).HeaderText = "Afschriften";
            ((TotalGiroClient)Master).HelpUrl = "~/Help/NotasHelp.aspx";

            ctlPortfolioNavigationBar.Visible = CommonAdapter.GetCurrentLoginType() != LoginTypes.Customer;

            gvNotaDocuments.Sort("FirstNotaNumber", SortDirection.Descending);
            
            ddlAccount.DataBind();
            gvNotaDocuments.DataBind();
            ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);
        }
    }

    protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvNotaDocuments.Caption = "Afschriften - " + ddlAccount.SelectedItem.Text;
    }
}
