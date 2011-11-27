using System;
using System.Web.UI.WebControls;

public partial class RebalanceExcludeInstruments : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
		{
            ((EG)this.Master).setHeaderText = "Check Instrument Prices for Rebalance";

            //if (Session["InstrumentsToExclude"] != null)
            //{
            //    int[] ids = (int[])Session["InstrumentsToExclude"];
            //    if (ids != null && ids.Length > 0)
            //        gvInstrumentsToExclude.SetSelectedIds(ids);
            //}
            gvInstrumentsToExclude.Sort("DisplayName", SortDirection.Ascending);
        }
    }

    ////Go ahead -> Process instructions
    //protected void btnOK_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        int[] instrumentsToExclude = gvInstrumentsToExclude.GetSelectedIds();
    //        Session["InstrumentsToExclude"] = instrumentsToExclude;
    //        goBack();
    //    }
    //    catch (Exception ex)
    //    {
    //        lblError.Text = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
    //    }
    //}

    //private void goBack()
    //{
    //    try
    //    {
    //        String scriptString = "<script language=JavaScript>history.go(document.getElementById('hdnClientCounter').value);";
    //        scriptString += "</";
    //        scriptString += "script>";
    //        Page.RegisterClientScriptBlock("keyClientBlock", scriptString);

    //    }
    //    catch (Exception ex)
    //    {
    //        lblError.Text = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
    //    }
    //}
}
