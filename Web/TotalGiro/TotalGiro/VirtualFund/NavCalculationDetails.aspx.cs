using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ApplicationLayer.VirtualFunds;
using System.Data;

public partial class NavCalculationDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Nav Calculation Details";

            CalculationID = (Session["calculationID"] != null ? (int)Session["calculationID"] : 0);
            hdnCalculationId.Value = CalculationID.ToString();
            loadFundDetails();
            this.ctlJournalEntryLines.VirtualFundID = 3000;
        }
    }

    private void loadFundDetails()
    {
        B4F.TotalGiro.ApplicationLayer.VirtualFunds.NavCalculationDetailsAdapter.CalculationDetails calcDetails = B4F.TotalGiro.ApplicationLayer.VirtualFunds.NavCalculationDetailsAdapter.GetCalculationDetails(CalculationID);
        if (calcDetails != null)
        {
            this.txtFundName.Text = calcDetails.Fund.FundName;
            this.txtHoldingsAccount.Text = calcDetails.Fund.HoldingsAccountName;
            this.txtTradingAccount.Text = calcDetails.Fund.TradingAccountName;
            this.txtValuationDate.Text = calcDetails.ValuationDate.ToLongDateString();
            this.txtStatus.Text = calcDetails.DisplayStatus;
            this.txtParticipationsBefore.Text = calcDetails.ParticipationsBefore.ToString("#0.000000");
            this.txtParticipationsNow.Text = calcDetails.ParticipationsDuring.ToString("#0.000000");
            this.txtParticipationsAfter.Text = calcDetails.ParticipationsAfter.ToString("#0.000000");
            this.txtGrossAssetValue.Text = calcDetails.GrossAssetValueDisplay;
            this.txtNettAssetValue.Text = calcDetails.NettAssetValueDisplay;
            this.txtNAVperUnit.Text = calcDetails.NavPerUnitDisplay;
            this.txtPublicOfferPrice.Text = calcDetails.PublicOfferPriceDisplay;

        }
        JournalEntryId = calcDetails.JournalEntryKey;
        ctlJournalEntryLines.JournalEntryId = JournalEntryId;
    }

    private void displayBookingDetails()
    {
        //DataSet ds = B4F.TotalGiro.ApplicationLayer.GeneralLedger.MemorialBookingLinesAdapter.GetMemorialBookingDetails(JournalEntryId);
        //if (ds.Tables[0].Rows.Count > 0)
        //{
        //    DataRow dataRow = ds.Tables[0].Rows[0];

        //    JournalManagementCompanyId = (int)dataRow["Journal_ManagementCompany_Key"];
        //    ctlJournalEntryLines.JournalManagementCompanyId = JournalManagementCompanyId;

        //    ctlJournalEntryLines.DefaultCurrencyId = 600;

        //    lblJournalEntryNumber.Text = (string)dataRow["JournalEntryNumber"];
        //    hdnJournalEntryId.Value = ((int)dataRow["Key"]).ToString();
        //    lblStatus.Text = (string)dataRow["DisplayStatus"];
        //    lblJournal.Text = (string)dataRow["Journal_FullDescription"];

        //    DateTime transactionDate = (DateTime)dataRow["TransactionDate"];
        //    lblTransactionDate.Text = (transactionDate != DateTime.MinValue ? transactionDate.ToString("dd-MM-yyyy") : "");

        //    lblDescription.Text = (dataRow["Description"] != DBNull.Value ? (string)dataRow["Description"] : "");

        //    JournalEntryStati status = (JournalEntryStati)dataRow["Status"];
        //    btnEditBooking.Enabled = (status == JournalEntryStati.New);
        //    btnBook.Enabled = (status != JournalEntryStati.Booked);
        //}
        //else
        //    lblErrorMessageMain.Text = string.Format("No Memorial Booking with ID '{0}' was found.", JournalEntryId);

        //ctlJournalEntryLines.Visible = (ds.Tables[0].Rows.Count > 0);
    }

    protected int CalculationID
    {
        get
        {
            object i = ViewState["CalculationID"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["CalculationID"] = value; }
    }

    protected void btnImportOrders_Click(object sender, EventArgs e)
    {
        NavCalculationDetailsAdapter.AddOrdersToNavCalculation(this.CalculationID);
        loadFundDetails();
    }

    protected void btnBookNav_Click(object sender, EventArgs e)
    {
        NavCalculationDetailsAdapter.BookNavCalculation(this.CalculationID);
        loadFundDetails();
    }

    protected int JournalEntryId
    {
        get
        {
            object i = ViewState["JournalEntryId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { ViewState["JournalEntryId"] = value; }
    }

    protected void btnNewLine_Click(object sender, EventArgs e)
    {
        try
        {
            ctlJournalEntryLines.InsertLine(0);
        }
        catch (Exception ex)
        {
            lblErrorMessageLedger.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
