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

public partial class Test_TestManagementFee : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Test Management Fee";
            ddlAccount.SelectedValue = 957.ToString();
            dpDateFrom.SelectedDate = new DateTime(2006, 1, 1);
            dpDateTo.SelectedDate = new DateTime(2006, 3, 31);
            ddlAccount.DataBind();
            doSearch();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        doSearch();
    }

    private void doSearch()
    {
        dpDateFrom.IsExpanded = false;
        dpDateTo.IsExpanded = false;
        hdnDateFrom.Value = dpDateFrom.SelectedDate.ToShortDateString();
        hdnDateTo.Value = dpDateTo.SelectedDate.ToShortDateString();
        gvValuations.DataBind();
        gvFeeNotas.DataBind();
    }
}
