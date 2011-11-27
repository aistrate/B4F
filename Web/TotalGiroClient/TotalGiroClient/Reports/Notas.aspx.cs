using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Client.Web.Reports
{
    public partial class Notas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Afschriften";
                ((TotalGiroClient)Master).HelpUrl = "~/Help/NotasHelp.aspx";

                ctlPortfolioNavigationBar.Visible = CommonAdapter.GetCurrentLoginType() != LoginTypes.Customer;

                ddlAccount.DataBind();

                if (AccountId != 0)
                    ddlAccount.SelectedValue = AccountId.ToString();

                ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);

                gvNotaDocuments.Sort("FirstNotaNumber", SortDirection.Descending);
                gvNotaDocuments.DataBind();
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

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvNotaDocuments.Caption = "Afschriften - " + ddlAccount.SelectedItem.Text;

            AccountId = int.Parse(ddlAccount.SelectedValue);
        }
    } 
}
