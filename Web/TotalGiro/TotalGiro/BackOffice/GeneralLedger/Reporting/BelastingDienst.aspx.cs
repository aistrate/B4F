using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Communicator;
using B4F.TotalGiro.Communicator.BelastingDienst;

public partial class BelastingDienst : System.Web.UI.Page
{
    public enum VIEWPAGES
    {
        FileDetails = 0,
        Records,
        RecordDetails
        
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "BelastingDienst Reporting";
            setupPage();
        }

        lblErrorMessage.Text = "";

    }

    private IDividWepFile GetLastDividWepFileCreated()
    {
        return BelastingdienstAdapter.GetLastDividWepFileCreated();
    }

    private void setupPage()
    {
        IDividWepFile lastDividWepFileCreated = GetLastDividWepFileCreated();
        int lastFinancialYear = lastDividWepFileCreated.FinancialYear;
        this.txtLastFileCreated.Text = lastFinancialYear.ToString();
        this.txtNextFinancialYear.Text = (lastFinancialYear + 1).ToString();
        DateTime currentDate = DateTime.Now;
        this.txtCurrentDate.Text = currentDate.ToShortDateString();

        this.btnCreateDividWep.Enabled = currentDate.Year > lastFinancialYear + 1;

    }

    protected void btnCreateDividWep_Click(object sender, EventArgs e)
    {
        IDividWepFile lastDividWepFileCreated = GetLastDividWepFileCreated();
        BelastingdienstAdapter.CreateDividWepFileforYear(lastDividWepFileCreated.FinancialYear+1);

    }

    protected void gvDividWepFiles_OnRowCommand(Object sender, GridViewCommandEventArgs e)
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
                    gvDividWepFiles.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    //int orderId = (int)gvAggregatedOrders.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWFILE":
                            this.mlvFileView.ActiveViewIndex = (int) VIEWPAGES.FileDetails;
                            this.RecordView.Visible = true;
                            return;
                        case "VIEWFILERECORDS":
                            this.mlvFileView.ActiveViewIndex = (int)VIEWPAGES.Records;
                            return;
                    }
                }
            }

            gvDividWepFiles.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            //lblErrorAggregatedOrders.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }
}
