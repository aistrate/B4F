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
using B4F.TotalGiro.ApplicationLayer.Instructions;

public partial class WithdrawalInstructionManagement: System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlModelPortfolio"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountNumber"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountName"), btnSearch);
        
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Withdrawal Instructions Management Console";
            gvInstructionOverview.Sort("WithdrawalDate", SortDirection.Ascending);
        }
        else
        {
            Session["SelectedInstructionIds"] = null;
        }

        if (dvInstructionEdit.Visible)
        {
            DatePicker dtpExecDate = (DatePicker)dvInstructionEdit.FindControl("dtpExecDate");
            dtpExecDate.SelectionChanged += new EventHandler(dtpExecDate_SelectionChanged);
            DatePicker dtpWithdrawalDate = (DatePicker)dvInstructionEdit.FindControl("dtpWithdrawalDate");
            dtpWithdrawalDate.SelectionChanged += new EventHandler(dtpWithdrawalDate_SelectionChanged);
        }

        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
        btnHideOrders.Visible = gvOrders.Visible;
    }

    protected void gvInstructionOverview_OnRowCommand(object sender, GridViewCommandEventArgs e)
	{
        try
        {
            lblError.Text = string.Empty;
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvInstructionOverview.EditIndex = -1;
                    gvInstructionOverview.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITINSTRUCTION":
                            dvInstructionEdit.DataBind();
                            if (dvInstructionEdit.DataItemCount > 0)
                            {
                                setEditMode(true);
                                gvInstructionOverview.Enabled = false;
                            }
                            break;
                        case "CANCELINSTRUCTION":
                            int key = (int)gvInstructionOverview.SelectedDataKey.Value;

                            if (WithdrawalInstructionManagementAdapter.CancelInstructions(new int[] { key }) == 1)
                            {
                                lblResult.Text = string.Format("Instruction {0} was cancelled successfully", key);
                                gvInstructionOverview.DataBind();
                            }
                            break;
                        case "VIEWORDERS":
                            gvOrders.DataBind();
                            gvOrders.Visible = true;
                            btnHideOrders.Visible = true;
                            return;
                        case "VIEWMONEYORDER":
                            gvMoneyOrder.Visible = true;
                            btnHideMoneyOrders.Visible = true;
                            return;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
            gvInstructionOverview.SelectedIndex = -1;
        }
	}

    protected void dvInstructionEdit_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (dvInstructionEdit.DataItemCount > 0)
            {
                int counterAccountID = ((InstructionEditView)dvInstructionEdit.DataItem).CounterAccountID;
                if (counterAccountID != 0)
                {
                    DropDownList ddlCounterAccount = (DropDownList)dvInstructionEdit.FindControl("ddlCounterAccount");
                    ddlCounterAccount.SelectedValue = counterAccountID.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void dvInstructionEdit_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {

        try
        {
            if (e.CommandName.ToUpper() == "DVCANCELEDIT")
                setEditMode(false);
            else if (e.CommandName.ToUpper() == "DVSAVEINSTRUCTION")
            {
                Label lblInstructionID = (Label)dvInstructionEdit.FindControl("InstructionID");
                int instructionID = Int32.Parse(lblInstructionID.Text);

                DatePicker dtpWithdrawalDate = (DatePicker)dvInstructionEdit.FindControl("dtpWithdrawalDate");
                DateTime withdrawalDate = dtpWithdrawalDate.SelectedDate;

                DatePicker dtpExecDate = (DatePicker)dvInstructionEdit.FindControl("dtpExecDate");
                DateTime execDate = dtpExecDate.SelectedDate;

                DecimalBox dbWithdrawalAmount = (DecimalBox)dvInstructionEdit.FindControl("dbWithdrawalAmount");
                decimal amount = dbWithdrawalAmount.Value;

                TextBox txtTransferDescription = (TextBox)dvInstructionEdit.FindControl("txtTransferDescription");
                string transferDescription = txtTransferDescription.Text;

                int? counterAccountID = null;
                int test;
                DropDownList ddlCounterAccount = (DropDownList)dvInstructionEdit.FindControl("ddlCounterAccount");
                if (int.TryParse(ddlCounterAccount.SelectedValue, out test) && test != 0 && test != int.MinValue)
                    counterAccountID = test;

                CheckBox chkNoChargesWithdrawal = (CheckBox)dvInstructionEdit.FindControl("chkNoChargesWithdrawal");

                WithdrawalInstructionManagementAdapter.EditInstruction(instructionID, withdrawalDate, execDate, amount, counterAccountID, transferDescription, chkNoChargesWithdrawal.Checked);
                gvInstructionOverview.DataBind();

                setEditMode(false);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ctlAccountFinder.DoSearch();
        gvInstructionOverview.Visible = true;
        lblError.Visible = true;
        lblResult.Visible = true;
        btnProcessInstructions.Visible = true;
    }

    protected void dtpExecDate_SelectionChanged(object sender, EventArgs e)
    {
        RequiredFieldValidator rfvExecDate = (RequiredFieldValidator)dvInstructionEdit.FindControl("rfvExecDate");
        rfvExecDate.Validate();
        CompareValidator cvExecDate = (CompareValidator)dvInstructionEdit.FindControl("cvExecDate");
        cvExecDate.Validate();
    }

    protected void dtpWithdrawalDate_SelectionChanged(object sender, EventArgs e)
    {
        RequiredFieldValidator rfvWithdrawalDate = (RequiredFieldValidator)dvInstructionEdit.FindControl("rfvWithdrawalDate");
        rfvWithdrawalDate.Validate();
        RangeValidator rvWithdrawalDate = (RangeValidator)dvInstructionEdit.FindControl("rvWithdrawalDate");
        rvWithdrawalDate.Validate();
    }

	//Process instructions
	protected void btnProcessInstructions_Click(object sender, EventArgs e)
	{
        try
        {
            int[] instructionIds = gvInstructionOverview.GetSelectedIds();

            int instructionsUpdated = WithdrawalInstructionManagementAdapter.ProcessInstructions(instructionIds);
            lblResult.Text = string.Format("Number of instructions updated: {0}<br />", instructionsUpdated);

            // reset session stuff
            Session["SelectedInstructionIds"] = null;

            gvInstructionOverview.ClearSelection();
            gvInstructionOverview.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = getErrorMessage(ex);
        }
	}

    ////Cancel instructions
    //protected void btnCancelInstructions_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        int[] instructionIds = gvInstructionOverview.GetSelectedIds();

    //        if (instructionIds.Length > 0)
    //        {
    //            int instructionsCancelled = WithdrawalInstructionManagementAdapter.CancelInstructions(instructionIds);
    //            lblResult.Text = string.Format("Number of instructions cancelled: {0}<br />", instructionsCancelled);

    //            gvInstructionOverview.ClearSelection();
    //            gvInstructionOverview.DataBind();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        lblError.Text = getErrorMessage(ex);
    //    }
    //}

    protected void btnHideOrders_Click(object sender, EventArgs e)
    {
        try
        {
            gvOrders.Visible = false;
            gvInstructionOverview.SelectedIndex = -1;
            btnHideOrders.Visible = false;
        }
        catch (Exception ex)
        {
            lblError.Text = getErrorMessage(ex);
        }
    }

    protected void btnHideMoneyOrders_Click(object sender, EventArgs e)
    {
        try
        {
            gvMoneyOrder.Visible = false;
            gvInstructionOverview.SelectedIndex = -1;
            btnHideMoneyOrders.Visible = false;
        }
        catch (Exception ex)
        {
            lblError.Text = getErrorMessage(ex);
        }
    }

    private string getErrorMessage(Exception ex)
    {
        return ex.Message + (ex.InnerException != null ? "<br />" + ex.InnerException.Message : "") + "<br />";
    }

    private void setEditMode(bool editMode)
    {
        dvInstructionEdit.Visible = editMode;
        vsInstructionProps.Visible = editMode;
        //btnProcessInstructions.Enabled = !editMode;
        if (!editMode)
        {
            gvInstructionOverview.Enabled = true;
            gvInstructionOverview.SelectedIndex = -1;
        }
        RangeValidator rvWithDrawalDate = (RangeValidator)dvInstructionEdit.FindControl("rvWithDrawalDate");
        rvWithDrawalDate.MinimumValue = DateTime.Today.ToShortDateString();
    }
}
