using System;

namespace B4F.TotalGiro.Security
{
    /// <summary>
    /// Abstract class used as a template for concrete security provider classes.
    /// Security providers actually do the work that class <see cref="B4F.TotalGiro.Security.SecurityManager">SecurityManager</see>
    /// exposes an interface for to the outside world. Unlike <b>SecurityManager</b>, this is not a <i>static</i> class.
    /// </summary>
    public abstract class SecurityProvider
    {
        /// <summary>
        /// Sets the current user for the application.
        /// </summary>
        /// <param name="userName">The user to become the current one.</param>
        /// <param name="roleNameList">The roles the current user will be in.</param>
        public abstract void SetUser(string userName, string roleNameList);

        /// <summary>
        /// Checks if the current user is in the role specified. 
        /// </summary>
        /// <param name="roleName">A role name.</param>
        /// <returns><b>true</b> if the current user is in the specified role; otherwise, <b>false</b>.</returns>
        public abstract bool IsCurrentUserInRole(string roleName);

        /// <summary>
        /// Gets the current user in the application.
        /// </summary>
        public abstract string CurrentUser { get; }

        public virtual void SignOutUser()
        {
        }

        /// <summary>
        /// Gets all user names in the system.
        /// </summary>
        /// <returns>An array of user names.</returns>
        public abstract string[] GetAllUserNames();

        public virtual DateTime GetLastLoginDate(string userName)
        {
            return DateTime.MinValue;
        }

        public virtual bool UserExists(string userName)
        {
            return false;
        }

        public virtual string GeneratePassword(int length)
        {
            return GeneratePassword(length, 0);
        }

        public virtual string GeneratePassword(int length, int nonAlphaNumChars)
        {
            throw new ApplicationException("Current security provider cannot generate passwords.");
        }

        public virtual void SetPassword(string userName, string password)
        {
        }

        public virtual void CreateUser(string userName, string password, string email, bool isActive)
        {
            throw new ApplicationException("Current security provider cannot create users.");
        }

        public virtual void DeleteUser(string userName)
        {
        }

        public virtual void SetActive(string userName, bool isActive)
        {
            throw new ApplicationException("Current security provider cannot activate users.");
        }

        public virtual void AddUserToRole(string userName, string roleName)
        {
            throw new ApplicationException("Current security provider cannot add users to roles.");
        }

        public virtual bool IsUserLockedOut(string userName)
        {
            return false;
        }

        public virtual bool UnlockUser(string userName)
        {
            return true;
        }
    }
}
