using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.BackOffice.CorporateActions;

public partial class InstrumentConversionOverview : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ctlInstrumentFinder.Search += new EventHandler(ctlInstrumentFinder_Search);
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Instrument Conversions Overview";
            gvConversions.Sort("ChangeDate", SortDirection.Ascending);
        }
    }

    protected void ctlInstrumentFinder_Search(object sender, EventArgs e)
    {
        gvConversions.Visible = true;
        gvConversions.DataBind();
    }

    protected void lbtDetails_Command(object sender, CommandEventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("instrumentConversionID={0}&Edit={1}", (string)e.CommandArgument, true));
            Response.Redirect(string.Format("~/BackOffice/CorporateActions/InstrumentConversionDetails.aspx{0}", qStr));
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

    protected void btnNewConversion_Click(object sender, EventArgs e)
    {
        try
        {
            string qStr = QueryStringModule.Encrypt(string.Format("Edit={0}", false));
            Response.Redirect(string.Format("~/BackOffice/CorporateActions/InstrumentConversionDetails.aspx{0}", qStr));

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }
}
