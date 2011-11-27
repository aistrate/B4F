<%@ Application Language="C#" %>

<script runat="server">

    public override void Init()
    {
        base.Init();

        log4net.Config.XmlConfigurator.Configure();
        
    }
    private void Application_Error(object sender, EventArgs e) 
    {
        Type exceptionType = getInnermostException(Server.GetLastError()).GetType();

        if (exceptionType == typeof(System.Security.SecurityException))
            Server.Transfer("~/Security/NotAuthorized.aspx");
        else if (exceptionType == typeof(System.Security.Authentication.AuthenticationException))
            Server.Transfer("~/Security/NotRegistered.aspx");
        else
            Utility.LastUnhandledError = new UnhandledError(HttpContext.Current);
    }

    private Exception getInnermostException(Exception ex)
    {
        if (ex.InnerException != null)
            return getInnermostException(ex.InnerException);
        else
            return ex;
    }
       
</script>
