using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using B4F.TotalGiro.ApplicationLayer.Tools;

public partial class Tools_RebalanceIndicator : System.Web.UI.Page
{
    public DataTable ModelOverview
    {
        get
        {
            object dt = ViewState["ModelOverview"];
            if (dt != null)
                return (DataTable)dt;
            else
                return null;
        }
        set
        {
            ViewState["ModelOverview"] = value;
        }
    }

    
    public DataTable ModelDetails
    {
        get
        {
            object dt = ViewState["ModelDetails"];
            if (dt != null)
                return (DataTable)dt;
            else
                return null;
        }
        set
        {
            ViewState["ModelDetails"] = value;
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = "";
            if (!IsPostBack)
            {
                ((EG)this.Master).setHeaderText = "Rebalance Indicator";

                ctlDepositDate.SelectedDate = DateTime.Today.AddMonths(-6);
                dbMaxDeviation.Value = 5;
                rvDepositDate.MaximumValue = DateTime.Today.ToString("yyyy-MM-dd");
                rvDepositDate.MinimumValue = new DateTime(2000,1,1).ToString("yyyy-MM-dd");
                rvEndDate.MaximumValue = DateTime.Today.ToString("yyyy-MM-dd");
                rvEndDate.MinimumValue = new DateTime(2000, 1, 1).ToString("yyyy-MM-dd");
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        try
        {
            string message = "";
            DataSet ds = RebalanceIndicatorAdapter.GetModelRebalanceIndications(
                txtModelName.Text,
                ctlDepositDate.SelectedDate,
                ctlEndDate.SelectedDate,
                dbStartBalance.Value,
                dbMaxDeviation.Value,
                chkIncludeModelChanges.Checked,
                chkIncludeInactiveModels.Checked,
                out message);

            ModelOverview = ds.Tables["Overview"];
            gvOverview.DataSource = ModelOverview;
            ModelDetails = ds.Tables["Details"];
            gvOverview.DataBind();

            gvDetails.DataSource = null;
            gvDetails.DataBind();
            lblErrorMessage.Text = message;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvOverview_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            int key = int.Parse(gvOverview.SelectedValue.ToString());
            ModelDetails.DefaultView.RowFilter = string.Format("ModelID={0}", key);
            gvDetails.DataSource = ModelDetails;
            gvDetails.DataBind();
            gvDetails.Visible = true;
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void gvOverview_Sorting(object sender, GridViewSortEventArgs e)
    {
        sortDirection = (SortDirection)Math.Abs((int)sortDirection - 1);

        string[] sortExprs = e.SortExpression.Split(' ');
        e.SortExpression = string.Format("{0} {1}",
            sortExprs[0],
            sortDirection == SortDirection.Ascending ? "ASC" : "DESC");

        DataView dv = ModelOverview.DefaultView;
        dv.Sort = e.SortExpression;
        gvOverview.DataSource = dv;
        gvOverview.DataBind();
    }

    protected void gvOverview_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvDetails.Visible = false;
        gvOverview.PageIndex = e.NewPageIndex;
        DataView dv = ModelOverview.DefaultView;
        gvOverview.DataSource = dv;
        gvOverview.DataBind();
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
