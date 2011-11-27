using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Portfolio;
using B4F.TotalGiro.Client.Web.Util;

namespace B4F.TotalGiro.Client.Web.Portfolio
{
    public partial class PositionTxsCash : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            elbErrorMessage.Text = "";

            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Verwerkte geldboekingen";
                ((TotalGiroClient)Master).HelpUrl = "~/Help/PortfolioHelp.aspx#cashMutations";

                gvCashMutations.Sort("TransactionDate", SortDirection.Descending);
                showAll();
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

        private void showAll()
        {
            try
            {
                gvCashMutations.Visible = (AccountId != 0);
                if (gvCashMutations.Visible)
                    showGridView();

                showPositionDetails();
            }
            catch (Exception ex)
            {
                gvCashMutations.Visible = false;
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }

        private void showPositionDetails()
        {
            lblAccount.Text = string.Empty;
            lblValue.Text = string.Empty;

            if (AccountId != 0)
            {
                string accountDescription, valueDisplayString;

                PositionTxsCashAdapter.GetCashPositionDetails(AccountId, out accountDescription, out valueDisplayString);

                lblAccount.Text = accountDescription;
                lblValue.Text = valueDisplayString;
            }
        }

        private void showGridView()
        {
            gvCashMutations.PageIndex = 0;
            gvCashMutations.DataBind();
        }
    }
}
