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

public partial class TestCommission : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        //if (Roles.IsUserInRole("Asset Manager"))
        //    Response.Redirect("RuleOverview.aspx");
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        //if (SecurityManager.IsCurrentUserInRole("Stichting Employee"))
            Response.Redirect("../Orders/ApproveOrders.aspx");
        //else
        //    lblMessage.Text = "Not authorized!";
    }
    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Security/ChangePassword.aspx");
    }
}
