using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.Web.WebControls;
using System.Data;
using B4F.TotalGiro.Utils;
using System.Globalization;

public partial class CashPositionTransactions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Cash Mutations";


            showPositionDetails();
            gvPositionTxsCashBaseCurrency.Sort("TransactionDate", SortDirection.Descending);
        }

        ctlDateFrom.DateChanged += new EventHandler(ctlDate_DateChanged);
        ctlDateTo.DateChanged += new EventHandler(ctlDate_DateChanged);

        gvPositionTxsCashBaseCurrency.MultipleSelection = false;
        lblErrorMessage.Text = "";
    }

    protected int SubPositionId
    {
        get
        {
            object i = Session["SelectedSubPositionId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { Session["SelectedSubPositionId"] = value; }
    }

    protected PositionType ActivePositionType
    {
        get { return (PositionType)mvwGridViews.ActiveViewIndex; }
        set { mvwGridViews.ActiveViewIndex = (int)value; }
    }

    private void showPositionDetails()
    {
        if (SubPositionId != 0)
        {
            string accountDescription, instrumentDescription, valueDisplayString;

            CashPositionTransactionsAdapter.GetSubPositionDetails(SubPositionId,
                out accountDescription, out instrumentDescription, out valueDisplayString);

            lblAccount.Text = accountDescription;
            lblInstrument.Text = instrumentDescription;
            lblValue.Text = valueDisplayString;
        }
    }

    protected MultipleSelectionGridView ActiveGridView
    {
        get
        {
            switch (ActivePositionType)
            {
                //case PositionType.Security:
                //    return gvPositionTxsSecurity;
                case PositionType.CashBaseCurrency:
                    return gvPositionTxsCashBaseCurrency;
                //case PositionType.CashForeignCurrency:
                //    return gvPositionTxsCashForeignCurrency;
                default:
                    return null;
            }
        }
    }

    protected void gvPositionTx_Sorting(object sender, GridViewSortEventArgs e)
    {
        sortDirection = (SortDirection)Math.Abs((int)sortDirection - 1);

        string[] sortExprs = e.SortExpression.Split(' ');
        e.SortExpression = string.Format("{0} {1}",
            sortExprs[0],
            sortDirection == SortDirection.Ascending ? "ASC" : "DESC");
    }

    protected void gvPositionTx_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.ToUpper() == "DETAILS")
        {
            string reference = Convert.ToString(e.CommandArgument);
            switch (reference[0])
            {
                case 'T':
                    Session["TradeId"] = int.Parse(reference.Substring(1, reference.Length - 1));
                    Response.Redirect("~/Orders/Common/TradeDetails.aspx");
                    break;
                default:
                    Session["BookingId"] = int.Parse(reference.Substring(1, reference.Length - 1));
                    Response.Redirect("~/BackOffice/GeneralOperationsDetails.aspx");
                    break;
            }



            if (reference[0] == 'T')
            {
            }
            else
            {
                Session["BookingId"] = int.Parse(reference.Substring(1, reference.Length - 1));
                Response.Redirect("~/BackOffice/GeneralOperationsDetails.aspx");
            }
        }
    }

    protected void ctlDate_DateChanged(object sender, EventArgs e)
    {
        try
        {
            gvPositionTxsCashBaseCurrency.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            DataSet ds = CashPositionTransactionsAdapter.GetCashPositionTransactionsForExport(SubPositionId, ctlDateFrom.SelectedDate, ctlDateTo.SelectedDate);
            Utility.ExportToExcel(Response, ds);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }

    }

    private SortDirection sortDirection
    {
        get
        {
            object e = ViewState["SortDirection"];
            return ((e == null) ? SortDirection.Ascending : (SortDirection)e);
        }
        set { ViewState["SortDirection"] = value; }
    }
}
