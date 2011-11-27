using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using B4F.TotalGiro.BackOffice.Orders;
using System.Text;
using B4F.TotalGiro.ApplicationLayer.BackOffice;
using B4F.TotalGiro.Utils;

public partial class ApproveMoneyTransfers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            pnlErrorMess.Visible = false;
            lblMess.Text = "";
            Utility.SetDefaultButton(Page, dtpDateFrom, btnRefresh);
            Utility.SetDefaultButton(Page, dtpDateTo, btnRefresh);
            Utility.SetDefaultButton(Page, dbMinimumAmount, btnRefresh);
            Utility.SetDefaultButton(Page, dbMaximumAmount, btnRefresh);
            Utility.SetDefaultButton(Page, txtAccountNumber, btnRefresh);
            Utility.SetDefaultButton(Page, txtBeneficiary, btnRefresh);
            Utility.SetDefaultButton(Page, txtDescription, btnRefresh);
            
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Approve Money Transfer Orders";
                gvMoneyOrders.Sort("Reference", SortDirection.Descending);
            }
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            gvMoneyOrders.DataBind();

        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnResetFilter_Click(object sender, EventArgs e)
    {
        try
        {
            this.dtpDateFrom.Clear();
            this.dtpDateTo.Clear();
            this.dbMinimumAmount.Clear();
            this.dbMaximumAmount.Clear();
            this.txtAccountNumber.Text = "";
            this.txtBeneficiary.Text = "";
            this.txtDescription.Text = "";
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        try
        {
            if (MoneyTransferOrderOverviewAdapter.ApproveMoneyTransferOrders(gvMoneyOrders.GetSelectedIds()))
                gvMoneyOrders.DataBind();
        }
        catch (Exception ex)
        {
            pnlErrorMess.Visible = true;
            lblMess.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvMoneyOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((bool)((DataRowView)e.Row.DataItem)["IsEditable"])
            {
                e.Row.FindControl("lbtCancel").Visible = true;
            }

            int accountId = (int)((DataRowView)e.Row.DataItem)["TransfereeAccount_Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void gvMoneyOrders_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                GridView theGrid = (GridView)sender;
                theGrid.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int key = (int)(theGrid.SelectedValue);
                theGrid.SelectedIndex = -1;

                switch (((GridViewCommandEventArgs)e).CommandName.ToUpper())
                {
                    case "EDIT":
                        string qStr = QueryStringModule.Encrypt(string.Format("MoneyTransferOrderID={0}&Edit={1}", key, true));
                        Response.Redirect(string.Format("MoneyTransferOrder.aspx{0}", qStr));

                        break;
                    case "CANCEL":
                        if (MoneyTransferOrderOverviewAdapter.CancelMoneyTransferOrder(key))
                            gvMoneyOrders.DataBind();
                        break;
                }
            }
            ((GridView)sender).SelectedIndex = -1;
        }
    }
}
