using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.Security;
using B4F.Web.WebControls;

public partial class PositionTransactions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Position Transactions";

            //ActivePositionType = PositionTransactionsAdapter.GetPositionType(PositionId);

            showPositionDetails();
            ActiveGridView.Sort("TransactionDate", SortDirection.Descending);
        }

        ActiveGridView.MultipleSelection = CanStorno;
        btnStorno.Visible = CanStorno;
        
        lblErrorMessage.Text = "";
    }

    protected int PositionId
    {
        get
        {
            object i = Session["SelectedPositionId"];
            return ((i == null) ? 0 : (int)i);
        }
        set { Session["SelectedPositionId"] = value; }
    }

    protected bool CanStorno
    {
        get { return ActivePositionType != PositionType.CashBaseCurrency && SecurityManager.IsCurrentUserInRole("Portfolio: Storno Ability"); }
    }

    protected PositionType ActivePositionType
    {
        get { return (PositionType)mvwGridViews.ActiveViewIndex; }
        set { mvwGridViews.ActiveViewIndex = (int)value; }
    }

    protected MultipleSelectionGridView ActiveGridView
    {
        get
        {
            switch (ActivePositionType)
            {
                case PositionType.Security:
                    return gvPositionTxsSecurity;
                //case PositionType.CashBaseCurrency:
                //    return gvPositionTxsCashBaseCurrency;
                //case PositionType.CashForeignCurrency:
                //    return gvPositionTxsCashForeignCurrency;
                default:
                    return null;
            }
        }
    }

    private void showPositionDetails()
    {
        if (PositionId != 0)
        {
            string accountDescription, instrumentDescription, valueDisplayString;

            PositionTransactionsAdapter.GetPositionDetails(PositionId,
                out accountDescription, out instrumentDescription, out valueDisplayString);

            lblAccount.Text = accountDescription;
            lblInstrument.Text = instrumentDescription;
            lblValue.Text = valueDisplayString;
        }
    }

    protected void btnStorno_Click(object sender, EventArgs e)
    {
        try
        {
            int[] selectedPositionTxIds = ActiveGridView.GetSelectedIds();
            
            if (selectedPositionTxIds.Length > 0)
            {
                if (!EditMode)
                {
                    EditMode = true;
                    hdnPositionTxId.Value = selectedPositionTxIds[0].ToString();
                    txtReason.Text = "";
                }
                else
                {
                    Exception[] exceptions = PositionTransactionsAdapter.StornoTransactions(
                        PositionId, selectedPositionTxIds, int.Parse(ddlStornoAccount.SelectedValue), txtReason.Text);

                    if (selectedPositionTxIds.Length > exceptions.Length)
                    {
                        lblErrorMessage.Text = string.Format("<br/>{0} transactions have been successfully stornoed.<br/>",
                            selectedPositionTxIds.Length - exceptions.Length);

                        showPositionDetails();
                    }

                    if (exceptions.Length > 0)
                    {
                        lblErrorMessage.Text += string.Format("<br/>{0} transactions could not be stornoed:<br/>", exceptions.Length);

                        foreach (Exception ex in exceptions)
                            lblErrorMessage.Text += string.Format("<br/>{0}", Utility.GetCompleteExceptionMessage(ex));
                    }

                    EditMode = false;
                    ActiveGridView.ClearSelection();
                }
            }

            ActiveGridView.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        EditMode = false;
    }

    protected void gvPositionTx_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName.ToUpper())
        {
            case "DETAILS":
                Session["TradeId"] = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("~/Orders/Common/TradeDetails.aspx");
                break;
        }
    }

    private bool EditMode
    {
        get
        {
            object b = ViewState["EditMode"];
            return ((b == null) ? false : (bool)b);
        }
        set
        {
            ActiveGridView.Enabled = !value;
            pnlStornoFields.Visible = value;
            btnCancel.Visible = value;
            ViewState["EditMode"] = value;
        }
    }
}
