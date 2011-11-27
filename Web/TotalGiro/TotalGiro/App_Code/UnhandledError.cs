using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class UnhandledError
{
    string executionPath;
    DateTime timestamp;
    string userName;
    string userHostName;
    Exception error;

    public UnhandledError(HttpContext context)
    {
        executionPath = context.Request.AppRelativeCurrentExecutionFilePath.Substring(1);
        timestamp = context.Timestamp;
        userName = context.User.Identity.Name;
        userHostName = context.Request.UserHostName;
        error = context.Server.GetLastError();

        if (error != null && error.GetType() == typeof(System.Web.HttpUnhandledException))
            error = error.InnerException;
    }

    public string ExecutionPath { get { return executionPath; } }
    public DateTime Timestamp { get { return timestamp; } }
    public string UserName { get { return userName; } }
    public string UserHostName { get { return userHostName; } }
    public Exception Error { get { return error; } }
}
