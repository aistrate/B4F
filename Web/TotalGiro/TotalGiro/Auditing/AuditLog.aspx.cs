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

public partial class Auditing_AuditLog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Audit Log";

            dpLastUpdatedFrom.SelectedDate = DateTime.Today;
            saveSearchFields();

            gvAuditLogEntities.Sort("LastUpdated", SortDirection.Descending);
        }
    }

    protected void gvAuditLogEntities_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["EntityClass"] = (string)gvAuditLogEntities.SelectedDataKey["EntityClass"];
        Session["EntityKey"] = (int)gvAuditLogEntities.SelectedDataKey["EntityKey"];
        
        Response.Redirect("AuditLogDetails.aspx");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        saveSearchFields();
        gvAuditLogEntities.Visible = true;
        gvAuditLogEntities.DataBind();
    }

    private void saveSearchFields()
    {
        Session["EntityType"] = txtEntityType.Text.Trim();
        
        Session["AuditEntityKey"] = null;
        if (txtKey.Text.Trim() != "")
            Session["AuditEntityKey"] = int.Parse(txtKey.Text.Trim());
        
        Session["CreatedFrom"] = dpCreatedFrom.SelectedDate;
        Session["CreatedTo"] = dpCreatedTo.SelectedDate;
        Session["CreatedBy"] = txtCreatedBy.Text.Trim();
        Session["LastUpdatedFrom"] = dpLastUpdatedFrom.SelectedDate;
        Session["LastUpdatedTo"] = dpLastUpdatedTo.SelectedDate;
        Session["LastUpdatedBy"] = txtLastUpdatedBy.Text.Trim();
    }
}
