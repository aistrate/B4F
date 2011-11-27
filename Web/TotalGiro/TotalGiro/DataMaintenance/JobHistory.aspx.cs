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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance;

public partial class DataMaintenance_JobHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dpDateFrom.SelectedDate = DateTime.Today.AddDays(-10).Date;
            dpDateTo.SelectedDate = DateTime.Today.Date;

            ((EG)this.Master).setHeaderText = "Job History Details";

            gvJobHistory.Sort("CreationDate", SortDirection.Descending);
            gvJobHistory.SelectedIndex = -1;
        }
    }

    protected void gvJobHistory_OnRowCommand(Object sender, GridViewCommandEventArgs e)
    {
        try 
	    {	        
    		lblErrorMessage.Text = "";
            lblDetails.Text = "";

            if (e.CommandName.ToString().ToUpper() == "SELECT")
            {
                int rowIndex = int.Parse((string)e.CommandArgument);
                int jobHistID = (int)gvJobHistory.DataKeys[rowIndex].Value;
                string details;
                if (JobHistoryAdapter.GetJobHistoryDetail(jobHistID, out details))
                    lblDetails.Text = details;
            }
	    }
	    catch (Exception ex)
	    {
    		lblErrorMessage.Text = Util.GetMessageFromException(ex);
	    }
    }
}
