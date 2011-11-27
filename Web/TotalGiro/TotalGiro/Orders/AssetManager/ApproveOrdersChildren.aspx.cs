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
using B4F.TotalGiro.ApplicationLayer.Orders.AssetManager;

public partial class ApproveOrdersChildren : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Approve Orders Children";
        }
        gvApproveOrdersChildren.DataBind();
    }

    protected void gvApproveOrdersChildren_OnRowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.ToString().ToUpper() == "CANCEL")
        {
            int orderid;
            if (e.CommandArgument != null && Int32.TryParse((string)e.CommandArgument, out orderid))
                ApproveOrdersChildrenAdapter.CancelOrder(orderid);
            gvApproveOrdersChildren.DataBind();
        }
    }
}
