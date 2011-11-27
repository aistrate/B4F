using System;
using System.Web;
using System.Reflection;

public class UnhandledError
{
    public UnhandledError(HttpContext context)
    {
        ExecutionPath = context.Request.Url.PathAndQuery;
        Timestamp = context.Timestamp;
        UserName = context.User != null ? context.User.Identity.Name : "";
        UserHostName = context.Request.UserHostName;
        Exception = context.Server.GetLastError();

        if (Exception != null && Exception.GetType() == typeof(HttpUnhandledException))
            Exception = Exception.InnerException;

        if (Exception is TargetInvocationException && Exception.InnerException != null)
            Exception = Exception.InnerException;
    }

    public string ExecutionPath { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string UserName { get; private set; }
    public string UserHostName { get; private set; }
    public Exception Exception { get; private set; }
}
