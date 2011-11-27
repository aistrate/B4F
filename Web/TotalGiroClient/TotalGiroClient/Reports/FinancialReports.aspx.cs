using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Client.Web.Reports
{
    public partial class FinancialReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Rapportages";
                ((TotalGiroClient)Master).HelpUrl = "~/Help/FinancialReportsHelp.aspx";

                ctlPortfolioNavigationBar.Visible = CommonAdapter.GetCurrentLoginType() != LoginTypes.Customer;

                ddlAccount.DataBind();

                if (AccountId != 0)
                    ddlAccount.SelectedValue = AccountId.ToString();

                ddlAccount_SelectedIndexChanged(ddlAccount, EventArgs.Empty);

                gvFinancialReportDocuments.Sort("CreationDate", SortDirection.Descending);
                gvFinancialReportDocuments.DataBind();
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
            gvFinancialReportDocuments.Caption = "Rapportages - " + ddlAccount.SelectedItem.Text;

            AccountId = int.Parse(ddlAccount.SelectedValue);
        }
    } 
}
