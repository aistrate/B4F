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
using System.IO;
using B4F.TotalGiro.ApplicationLayer.Orders.AssetManager;

public partial class AggregateSend : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorAggregate.Text = "";
            lblErrorAggregatedOrders.Text = "";
            if (!IsPostBack)
		    {
                ((EG)this.Master).setHeaderText = "Aggregate/Send Orders";
                
                gvApprovedOrders.Sort("TradedInstrument_DisplayName", SortDirection.Ascending);
                gvAggregatedOrders.Sort("TradedInstrument_DisplayName", SortDirection.Ascending);
            }
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

	protected void gvAggregatedOrders_OnRowCommand(Object sender, GridViewCommandEventArgs e)
	{
        try
        {
		    if (e.CommandName.ToUpper() == "SELECT")
		    {
                int rowIndex = int.Parse((string)e.CommandArgument);
			    int orderid = (int)gvAggregatedOrders.DataKeys[rowIndex].Value;

			    Session["AggOrderEditID"] = orderid;
			    Response.Redirect("AggOrderChildren.aspx");
		    }
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

	//Aggregate
    protected void btnAggregate_Click(object sender, EventArgs e)
	{
        try
        {
            string errorMessage;

            AggregateSendAdapter.AggregateOrders(gvApprovedOrders.GetSelectedIds(), out errorMessage);

            lblErrorAggregate.Text = errorMessage;

            gvApprovedOrders.ClearSelection();
            gvApprovedOrders.DataBind();
            gvAggregatedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
	}

    //Aggregate Special
    protected void btnAggregateSpecial_Click(object sender, EventArgs e)
    {
        try
        {
            string errorMessage;

            AggregateSendAdapter.AggregateOrders(gvApprovedOrders.GetSelectedIds(), out errorMessage, true, gvAggregatedOrders.GetSelectedIds());

            lblErrorAggregate.Text = errorMessage;

            gvApprovedOrders.DataBind();
            gvAggregatedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    //UnApprove
    protected void btnUnApprove_Click(object sender, EventArgs e)
	{
        try
        {
            AggregateSendAdapter.UnApproveOrders(gvApprovedOrders.GetSelectedIds());

            gvApprovedOrders.ClearSelection();
            gvApprovedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    //DeAggregate
    protected void btnDeAggregate_Click(object sender, EventArgs e)
	{
        try
        {
            int[] selectedOrderIds = gvAggregatedOrders.GetSelectedIds();

            AggregateSendAdapter.DeAggregateOrders(selectedOrderIds);

            gvAggregatedOrders.ClearSelection();
            gvApprovedOrders.DataBind();
            gvAggregatedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregatedOrders.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

	//Send
	protected void btnSend_Click(object sender, EventArgs e)
	{
        try
        {
            int[] selectedOrderIds = gvAggregatedOrders.GetSelectedIds();

            AggregateSendAdapter.SendOrders(selectedOrderIds);

            gvAggregatedOrders.ClearSelection();
            gvAggregatedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregatedOrders.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            gvAggregatedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = string.Format("{0}<br /><br />", ex.Message);
        }

    }
}
