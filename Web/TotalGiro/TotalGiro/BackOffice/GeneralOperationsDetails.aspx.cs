using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GeneralOperationsDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "General Operations Booking Details";
        }
    }

    protected void btnAuditLogDetails_Click(object sender, EventArgs e)
    {
        Session["EntityClass"] = dvTrade.DataKey[0];
        Session["EntityKey"] = dvTrade.DataKey[1];

        Response.Redirect("~/Auditing/AuditLogDetails.aspx");
    }
}
