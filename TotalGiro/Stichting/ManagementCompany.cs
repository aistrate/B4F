using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Class representing a company involved in Asset Management
    /// </summary>
    public abstract class ManagementCompany : IManagementCompany
    {



        public ManagementCompany() { }

        public bool ShowLogoByDefault { get; set; }
        public IAccountTypeExternal DefaultAccountforTransfer { get; set; }
        public ICurrency BaseCurrency { get; set; }
        public IEffectenGiro StichtingDetails { get; set; }
        public IGLAccount ManagementFeeIncomeAccount { get; set; }
        public IGLAccount ManagementFeeFixedCostsAccount { get; set; }
        public INostroAccount OwnAccount { get; set; }
        public int Key { get; set; }
        public ITradingAccount TradingAccount { get; set; }
        public string CompanyName { get; set; }
        public string InitialClientUserRoles { get; set; }
        public string PdfReportsFolder { get; set; }
        public string Initials { get; set; }

        public abstract ManagementCompanyType CompanyType { get; }

        /// <summary>
        /// Flag for ManagementCompany
        /// </summary>
        public virtual bool IsStichting
        {
            get
            {
                return (CompanyType == ManagementCompanyType.EffectenGiro);
            }
        }

        /// <summary>
        /// Details data of the object that can be serialized
        /// </summary>
        public virtual ManagementCompanyDetails Details
        {
            get
            {
                return new ManagementCompanyDetails(this);
            }
        }

        public string[] InitialClientUserRoleList
        {
            get
            {
                return InitialClientUserRoles != null && InitialClientUserRoles.Trim() != "" ?
                            (from role in InitialClientUserRoles.Split(',') select role.Trim()).ToArray() : 
                             new string[] {};
            }
        }

        #region Equality

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="lhs">First instrument</param>
        /// <param name="rhs">Second instrument</param>
        /// <returns>Flag</returns>
        public static bool operator ==(ManagementCompany lhs, IManagementCompany rhs)
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
        /// Overridden unequality operator
        /// </summary>
        /// <param name="lhs">First instrument</param>
        /// <param name="rhs">Second instrument</param>
        /// <returns>Flag</returns>
        public static bool operator !=(ManagementCompany lhs, IManagementCompany rhs)
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

        public override bool Equals(object obj)
        {
            if (!(obj is IManagementCompany))
            {
                return false;
            }
            return this == (IManagementCompany)obj;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overridden creation of a hashcode.
        /// </summary>
        /// <returns>Unique number</returns>
        public override int GetHashCode()
        {
            return Key;
        }

        /// <summary>
        /// Overridden composition of a name for an object of this class
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return CompanyName;
        }

        #endregion

        #region Private Variables

        private int tradingAccountKey;

        #endregion

    }
}
