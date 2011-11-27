using System;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// The Account class is an abstract class. It is on top of the hierarchy, since all other account classes inherit from the Account class.
    /// The class only provides some common properties and has the basic functionality for comparison.
    /// </summary>
    abstract public class Account : TotalGiroBase<IAccount>, IAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Account">Account</see> class.
        /// </summary>
        protected Account() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.Account">Account</see> class.
        /// </summary>
        /// <param name="number">The Account's number</param>
        /// <param name="shortName">Shortname of the account</param>
        public Account(string number, string shortName)
        {
            this.Number = number;
			this.ShortName = shortName;
        }


		#region IAccount Members

		/// <summary>
        /// The AccountType defines the type of account. For example customer or counterparty.
        /// </summary>
        public abstract AccountTypes AccountType { get; }

        /// <summary>
        /// Does this account belong to a management company.
        /// </summary>
        public virtual bool IsCompanyAccount
        {
            get { return false; }
        }

        public virtual bool IsAccountTypeCustomer
        {
            get
            {
                return ((AccountType == AccountTypes.Customer)
                          || (AccountType == AccountTypes.Nostro)
                          || (AccountType == AccountTypes.Crumble)
                          || (AccountType == AccountTypes.VirtualFundHoldingsAccount)
                          || (AccountType == AccountTypes.VirtualFundTradingAccount));
            }
        }

        /// <summary>
        /// Abstract property to get the Base currency of the account
        /// </summary>
        public abstract ICurrency BaseCurrency { get; set; }			

        /// <summary>
        /// The Account number of the account
        /// </summary>
		public virtual string Number
		{
			get { return this.number; }
			set { this.number = value; }
		}

        /// <summary>
        /// The short name of the account
        /// </summary>
		public virtual string ShortName
		{
			get { return this.shortName; }
			set { this.shortName = value; }
		}

        /// <summary>
        /// Is the Account active
        /// </summary>
        public virtual string FullName { get; set; }

        /// <summary>
        /// The date that the account was created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return (creationDate.HasValue ? creationDate.Value : DateTime.MinValue); }
            set { creationDate = value; }
        }

        /// <summary>
        /// The date that the account was last modified.
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        public virtual string DisplayNumberWithName
        {
            get { return string.Format("{0} ({1})", Number, ShortName); }
        }

        /// <summary>
        /// Is this an internal account. Does it reside in the totalgiro system.
        /// Internal means that positions and other important issues are dealt with for the account.
        /// </summary>
        public virtual bool IsInternal
        {
            get { return false; }
        }

        /// <summary>
        /// The way how the account stores its positions
        /// </summary>
        public virtual StorePositionsLevel StorePositions
        {
            get { return StorePositionsLevel.Not; }
        }

        public virtual ICounterAccount CounterAccount
        {
            get { return counterAccount; }
            set { counterAccount = value; }
        }

        public virtual AccountStati Status
        {
            get { return status; }
            set { status = value; }
        }

        public virtual DateTime LastDateStatusChanged
        {
            get { return lastDateStatusChanged.HasValue ? this.lastDateStatusChanged.Value: DateTime.MinValue; }
            set { lastDateStatusChanged = value; }
        }

        public virtual bool NeedsAttention
        {
            get { return needsAttention; }
            set { needsAttention = value; }
        }

		#endregion


		#region Equality

        /// <summary>
        /// Method to compare whether two account objects are identical
        /// </summary>
        /// <param name="lhs">Left hands side account to be compared</param>
        /// <param name="rhs">Right hands side account to be compared</param>
        /// <returns>Returns True when both instances are equal</returns>
		public static bool operator ==(Account lhs, IAccount rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (lhs.Key == rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Method to compare whether two account objects are not identical
        /// </summary>
        /// <param name="lhs">Left hands side account to be compared</param>
        /// <param name="rhs">Right hands side account to be compared</param>
        /// <returns>Returns True when both instances are not equal</returns>
        public static bool operator !=(Account lhs, IAccount rhs)
		{
			if ((Object)lhs == null || (Object)rhs == null)
			{
				if ((Object)lhs == null && (Object)rhs == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				if (lhs.Key != rhs.Key)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		#endregion


        #region Overrides

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// Override of the ToString method, to display a descriptive name.
        /// </summary>
        /// <returns>It returns the account number of the current account instance</returns>
        public override string ToString()
		{
			return this.Number;
		}

        #endregion


        #region PrivateVariables

		private string number;
		private string shortName;
        private DateTime lastUpdated = DateTime.MinValue;
        private ICounterAccount counterAccount;
        private bool isActive = true;
        private AccountStati status = AccountStati.Active;
        private DateTime? lastDateStatusChanged;
        private bool needsAttention;
        protected AccountTypes accountType;
        private DateTime? creationDate;

		#endregion
	}
}
