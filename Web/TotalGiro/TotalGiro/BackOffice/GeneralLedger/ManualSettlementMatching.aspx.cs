using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;



public partial class ManualSettlementMatching : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Manual Settlement Matching";
            this.pnSelections.Visible = false;
            this.pnlMainSource.Visible = true;
            this.btnPlace.Text = "Place on Selection Grid";

            dpStartDate.SelectedDate = DateTime.Today.AddMonths(-3);
        }

        dpStartDate.SelectionChanged += new EventHandler(dpDate_SelectionChanged);
        dpEndDate.SelectionChanged += new EventHandler(dpDate_SelectionChanged);
    }

    protected void dpDate_SelectionChanged(object sender, EventArgs e)
    {
        try
        {
            gvNewBankSettlements.DataBind();
            gvUnsettledTrades.DataBind();
        }
        catch (Exception ex)
        {
            lblErrMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnPlace_OnClick(object sender, EventArgs e)
    {
        try
        {
            if ((this.btnPlace.Text == "Place on Selection Grid") && setupSelections())
            {
                this.btnPlace.Text = "Review Original Selections";
                this.btnCreateEntries.Visible = true;
                this.pnlMainSource.Visible = false;
                dpStartDate.Enabled = false;
                dpEndDate.Enabled = false;
            }
            else
            {
                this.btnPlace.Text = "Place on Selection Grid";
                this.btnCreateEntries.Visible = false;
                this.pnlMainSource.Visible = true;
                dpStartDate.Enabled = true;
                dpEndDate.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            lblErrMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateEntries_OnClick(object sender, EventArgs e)
    {
        try
        {
            int[] idsBankSettlements = gvNewBankSettlements.GetSelectedIds();
            int[] idsTradeSelection = gvUnsettledTrades.GetSelectedIds();
            ManualSettlementMatchingAdapter.CreateExternalSideSettlement(idsTradeSelection, idsBankSettlements);
            this.btnPlace.Text = "Place on Selection Grid";
            this.btnCreateEntries.Visible = false;
            this.pnlMainSource.Visible = true;
            this.pnSelections.Visible = false;
            DataBind();
        }
        catch (Exception ex)
        {
            lblErrMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private bool setupSelections()
    {
        bool anyValues = false;
        Money tradeBalance = null;
        Money bankStatementBalance = null;
        int[] idsBankSettlements = gvNewBankSettlements.GetSelectedIds();
        int[] idsTradeSelection = gvUnsettledTrades.GetSelectedIds();

        hdfSettlementDifference.Value = "0";
        if ((idsBankSettlements != null) && (idsBankSettlements.Count() > 0))
        {
            anyValues = true;
            DataSet ds = B4F.TotalGiro.ApplicationLayer.GeneralLedger.ManualSettlementMatchingAdapter.GetUnMappedBankStatements(idsBankSettlements);
            this.gvBankStatementBookingSelections.DataSource = ds;
            summarizeBankStatements(ds, out bankStatementBalance);
        }

        if ((idsTradeSelection != null) && (idsTradeSelection.Count() > 0))
        {
            anyValues = true;
            DataSet ds = B4F.TotalGiro.ApplicationLayer.GeneralLedger.ManualSettlementMatchingAdapter.GetUnsettledTrades(idsTradeSelection);
            this.gvTradeBookingSelections.DataSource = ds;
            summarizeTradeSelections(ds, out tradeBalance);
        }

        Money diff = bankStatementBalance - tradeBalance;
        if (diff != null && diff.IsNotZero)
            hdfSettlementDifference.Value = diff.Quantity.ToString();

        this.gvBankStatementBookingSelections.DataBind();
        this.gvTradeBookingSelections.DataBind();
        this.gvBankSummary.DataBind();
        this.gvTradeSummary.DataBind();


        this.pnSelections.Visible = anyValues;
        return anyValues;
    }

    private void summarizeBankStatements(DataSet input, out Money bankStatementBalance)
    {
        var summary = from row in input.Tables[0].AsEnumerable()
                      group new { Debit1 = row.Field<Money>("Debit"), Credit1 = row.Field<Money>("Credit") } by
                              new
                              {
                                  DebitCurrency = (ICurrency)row.Field<Money>("Debit").Underlying,
                                  CreditCurrency = (ICurrency)row.Field<Money>("Credit").Underlying
                              }
                          into g
                          let debits = from pair in g select pair.Debit1
                          let credits = from pair in g select pair.Credit1
                          let firstchoice = new
                          {
                              Debit = (debits != null ? debits.Sum() : null),
                              Credit = (credits != null ? credits.Sum() : null),
                          }
                          where (firstchoice.Credit - firstchoice.Debit).IsNotZero
                          select firstchoice;
        bankStatementBalance = (from a in summary select a.Credit - a.Debit).FirstOrDefault();
        this.gvBankSummary.DataSource = summary;
    }

    private void summarizeTradeSelections(DataSet input, out Money tradeBalance)
    {
        var summary = from row in input.Tables[0].AsEnumerable()
                      group new { 
                          CounterValue = row.Field<Money>("CounterValue"), 
                          TxSide = row.Field<B4F.TotalGiro.Orders.Side>("TxSide"), 
                          Size1 = row.Field<Decimal>("TotalGiroTradeSizeQuantity") 
                      } by
                              new
                              {
                                  InstrumentName = row.Field<String>("TotalGiroInstrumentName"),
                                  CounterValueCurrency = (ICurrency)row.Field<Money>("CounterValue").Underlying,
                                  TxSide = row.Field<B4F.TotalGiro.Orders.Side>("TxSide")
                              }
                          into g
                          let debits = from pair in g where (int)pair.TxSide > 0 select pair.CounterValue
                          let credits = from pair in g where (int)pair.TxSide < 0 select pair.CounterValue
                          let Sizes = from pair in g select pair.Size1
                          let firstchoice = new
                          {
                              InstrumentName = g.Key.InstrumentName,
                              Debit = (debits != null && debits.Count() > 0 ? debits.Sum() : new Money(0M, g.Key.CounterValueCurrency)),
                              Credit = (credits != null && credits.Count() > 0 ? credits.Sum() : new Money(0M, g.Key.CounterValueCurrency)),
                              Size = Sizes.Sum()
                          }
                          select firstchoice;

        if (summary.Count() > 1)
        {
            var summary2 = summary.ToList();
            summary2.Add(new
                {
                    InstrumentName = "Total",
                    Debit = (from a in summary where a.Debit != null select a.Debit.BaseAmount).Sum(),
                    Credit = (from a in summary where a.Credit != null select a.Credit.BaseAmount).Sum(),
                    Size = 0M
                });
            tradeBalance = (from a in summary2 where a.InstrumentName == "Total" select a.Credit + a.Debit).FirstOrDefault();
            this.gvTradeSummary.DataSource = summary2;
        }
        else
        {
            tradeBalance = (from a in summary select a.Credit + a.Debit).FirstOrDefault();
            this.gvTradeSummary.DataSource = summary;
        }
    }
}
