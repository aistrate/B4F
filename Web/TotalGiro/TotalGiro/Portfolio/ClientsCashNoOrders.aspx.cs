using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ClientsCashNoOrders : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((EG)this.Master).setHeaderText = "Client Portfolio's with Cash , yet No Orders";
    }

    protected void gvPositions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }
}
