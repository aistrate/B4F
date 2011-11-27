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

public partial class KickbackOverview: System.Web.UI.Page
{
    #region Properties

    public int KickbackYear
    {
        get
        {
            object i = Session["KickbackYear"];
            return ((i == null) ? 0 : (int)i);
        }
        set { Session["KickbackYear"] = value; }
    }

    public int KickbackQuarter
    {
        get
        {
            object i = Session["KickbackQuarter"];
            return ((i == null) ? 0 : (int)i);
        }
        set { Session["KickbackQuarter"] = value; }
    }

    public string KickbackDatasetFeeTypeInfo
    {
        get
        {
            object i = Session["KickbackDatasetFeeTypeInfo"];
            return ((i == null) ? "" : (string)i);
        }
        set { Session["KickbackDatasetFeeTypeInfo"] = value; }
    }

    public string KickbackDatasetAvgHldFeeTypeInfo
    {
        get
        {
            object i = Session["KickbackDatasetAvgHldFeeTypeInfo"];
            return ((i == null) ? "" : (string)i);
        }
        set { Session["KickbackDatasetAvgHldFeeTypeInfo"] = value; }
    }

    #endregion
    
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.EnableScrollToBottom(this, hdnScrollToBottom);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlModelPortfolio"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlRemisier"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlRemisierEmployee"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountNumber"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountName"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlContactActive"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlYear, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlQuarter, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlContinuationStatus, btnSearch);
        
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Kickback Fee Overview";
            gvKickbackUnitOverview.Sort("Account_Number", SortDirection.Ascending);
            gvKickbackUnitOverviewSummary.Sort("Period", SortDirection.Ascending);
            ddlQuarter.SelectedValue = Util.GetQuarter(DateTime.Today).ToString();
            for (int i = 2008; i <= DateTime.Today.Year; i++)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlYear.SelectedValue = DateTime.Today.Year.ToString();
        }

        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
        btnHideManagementPeriodUnits.Visible = gvManagementPeriodUnits.Visible;
    }

    protected void gvKickbackUnitOverview_OnRowCommand(object sender, GridViewCommandEventArgs e)
	{
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvKickbackUnitOverview.EditIndex = -1;
                    gvKickbackUnitOverview.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWDETAILS":
                            gvManagementPeriodUnits.Visible = true;
                            btnHideManagementPeriodUnits.Visible = true;
                            pnlAverageHoldingFees.Visible = false;
                            gvAverageHoldingFees.Visible = false;
                            btnHideManagementPeriodUnits.Focus();
                            Utility.ScrollToBottom(hdnScrollToBottom);
                            return;
                        //case "CANCELINSTRUCTION":
                        //    int key = (int)gvKickbackUnitOverview.SelectedDataKey.Value;

                        //    if (WithdrawalInstructionManagementAdapter.CancelInstructions(new int[] { key }) == 1)
                        //    {
                        //        lblResult.Text = string.Format("Instruction {0} was cancelled successfully", key);
                        //        gvKickbackUnitOverview.DataBind();
                        //    }
                        //    break;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
            gvKickbackUnitOverview.SelectedIndex = -1;
        }
	}

    protected void gvKickbackUnitOverview_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            if (KickbackYear == 0 || KickbackQuarter == 0)
                return;
            
            int idx = 0;
            int[] ValueColumns = { 7, 10, 13 };
            int[] FeeColumns = { 8, 11, 14 };
            int[] periods = Util.GetPeriodsFromQuarter(KickbackYear, KickbackQuarter);
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
            int[] LeftAlignedBorderColumns = { 7, 10, 13, 16 };

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

    protected void gvKickbackUnitOverview_RowDataBound(object sender, GridViewRowEventArgs e)
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
            KickbackDatasetFeeTypeInfo = data.Row[t.Columns.Count - 1].ToString();
        }
    }

    protected void gvManagementPeriodUnits_DataBound(object sender, EventArgs e)
    {
        int startFeeCol = 6;

        if (!string.IsNullOrEmpty(KickbackDatasetFeeTypeInfo))
        {
            int[] keys = KickbackDatasetFeeTypeInfo.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            for (int i = 0; i < 2; i++)
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
            KickbackDatasetAvgHldFeeTypeInfo = data.Row[t.Columns.Count - 1].ToString();
        }
    }

    protected void gvAverageHoldingFees_DataBound(object sender, EventArgs e)
    {
        int startFeeCol = 5;

        if (!string.IsNullOrEmpty(KickbackDatasetAvgHldFeeTypeInfo))
        {
            int[] keys = KickbackDatasetAvgHldFeeTypeInfo.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            for (int i = 0; i < 2; i++)
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
        KickbackYear = (string.IsNullOrEmpty(ddlYear.SelectedValue) ? 0 : Convert.ToInt32(ddlYear.SelectedValue));
        KickbackQuarter = (string.IsNullOrEmpty(ddlQuarter.SelectedValue) ? 0 : Convert.ToInt32(ddlQuarter.SelectedValue));
        ctlAccountFinder.DoSearch();
        gvKickbackUnitOverview.Visible = true;
        gvKickbackUnitOverview.SelectedIndex = -1;
        pnlKickbackUnitOverviewSummary.Visible = chkShowSummary.Checked;
        lblError.Visible = true;
        lblResult.Visible = true;
        btnExport.Visible = true;
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            int year = (string.IsNullOrEmpty(ddlYear.SelectedValue) ? 0 : Convert.ToInt32(ddlYear.SelectedValue));
            int quarter = (string.IsNullOrEmpty(ddlQuarter.SelectedValue) ? 0 : Convert.ToInt32(ddlQuarter.SelectedValue));

            if (year > 0 && quarter > 0)
            {
                int exportCount = ManagementFeeOverviewAdapter.ExportKickBackReportData(ctlAccountFinder.AssetManagerId, year, quarter);
                lblResult.Text = string.Format("{0} records were exported for {1} Q{2}", exportCount, year, quarter);
            }
            else
                lblError.Text = "Select a valid Year and/or Quarter";
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
            gvKickbackUnitOverview.SelectedIndex = -1;
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
