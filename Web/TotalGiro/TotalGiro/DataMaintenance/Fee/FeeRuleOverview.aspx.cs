using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.Web.WebControls;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee;
using System.Data;

public partial class DataMaintenance_Fee_FeeRuleOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Fee Rule Overview";
            lblErrorMessage.Text = string.Empty;
        }
    }

    protected void gvFeeRules_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlFeeCalc = (DropDownList)e.Row.FindControl("ddlFeeCalculationID");
                if (ddlFeeCalc != null)
                    ddlFeeCalc.SelectedValue = ((DataRowView)e.Row.DataItem)["FeeCalculation_Key"].ToString();

            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvFeeRules_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            DropDownList ddlFeeCalculationID = (DropDownList)(gvFeeRules.Rows[e.RowIndex].FindControl("ddlFeeCalculationID"));
            e.NewValues["feeCalculationId"] = int.Parse(ddlFeeCalculationID.SelectedValue);

            YearMonthPicker ppStartPeriod = (YearMonthPicker)(gvFeeRules.Rows[e.RowIndex].FindControl("ppStartPeriod"));
            e.NewValues["startPeriod"] = ppStartPeriod.SelectedPeriod;
            
            YearMonthPicker ppEndPeriod = (YearMonthPicker)(gvFeeRules.Rows[e.RowIndex].FindControl("ppEndPeriod"));
            e.NewValues["endPeriod"] = ppEndPeriod.SelectedPeriod;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void cvEndPeriod_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = !(ppEndPeriod.SelectedPeriod > 0 && (ppStartPeriod.SelectedPeriod == 0 || ppStartPeriod.SelectedPeriod >= ppEndPeriod.SelectedPeriod));
    }

    protected void ddlAccount_DataBound(object sender, EventArgs e)
    {
        if (ddlAccount.Items.Count == 2)
            ddlAccount.SelectedIndex = 1;
    }

    protected void btnFilterAccount_Click(object sender, EventArgs e)
    {
        pnlAccountFinder.Visible = !pnlAccountFinder.Visible;
        btnFilterAccount.Text = "Filter  " + (pnlAccountFinder.Visible ? "<<" : ">>");
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;
            gvFeeRules.DataBind();

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnResetFilter_Click(object sender, EventArgs e)
    {
        try
        {
            Server.Transfer("FeeRuleOverview.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCreateFeeRule_Click(object sender, EventArgs e)
    {
        try
        {
            if (!InsertFeeRuleMode)
                InsertFeeRuleMode = true;
            else
            {
                rfvFeeCalculation.Validate();
                cvEndPeriod.Validate();
                if (rfvFeeCalculation.IsValid && cvEndPeriod.IsValid)
                {
                    if (FeeRuleOverviewAdapter.CreateDefaultFeeRule(
                        Convert.ToInt32(ddlFeeCalculation.SelectedValue),
                        chkExecutionOnly.Checked,
                        chkHasEmployerRelation.Checked,
                        chkSendByPost.Checked,
                        ppStartPeriod.SelectedPeriod,
                        ppEndPeriod.SelectedPeriod))
                    {
                        gvFeeRules.DataBind();
                        InsertFeeRuleMode = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCancelCreateFeeRule_Click(object sender, EventArgs e)
    {
        InsertFeeRuleMode = false;
    }

    private bool InsertFeeRuleMode
    {
        get 
        {
            object t = ViewState["InsertFeeRuleMode"];
            return ((t == null) ? false : (bool)t);
        }
        set
        {
            pnlFeeRuleEntry.Visible = value;
            btnCancelCreateFeeRule.Visible = value;

            gvFeeRules.Enabled = !value;
            if (value)
            {
                ddlFeeCalculation.Focus();
                Utility.ScrollToBottom(hdnScrollToBottom);
            }
            ViewState["InsertFeeRuleMode"] = value;
        }
    }

}
