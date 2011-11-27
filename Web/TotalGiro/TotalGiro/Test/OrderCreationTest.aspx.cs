using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.ApplicationLayer.Test;


public partial class OrderCreationTest : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			((EG)this.Master).setHeaderText = Page.Title;

		}
        if (!ddlAccounts.SelectedValue.Equals(""))
        {
            string strSearchBankAccount = ddlAccounts.SelectedItem.Text;
            string cashPositionString, openOrderCashString;

            OrderCreationTestAdapter.GetTotalCashAmount(OrderCreationTestAdapter.GetSelectedAccountID(strSearchBankAccount), out cashPositionString, out openOrderCashString);

            lblCashPos.Text = cashPositionString;
            lblOpenOrderCash.Text = openOrderCashString;

            if (ddlCurrency.SelectedItem == null)
                ddInstrument_SelectedIndexChanged(sender, e);
        }
	}

	protected void btnPlaceOrder_Click(object sender, EventArgs e)
	{
        try
        {
            if (ddlCurrency.SelectedItem == null)
                throw new ApplicationException("Select a currency first.");
            
            decimal amount = 0;
            decimal size = 0;

            if (rblSizeAmount.SelectedValue == "Amount")
                amount = Convert.ToDecimal(txtQuantity.Text);
            else
                size = Convert.ToDecimal(txtSize.Text);

            OrderCreationTestAdapter.PlaceOrder(OrderCreationTestAdapter.GetSelectedAccountID(ddlAccounts.SelectedItem.Text), Int32.Parse(ddInstrument.SelectedValue),
                ddlCurrency.SelectedItem.Text, rblSizeAmount.SelectedValue == "Amount", rblBuySell.SelectedValue == "Sell",
                rblInclExclCom.SelectedIndex == 0, amount, size);

            GridView1.DataBind();
            Response.Redirect("OrderCreationTest.aspx");
        }
        catch (Exception ex)
        {
            lblErrorPlaceOrder.Text = ex.Message;
        }
	}

	protected void ddInstrument_SelectedIndexChanged(object sender, EventArgs e)
	{
		string instrumentCurrency, baseCurrency;
        OrderCreationTestAdapter.GetCurrencies(OrderCreationTestAdapter.GetSelectedAccountID(ddlAccounts.SelectedItem.Text), Int32.Parse(ddInstrument.SelectedValue),
                                               out baseCurrency, out instrumentCurrency);

        ddlCurrency.Items.Clear();

        ListItem liBC = new ListItem(baseCurrency, baseCurrency);
        liBC.Selected = true;
        ddlCurrency.Items.Add(liBC);

        if (instrumentCurrency.CompareTo(baseCurrency) != 0)
            ddlCurrency.Items.Add(new ListItem(instrumentCurrency, instrumentCurrency));
	}

	protected void rblSizeAmount_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (rblSizeAmount.SelectedValue == "Size")
		{
			if (rblBuySell.SelectedValue == "Buy")
			{
				lblBuySellError.Visible = true;
				rblBuySell.SelectedValue = "Sell";
			}
			lblAmount.Enabled = false;
			lblSize.Enabled = true;
			txtSize.Enabled = true;
			txtQuantity.Enabled = false;
			ddlCurrency.Enabled = false;
            rblInclExclCom.Visible = false;
            lblInclExclComm.Visible = false;
		}
		else
		{
			lblAmount.Enabled = true;
			lblSize.Enabled = false;
			txtSize.Enabled = false;
			txtQuantity.Enabled = true;
			ddlCurrency.Enabled = true;
            rblInclExclCom.Visible = true;
            rblInclExclCom.SelectedIndex = 0;
            lblInclExclComm.Visible = true;
        }

	}
	protected void rblBuySell_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (rblBuySell.SelectedValue == "Buy" && rblSizeAmount.SelectedValue == "Size")
		{
			rblBuySell.SelectedValue = "Sell";
			lblBuySellError.Visible = true;
		}

	}

	protected void btnBuyMP_Click(object sender, EventArgs e)
	{
		try
		{
            //OrderCreationTestAdapter.BuyModelPortfolio(OrderCreationTestAdapter.GetSelectedAccountID(ddlAccounts.SelectedItem.Text));

			GridView1.DataBind();
		}
		catch (ApplicationException appex)
		{
			lblErrorMP.Text = appex.Message;
		}

	}
	protected void rblFundMP_SelectedIndexChanged(object sender, EventArgs e)
	{
        try
        {
            if (rblFundMP.SelectedValue == "Modelportfolio")
            {
                pnlFund.Visible = false;
                pnlModelPortfolio.Visible = true;

                lblModelportfolio.Text = OrderCreationTestAdapter.GetModelPortfolioDescription(OrderCreationTestAdapter.GetSelectedAccountID(ddlAccounts.SelectedItem.Text));
                lblErrorMP.Text = "";
            }
            else
            {
                pnlFund.Visible = true;
                pnlModelPortfolio.Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblErrorMP.Text = ex.Message;
        }
        
	}
	protected void ddlAccounts_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
            lblModelportfolio.Text = OrderCreationTestAdapter.GetModelPortfolioDescription(OrderCreationTestAdapter.GetSelectedAccountID(ddlAccounts.SelectedItem.Text));

			btnBuyMP.Enabled = true;
			lblErrorMP.Text = "";
		}
		catch (Exception)
		{
			lblModelportfolio.Text = "";
			btnBuyMP.Enabled = false;
			lblErrorMP.Text = "No modelportfolio found for this account.";
		}
	}
	protected void btnClearOrders_Click(object sender, EventArgs e)
	{
        if (!ddlAccounts.SelectedValue.Equals(""))
        {
            string accountNumber = ddlAccounts.SelectedItem.Text;

            SqlConnection sqlconn = new SqlConnection("Data Source=REAL;Initial Catalog=TotalGiroDev;User ID=sa");
            sqlconn.Open();

            SqlCommand sqlcmd = new SqlCommand("zzz_DeleteStuffForAccount", sqlconn);
            sqlcmd.CommandType = CommandType.StoredProcedure;

            SqlParameter myParam = new SqlParameter("number", SqlDbType.VarChar, 50);
            myParam.Value = accountNumber;
            sqlcmd.Parameters.Add(myParam);

            sqlcmd.ExecuteNonQuery();

            //SqlCommand sqlcmd;
            //sqlcmd = new SqlCommand("delete from PositionsTx", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from Positions", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from TransactionsCash", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from TransactionsNTM", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from TransactionsAllocation", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from TransactionsExecution", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from TransactionsOrder", sqlconn);
            //sqlcmd.ExecuteNonQuery();
            //sqlcmd = new SqlCommand("delete from Orders", sqlconn);
            //sqlcmd.ExecuteNonQuery();

            sqlconn.Close();
            GridView1.DataBind();
            Response.Redirect("OrderCreationTest.aspx");
        }
	}
}
