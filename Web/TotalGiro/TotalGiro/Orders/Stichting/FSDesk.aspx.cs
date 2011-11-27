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
using System.Globalization;
using System.Text;
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.UC;

public partial class FSDesk : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "FundSettle Desk";
            gvRoutedOrders.Sort("CreationDate", SortDirection.Descending);
            gvRoutedOrders.SelectedIndex = -1;
        }
        
        if (ctlOrderFill.Visible)
        {
            ctlOrderFill.TransactionDateChanged += new OrderFillEventHandler(ctlOrderFill_TransactionDateChanged);
            ctlOrderFill.PriceChanged += new OrderFillEventHandler(ctlOrderFill_PriceChanged);
            ctlOrderFill.Filled += new EventHandler(ctlOrderFill_Filled);
            ctlOrderFill.Cancelled += new EventHandler(ctlOrderFill_Cancelled);
        }

        ctlOrderFill.IsCounterpartyVisible = false;

        lblErrorMessage.Text = "";
    }

	protected void gvRoutedOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) 
		{
			switch ((OrderStati)((DataRowView)e.Row.DataItem)["Status"])
            {
				case OrderStati.Routed:
                    e.Row.FindControl("lbtCancelOrder").Visible = true;
                    e.Row.FindControl("lbtReset").Visible = true;
                    break;
                case OrderStati.Placed:
                    e.Row.FindControl("lbtFillOrder").Visible = true;
                    e.Row.FindControl("lbtReset").Visible = true;
                    e.Row.FindControl("lbtCancelOrder").Visible = true;
                    break;
                case OrderStati.PartFilled:
                    e.Row.FindControl("lbtFillOrder").Visible = true;
                    e.Row.FindControl("lbtViewTransaction").Visible = true;
                    break;
                case OrderStati.New:
                    e.Row.FindControl("lbtEditOrder").Visible = true;
                    e.Row.FindControl("lbtCancelOrder").Visible = true;
					break;
				case OrderStati.Terminated:
                    break;
				default:
					e.Row.FindControl("lbtViewTransaction").Visible = true;
					break;
            }
		}
	}

	protected void gvRoutedOrders_RowCommand(Object sender, GridViewCommandEventArgs e)
	{
        gvTransactions.Visible = false;

        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvRoutedOrders.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int orderId = (int)gvRoutedOrders.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "CANCELORDER":
                        FSDeskAdapter.CancelOrder(orderId);
                        gvRoutedOrders.DataBind();
                        break;
                    case "FILLORDER":
                        ctlOrderFill.DataBind();
                        if (ctlOrderFill.DataItemCount > 0)
                            setFillMode(true);
                        return;
                    case "VIEWTRANSACTION":
                        gvTransactions.DataBind();
                        gvTransactions.Visible = true;
                        return;
                    case "EDITORDER":
                        dvOrderEdit.DataBind();
                        if (dvOrderEdit.DataItemCount > 0)
                            setEditMode(true);
                        return;
                    case "RESETORDER":
                        FSDeskAdapter.ResetOrder(orderId);
                        gvRoutedOrders.DataBind();
                        break;
                }
            }
        }
        gvRoutedOrders.SelectedIndex = -1;
	}

    //Unapprove
    protected void btnUnapprove_Click(object sender, EventArgs e)
    {
        try
        {
            int[] selectedOrderIds = gvRoutedOrders.GetSelectedIds();

            if (selectedOrderIds.Length > 0)
            {
                FSDeskAdapter.UnApproveOrders(selectedOrderIds);
                gvRoutedOrders.DataBind();
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = "Error unapproving order: " + Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void ctlOrderFill_TransactionDateChanged(object sender, OrderFillEventArgs e)
    {
        e.OrderFillView.SettlementDate = FSDeskAdapter.GetSettlementDate(e.OrderFillView.TransactionDate, e.OrderFillView.OrderId);
    }

    protected void ctlOrderFill_PriceChanged(object sender, OrderFillEventArgs e)
    {
        ManualDeskAdapter.PriceChanged(e.OrderFillView);
    }

    protected void ctlOrderFill_Filled(object sender, EventArgs e)
    {
        setFillMode(false);
        gvRoutedOrders.DataBind();
    }

    protected void ctlOrderFill_Cancelled(object sender, EventArgs e)
    {
        setFillMode(false);
    }

    protected void dvOrderEdit_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {

        if (e.CommandName.ToUpper() == "DVCANCELEDIT")
        {
            dvOrderEdit.Visible = false;
        }
        else if (e.CommandName.ToUpper() == "DVSAVEORDER")
        {
            Label lblOrderID = (Label)dvOrderEdit.FindControl("OrderID");
            int orderID = Int32.Parse(lblOrderID.Text);

            DropDownList ddlRoute = (DropDownList)dvOrderEdit.FindControl("ddlRoutes");
            int routeid = short.Parse(ddlRoute.SelectedValue);

            FSDeskAdapter.SaveOrderInfo(orderID, routeid);
            gvRoutedOrders.DataBind();

            dvOrderEdit.Visible = false;
        }
    }

	// Send action collects the checked orders and fills an export file
	// for Fund Settle with the order information.
    protected void btnCreateFundSettleFile_Click(object sender, EventArgs e)
	{
		try
		{
			int[] orderIds = gvRoutedOrders.GetSelectedIds();

            if (orderIds == null || orderIds.Length == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            if (orderIds.Length > 150)
                throw new ApplicationException("Too many orders selected. Fund Settle only allows a maximum of 150 orders.");
            if (orderIds.Length > 0)
			{
				string errorMessage;
                FSDeskAdapter.SendOrders(orderIds, out errorMessage);

                lblErrorMessage.Text = errorMessage;

                gvRoutedOrders.ClearSelection();
				gvRoutedOrders.DataBind();
			}
		}
		catch (Exception ex)
		{
            lblErrorMessage.Text = "Error creating export file: " + Utility.GetCompleteExceptionMessage(ex);
		}
	}

	// Go to the Fund Settle Export files overview.
    protected void btnFSExports_Click(object sender, EventArgs e)
	{
		try
		{
            Server.Transfer("FSFileOverview.aspx");
        }
		catch (Exception ex)
		{
            lblErrorMessage.Text = "Error viewing export files: " + Utility.GetCompleteExceptionMessage(ex);
		}
	}

    private void setFillMode(bool fillMode)
    {
        enableMainControls(!fillMode);
        ctlOrderFill.Visible = fillMode;
        if (!fillMode)
            gvRoutedOrders.SelectedIndex = -1;
    }

    private void setEditMode(bool editMode)
    {
        enableMainControls(!editMode);
        dvOrderEdit.Visible = editMode;
        if (!editMode)
            gvRoutedOrders.SelectedIndex = -1;
    }

    private void enableMainControls(bool enabled)
    {
        ctlInstrumentFinder.Enabled = enabled;
        Utility.EnableGridView(gvRoutedOrders, enabled, gvRoutedOrders.Columns.Count - 1);
        btnCreateFundSettleFile.Enabled = enabled;
        btnFSExports.Enabled = enabled;
        btnUnapprove.Enabled = enabled;
    }
}
