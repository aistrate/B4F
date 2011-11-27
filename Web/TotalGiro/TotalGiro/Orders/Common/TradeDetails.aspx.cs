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

public partial class Orders_Common_TradeDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Trade Details";
        }
    }

    protected void btnAuditLogDetails_Click(object sender, EventArgs e)
    {
        Session["EntityClass"] = dvTrade.DataKey[0];
        Session["EntityKey"] = dvTrade.DataKey[1];

        Response.Redirect("~/Auditing/AuditLogDetails.aspx");
    }

    protected void dvTrade_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        int orderid;
        if (int.TryParse(e.CommandArgument.ToString(), out orderid))
        {
            Session["OrderId"] = orderid;
            Response.Redirect("~/Orders/Common/OrderDetails.aspx");
        }
    }
    protected void dvTrade_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (dvTrade.DataItemCount > 0)
            {
                bool isBond = (bool)((System.Data.DataRowView)(dvTrade.DataItem)).Row["IsBond"];
                dvTrade.Rows[12].Visible = isBond;
            }
        }
        catch (Exception)
        {
        }
    }
}
