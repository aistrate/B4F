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

public partial class AppErrors : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((EG)this.Master).setHeaderText = "Application Error";
        }
    }
    
    protected void btnDetails_Click(object sender, EventArgs e)
    {
        if (switchDetailsVisible())
        {
            btnDetails.Text = "Hide Details <<";
            showErrorDetails();
        }
        else
        {
            btnDetails.Text = "Show Details >>";
            pnlErrorInfo.Visible = false;
            lblErrorDetails.Text = "";
        }
    }

    private bool switchDetailsVisible()
    {
        bool detailsVisible = (hidDetailsVisible.Value == "true");

        hidDetailsVisible.Value = (detailsVisible ? "false" : "true");

        return !detailsVisible;
    }

    private void showErrorDetails()
    {
        if (Utility.LastUnhandledError != null)
        {
            UnhandledError error = Utility.LastUnhandledError;
            
            pnlErrorInfo.Visible = true;
            lblExecutionPath.Text = makeBold(error.ExecutionPath);
            lblTimestamp.Text = makeBold(error.Timestamp.ToString());
            lblUserName.Text = makeBold(error.UserName);
            lblUserHostName.Text = makeBold(error.UserHostName);
            
            if (error.Error != null)
                lblErrorDetails.Text = formatExceptionList(error.Error);
            else
                lblErrorDetails.Text = "";
        }
        else
        {
            pnlErrorInfo.Visible = false;
            lblErrorDetails.Text = "<br><b>No error information was found.</b>";
        }
    }

    private string makeBold(string htmlText)
    {
        return "<b>" + htmlText + "</b>";
    }

    private string formatExceptionList(Exception ex)
    {
        string currentExceptionHtml = string.Format("<br><h5 style='color:Firebrick'>{0}</h5><b>[{1}]</b><br>{2}",
                                                    ex.Message, ex.GetType().ToString(), formatStackTrace(ex.StackTrace));
        
        if (ex.InnerException != null)
            return formatExceptionList(ex.InnerException) + "<p>" + currentExceptionHtml;
        else
            return currentExceptionHtml;
    }

    private string formatStackTrace(string stackTrace)
    {
        // make every stack line start with '--- '
        stackTrace = "--- " + stackTrace.Replace("\n", "<br>---");

        // make source pathnames bold
        string result = stackTrace;
        int start = 0;
        int end = 0;
        while (start >= 0)
        {
            start = stackTrace.IndexOf(" in ", end);
            if (start >= 0)
            {
                end = stackTrace.IndexOf("--- ", start);
                if (end == -1)
                    end = stackTrace.Length;

                string sourceName = stackTrace.Substring(start + 4, end - start - 4);
                result = result.Replace(sourceName, "<b>" + sourceName + "</b>");
            }
        }

        return result;
    }
}
