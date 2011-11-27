using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    /// <summary>
    /// This class holds login information
    /// </summary>
    public abstract class Login : ILogin
    {
        protected Login()
        {
            this.isActive = true;
        }

        /// <summary>
        /// Unique identifier of login
        /// </summary>
        public virtual int Key  
        {
            get { return key; }
            set { key = value; }
        }       
        
        /// <summary>
        /// Get/set username
        /// </summary>
        public virtual string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public virtual bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public abstract LoginTypes LoginType { get; }

        /// <summary>
        /// Get/set time login was created
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }       

        /// <summary>
        /// Get/set time when login was last changed
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return lastUpdated; }
        }

        public virtual bool IsEmployee 
        {
            get { return false; } 
        }

        #region Private Variables

        private int key;
        private string userName;
        private bool isActive;
        private DateTime creationDate;
        private DateTime lastUpdated;

        #endregion

    }
}
