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
using B4F.TotalGiro.ApplicationLayer.GeneralLedger;

public partial class TestRegex : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DataSet ds = BankStatementLinesAdapter.GetMovements();
        lblCounter.Text = string.Format("Count: <b>{0}</b>", ds.Tables[0].Rows.Count);
        gvMovements.Sort("DateString", SortDirection.Descending);
    }
}
