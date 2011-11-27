using System;
using System.Web.UI.WebControls;
using B4F.TotalGiro.ApplicationLayer.Instructions;

public partial class InstructionManagement : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlAccountFinder.Search += new EventHandler(ctlAccountFinder_Search);
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Rebalance Instructions Management Console";
            gvInstructionOverview.Sort("ExecutionDate", SortDirection.Ascending);

            decimal minValue;
            int pricingType;
            ddlPricingTypes.DataBind();
            if (InstructionManagementAdapter.GetToleranceParameters(out minValue, out pricingType))
            {
                txtTolerance.Text = minValue.ToString();
                ddlPricingTypes.SelectedIndex = (pricingType - 1);
            }
        }
        else
        {
            Session["SelectedInstructionIds"] = null;
        }

        if (dvInstructionEdit.Visible)
        {
            DatePicker dtpExecDate = (DatePicker)dvInstructionEdit.FindControl("dtpExecDate");
            dtpExecDate.SelectionChanged += new EventHandler(dtpExecDate_SelectionChanged);
            dtpExecDate.Expanded += new EventHandler(dtpExecDate_Expanded);
        }

        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
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
                            RangeValidator rvExecDate = (RangeValidator)dvInstructionEdit.FindControl("rvExecDate");
                            rvExecDate.MinimumValue = DateTime.Today.ToShortDateString();
                            if (dvInstructionEdit.DataItemCount > 0)
                            {
                                setEditMode(true);
                                if (((InstructionEditView)dvInstructionEdit.DataItem).Exclusions.Count > 0)
                                {
                                    CheckBox chkExcl = (CheckBox)this.dvInstructionEdit.FindControl("chkExclude");
                                    chkExcl.Checked = true;
                                    chkExclude_CheckedChanged(null, null);
                                }
                                gvInstructionOverview.Enabled = false;
                                LinkButton lbtDvCancelEdit = (LinkButton)dvInstructionEdit.FindControl("lbtDvCancelEdit");
                                lbtDvCancelEdit.Focus();
                            }
                            break;
                        case "VIEWORDERS":
                            gvOrders.DataBind();
                            gvOrders.Visible = true;
                            btnHideOrders.Visible = true;
                            return;
                    }
                    //gvInstructionOverview.SelectedIndex = -1;
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
            gvInstructionOverview.SelectedIndex = -1;
        }
	}

    protected void dvInstructionEdit_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {

        try
        {
            lblError.Text = "";
            if (e.CommandName.ToUpper() == "DVCANCELEDIT")
                setEditMode(false);
            else if (e.CommandName.ToUpper() == "DVSAVEINSTRUCTION")
            {
                Label lblInstructionID = (Label)dvInstructionEdit.FindControl("InstructionID");
                int instructionID = Int32.Parse(lblInstructionID.Text);

                DropDownList ddlOrderActionType = (DropDownList)dvInstructionEdit.FindControl("ddlOrderActionTypes");
                int orderActionTypeID = short.Parse(ddlOrderActionType.SelectedValue);

                DatePicker dtpExecDate = (DatePicker)dvInstructionEdit.FindControl("dtpExecDate");
                DateTime execDate = dtpExecDate.SelectedDate;

                CheckBox chkNoCharges = (CheckBox)dvInstructionEdit.FindControl("chkNoCharges");
                bool doNotChargeCommission = chkNoCharges.Checked;

                InstrumentsModelsSelector selector = (InstrumentsModelsSelector)dvInstructionEdit.FindControl("ucInstrumentsModelsSelector");

                InstructionManagementAdapter.EditInstruction(instructionID, orderActionTypeID, execDate, doNotChargeCommission, selector.Exclusions);
                gvInstructionOverview.DataBind();

                setEditMode(false);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void ctlAccountFinder_Search(object sender, EventArgs e)
    {
        gvInstructionOverview.Visible = true;
        lblError.Visible = true;
        lblResult.Visible = true;
        btnProcessInstructions.Visible = true;
        btnCancelInstructions.Visible = true;
        btnCheckPrices.Visible = true;
        lblTolerance.Visible = true;
        txtTolerance.Visible = true;
        ddlPricingTypes.Visible = true;
    }

    protected void dtpExecDate_SelectionChanged(object sender, EventArgs e)
    {
        RequiredFieldValidator rfvExecDate = (RequiredFieldValidator)dvInstructionEdit.FindControl("rfvExecDate");
        rfvExecDate.Validate();
        RangeValidator rvExecDate = (RangeValidator)dvInstructionEdit.FindControl("rvExecDate");
        rvExecDate.Validate();
        DatePicker dtpExecDate = (DatePicker)dvInstructionEdit.FindControl("dtpExecDate");
        dtpExecDate.Focus();
        
    }

    protected void dtpExecDate_Expanded(object sender, EventArgs e)
    {
        DatePicker dtpExecDate = (DatePicker)dvInstructionEdit.FindControl("dtpExecDate");
        dtpExecDate.Focus();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        ctlAccountFinder.DoSearch();
        gvInstructionOverview.Visible = true;
        btnProcessInstructions.Visible = true;
        gvOrders.Visible = false;
        btnHideOrders.Visible = false;
    }

	//Process instructions
	protected void btnProcessInstructions_Click(object sender, EventArgs e)
	{
        try
        {
            int[] instructionIds = gvInstructionOverview.GetSelectedIds();
            int pricingType = int.Parse(ddlPricingTypes.SelectedValue);
            decimal minimumQty = decimal.Parse(txtTolerance.Text);

            int instructionsUpdated = InstructionManagementAdapter.ProcessInstructions(instructionIds, pricingType, minimumQty);
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

    //Check Instruments Prices
    protected void btnCheckPrices_Click(object sender, EventArgs e)
	{
        try
        {
            int[] instructionIds = gvInstructionOverview.GetSelectedIds();
            if (instructionIds.Length > 0)
            {
                Session["SelectedInstructionIds"] = instructionIds;
                Server.Transfer("RebalanceExcludeInstruments.aspx");
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }
        catch (Exception ex)
        {
            lblError.Text = getErrorMessage(ex);
        }
	}

    protected void chkExclude_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkExcl = (CheckBox)this.dvInstructionEdit.FindControl("chkExclude");
        Panel pnlSelector = (Panel)this.dvInstructionEdit.FindControl("pnlSelector");
        pnlSelector.Visible = chkExcl.Checked;
        if (!chkExcl.Checked)
        {
            InstrumentsModelsSelector selector = (InstrumentsModelsSelector)this.dvInstructionEdit.FindControl("ucInstrumentsModelsSelector");
            selector.Clear();
            dvInstructionEdit.Width = new Unit(500);
        }
        else
            dvInstructionEdit.Width = new Unit(1100);
        LinkButton lbtDvCancelEdit = (LinkButton)dvInstructionEdit.FindControl("lbtDvCancelEdit");
        lbtDvCancelEdit.Focus();
    }

    //Cancel instructions
    protected void btnCancelInstructions_Click(object sender, EventArgs e)
	{
        try
        {
            int[] instructionIds = gvInstructionOverview.GetSelectedIds();

            if (instructionIds.Length > 0)
		    {
                int instructionsCancelled = InstructionManagementAdapter.CancelInstructions(instructionIds);
                lblResult.Text = string.Format("Number of instructions cancelled: {0}<br />", instructionsCancelled);

                gvInstructionOverview.ClearSelection();
			    gvInstructionOverview.DataBind();
		    }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
	    }
        catch (Exception ex)
        {
            lblError.Text = getErrorMessage(ex);
        }
    }

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

    private string getErrorMessage(Exception ex)
    {
        return ex.Message + (ex.InnerException != null ? "<br />" + ex.InnerException.Message : "") + "<br />";
    }

    private void setEditMode(bool editMode)
    {
        dvInstructionEdit.Visible = editMode;
        dvInstructionEdit.Width = new Unit(500);
        vsInstructionProps.Visible = editMode;
        btnProcessInstructions.Enabled = !editMode;
        btnCheckPrices.Enabled = !editMode;
        btnCancelInstructions.Enabled = !editMode;
        txtTolerance.Enabled = !editMode;
        lblTolerance.Enabled = !editMode;
        ddlPricingTypes.Enabled = !editMode;

        if (!editMode)
        {
            gvInstructionOverview.Enabled = true;
            gvInstructionOverview.SelectedIndex = -1;
        }
    }
}
