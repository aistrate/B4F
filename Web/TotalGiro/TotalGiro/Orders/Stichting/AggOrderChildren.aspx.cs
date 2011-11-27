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
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;

public partial class AggOrderChildren : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        if (!IsPostBack)
		{
			((EG)this.Master).setHeaderText = "Aggregate Child Orders";
		} 
		gvAggregatedChildOrders.DataBind();
	}

    protected void gvAggregatedChildOrders_RowCommand(Object sender, GridViewCommandEventArgs e)
	{
        try
        {
            lblError.Text = "";
            int rowIndex = Int32.Parse((String)e.CommandArgument);
            int orderid = (int)gvAggregatedChildOrders.DataKeys[rowIndex].Value;

            AggOrderChildrenAdapter.DeleteOrder(orderid);

            gvAggregatedChildOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        }
	}
}
