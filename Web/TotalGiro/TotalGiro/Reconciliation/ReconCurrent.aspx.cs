using System;
using System.IO;
using System.Net;
using System.Data;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using B4F.TotalGiro.ApplicationLayer.ReportExecutionEngine;

public partial class Reconciliation_ReconCurrent : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Current Reconciliations";
        }

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        ReportExecutionService reportExecutionService = new ReportExecutionService();   // Web Service proxy
        reportExecutionService.Credentials = new NetworkCredential("reportservice", "Asdfghjkl?", "kswh107");


        string reportPath = "/ReconCurrent/ReconCurrent";
        string format = "PDF";


        // Render arguments
        byte[] result = null;
        string historyID = null;

        string encoding;
        string mimeType;
        string extension;
        Warning[] warnings = null;
        string[] streamIDs = null;

        ExecutionInfo execInfo = new ExecutionInfo();
        ExecutionHeader execHeader = new ExecutionHeader();

        reportExecutionService.ExecutionHeaderValue = execHeader;

        execInfo = reportExecutionService.LoadReport(reportPath, historyID);

        String SessionId = reportExecutionService.ExecutionHeaderValue.ExecutionID;

        result = reportExecutionService.Render(format, null, out extension, out encoding, out mimeType, out warnings, out streamIDs);

        Response.ClearContent();
        Response.AppendHeader("content-length", result.Length.ToString());
        Response.ContentType = "application/pdf";
        Response.BinaryWrite(result);
        Response.End();
        //Response.Flush();
        //Response.Close();

    }
}
