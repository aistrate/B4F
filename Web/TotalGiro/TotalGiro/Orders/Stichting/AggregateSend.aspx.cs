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
using B4F.TotalGiro.ApplicationLayer.Orders.Stichting;

public partial class AggregateSend : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
		{
			((EG)this.Master).setHeaderText = "Aggregate/Send Orders";

            gvApprovedOrders.Sort("TradedInstrument_DisplayName", SortDirection.Ascending);
            gvAggregatedOrders.Sort("TradedInstrument_DisplayName", SortDirection.Ascending);
        }

        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
        if (dvOrderEdit.Visible)
        {
            dbPrice.ValueChanged += new EventHandler(dbPrice_ValueChanged);
            dpTransactionDate.SelectionChanged += new EventHandler(dpTransactionDate_SelectionChanged);
        }
        else if (dvOrderFXConvert.Visible)
        {
            dbExRate.ValueChanged += new EventHandler(dbExRate_ValueChanged);
            dbConvertedAmount.ValueChanged += new EventHandler(dbConvertedAmount_ValueChanged);
        }
    }

	protected void gvAggregatedOrders_OnRowCommand(Object sender, GridViewCommandEventArgs e)
	{
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    lblErrorAggregatedOrders.Text = string.Empty;
                    gvAggregatedOrders.EditIndex = -1;

                    // Select row
                    gvAggregatedOrders.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int orderId = (int)gvAggregatedOrders.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWCHILDREN":
                            Session["StgAggOrderEditID"] = orderId;
                            Response.Redirect("AggOrderChildren.aspx");
                            break;
                        case "CONVERTORDER":
                            prepareForEdit(true);

                            mvwOrderEditting.Visible = true;
                            mvwOrderEditting.ActiveViewIndex = 0;
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                        case "CONVERTFX":
                            prepareForEdit(true);

                            mvwOrderEditting.Visible = true;
                            mvwOrderEditting.ActiveViewIndex = 1;
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                        case "DEAGGREGATE":
                            AggregateSendAdapter.DeAggregateOrder(orderId);
                            gvApprovedOrders.DataBind();
                            gvAggregatedOrders.DataBind();
                            break;
                    }
                }
            }

            gvAggregatedOrders.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            lblErrorAggregatedOrders.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
	}

    protected void dvOrderEdit_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (dvOrderEdit.DataItemCount > 0)
            {
                bool isBondOrder = ((OrderEditView)dvOrderEdit.DataItem).IsBondOrder;
                lblTransactionDate.Parent.Parent.Visible = isBondOrder;
                lblExpectedSettlementDate.Parent.Parent.Visible = isBondOrder;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void dpTransactionDate_SelectionChanged(object sender, EventArgs e)
    {
        try
        {
            DateTime date = AggregateSendAdapter.GetSettlementDate(Int32.Parse(
                lblEOOrderID.Text), 
                dpTransactionDate.SelectedDate);
            dpExpectedSettlementDate.SelectedDate = date;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }


	//Aggregate
    protected void btnAggregate_Click(object sender, EventArgs e)
	{
        try
        {
            string errorMessage;

            lblErrorAggregate.Text = "";
            AggregateSendAdapter.AggregateOrders(gvApprovedOrders.GetSelectedIds(), out errorMessage);

            lblErrorAggregate.Text = errorMessage;

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

            lblErrorAggregate.Text = "";
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
            lblErrorAggregate.Text = "";
            int[] selectedOrderIds = gvApprovedOrders.GetSelectedIds();

            if (AggregateSendAdapter.UnApproveOrders(selectedOrderIds))
                gvApprovedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregate.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    // Nett
    protected void btnNett_Click(object sender, EventArgs e)
    {
        try
        {
            string errorMessage;
            lblErrorAggregatedOrders.Text = "";
            int[] selectedOrdersIds = gvAggregatedOrders.GetSelectedIds();

            AggregateSendAdapter.NettOrders(selectedOrdersIds, out errorMessage);
            lblErrorAggregatedOrders.Text = errorMessage;

            gvAggregatedOrders.ClearSelection();
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
            lblErrorAggregatedOrders.Text = "";
            int[] selectedOrdersIds = gvAggregatedOrders.GetSelectedIds();

            try
            {
                AggregateSendAdapter.SendOrders(selectedOrdersIds);
            }
            catch (Exception ex)
            {
                lblErrorAggregatedOrders.Text = ex.Message;
            }
            gvAggregatedOrders.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorAggregatedOrders.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    protected void dvOrderEdit_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        try
        {
            switch (e.CommandName.ToUpper())
            {
                case "EOSAVEORDER":
                    int orderID = Int32.Parse(lblEOOrderID.Text);
                    decimal price = dbPrice.Value;
                    bool isBondOrder = Convert.ToBoolean(hdnIsBondOrder.Value);

                    if (isBondOrder)
                        AggregateSendAdapter.ConvertBondOrder(price, orderID, dpExpectedSettlementDate.SelectedDate);
                    else
                        AggregateSendAdapter.ConvertOrder(price, orderID);
                    gvAggregatedOrders.DataBind();
                    mvwOrderEditting.Visible = false;
                    prepareForEdit(false);
                    break;
                case "EOCANCELEDIT":
                    mvwOrderEditting.Visible = false;
                    prepareForEdit(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void dvOrderFXConvert_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.ToUpper() == "FXSAVEORDER")
            {
                int orderID = Int32.Parse(lblFXOrderID.Text);

                decimal exrate = dbExRate.Value;
                decimal amount = dbConvertedAmount.Value;

                AggregateSendAdapter.ConvertFx(exrate, amount, orderID);
                gvAggregatedOrders.DataBind();

            }
            mvwOrderEditting.Visible = false;
            prepareForEdit(false);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void dbPrice_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            bool enable = false;
            lblErrorMessage.Text = "";

            if (dbPrice.Text != string.Empty)
            {
                decimal price = dbPrice.Value;

                int orderID = Int32.Parse(lblEOOrderID.Text);
                bool isBondOrder = Boolean.Parse(hdnIsBondOrder.Value);

                OrderEditView oew = AggregateSendAdapter.CheckPrice(new OrderEditView(orderID, "", price, dbPrice.DecimalPlaces, isBondOrder, DateTime.MinValue));
                lblErrorMessage.Text = oew.Warning;
                enable = true;
            }
            lbtEOSaveOrder.Enabled = enable;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = (ex.InnerException != null ? ex.InnerException.Message : "Invalid input:" + ex.Message);
        }
    }

    protected void dbExRate_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            bool enable = false;
            lblErrorMessage.Text = "";

            if (dbExRate.Text != string.Empty && dbExRate.Value > 0)
            {
                decimal originalAmount = Convert.ToDecimal(hdfOriginalAmount.Value);
                decimal newAmount = Math.Round(originalAmount * dbExRate.Value, 2);
                dbConvertedAmount.Value = newAmount;
                enable = true;
            }
            lbtFxSaveOrder.Enabled = enable;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = (ex.InnerException != null ? ex.InnerException.Message : "Invalid input:" + ex.Message);
        }
    }

    protected void dbConvertedAmount_ValueChanged(object sender, EventArgs e)
    {
        try
        {
            bool enable = false;
            lblErrorMessage.Text = "";

            if (dbConvertedAmount.Text != string.Empty && dbConvertedAmount.Value > 0)
            {
                decimal originalAmount = Convert.ToDecimal(hdfOriginalAmount.Value);
                decimal newExRate = Math.Round(dbConvertedAmount.Value / originalAmount, 5);
                dbExRate.Value = newExRate;
                enable = true;
            }
            lbtFxSaveOrder.Enabled = enable;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = (ex.InnerException != null ? ex.InnerException.Message : "Invalid input:" + ex.Message);
        }
    }

    private void prepareForEdit(bool editIsBeginning)
    {
        lblErrorMessage.Text = "";
        if (!editIsBeginning)
            gvAggregatedOrders.SelectedIndex = -1;

        foreach (GridViewRow row in gvAggregatedOrders.Rows)
            if (row.RowType == DataControlRowType.DataRow)
                row.Cells[gvAggregatedOrders.Columns.Count - 1].Enabled = !editIsBeginning;

        gvAggregatedOrders.HeaderRow.Enabled = !editIsBeginning;
        gvAggregatedOrders.BottomPagerRow.Enabled = !editIsBeginning;
        btnAggregate.Enabled = !editIsBeginning;
        btnAggregateSpecial.Enabled = !editIsBeginning;
        btnUnApprove.Enabled = !editIsBeginning;
        btnNett.Enabled = !editIsBeginning;
        btnSend.Enabled = !editIsBeginning;
    }

    #region DetailsView Controls

    protected DecimalBox dbPrice { get { return (DecimalBox)Utility.FindControl(dvOrderEdit, "dbPrice"); } }
    protected DatePicker dpTransactionDate { get { return (DatePicker)Utility.FindControl(dvOrderEdit, "dpTransactionDate"); } }
    protected DatePicker dpExpectedSettlementDate { get { return (DatePicker)Utility.FindControl(dvOrderEdit, "dpExpectedSettlementDate"); } }
    protected Label lblTransactionDate { get { return (Label)Utility.FindControl(dvOrderEdit, "lblTransactionDate"); } }
    protected Label lblExpectedSettlementDate { get { return (Label)Utility.FindControl(dvOrderEdit, "lblExpectedSettlementDate"); } }
    protected Label lblEOOrderID { get { return (Label)Utility.FindControl(dvOrderEdit, "OrderID"); } }
    protected HiddenField hdnIsBondOrder { get { return (HiddenField)Utility.FindControl(dvOrderEdit, "hdnIsBondOrder"); } }
    protected LinkButton lbtEOSaveOrder { get { return (LinkButton)Utility.FindControl(dvOrderEdit, "lbtEOSaveOrder"); } }

    protected DecimalBox dbExRate { get { return (DecimalBox)Utility.FindControl(dvOrderFXConvert, "dbExRate"); } }
    protected DecimalBox dbConvertedAmount { get { return (DecimalBox)Utility.FindControl(dvOrderFXConvert, "dbConvertedAmount"); } }
    protected Label lblFXOrderID { get { return (Label)Utility.FindControl(dvOrderFXConvert, "OrderID"); } }
    protected HiddenField hdfOriginalAmount { get { return (HiddenField)Utility.FindControl(dvOrderFXConvert, "hdfOriginalAmount"); } }
    protected LinkButton lbtFxSaveOrder { get { return (LinkButton)Utility.FindControl(dvOrderFXConvert, "lbtFxSaveOrder"); } }

    #endregion
}
