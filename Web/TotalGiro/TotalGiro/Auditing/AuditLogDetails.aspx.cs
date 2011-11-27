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

public partial class Auditing_AuditLogDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Audit Log Details";

            lblEntityType.Text = (string)Session["EntityClass"];
            lblKey.Text = ((int)Session["EntityKey"]).ToString();

            gvAuditLogFields.Sort("FieldName", SortDirection.Ascending);
        }
    }

    protected void gvAuditLogEvents_SelectedIndexChanged(object sender, EventArgs e)
    {
        gvAuditLogFields.Visible = true;

        string eventName = gvAuditLogEvents.SelectedRow.Cells[0].Text.ToUpper();
        gvAuditLogFields.Columns[1].Visible = (eventName != "CREATE");
        gvAuditLogFields.Columns[2].Visible = (eventName != "DELETE");
    }
}
