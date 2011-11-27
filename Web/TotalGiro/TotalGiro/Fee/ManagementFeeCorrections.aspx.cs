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
using B4F.TotalGiro.Accounts.ManagementPeriods;

public partial class ManagementFeeCorrections: System.Web.UI.Page
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

    public string TradeId
    {
        get
        {
            object i = Session["TradeId"];
            return ((i == null) ? "" : (string)i);
        }
        set { Session["TradeId"] = value; }
    }

    public bool ShowSettleDifference
    {
        get
        {
            object i = Session["ShowSettleDifference"];
            return ((i == null) ? false : Convert.ToBoolean(i));
        }
        set { Session["ShowSettleDifference"] = value; }
    }

    #endregion

    
    protected void Page_Load(object sender, EventArgs e)
    {
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlModelPortfolio"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountNumber"), btnSearch);
        Utility.SetDefaultButton(Page, (TextBox)ctlAccountFinder.FindControl("txtAccountName"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ctlAccountFinder.FindControl("ddlContactActive"), btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlYear, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlQuarter, btnSearch);
        Utility.SetDefaultButton(Page, (DropDownList)ddlOpenCloseStatus, btnSearch);
        
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Management Fee Corrections";
            gvMgtFeeCorrections.Sort("AverageHolding_Account_Number", SortDirection.Ascending);
            
            ddlYear.Items.Add(new ListItem("", "0")); 
            for (int i = 2008; i <= DateTime.Today.Year; i++)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlYear.SelectedValue = DateTime.Today.Year.ToString();
        }

        lblError.Text = string.Empty;
        lblResult.Text = string.Empty;
    }

    protected void gvMgtFeeCorrections_OnRowCommand(object sender, GridViewCommandEventArgs e)
	{
        try
        {
            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    gvMgtFeeCorrections.EditIndex = -1;
                    gvMgtFeeCorrections.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    switch (e.CommandName.ToUpper())
                    {
                        case "VIEWDETAILS":
                            string tradeID = ((GridViewRow)tableRow).Cells[11].Text;
                            if (string.IsNullOrEmpty(tradeID) || tradeID == "&nbsp;")
                            {
                                TradeId = "";
                                gvTransactions.Visible = false;
                            }
                            else
                            {
                                TradeId = tradeID;
                                gvTransactions.Visible = true;
                            }
                            gvAverageHoldingFees.Visible = true;
                            return;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            lblError.Text = Utility.GetCompleteExceptionMessage(ex);
            gvMgtFeeCorrections.SelectedIndex = -1;
        }
	}

    protected void gvMgtFeeCorrections_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int accountId = (int)((DataRowView)e.Row.DataItem)["AverageHolding_Account_Key"];
            AccountLabel lbl = (AccountLabel)e.Row.FindControl("ctlAccountLabel");
            lbl.AccountID = accountId;
            lbl.GetData();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Year = (string.IsNullOrEmpty(ddlYear.SelectedValue) ? 0 : Convert.ToInt32(ddlYear.SelectedValue));
        Quarter = (string.IsNullOrEmpty(ddlQuarter.SelectedValue) ? 0 : Convert.ToInt32(ddlQuarter.SelectedValue));
        ctlAccountFinder.DoSearch();
        gvMgtFeeCorrections.Visible = true;
        gvMgtFeeCorrections.SelectedIndex = -1;
        lblError.Visible = true;
        lblResult.Visible = true;
        btnSkip.Visible = true;
    }

    protected void gvTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Get the info about the feetypes being used
        DataRowView data = e.Row.DataItem as DataRowView;
        if (data != null)
        {
            DataTable t = data.Row.Table;
            ShowSettleDifference = !string.IsNullOrEmpty(data.Row[t.Columns.Count - 1].ToString());
        }
    }
    
    protected void gvTransactions_DataBound(object sender, EventArgs e)
    {
        gvTransactions.Columns[4].Visible = ShowSettleDifference;
    }


	//Process instructions
    protected void btnSkip_Click(object sender, EventArgs e)
	{
        try
        {
            int[] avgHoldingIds = gvMgtFeeCorrections.GetSelectedIds();

            if (avgHoldingIds.Length > 0)
            {
                lblResult.Text = "";
                BatchExecutionResults results = new BatchExecutionResults();
                bool success = ManagementFeeCorrectionsAdapter.SkipAverageHoldingCorrections(results, avgHoldingIds, ManagementTypes.ManagementFee);
                lblResult.Text = ManagementFeeCorrectionsAdapter.FormatErrorsForSkipAverageHoldingCorrections(results);

                if (success)
                {
                    gvMgtFeeCorrections.ClearSelection();
                    gvMgtFeeCorrections.DataBind();
                }
            }
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
