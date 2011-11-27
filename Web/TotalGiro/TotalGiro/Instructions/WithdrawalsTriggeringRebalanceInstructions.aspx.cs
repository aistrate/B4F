using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Instructions;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

public partial class WithdrawalsTriggeringRebalanceInstructions : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Cash Withdrawals Triggering Rebalance Instructions";
            gvAccounts.Sort("ActiveWithdrawalInstructions_FirstWithdrawalDate", SortDirection.Ascending);
            dtpExecDate.SelectedDate = DateTime.Today;
            ViewState["EditMode"] = false;
        }

        if (pnlRebalanceInstructionProps.Visible)
        {
            //dtpExecDate.SelectionChanged += new EventHandler(dtpExecDate_SelectionChanged);

            dtpExecDate.SelectionChanged += (s, ev) =>
                {
                    rfvExecDate.Validate();
                    rvExecDate.Validate();
                };
        }

        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
    }

    //protected void dtpExecDate_SelectionChanged(object sender, EventArgs e)
    //{
    //    rfvExecDate.Validate();
    //    rvExecDate.Validate();
    //}

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        gvAccounts.Visible = true;
        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
        btnCreateRebalanceInstructions.Visible = true;
        EditMode = false;
        btnHideInstructions_Click(null, null);
    }

    protected void gvAccounts_OnRowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            lblError.Text = string.Empty;
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvAccounts.EditIndex = -1;
                    gvAccounts.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWINSTRUCTIONS":
                            gvInstructions.DataBind();
                            gvInstructions.Visible = true;
                            btnHideInstructions.Visible = true;
                            return;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
            gvAccounts.SelectedIndex = -1;
        }
    }

    protected void btnCreateRebalanceInstructions_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            int[] accountIds = gvAccounts.GetSelectedIds();

            if (accountIds.Length > 0)
            {
                if (!EditMode)
                {
                    EditMode = true;
                    chkNoCharges.Checked = true;
                    dtpExecDate.SelectedDate = DateTime.Today;
                }
                else
                {
                    BatchExecutionResults results = new BatchExecutionResults();
                    InstructionEntryAdapter.CreateRebalanceInstructions(results, accountIds, OrderActionTypes.Rebalance, chkNoCharges.Checked, dtpExecDate.SelectedDate, null);
                    lblError.Text = InstructionEntryAdapter.FormatErrorsForCreateInstructions(results, "rebalance");

                    EditMode = false;
                    gvAccounts.ClearSelection();
                }
            }

            gvAccounts.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        EditMode = false;
    }

    protected void btnHideInstructions_Click(object sender, EventArgs e)
    {
        try
        {
            gvInstructions.Visible = false;
            gvInstructions.SelectedIndex = -1;
            btnHideInstructions.Visible = false;
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    private bool EditMode
    {
        get { return (bool)ViewState["EditMode"]; }
        set
        {
            gvAccounts.Enabled = !value;
            pnlRebalanceInstructionProps.Visible = value;
            rvExecDate.MinimumValue = DateTime.Today.ToShortDateString();
            btnCancel.Visible = value;
            if (value)
                btnHideInstructions_Click(null, null);

            ViewState["EditMode"] = value;
        }
    }
}
