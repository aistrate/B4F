using System;
using System.Data;
using System.Globalization;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class ImportFiles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Import third party files";

        }

        lblHistRatesUpdated.Text = "Historical Exchanges Rates updated till (tot en met): " +
            ImportFilesAdapter.GetMaxHistoricalExRateDate().ToShortDateString();
    }

    protected void btnImportExchangeRates_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorImportExchangeRates.Text = "";
            ImportFilesAdapter.ImportExchangeRates(dtpImportExchangeRates.SelectedDate);

            Server.Transfer("ImportFiles.aspx");

        }
        catch (Exception ex)
        {
            lblErrorImportExchangeRates.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }
    protected void btnImportFundPrices_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorImportFundPrices.Text = "hit...";

            Server.Transfer("ImportFundPrices.aspx");

        }
        catch (Exception ex)
        {
            lblErrorImportFundPrices.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }

    protected void btnImportPrices_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorImportPrices.Text = "";
            ImportFilesAdapter.ImportHistoricalPrices();
        }
        catch (Exception ex)
        {
            lblErrorImportPrices.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
    }

    










    protected void btnImportTransactionReceipts_Click(object sender, EventArgs e)
    {
        ImportFilesAdapter.ImportPdfReceiptsFromBo();
    }
}
