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
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Orders;

public partial class RuleOverview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Commission Rule Overview";
            gvCommissionRules.Sort("CommRuleName", SortDirection.Ascending);

            ddlAccount.SelectedValue = int.MinValue.ToString();
            ddlInstrument.SelectedValue = int.MinValue.ToString();
            DatePickerStartDate.SelectedDate = DateTime.Today;
        }
    }
    
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string qStr = QueryStringModule.Encrypt("id=0");
        Response.Redirect(string.Format("RuleEdit.aspx{0}", qStr));
    }

    protected void gvCommissionRules_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandSource.GetType() == typeof(LinkButton))
        {
            TableRow tableRow = (TableRow)((LinkButton)e.CommandSource).Parent.Parent;

            if (tableRow.GetType() == typeof(GridViewRow) && ((GridViewRow)tableRow).RowType == DataControlRowType.DataRow)
            {
                // Select row
                gvCommissionRules.SelectedIndex = ((GridViewRow)tableRow).RowIndex;

                int ruleId = (int)gvCommissionRules.SelectedDataKey.Value;

                switch (e.CommandName.ToUpper())
                {
                    case "EDITRULE":
                        gvCommissionRules.SelectedIndex = -1;
                        string qStr = QueryStringModule.Encrypt(string.Format("id={0}", ruleId));
                        Response.Redirect(string.Format("RuleEdit.aspx{0}", qStr));
                        break;
                    case "DELETERULE":
                        RuleOverviewAdapter.DeleteCommissionRule(ruleId);
                        gvCommissionRules.DataBind();
                        break;
                }
            }
        }

        gvCommissionRules.SelectedIndex = -1;
    }

    protected void btnFilterAccount_Click(object sender, EventArgs e)
    {
        pnlAccountFinder.Visible = !pnlAccountFinder.Visible;
        btnFilterAccount.Text = getFilterButtonLabel(pnlAccountFinder.Visible);
    }

    protected void btnFilterInstrument_Click(object sender, EventArgs e)
    {
        pnlInstrumentFinder.Visible = !pnlInstrumentFinder.Visible;
        btnFilterInstrument.Text = getFilterButtonLabel(pnlInstrumentFinder.Visible);
    }

    private string getFilterButtonLabel(bool findControlVisible)
    {
        return "Filter  " + (findControlVisible ? "<<" : ">>");
    }

    protected void ddlAccount_DataBound(object sender, EventArgs e)
    {
        if (ddlAccount.Items.Count == 2)
            ddlAccount.SelectedIndex = 1;
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;
            gvCommissionRules.DataBind();

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }

    protected void btnResetFilter_Click(object sender, EventArgs e)
    {
        try
        {
            Server.Transfer("RuleOverview.aspx");

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.Message + Environment.NewLine + (ex.InnerException != null ? ex.InnerException.Message : "");
        }
    }
}
