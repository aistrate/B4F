using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using System.Runtime.Serialization;
using System.Security.Permissions;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// This enumeration lists the type of manaement companies
    /// </summary>
    public enum ManagementCompanyType
    {
        /// <summary>
        /// This is the stichting
        /// </summary>
        EffectenGiro,
        /// <summary>
        /// This is an asset managing company
        /// </summary>
        AssetManager
    }

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.ManagementCompany">ManagementCompany</see> class
    /// </summary>
    public interface IManagementCompany
    {
        bool Equals(object obj);
        bool IsStichting { get; }
        bool ShowLogoByDefault { get; set; }
        IAccountTypeExternal DefaultAccountforTransfer { get; set; }
        ICurrency BaseCurrency { get; set; }
        IEffectenGiro StichtingDetails { get; set; }
        IGLAccount ManagementFeeFixedCostsAccount { get; set; }
        IGLAccount ManagementFeeIncomeAccount { get; set; }
        INostroAccount OwnAccount { get; set; }
        int GetHashCode();
        int Key { get; set; }
        ITradingAccount TradingAccount { get; set; }
        ManagementCompanyDetails Details { get; }
        ManagementCompanyType CompanyType { get; }
        string CompanyName { get; set; }
        string InitialClientUserRoles { get; set; }
        string PdfReportsFolder { get; set; }
        string ToString();
        string[] InitialClientUserRoleList { get; }
        string Initials { get; set; }

    }

    [Serializable]
    public struct ManagementCompanyDetails : ISerializable 
    {
        public ManagementCompanyDetails(IManagementCompany company)
        {
            Key = company.Key;
            BaseCurrencyID = company.BaseCurrency.Key;
            CompanyName = company.CompanyName;
            IsStichting = company.IsStichting;
        }

        public ManagementCompanyDetails(SerializationInfo info, StreamingContext context)
        {
            Key = (int)info.GetValue("Key", typeof(int));
            BaseCurrencyID = (int)info.GetValue("BaseCurrencyID", typeof(int));
            CompanyName = (string)info.GetValue("CompanyName", typeof(string));
            IsStichting = (bool)info.GetValue("IsStichting", typeof(bool));
        }

        public int Key;
        public int BaseCurrencyID;
        public string CompanyName;
        public bool IsStichting;

        #region ISerializable Members

        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key);
            info.AddValue("BaseCurrencyID", BaseCurrencyID);
            info.AddValue("CompanyName", CompanyName);
            info.AddValue("IsStichting", IsStichting);
        }

        #endregion
    }
}
