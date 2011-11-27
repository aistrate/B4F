using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ClientApplicationLayer.Portfolio;
using B4F.TotalGiro.Client.Web.Util;

namespace B4F.TotalGiro.Client.Web.Portfolio
{
    public partial class PositionTxsSecurities : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TotalGiroClient)Master).HeaderText = "Positie mutaties";
                ((TotalGiroClient)Master).HelpUrl = "~/Help/PortfolioHelp.aspx#positionTxs";

                gvPositionTransactions.Sort("TransactionDate", SortDirection.Descending);
            }

            elbErrorMessage.Text = "";
        }

        protected int PositionId
        {
            get
            {
                object i = Session["PositionId"];
                return (i == null ? 0 : (int)i);
            }
            set { Session["PositionId"] = value; }
        }

        protected void ddlInstrument_DataBound(object sender, EventArgs e)
        {
            if (PositionId != 0 && ddlInstrument.Items.FindByValue(PositionId.ToString()) != null)
                ddlInstrument.SelectedValue = PositionId.ToString();
            else if (ddlInstrument.Items.Count > 0)
                ddlInstrument.SelectedIndex = 0;

            showAll();
        }

        protected void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            showAll();
        }

        private void showAll()
        {
            try
            {
                PositionId = (ddlInstrument.SelectedValue != string.Empty ? int.Parse(ddlInstrument.SelectedValue) : 0);

                gvPositionTransactions.Visible = (PositionId != 0);
                if (gvPositionTransactions.Visible)
                    showTransactionsGridView();

                showPositionDetails();
            }
            catch (Exception ex)
            {
                gvPositionTransactions.Visible = false;
                elbErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            }
        }

        private void showPositionDetails()
        {
            lblAccount.Text = string.Empty;
            lblValue.Text = string.Empty;

            if (PositionId != 0)
            {
                string accountDescription, valueDisplayString;

                PositionTxsSecuritiesAdapter.GetFundPositionDetails(PositionId, out accountDescription, out valueDisplayString);

                lblAccount.Text = accountDescription;
                lblValue.Text = valueDisplayString;
            }
        }

        private void showTransactionsGridView()
        {
            gvPositionTransactions.Caption = "Mutaties: " +
                ddlInstrument.SelectedItem.Text;
            gvPositionTransactions.PageIndex = 0;
            gvPositionTransactions.DataBind();
        }
    } 
}
