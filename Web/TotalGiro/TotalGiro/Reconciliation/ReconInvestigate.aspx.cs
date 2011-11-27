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

public partial class Reconciliation_ReconInvestigate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Investigate Reconciliations";
            DatePickerEnd.SelectedDate = DateTime.Today;
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        ReportExecutionService reportExecutionService = new ReportExecutionService();   // Web Service proxy
        reportExecutionService.Credentials = new NetworkCredential("reportservice", "Asdfghjkl?", "kswh107");


        string reportPath = "/ReconCurrent/ReconInvest";
        string format = "PDF";


        // Render arguments
        byte[] result = null;
        string historyID = null;

        string encoding;
        string mimeType;
        string extension;
        Warning[] warnings = null;
        string[] streamIDs = null;

        ParameterValue[] parameters = new ParameterValue[4];
        parameters[0] = new ParameterValue();
        parameters[0].Name = "StartDate";
        parameters[1] = new ParameterValue();
        parameters[1].Name = "EndDate";
        parameters[2] = new ParameterValue();
        parameters[2].Name = "IsinCode";
        parameters[3] = new ParameterValue();
        parameters[3].Name = "AccountNumber";

        ExecutionInfo execInfo = new ExecutionInfo();
        ExecutionHeader execHeader = new ExecutionHeader();

        reportExecutionService.ExecutionHeaderValue = execHeader;
        
        execInfo = reportExecutionService.LoadReport(reportPath, historyID);

        String SessionId = reportExecutionService.ExecutionHeaderValue.ExecutionID;
        parameters[0].Value = DatePickerBegin.SelectedDate.ToShortDateString();
        parameters[1].Value = DatePickerEnd.SelectedDate.ToShortDateString();
        parameters[3].Value = ddlAccount.SelectedValue.ToString();
        parameters[2].Value = ddlInstrument.SelectedValue.ToString();
        reportExecutionService.SetExecutionParameters(parameters, "nl-nl");
        result = reportExecutionService.Render(format, null, out extension, out encoding, out mimeType, out warnings, out streamIDs);

        Response.ClearContent();
        Response.AppendHeader("content-length", result.Length.ToString());
        Response.ContentType = "application/pdf";
        Response.BinaryWrite(result);
        Response.End();
        //Response.Flush();
        //Response.Close();



    }
    protected void btnFilterAccount_Click(object sender, EventArgs e)
    {
        pnlAccountFinder.Visible = !pnlAccountFinder.Visible;
        pnlInstrumentFinder.Visible = false;
        setFilterButtonLabels();
    }

    protected void btnFilterInstrument_Click(object sender, EventArgs e)
    {
        pnlInstrumentFinder.Visible = !pnlInstrumentFinder.Visible;
        pnlAccountFinder.Visible = false;
        setFilterButtonLabels();
    }

    private string getFilterButtonLabel(bool findControlVisible)
    {
        return "Filter  " + (findControlVisible ? "<<" : ">>");
    }

    private void setFilterButtonLabels()
    {
        btnFilterAccount.Text = getFilterButtonLabel(pnlAccountFinder.Visible);
        btnFilterInstrument.Text = getFilterButtonLabel(pnlInstrumentFinder.Visible);
 }

}
