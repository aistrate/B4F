using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace B4F.TotalGiro.Security
{
    /// <summary>
    /// Security provider that takes security data from the HTTP context of ASP.NET, and the current membership and role providers 
    /// set for the application (in <i>Web.Config</i>).
    /// Used when running code in production (live on a web server). This is the <b>default</b> security provider used by 
    /// <see cref="B4F.TotalGiro.Security.SecurityManager">SecurityManager</see>.
    /// </summary>
    public class SqlSecurityProvider : SecurityProvider
    {
        /// <summary>
        /// Normally this would set the current user for the application, but for this particular security provider it just throws an exception.
        /// That is because the currently logged-in user in a production environment shouldn't be changeable programatically.
        /// </summary>
        /// <param name="userName">The user to become the current one (ignored).</param>
        /// <param name="roleNameList">The roles the current user will be in (ignored).</param>
        public override void SetUser(string userName, string roleNameList)
        {
            throw new ApplicationException(
                "Not allowed to change the user in a live web setup. When unit testing please call SecurityManager.Initialize() first");
        }

        /// <summary>
        /// Checks if the currently logged-in user is in the role specified. 
        /// It uses the role provider set for the application (in <i>Web.Config</i>).
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns><b>true</b> if the current user is in the specified role; otherwise, <b>false</b>.</returns>
        public override bool IsCurrentUserInRole(string roleName)
        {
            // Roles.IsUserInRole() doesn't break if HttpContext.Current is null, 
            // but we check for that anyway, as a test on whether this is a live web setup
            AssertCurrentIdentityExists();
            return Roles.IsUserInRole(roleName);
        }

        /// <summary>
        /// Gets the currently logged-in user from the HTTP context.
        /// </summary>
        public override string CurrentUser
        {
            get
            {
                AssertCurrentIdentityExists();
                return HttpContext.Current.User.Identity.Name;
            }
        }

        public override void SignOutUser()
        {
            FormsAuthentication.SignOut();
        }

        protected void AssertCurrentIdentityExists()
        {
            if (!CurrentIdentityExists)
                throw new ApplicationException("Retrieving CurrentUser: HTTP context or user info not available");
        }

        protected bool CurrentIdentityExists
        {
            get { return HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null; }
        }

        /// <summary>
        /// Gets all user names in the system. It uses the membership provider set for the application (in <i>Web.Config</i>).
        /// </summary>
        /// <returns>An array of user names.</returns>
        public override string[] GetAllUserNames()
        {
            MembershipUserCollection userColl = Membership.GetAllUsers();

            string[] userNames = new string[userColl.Count];
            int i = 0;

            foreach (MembershipUser user in userColl)
                if (user.IsApproved)
                    userNames[i++] = user.UserName;

            return userNames;
        }

        public override DateTime GetLastLoginDate(string userName)
        {
            userName = userName.Trim();
            MembershipUserCollection userColl = Membership.FindUsersByName(userName);
            if (userColl.Count > 0)
                return userColl[userName].LastLoginDate;

            return DateTime.MinValue;
        }

        public override bool UserExists(string userName)
        {
            return Membership.GetUser(userName) != null;
        }

        public override string GeneratePassword(int length, int nonAlphaNumChars)
        {
            const int maxLength = 32;

            if (length > maxLength)
                throw new ApplicationException(string.Format("Password length must be between 1 and {0} characters.", maxLength));

            length = Math.Max(1, length);
            nonAlphaNumChars = Math.Max(0, Math.Min(length, nonAlphaNumChars));

            int alphaNumChars = length - nonAlphaNumChars;

            string password = generatePassword(@"[^2-9a-kmnp-zA-HJ-NP-Z]", alphaNumChars, 0);

            if (nonAlphaNumChars > 0)
                password += generatePassword(@"[^~@#$%^&*()=+\[\]{}\\/<>?]", nonAlphaNumChars, nonAlphaNumChars);

            return password;
        }

        private string generatePassword(string excludeRegex, int length, int nonAlphaNumChars)
        {
            const int multiplier = 4;

            string password = "";

            do
            {
                password = Membership.GeneratePassword(multiplier * length, multiplier * nonAlphaNumChars);
                password = Regex.Replace(password, excludeRegex, "");
                password = Regex.Replace(password, @"(?<chars>.+)\k<chars>", @"${chars}");
            }
            while (password.Length < length);

            return password.Substring(0, length);
        }

        public override void SetPassword(string userName, string password)
        {
            MembershipUser user = Membership.GetUser(userName);
            if (user != null)
            {
                string tempPassword = user.ResetPassword();
                user.ChangePassword(tempPassword, password);
            }
            else
                throw new ApplicationException("User name not found.");
        }
        
        public override void CreateUser(string userName, string password, string email, bool isActive)
        {
            MembershipCreateStatus status;
            Membership.CreateUser(userName, password, email, null, null, isActive, out status);
            if (status != MembershipCreateStatus.Success)
                throw new MembershipCreateUserException(status);
        }

        public override void DeleteUser(string userName)
        {
            Membership.DeleteUser(userName, true);
        }

        public override void SetActive(string userName, bool isActive)
        {
            MembershipUser user = Membership.GetUser(userName);
            if (user != null)
            {
                user.IsApproved = isActive;
                Membership.UpdateUser(user);
            }
            else
                throw new ApplicationException("User name not found.");
        }

        public override void AddUserToRole(string userName, string roleName)
        {
            Roles.AddUserToRole(userName, roleName);
        }

        public override bool IsUserLockedOut(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            return (user != null ? user.IsLockedOut : false);
        }

        public override bool UnlockUser(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            if (user != null)
                return (user.IsLockedOut ? user.UnlockUser() : true);
            else
                throw new ApplicationException("User name not found.");
        }
    }
}
