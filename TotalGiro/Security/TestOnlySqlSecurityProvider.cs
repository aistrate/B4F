using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using System.Threading;

namespace B4F.TotalGiro.Security
{
    /// <summary>
    /// Security provider that takes security data from a manually-created HTTP context, and the current membership and role providers 
    /// set for the application (in <i>Web.Config</i>). Used only when running unit tests (NOT to be used in production).
    /// Specify that this security provider is the one to be used by calling 
    /// <see cref="B4F.TotalGiro.Security.SecurityManager.Initialize">SecurityManager.Initialize</see>() before the unit test.
    /// </summary>
    public class TestOnlySqlSecurityProvider: SqlSecurityProvider
    {
        /// <summary>
        /// Sets the current user for unit tests. 
        /// </summary>
        /// <param name="userName">The user to become the current one.</param>
        /// <param name="roleNameList">The roles the current user will be in. The parameter is ignored by this security provider; 
        /// the role provider set for the application (in <i>Web.Config</i>) will be used to retrieve roles.</param>
        public override void SetUser(string userName, string roleNameList)
        {
            HttpRequest request = new HttpRequest("", "http://www.bits4finance.com", "");
            HttpContext.Current = new HttpContext(request, new HttpResponse(new StringWriter()));

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                2, userName, DateTime.Now, DateTime.Now.AddMinutes(30), false, "", "/TotalGiro");
            IIdentity identity = new FormsIdentity(ticket);
            IPrincipal user = new RolePrincipal(identity);

            Thread.CurrentPrincipal = user;
            HttpContext.Current.User = user;
        }

        /// <summary>
        /// Checks if the current user 
        /// (set previously with <see cref="B4F.TotalGiro.Security.TestOnlySqlSecurityProvider.SetUser">SetUser</see>) is in the role specified. 
        /// It uses the role provider set for the application (in <i>Web.Config</i>).
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns><b>true</b> if the current user is in the specified role; otherwise, <b>false</b>.</returns>
        public override bool IsCurrentUserInRole(string roleName)
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null || HttpContext.Current.User.Identity == null)
                throw new ApplicationException("IsCurrentUserInRole: HTTP context or user info not available");
            else
                return HttpContext.Current.User.IsInRole(roleName);
        }
    }
}
