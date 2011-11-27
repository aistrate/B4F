<%@ Application Language="C#" %>

<script runat="server">

    void Application_Error(object sender, EventArgs e) 
    {
        UnhandledError error = new UnhandledError(HttpContext.Current);
        SafeSession.Current["LastUnhandledError"] = error;
        
        // This solves a nasty bug, which happened when <customErrors> was Off (or RemoteOnly in development 
        // mode). If logging in with a username that, by error, had no TotalGiro Login, after successful
        // authentication the user will have been trapped into the default ASP.NET error page, with no way 
        // to switch to a different username (except for closing down ALL browser windows, thus removing the
        // authentication cookie).
        if (error.Exception is B4F.TotalGiro.Stichting.Login.NoLoginForCurrentUserException)
            Response.Redirect("~/Authenticate/AppErrors.aspx");
    }

</script>
