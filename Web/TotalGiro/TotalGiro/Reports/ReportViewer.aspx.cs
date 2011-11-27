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

public partial class Reports_ReportViewer : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string extension = "";
        Response.ContentType = "application/pdf";                       // set the MIME type here
        Response.AddHeader("content-disposition", extension);           //Response.AddHeader("content-disposition", "attachment; filename=Test." + extension);
        Response.BinaryWrite((byte[])Session["report"]);
    }
}
