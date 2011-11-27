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

public partial class ManualDesk : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = "";
            Utility.EnableScrollToBottom(this, hdnScrollToBottom);
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Manual Desk";
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
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

	protected void gvRoutedOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) 
		{
            switch ((OrderStati)((DataRowView)e.Row.DataItem)["Status"])
            {
				case OrderStati.Routed:
                    e.Row.FindControl("lbtPlaceOrder").Visible = true;
                    e.Row.FindControl("lbtCancelOrder").Visible = true;
                    break;
                case OrderStati.Placed:
                    e.Row.FindControl("lbtFillOrder").Visible = true;
                    break;
                case OrderStati.PartFilled:
                    e.Row.FindControl("lbtFillOrder").Visible = true;
                    e.Row.FindControl("lbtViewTransaction").Visible = true;
                    break;
                case OrderStati.New:
                    e.Row.FindControl("lbtEditOrder").Visible = true;
                    e.Row.FindControl("lbtSendOrder").Visible = true;
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

	protected void gvRoutedOrders_RowCommand(object sender, GridViewCommandEventArgs e)
	{
        gvTransactions.Visible = false;
        btnHideTransactions.Visible = false;
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvRoutedOrders.EditIndex = -1;

                    // Select row
                    gvRoutedOrders.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int orderId = (int)gvRoutedOrders.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "CANCELORDER":
                            ManualDeskAdapter.CancelOrder(orderId);
                            gvRoutedOrders.DataBind();
                            break;
                        case "SENDORDER":
                            ManualDeskAdapter.SendOrder(orderId);
                            gvRoutedOrders.DataBind();
                            break;
                        case "PLACEORDER":
                            ManualDeskAdapter.PlaceOrder(orderId);
                            gvRoutedOrders.DataBind();
                            break;
                        case "FILLORDER":
                            ctlOrderFill.DataBind();
                            if (ctlOrderFill.DataItemCount > 0)
                                setFillMode(true);
                            return;
                        case "VIEWTRANSACTION":
                            gvTransactions.Visible = true;
                            gvTransactions.DataBind();
                            btnHideTransactions.Visible = true;
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                        case "EDITORDER":
                            dvOrderEdit.DataBind();
                            if (dvOrderEdit.DataItemCount > 0)
                                setEditMode(true);
                            return;
                    }
                }
            }

            gvRoutedOrders.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
            gvRoutedOrders.SelectedIndex = -1;
        }
	}

    protected void ctlOrderFill_TransactionDateChanged(object sender, OrderFillEventArgs e)
    {
        e.OrderFillView.SettlementDate =
            ManualDeskAdapter.GetSettlementDate(e.OrderFillView.TransactionDate, e.OrderFillView.OrderId, e.OrderFillView.ExchangeId, e.OrderFillView.CounterpartyAccountId);
    }

    protected void ctlOrderFill_PriceChanged(object sender, OrderFillEventArgs e)
    {
        try
        {
            ManualDeskAdapter.PriceChanged(e.OrderFillView);
            Utility.ScrollToBottom(hdnScrollToBottom);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
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
        try
        {
            if (e.CommandName.ToUpper() == "DVCANCELEDIT")
                setEditMode(false);
            else if (e.CommandName.ToUpper() == "DVSAVEORDER")
            {
                Label lblOrderID = (Label)dvOrderEdit.FindControl("OrderID");
                int orderID = Int32.Parse(lblOrderID.Text);

                DecimalBox dbNumberOfDecimals = (DecimalBox)dvOrderEdit.FindControl("dbNumberOfDecimals");
                short numberofdecimals = Convert.ToInt16(dbNumberOfDecimals.Value);

                DropDownList ddlRoute = (DropDownList)dvOrderEdit.FindControl("ddlRoutes");
                int routeid = short.Parse(ddlRoute.SelectedValue);

                ManualDeskAdapter.SaveOrderInfo(orderID, numberofdecimals, routeid);
                gvRoutedOrders.DataBind();

                setEditMode(false);
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    // Unapprove
    protected void btnUnapprove_Click(object sender, EventArgs e)
    {
        try
        {
            int[] selectedOrderIds = gvRoutedOrders.GetSelectedIds();
            ManualDeskAdapter.UnApproveOrders(selectedOrderIds);
            gvRoutedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnHideTransactions_Click(object sender, EventArgs e)
    {
        try
        {
            gvTransactions.Visible = false;
            btnHideTransactions.Visible = false;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


    private void setFillMode(bool fillMode)
    {
        enableMainControls(!fillMode);
        ctlOrderFill.Visible = fillMode;
        if (!fillMode)
            gvRoutedOrders.SelectedIndex = -1;
        else
            Utility.ScrollToBottom(hdnScrollToBottom);
    }

    private void setEditMode(bool editMode)
    {
        enableMainControls(!editMode);
        dvOrderEdit.Visible = editMode;
        if (!editMode)
            gvRoutedOrders.SelectedIndex = -1;
        else
            Utility.ScrollToBottom(hdnScrollToBottom);
    }

    private void enableMainControls(bool enabled)
    {
        ctlInstrumentFinder.Enabled = enabled;
        Utility.EnableGridView(gvRoutedOrders, enabled, gvRoutedOrders.Columns.Count - 1);
        btnUnapprove.Enabled = enabled;
    }
}
