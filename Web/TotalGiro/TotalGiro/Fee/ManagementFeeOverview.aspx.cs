using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.ApplicationLayer.Instructions;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.ApplicationLayer.Fee;

public partial class ManagementFeeOverview: System.Web.UI.Page
{
    #region Properties

    public int Year
    {
        get
        {
            object i = Session["Year"];
            return ((i == null) ? 0 : (int)i);
        }
        set { Session["Year"] = value; }
    }

    public int Quarter
    {
        get
        {
            object i = Session["Quarter"];
            return ((i == null) ? 0 : (int)i);
        }
        set { Session["Quarter"] = value; }
    }

    public string TradeIds
    {
        get
        {
            object i = Session["TradeIds"];
            return ((i == null) ? "" : (string)i);
        }
        set { Session["TradeIds"] = value; }
    }

    public string DatasetFeeTypeInfo
    {
        get
        {
            object i = Session["DatasetFeeTypeInfo"];
            return ((i == null) ? "" : (string)i);
        }
        set { Session["DatasetFeeTypeInfo"] = value; }
    }

    public string DatasetAvgHldFeeTypeInfo
    {
        get
        {
            object i = Session["DatasetAvgHldFeeTypeInfo"];
            return ((i == null) ? "" : (string)i);
        }
        set { Session["DatasetAvgHldFeeTypeInfo"] = value; }
    }

    #endregion

    
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlModelPortfolio"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountNumber"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountName"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlContactActive"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlYear, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlQuarter, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlContinuationStatus, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlTradeStatus, btnSearch);
        
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Management Fee Overview";
            gvMgtFeeUnitOverview.Sort("ManagementEndDate", SortDirection.Ascending);
            gvMgtFeeUnitOverviewSummary.Sort("Period", SortDirection.Ascending);
            ddlQuarter.SelectedValue = Util.GetQuarter(DateTime.Today).ToString();
            for (int i = 2008; i <= DateTime.Today.Year; i++)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlYear.SelectedValue = DateTime.Today.Year.ToString();
        }

        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
        btnHideManagementPeriodUnits.Visible = gvManagementPeriodUnits.Visible;
    }

    protected void gvMgtFeeUnitOverview_OnRowCommand(object sender, GridViewCommandEventArgs e)
	{
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvMgtFeeUnitOverview.EditIndex = -1;
                    gvMgtFeeUnitOverview.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWDETAILS":
                            string tradeID = ((GridViewRow)tableRow).Cells[14].Text;
                            if (string.IsNullOrEmpty(tradeID) || tradeID == "&nbsp;")
                            {
                                TradeIds = "";
                                gvTransactions.Visible = false;
                            }
                            else
                            {
                                TradeIds = tradeID;
                                gvTransactions.Visible = true;
                            }
                            gvManagementPeriodUnits.Visible = true;
                            btnHideManagementPeriodUnits.Visible = true;
                            pnlAverageHoldingFees.Visible = false;
                            gvAverageHoldingFees.Visible = false;
                            btnHideManagementPeriodUnits.Focus();
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                        //case "CANCELINSTRUCTION":
                        //    int key = (int)gvMgtFeeUnitOverview.SelectedDataKey.Value;

                        //    if (WithdrawalInstructionManagementAdapter.CancelInstructions(new int[] { key }) == 1)
                        //    {
                        //        lblResult.Text = string.Format("Instruction {0} was cancelled successfully", key);
                        //        gvMgtFeeUnitOverview.DataBind();
                        //    }
                        //    break;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
            gvMgtFeeUnitOverview.SelectedIndex = -1;
        }
	}

    protected void gvMgtFeeUnitOverview_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            if (Year == 0 || Quarter == 0)
                return;
            
            int idx = 0;
            int[] ValueColumns = { 5, 8, 11 };
            int[] FeeColumns = { 6, 9, 12 };
            int[] periods = Util.GetPeriodsFromQuarter(Year, Quarter);
            int counter = 0;

            foreach (TableCell cell in e.Row.Cells)
            {
                if (!string.IsNullOrEmpty(cell.Text) && cell.Text != "&nbsp;")
                {
                    if (ValueColumns.Contains(idx))
                        cell.Text = string.Format("Val_{0}", periods[counter].ToString().Substring(2));
                    else if (FeeColumns.Contains(idx))
                    {
                        cell.Text = string.Format("Fee_{0}", periods[counter].ToString().Substring(2));
                        counter++;
                    }
                }
                idx++;
            }
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int idx = 0;
            int[] LeftAlignedBorderColumns = { 5, 8, 11, 14 };

            foreach (TableCell cell in e.Row.Cells)
            {
                if (LeftAlignedBorderColumns.Contains(idx))
                {
                    cell.Style.Add("border-left-width", "1px");
                    cell.Style.Add("border-left-color", "black");
                    cell.Style.Add("border-left-style", "solid");
                }
                idx++;
            }
        }
    }

    protected void gvMgtFeeUnitOverview_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["Account_Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void gvManagementPeriodUnits_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Get the info about the feetypes being used
        DataRowView data = e.Row.DataItem as DataRowView;
        if (data != null)
        {
            DataTable t = data.Row.Table;
            DatasetFeeTypeInfo = data.Row[t.Columns.Count - 1].ToString();
        }
    }

    protected void gvManagementPeriodUnits_DataBound(object sender, EventArgs e)
    {
        int startFeeCol = 5;

        if (!string.IsNullOrEmpty(DatasetFeeTypeInfo))
        {
            int[] keys = DatasetFeeTypeInfo.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            for (int i = 0; i < 4; i++)
            {
                if (keys.Length > i)
                    gvManagementPeriodUnits.Columns[startFeeCol + i].HeaderText = ((FeeTypes)keys[i]).ToString();
                else
                    gvManagementPeriodUnits.Columns[startFeeCol + i].Visible = false;
            }
        }
    }

    protected void gvManagementPeriodUnits_OnRowCommand(object sender, GridViewCommandEventArgs e)
	{
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvManagementPeriodUnits.EditIndex = -1;
                    gvManagementPeriodUnits.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWHOLDINGS":
                            gvManagementPeriodUnits.Visible = true;
                            if (!string.IsNullOrEmpty(TradeIds))
                                gvTransactions.Visible = true;
                            pnlAverageHoldingFees.Visible = true;
                            gvAverageHoldingFees.Visible = true;
                            btnHideManagementPeriodUnits.Visible = true;
                            btnHideManagementPeriodUnits.Focus();
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                        case "RECALCULATE":
                            int unitID = Convert.ToInt32(gvManagementPeriodUnits.SelectedDataKey.Value);
                            if (ManagementFeeOverviewAdapter.RecalculateFees(unitID))
                            {
                                gvManagementPeriodUnits.Visible = true;
                                if (!string.IsNullOrEmpty(TradeIds))
                                    gvTransactions.Visible = true;
                                btnHideManagementPeriodUnits.Visible = true;
                                btnHideManagementPeriodUnits.Focus();
                            }
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
        }
	}

    protected void gvAverageHoldingFees_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Get the info about the feetypes being used
        DataRowView data = e.Row.DataItem as DataRowView;
        if (data != null)
        {
            DataTable t = data.Row.Table;
            DatasetAvgHldFeeTypeInfo = data.Row[t.Columns.Count - 1].ToString();
        }
    }

    protected void gvAverageHoldingFees_DataBound(object sender, EventArgs e)
    {
        int startFeeCol = 5;

        if (!string.IsNullOrEmpty(DatasetAvgHldFeeTypeInfo))
        {
            int[] keys = DatasetAvgHldFeeTypeInfo.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            for (int i = 0; i < 3; i++)
            {
                if (keys.Length > i)
                    gvAverageHoldingFees.Columns[startFeeCol + i].HeaderText = ((FeeTypes)keys[i]).ToString();
                else
                    gvAverageHoldingFees.Columns[startFeeCol + i].Visible = false;
            }
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Year = (string.IsNullOrEmpty(ddlYear.SelectedValue) ? 0 : Convert.ToInt32(ddlYear.SelectedValue));
        Quarter = (string.IsNullOrEmpty(ddlQuarter.SelectedValue) ? 0 : Convert.ToInt32(ddlQuarter.SelectedValue));
        ctlAccountFinder.DoSearch();
        gvMgtFeeUnitOverview.Visible = true;
        gvMgtFeeUnitOverview.SelectedIndex = -1;
        pnlMgtFeeUnitOverviewSummary.Visible = chkShowSummary.Checked;
        lblError.Visible = true;
        lblResult.Visible = true;
        btnCreateMgtFeeTransaction.Visible = true;
    }

	//Process instructions
	protected void btnCreateMgtFeeTransaction_Click(object sender, EventArgs e)
	{
        try
        {
            int[] unitIds = gvMgtFeeUnitOverview.GetSelectedIds();

            if (unitIds.Length > 0)
            {
                lblResult.Text = "";
                BatchExecutionResults results = new BatchExecutionResults();
                ManagementFeeOverviewAdapter.CreateMgtFeeTransactions(results, unitIds, Year, Quarter, B4F.TotalGiro.Accounts.ManagementPeriods.ManagementTypes.ManagementFee, true);
                lblResult.Text = ManagementFeeOverviewAdapter.FormatErrorsForCreateMgtFeeTransactions(results);


                gvMgtFeeUnitOverview.ClearSelection();
                gvMgtFeeUnitOverview.DataBind();
            }
            else
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
        }
        catch (Exception ex)
        {
            lblError.Text = getErrorMessage(ex);
        }
	}

    protected void btnHideManagementPeriodUnits_Click(object sender, EventArgs e)
    {
        try
        {
            gvManagementPeriodUnits.Visible = false;
            pnlAverageHoldingFees.Visible = false;
            gvAverageHoldingFees.Visible = false;
            gvMgtFeeUnitOverview.SelectedIndex = -1;
            btnHideManagementPeriodUnits.Visible = false;
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
}
