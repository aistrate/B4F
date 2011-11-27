using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Instructions;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Utils;

public partial class InstructionEntry : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = "";
            Utility.EnableScrollToBottom(this, hdnScrollToBottom);
            if (!IsPostBack)
            {
                if (SecurityManager.IsCurrentUserInRole("Service Desk: Instruction Management"))
                    ((EG)this.Master).setHeaderText = "Ad-Hoc Withdrawal Instruction Entry";
                else
                    ((EG)this.Master).setHeaderText = "Instruction Entry";
                gvAccounts.Sort("ShortName", SortDirection.Ascending);
                gvInstructions.Sort("CreationDate", SortDirection.Descending);
                dtpExecDate.SelectedDate = DateTime.Today;
                dtpRelevanceDate.SelectedDate = DateTime.Today;
                ViewState["EditMode"] = false;
            }

            if (pnlRebalanceInstructionProps.Visible)
            {
                dtpExecDate.SelectionChanged += new EventHandler(dtpExecDate_SelectionChanged);
            }
            if (pnlWithdrawalInstruction.Visible)
            {
                dtpWithdrawalDate.SelectionChanged += new EventHandler(dtpWithdrawalDate_SelectionChanged);
                dtpRelevanceDate.SelectionChanged += new EventHandler(dtpRelevanceDate_SelectionChanged);
                dtpWithdrawalDate.Expanded += new EventHandler(dtpWithdrawalDates_Expanded);
                dtpRelevanceDate.Expanded += new EventHandler(dtpWithdrawalDates_Expanded);
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void dtpWithdrawalDates_Expanded(object sender, EventArgs e)
    {
        try
        {
            DatePicker picker = (DatePicker)sender;
            if (picker != null)
            {
                if (picker.IsExpanded)
                {
                    if (picker.ID == "dtpWithdrawalDate")
                        dtpRelevanceDate.IsExpanded = false;
                    else
                        dtpWithdrawalDate.IsExpanded = false;
                }
                ddlCounterAccount.Visible = !picker.IsExpanded;
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void gvAccounts_OnRowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.ToUpper() == "SELECT")
            {
                //int rowIndex = int.Parse((string)e.CommandArgument);
                //int accountId = (int)gvAccounts.DataKeys[rowIndex].Value;
                pnlInstructions.Visible = true;
            }
            else if (e.CommandName.ToUpper() == "SETCOUNTERACCOUNT")
            {
                int accountkey = Convert.ToInt32(e.CommandArgument);
                Session["accountnrid"] = accountkey;
                Response.Redirect("~/DataMaintenance/Accounts/Account.aspx");
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        try
        {
            gvAccounts.Visible = true;
            gvAccounts.DataBind();
            if (!SecurityManager.IsCurrentUserInRole("Service Desk: Instruction Management"))
            {
                btnRebalance.Visible = true;
                btnDeparture.Visible = true;
            }
            btnWithdrawal.Visible = true;
            EditMode = false;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void chkExclude_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            this.pnlSelector.Visible = chkExclude.Checked;
            if (!chkExclude.Checked)
                this.ucInstrumentsModelsSelector.Clear();
            btnCancel.Focus();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnRebalance_Click(object sender, EventArgs e)
    {
        try
        {
            hdfInstructionType.Value = "RB";
            int[] accountIds = gvAccounts.GetSelectedIds();

            if (accountIds.Length > 0)
            {
                if (!EditMode)
                {
                    EditMode = true;
                    chkNoCharges.Checked = true;
                    ddlOrderActionTypes.SelectedValue = "2";
                    dtpExecDate.SelectedDate = DateTime.Today;
                }
                else
                {
                    OrderActionTypes orderActionType = OrderActionTypes.Rebalance;
                    if (ddlOrderActionTypes.SelectedValue != string.Empty)
                        orderActionType = (OrderActionTypes)Convert.ToInt32(ddlOrderActionTypes.SelectedValue);

                    lblErrorMessage.Text = "";
                    BatchExecutionResults results = new BatchExecutionResults();
                    InstructionEntryAdapter.CreateRebalanceInstructions(results, accountIds, orderActionType, chkNoCharges.Checked, dtpExecDate.SelectedDate, ucInstrumentsModelsSelector.Exclusions);
                    lblErrorMessage.Text = InstructionEntryAdapter.FormatErrorsForCreateInstructions(results, "rebalance");

                    EditMode = false;
                    gvAccounts.ClearSelection();
                }
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            gvAccounts.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnWithdrawal_Click(object sender, EventArgs e)
    {
        try
        {
            hdfInstructionType.Value = "WD";
            int[] accountIds = gvAccounts.GetSelectedIds();
            Session["SelectedAccountIds"] = accountIds;

            if (accountIds.Length > 0)
            {
                if (!EditMode)
                {
                    EditMode = true;

                    if (SecurityManager.IsCurrentUserInRole("Service Desk: Instruction Management"))
                        chkNoChargesWithdrawal.Checked = false;
                    else
                        chkNoChargesWithdrawal.Checked = true;

                    dbWithdrawalAmount.Clear();
                    ddlCounterAccount.SelectedValue = int.MinValue.ToString();
                    dtpRelevanceDate.SelectedDate = DateTime.Today;
                    dtpWithdrawalDate.Clear();
                    txtTransferDescription.Text = "";
                }
                else
                {
                    int? counterAccountID = null;
                    if (!ddlCounterAccount.SelectedValue.Equals(int.MinValue.ToString()))
                        counterAccountID = int.Parse(ddlCounterAccount.SelectedValue);

                    lblErrorMessage.Text = "";
                    BatchExecutionResults results = new BatchExecutionResults();
                    InstructionEntryAdapter.CreateWithdrawalInstructions(results, accountIds, dtpRelevanceDate.SelectedDate, dtpWithdrawalDate.SelectedDate, dbWithdrawalAmount.Value, counterAccountID, txtTransferDescription.Text, chkNoChargesWithdrawal.Checked);
                    lblErrorMessage.Text = InstructionEntryAdapter.FormatErrorsForCreateInstructions(results, "withdrawal");

                    EditMode = false;
                    gvAccounts.ClearSelection();
                }
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();


            gvAccounts.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnDeparture_Click(object sender, EventArgs e)
    {
        try
        {
            hdfInstructionType.Value = "CD";
            int[] accountIds = gvAccounts.GetSelectedIds();
            Session["SelectedAccountIds"] = accountIds;

            if (accountIds.Length > 0)
            {
                if (!EditMode)
                {
                    EditMode = true;

                    cldExecutionDateDeparture.SelectedDate = DateTime.Today;
                    ddlCounterAccountDeparture.SelectedValue = int.MinValue.ToString();
                    txtTransferDescriptionDeparture.Text = "";
                }
                else
                {
                    int? counterAccountID = null;
                    if (!ddlCounterAccountDeparture.SelectedValue.Equals(int.MinValue.ToString()))
                        counterAccountID = Utility.GetKeyFromDropDownList(ddlCounterAccountDeparture);

                    lblErrorMessage.Text = "";
                    BatchExecutionResults results = new BatchExecutionResults();
                    InstructionEntryAdapter.CreateDepartureInstructions(
                        results,
                        accountIds, 
                        cldExecutionDateDeparture.SelectedDate, 
                        counterAccountID, 
                        txtTransferDescriptionDeparture.Text, 
                        chkNoChargesDeparture.Checked);
                    lblErrorMessage.Text = InstructionEntryAdapter.FormatErrorsForCreateInstructions(results, "departure");

                    EditMode = false;
                    gvAccounts.ClearSelection();
                }
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();


            gvAccounts.DataBind();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            EditMode = false;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void btnHideInstructions_Click(object sender, EventArgs e)
    {
        try
        {
            pnlInstructions.Visible = false;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = string.Format("<br/>{0}<br/>", Utility.GetCompleteExceptionMessage(ex));
        }
    }

    protected void dtpExecDate_SelectionChanged(object sender, EventArgs e)
    {
        rfvExecDate.Validate();
        rvExecDate.Validate();
        btnRebalance.Focus();
    }

    protected void dtpWithdrawalDate_SelectionChanged(object sender, EventArgs e)
    {
        rvWithDrawalDate.Validate();
        rvExecDate.Validate();
        btnWithdrawal.Focus();
    }

    protected void dtpRelevanceDate_SelectionChanged(object sender, EventArgs e)
    {
        rvRelevanceDate.Validate();
        rvRelevanceDate.Validate();
        cvRelevanceDate.Validate();
        btnWithdrawal.Focus();
    }

    private bool EditMode
    {
        get { return (bool)ViewState["EditMode"]; }
        set
        {
            pnlInstructions.Visible = false;
            pnlRebalanceInstructionProps.Visible = false;
            pnlWithdrawalInstruction.Visible = false;
            pnlClientDepartureInstructionProps.Visible = false;

            if (!SecurityManager.IsCurrentUserInRole("Service Desk: Instruction Management"))
            {
                btnRebalance.Visible = !value;
                btnDeparture.Visible = !value;
            }
            btnWithdrawal.Visible = !value;
            switch (hdfInstructionType.Value)
            {
                case "WD":
                    pnlWithdrawalInstruction.Visible = value;
                    btnWithdrawal.Visible = true;
                    gvAccounts.Enabled = !value; 
                    break;
                case "CD":
                    pnlClientDepartureInstructionProps.Visible = value;
                    btnDeparture.Visible = true;
                    gvAccounts.Enabled = !value; 
                    break;
                default:
                    pnlRebalanceInstructionProps.Visible = value;
                    btnRebalance.Visible = true;
                    ucInstrumentsModelsSelector.Clear();
                    pnlSelector.Visible = false;
                    chkExclude.Checked = false;
                    break;
            }

            rvExecDate.MinimumValue = DateTime.Today.ToShortDateString();
            rvWithDrawalDate.MinimumValue = DateTime.Today.ToShortDateString();
            rvRelevanceDate.MinimumValue = DateTime.Today.ToShortDateString();
            rvExecutionDateDeparture.MinimumValue = DateTime.Today.ToShortDateString();
            btnCancel.Visible = value;
            ViewState["EditMode"] = value;
            if (!value)
                Session["SelectedAccountIds"] = null;
            else
                Utility.ScrollToBottom(hdnScrollToBottom);
        }
    }
}
