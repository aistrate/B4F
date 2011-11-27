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
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission;

public partial class CalculationsOverview : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        if (!IsPostBack)
		{
			((EG)this.Master).setHeaderText = "Commission Calculation Overview";
		}
	}
	protected void gvCalcOverview_RowCommand(object sender, GridViewCommandEventArgs e)
	{
        try
        {
            lblErrorMessage.Text = string.Empty;
            gvCalcOverview.DataBind();

            if (e.CommandSource.GetType() == typeof(LinkButton))
            {
                TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

                if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
                {
                    // Select row
                    gvCalcOverview.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                    int calcID = (int)gvCalcOverview.SelectedDataKey.Value;

                    switch (e.CommandName.ToUpper())
                    {
                        case "EDITCALC":
                            gvCalcOverview.SelectedIndex = -1;
                            string qStr = QueryStringModule.Encrypt(string.Format("id={0}", calcID));
                            Response.Redirect(string.Format("CalculationsEdit.aspx{0}", qStr));
                            break;

                        case "DELETECALC":
                            CalculationsOverviewAdapter.deleteCommissionCalculation(calcID);
                            gvCalcOverview.DataBind();
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }

		gvCalcOverview.SelectedIndex = -1;
	}

    protected void gvCalcOverview_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.gvCalcLines.Visible = true;
    }

    protected void gvCalcOverview_PageIndexChanged(object sender, EventArgs e)
    {
        this.gvCalcLines.Visible = false;
    }

    protected void btnAdd_Click(object sender, EventArgs e)
	{
        string qStr = QueryStringModule.Encrypt("id=0");
        Response.Redirect(string.Format("CalculationsEdit.aspx?{0}", qStr));
	}

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;
            gvCalcOverview.DataBind();

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }


}
