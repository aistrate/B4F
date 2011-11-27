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
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

public partial class Orders_Stichting_OrderBook : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "OrderBook";
        }
    }

    protected string Abbreviation(string strTotal, int length)
    {
        string strRet = strTotal;
        if (strTotal.Length > length)
        {
            strRet = strTotal.Substring(0, length - 1);
        }
        return strRet;
    }
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        gvOrders.Visible = false;
        
        try
        {
            lblErrorMessage.Text = string.Empty;
            if (txtOrderID.Text == string.Empty && ddlSecCategory.SelectedIndex == 0 && txtAccountNr.Text == string.Empty && txtAccountName.Text == string.Empty && txtIsin.Text == string.Empty && txtInstrumentName.Text == string.Empty && cldDateFrom.IsEmpty && cldDateTo.IsEmpty)
                lblErrorMessage.Text = "Search criteria are mandatory";
            else
            {
                gvOrders.Visible = true;
                gvTransactions.Visible = false;
                gvOrders.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Util.GetMessageFromException(ex);
        }
    }

    protected void gvOrders_DataBound(object sender, EventArgs e)
    {
       lblLastRefreshed.Text = string.Format("{0:G}", (DateTime)Session["RefreshedTimeOrderBook"]);
       lblLastRefreshedLabel.Visible = true;
    }

    protected void gvOrders_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToUpper())
        {
            case "SELECT":
                gvTransactions.Visible = true;
                gvTransactions.DataBind();
                Utility.ScrollToBottom(hdnScrollToBottom);
                break;
            case "DETAILS":
                GridViewRow row = (GridViewRow)((WebControl)e.CommandSource).Parent.Parent;
                Session["OrderId"] = (int)gvOrders.DataKeys[row.RowIndex].Value;
                Response.Redirect("~/Orders/Common/OrderDetails.aspx");
                break;
        }
    }

    protected void gvOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                InstrumentSize filledValue = ((DataRowView)e.Row.DataItem)["FilledValue"] as InstrumentSize;
                if (filledValue == null)
                    e.Row.Controls[e.Row.Controls.Count - 1].Controls[0].Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Util.GetMessageFromException(ex);
        }
    }

    protected void btnResetFilter_Click(object sender, EventArgs e)
    {
        try
        {
            Server.Transfer("OrderBook.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Util.GetMessageFromException(ex);
        }
    }
}
