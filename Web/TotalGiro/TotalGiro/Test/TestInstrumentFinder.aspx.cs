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
using B4F.TotalGiro.Instruments;

public partial class TestInstrumentFinder : System.Web.UI.Page
{
    //protected void Page_Init(object sender, EventArgs e)
    //{
    //    ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
    //}
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Test";
            gvInstruments.Sort("Name", SortDirection.Ascending);
        }
    }

    //protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    //{
    //    gvInstruments.DataBind();
    //}
}
