using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Instruments.Nav;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.ApplicationLayer.TGTransactions;
using System.Data;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.ApplicationLayer.Portfolio;


public partial class VirtualFundTestPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnTest_Click(object sender, EventArgs e)
    {
        IDalSession session = NHSessionFactory.CreateSession();
        int accountid = int.Parse(this.txtAccountID.Text);

        IAccountTypeInternal testacc = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountid);
        if (testacc != null)
        {
            this.txtSettledCashPosition.Text = testacc.Portfolio.PortfolioCashGL.SettledCashTotalInBaseValue.ToString();
            this.txtUnSettledCashPosition.Text = testacc.Portfolio.PortfolioCashGL.UnSettledCashTotalInBaseValue.ToString();
        }

        session.Close();

    }

    protected void btnTestPositions_Click(object sender, EventArgs e)
    {
        IDalSession session = NHSessionFactory.CreateSession();
        int journalentrylineid = int.Parse(this.txtJournalEntryLineID.Text);

        IJournalEntryLine line = (IJournalEntryLine)JournalEntryMapper.GetJournalEntryLine(session, journalentrylineid);
        if (line != null)
        {
            line.BookLine();
            JournalEntryMapper.Update(session, line.Parent);
        }

        session.Close();

    }

    protected void btnMigrate_Click(object sender, EventArgs e)
    {
        //txtMessage.Text = "working";
        //if (ManagementFeeMigrationAdapter.MigrateOldManagementFees())
        //    txtMessage.Text = "Klaar";

    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        //txtMessage.Text = "working";
        //int tradeID = int.Parse(this.txtApproveID.Text);
        //if (B4F.TotalGiro.ApplicationLayer.TGTransactions.TransactionAdapter.ApproveExecution(tradeID)) ;
        //txtMessage.Text = "Klaar";

    }

    protected void btnClientSettle_Click(object sender, EventArgs e)
    {
        //txtMessage.Text = "working";
        //int tradeID = int.Parse(this.txtApproveID.Text);
        //if (B4F.TotalGiro.ApplicationLayer.TGTransactions.TransactionAdapter.ClientSettleExecution(tradeID)) ;
        //txtMessage.Text = "Klaar";

    }


    protected void btnDisplayAllocations_Click(object sender, EventArgs e)
    {
        txtMessage.Text = "working";
        int tradeID = int.Parse(this.txtExecutionID.Text);
        IDalSession session = NHSessionFactory.CreateSession();
        IOrderExecution exec = (IOrderExecution)TransactionMapper.GetTransaction(session, tradeID);
        this.txtExecutionValue.Text = exec.ValueSize.DisplayString;
        this.txtAllocationValue.Text = exec.Allocations.TotalAllocations.DisplayString;
        session.Close();

    }
    
    protected void btnTestClientLines_Click(object sender, EventArgs e)
    {
        //txtMessage.Text = "working";
        //IDalSession session = NHSessionFactory.CreateSession();
        //IAccountTypeInternal acc = (IAccountTypeInternal) AccountMapper.GetAccount(session, 622);

        //IGLSubPosition supPos = acc.PortfolioCashGL.GetSubPosition(acc.BaseCurrency, GLPositionSettleStatus.UnSettled);
        //CashDetailLineMapper.GetCashDetailLines(session, supPos);

        //session.Close();

    }


    protected void btnShowGridView_Click(object sender, EventArgs e)
    {
        this.gvCashLines.Visible = true;

    }

    //protected void gvBuyingPower_RowDataBound(object sender, GridViewRowEventArgs e)
    //{

    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        DataRowView dataRowView = (DataRowView)e.Row.DataItem;
    //        if ((bool)dataRowView["IsSubTotalLine"])
    //            e.Row.BackColor = System.Drawing.Color.Silver;
    //        else
    //            if ((bool)dataRowView["IsSummaryLine"])
    //                e.Row.BackColor = System.Drawing.Color.FromArgb(0x99CCFF);

    //    }


    //}





}
