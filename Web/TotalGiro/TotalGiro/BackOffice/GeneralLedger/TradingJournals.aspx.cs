using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class TradingJournals : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Trading Statement Journals";
        }
    }


    protected void lbtBookings_Command(object sender, CommandEventArgs e)
    {
        try
        {
            Session["journalId"] = int.Parse((string)e.CommandArgument);
            Response.Redirect("~/BackOffice/GeneralLedger/TradingBookings.aspx");
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = Utility.GetCompleteExceptionMessage(ex);
        }
    }

  

    
}
