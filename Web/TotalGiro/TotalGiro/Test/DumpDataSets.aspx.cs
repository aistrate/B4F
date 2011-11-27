using System;
using System.Data;
using System.Xml;
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;
using B4F.TotalGiro.ApplicationLayer.Reports;
using B4F.TotalGiro.ApplicationLayer.Test;
using B4F.TotalGiro.Utils;

public partial class Test_DumpDataSets : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Dump DataSets as XML";
            txtDumpToFile.Text = @"C:\Temp\Schemas\NotaDataSet.xml";
        }

        lblErrorMessage.Text = "";
    }

    private void dumpDataSetToXmlFile(DataSet ds)
    {
        XmlTextWriter xtw = new XmlTextWriter(txtDumpToFile.Text, null);
        ds.WriteXml(xtw, XmlWriteMode.WriteSchema);
        xtw.Close();

        lblErrorMessage.Text = "Done!";
    }




    protected void btnAggStichtingOrders_Click(object sender, EventArgs e)
    {
        // this is just an example
        try
        {
            DataSet ds = AggregateSendAdapter.GetAggregatedStgOrders();
            ds.Tables[0].Columns.Remove("ParentOrder");
            ds.Tables[0].Columns.Remove("Side");
            ds.Tables[0].Columns.Remove("RequestedInstrument");
            ds.Tables[0].Columns.Remove("Value");

            dumpDataSetToXmlFile(ds);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "ERROR: " + ex.Message;
        }
    }

    protected void btnTestPositionTransferReport_Click (object sender, EventArgs e)
    {
            try
        {
            DataSet ds = OperationalReportAdapter.GetClientTransferReport(3, true);

            dumpDataSetToXmlFile(ds);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "ERROR: " + ex.Message;
        }
}

    protected void btnNota_Click(object sender, EventArgs e)
    {
        try
        {
            //int count = DumpDataSetsAdapter.CountDocumentsSentByPost(1707, new DateTime(2009, 1, 1), new DateTime(2009, 1, 31));
            //lblErrorMessage.Text = count.ToString();

            int notaId = int.Parse(txtAccountId.Text);
            dumpDataSetToXmlFile(PrintNotasAdapter.BuildTestDataSet(notaId));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "ERROR: " + Util.GetMessageFromException(ex);
        }
    }
    
    protected void btnLetter_Click(object sender, EventArgs e)
    {
        int personId = 592;     // 592, 2500, 3253;
        try
        {
            dumpDataSetToXmlFile(DumpDataSetsAdapter.GetLetterDataSet(personId));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "ERROR: " + ex.Message;
        }
    }

    protected void btnFinancialReport_Click(object sender, EventArgs e)
    {
       // Use a valid AccountID
        bool PortfolioOverview = true;
        bool PortfolioSpecs = true;
        bool MoneyMutations = true;
        bool TransactionOverview = true;
        int accountId = 0; 

        if (txtAccountId.Text == "")
        {
            accountId = Int32.Parse("395");
        }
        else
        {
            accountId = Convert.ToInt32(txtAccountId.Text);
        }

        DateTime BegindateTime = new DateTime(2010,04,01);
        DateTime EndDateTime = new DateTime(2010, 06, 30);
        txtDumpToFile.Text = @"C:\Temp\Schemas\FinancialReportDataSet.xml";

        try
        {
            dumpDataSetToXmlFile(FinancialReportAdapter.GetQuarterReportDataSet(PortfolioOverview, PortfolioSpecs, TransactionOverview, MoneyMutations, accountId, BegindateTime, EndDateTime));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "ERROR: " + ex.Message;
        }
    }
    protected void btnPrintBulkQ3Report_Click(object sender, EventArgs e)
    {

    }
    protected void btnFiscaalJaarOpgaaf_Click(object sender, EventArgs e)
    {
        int year = 2008;
        string txtBetreft;
        string txtOmschrijving;

        int currentManagementCompanyId = 10;
        int accountId = 0;

        txtBetreft = "Uw fiscaal jaaropgaaf " + year.ToString();
        txtOmschrijving = "Jaarlijks doen wij u een opgave van uw beleggingen die bij onze stichting staan geregistreerd. Deze opgave kan u ten dienste staan bij de opstelling van uw aangifte inkomstenbelasting. " +
"De vermelde gegevens komen overeen met de opgave die wij op grond van de wet rechtstreeks aan de belastingdienst doen toekomen.";


        if (txtAccountId.Text == "")
        {
            accountId = Int32.Parse("681");
        }
        else
        {
            accountId = Convert.ToInt32(txtAccountId.Text);
        }

        DateTime now = DateTime.Now.Date;
        DateTime BeginDateOfYear = new DateTime(2007, 12, 31);          //DateTime BeginDateOfYear;           // = new DateTime(now.Year, 1, 1);
        DateTime EndDateOfYear = new DateTime(2008, 12, 31);

        txtDumpToFile.Text = @"C:\Temp\Schemas\FiscalYearOverview.xml";

        try
        {
            dumpDataSetToXmlFile(FinancialReportAdapter.GetFiscalYearReportDataSet(accountId, BeginDateOfYear, EndDateOfYear, txtBetreft, txtOmschrijving, currentManagementCompanyId, year));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "Error: " + ex.Message;
        }
    }
    protected void btnTestByMLim_Click(object sender, EventArgs e)
    {
        int year = 2008;
        string report = "Q1";
        int currentManagementCompanyId = 10;

        txtDumpToFile.Text = @"C:\Temp\Schemas\ReportPrintedStatus.xml";
        dumpDataSetToXmlFile(ReportResultsAdapter.GetPrintedReportsDataSet(currentManagementCompanyId, year, report));
    }
    protected void btnTestByMJN_Click(object sender, EventArgs e)
    {
        dumpDataSetToXmlFile(B4F.TotalGiro.ApplicationLayer.Reports.AccountDetailsReportAdapter.getAccountDetails(31752258));

    }
}
