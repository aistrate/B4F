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

public partial class Orders_Common_OrderDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Order Details";
        }
    }

    protected void btnAuditLogDetails_Click(object sender, EventArgs e)
    {
        Session["EntityClass"] = dvOrder.DataKey[0];
        Session["EntityKey"] = dvOrder.DataKey[1];

        Response.Redirect("~/Auditing/AuditLogDetails.aspx");
    }

    protected void dvOrder_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        int orderid;
        if (int.TryParse(e.CommandArgument.ToString(), out orderid))
        {
            Session["OrderId"] = orderid;
            Response.Redirect("~/Orders/Common/OrderDetails.aspx");
        }
    }
}
