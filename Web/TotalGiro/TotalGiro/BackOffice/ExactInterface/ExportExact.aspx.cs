using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;

public partial class ExportExact : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Export to Exact";
            dpDateUntil.SelectedDate = DateTime.Today;
        }
        
        lblErrorMessage.Text = "";
    }

    protected void btnExportToExact_Click(object sender, EventArgs e)
    {
        try
        {
            dpDateUntil.IsExpanded = false;
            BatchExecutionResults results = new BatchExecutionResults();
            ExportExactAdapter.ExportToExact(results, dpDateUntil.SelectedDate);
            lblErrorMessage.Text = createResultsMessage(results);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "<br/ >" + Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvExportedFiles_OnRowCommand(Object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    //lblErrorAggregatedOrders.Text = string.Empty;
                    //gvAggregatedOrders.EditIndex = -1;

                    // Select row
                    gvExportedFiles.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    //int orderId = (int)gvAggregatedOrders.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWRECORDS":
                            this.gvLedgerEntriesinFile.Visible = true;
                            return;
                        case "UNDOFILE":

                            return;
                    }
                }
            }

            gvExportedFiles.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            //lblErrorAggregatedOrders.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }



    private string createResultsMessage(BatchExecutionResults results)
    {
        const int MAX_ERRORS_DISPLAYED = 25;

        string message = "<br/>";

        if (results.SuccessCount == 0 && results.ErrorCount == 0)
            message += "No ledger entries need to be exported up to the selected date.";
        else
        {
            if (results.SuccessCount > 0)
                message += string.Format("{0} ledger {1} exported successfully.<br/><br/><br/>",
                                         results.SuccessCount, 
                                         (results.SuccessCount != 1 ? "entries were" : "entry was"));

            if (results.ErrorCount > 0)
            {
                string tooManyErrorsMessage = (results.ErrorCount > MAX_ERRORS_DISPLAYED ?
                                                    string.Format(" (only the first {0} errors are shown)", MAX_ERRORS_DISPLAYED) : "");

                message += string.Format("{0} error{1} occured while exporting{2}:<br/><br/><br/>",
                                         results.ErrorCount, (results.ErrorCount != 1 ? "s" : ""), tooManyErrorsMessage);

                int errors = 0;
                foreach (Exception ex in results.Errors)
                {
                    if (++errors > MAX_ERRORS_DISPLAYED)
                        break;
                    message += Utility.GetCompleteExceptionMessage(ex) + "<br/>";
                }
            }
        }

        return message;
    }
}
