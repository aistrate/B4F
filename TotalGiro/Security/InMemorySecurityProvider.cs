using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Security.Principal;
using System.Threading;

namespace B4F.TotalGiro.Security
{
    /// <summary>
    /// Security provider that takes security data from inside itself (no external data source is used). 
    /// Used only when running unit tests (NOT to be used in production). Helpful when users and roles need to be controlled inside unit tests.
    /// Specify that this security provider is the one to be used by calling 
    /// <see cref="B4F.TotalGiro.Security.SecurityManager.Initialize">SecurityManager.Initialize</see>() before the unit test.
    /// </summary>
    public class InMemorySecurityProvider : SecurityProvider
    {
        private string userName = "";
        private string roleNameList = "";

        /// <summary>
        /// Sets the current user and his/her associated roles. This information is local to an instance of this class
        /// (it's not persisted to the database). Only to be used inside unit tests.
        /// </summary>
        /// <param name="userName">The user to become the current one.</param>
        /// <param name="roleNameList">The roles the current user will be in.</param>
        public override void SetUser(string userName, string roleNameList)
        {
            this.userName = userName;
            this.roleNameList = roleNameList;

            IIdentity identity = new GenericIdentity(userName);
            IPrincipal user = new GenericPrincipal(identity, roleNameList.Split(','));

            Thread.CurrentPrincipal = user;
        }

        /// <summary>
        /// Checks if the current user is in the role specified.
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns><b>true</b> if the current user is in the specified role; otherwise, <b>false</b>.</returns>
        public override bool IsCurrentUserInRole(string roleName)
        {
            return ("," + roleNameList + ",").IndexOf("," + roleName + ",") >= 0;
        }

        /// <summary>
        /// Gets the current user. This user is the one set previously with 
        /// method <see cref="B4F.TotalGiro.Security.InMemorySecurityProvider.SetUser">SetUser</see>.
        /// </summary>
        public override string CurrentUser
        {
            get { return userName; }
        }

        /// <summary>
        /// Gets an array with the one user name previously set by 
        /// method <see cref="B4F.TotalGiro.Security.InMemorySecurityProvider.SetUser">SetUser</see>.
        /// </summary>
        /// <returns>An array with one user name.</returns>
        public override string[] GetAllUserNames()
        {
            if (userName != "")
                return new string[] { userName };
            else
                return new string[0];
        }
    }
}
