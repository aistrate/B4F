using System;

namespace B4F.TotalGiro.Security
{
    /// <summary>
    /// Specifies the security provider used by the application, that is, the source of security data (users and roles, as well as the current user).
    /// Used as a parameter for <see cref="B4F.TotalGiro.Security.SecurityManager.Initialize">SecurityManager.Initialize</see>().
    /// </summary>
    public enum SecuritySetupType
    {
        /// <summary>
        /// Specifies <see cref="B4F.TotalGiro.Security.SqlSecurityProvider">SqlSecurityProvider</see> as a security provider.
        /// Security data is coming from the HTTP context of ASP.NET, and the current membership and role providers 
        /// set for the application (in <i>Web.Config</i>).
        /// Used when running code in production (live on a web server).
        /// </summary>
        LiveWebWithSqlProvider = 0,
        /// <summary>
        /// Specifies <see cref="B4F.TotalGiro.Security.TestOnlySqlSecurityProvider">TestOnlySqlSecurityProvider</see> as a security provider.
        /// Security data is coming from a manually-created HTTP context, and the current membership and role providers 
        /// set for the application (in <i>Web.Config</i>).
        /// Used only when running unit tests (NOT to be used in production).
        /// </summary>
        TestOnlyWithSqlProvider,
        /// <summary>
        /// Specifies <see cref="B4F.TotalGiro.Security.InMemorySecurityProvider">InMemorySecurityProvider</see> as a security provider.
        /// Security data is coming from inside this class itself (no external data source is used). 
        /// Used only when running unit tests (NOT to be used in production). Helpful when users and roles need to be controlled inside unit tests.
        /// </summary>
        InMemory
    }

    /// <summary>
    /// The <b>main entry point</b> for security-related operations inside the application.
    /// It uses one of the security providers, and offers a provider-neutral interface for security operations.
    /// </summary>
    public static class SecurityManager
    {
        private static SecurityProvider securityProvider = new SqlSecurityProvider();

        /// <summary>
        /// Only used in unit tests, enabling them to control the current user, and/or the available users and roles.
        /// If not called, the default value will be 
        /// <b>SecuritySetupType.</b><see cref="B4F.TotalGiro.Security.SecuritySetupType.LiveWebWithSqlProvider">LiveWebWithSqlProvider</see>,
        /// which is what is used in production (live on a web server). Can be called repeatedy inside a test, with different values.
        /// </summary>
        /// <param name="setupType">Specifies the security provider to be used. 
        /// See <see cref="B4F.TotalGiro.Security.SecuritySetupType">SecuritySetupType</see> 
        /// for the available choices.</param>
        public static void Initialize (SecuritySetupType setupType)
        {
            switch (setupType)
            {
                case SecuritySetupType.LiveWebWithSqlProvider:
                    securityProvider = new SqlSecurityProvider();
                    break;
                case SecuritySetupType.TestOnlyWithSqlProvider:
                    securityProvider = new TestOnlySqlSecurityProvider();
                    break;
                case SecuritySetupType.InMemory:
                    securityProvider = new InMemorySecurityProvider();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the current user for the application (retrievable later through property 
        /// <see cref="B4F.TotalGiro.Security.SecurityManager.CurrentUser">CurrentUser</see>). 
        /// Only to be used in unit tests.
        /// </summary>
        /// <param name="userName">The user to become the current one.</param>
        /// <param name="roleNameList">The roles the current user will be in. Used only if the security provider is in-memory 
        /// (see <b>SecuritySetupType.</b><see cref="B4F.TotalGiro.Security.SecuritySetupType.InMemory">InMemory</see>); 
        /// otherwise is ignored, and the role provider set for the application (in <i>Web.Config</i>) will be used to retrieve roles.</param>
        public static void SetUser(string userName, string roleNameList)
        {
            securityProvider.SetUser(userName, roleNameList);
        }
        
        /// <summary>
        /// Checks if <see cref="B4F.TotalGiro.Security.SecurityManager.CurrentUser">CurrentUser</see> is in the role specified. 
        /// It uses the role provider set for the application (in <i>Web.Config</i>), unless the security provider is in-memory 
        /// (see <b>SecuritySetupType.</b><see cref="B4F.TotalGiro.Security.SecuritySetupType.InMemory">InMemory</see>).
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns><b>true</b> if the current user is in the specified role; otherwise, <b>false</b>.</returns>
        public static bool IsCurrentUserInRole(string roleName)
        {
            return securityProvider.IsCurrentUserInRole(roleName);
        }

        /// <summary>
        /// Gets the current user in the application. The source of this information depends on the current security provider 
        /// (see <see cref="B4F.TotalGiro.Security.SecurityManager.Initialize">SecurityManager.Initialize</see>).
        /// In production (live on a web server), this is the currently logged-on user. In unit tests, it is the user previously set with 
        /// method <see cref="B4F.TotalGiro.Security.SecurityManager.SetUser">SetUser</see>.
        /// </summary>
        public static string CurrentUser
        {
            get { return securityProvider.CurrentUser; }
        }

        public static void SignOutUser()
        {
            securityProvider.SignOutUser();
        }

        /// <summary>
        /// Gets all user names in the system. It uses the membership provider set for the application (in <i>Web.Config</i>),
        /// unless the security provider is in-memory 
        /// (see <b>SecuritySetupType.</b><see cref="B4F.TotalGiro.Security.SecuritySetupType.InMemory">InMemory</see>).
        /// </summary>
        /// <returns>An array of user names.</returns>
        public static string[] GetAllUserNames()
        {
            return securityProvider.GetAllUserNames();
        }

        public static DateTime GetLastLoginDate(string userName)
        {
            return securityProvider.GetLastLoginDate(userName);
        }

        public static bool UserExists(string userName)
        {
            return securityProvider.UserExists(userName);
        }

        public static string GeneratePassword(int length)
        {
            return securityProvider.GeneratePassword(length);
        }

        public static string GeneratePassword(int length, int nonAlphaNumChars)
        {
            return securityProvider.GeneratePassword(length, nonAlphaNumChars);
        }

        public static void SetPassword(string userName, string password)
        {
            securityProvider.SetPassword(userName, password);
        }

        public static void CreateUser(string userName, string password, string email, bool isActive)
        {
            securityProvider.CreateUser(userName, password, email, isActive);
        }

        public static void DeleteUser(string userName)
        {
            securityProvider.DeleteUser(userName);
        }

        public static void SetActive(string userName, bool isActive)
        {
            securityProvider.SetActive(userName, isActive);
        }

        public static void AddUserToRole(string userName, string roleName)
        {
            securityProvider.AddUserToRole(userName, roleName);
        }

        public static bool IsUserLockedOut(string userName)
        {
            return securityProvider.IsUserLockedOut(userName);
        }

        public static bool UnlockUser(string userName)
        {
            return securityProvider.UnlockUser(userName);
        }
    }
}
