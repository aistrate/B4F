using System;
using System.Text.RegularExpressions;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.Stichting.Login
{
    public enum LoginPersonStatus
    {
        NoLogin = 0,
        NoPassword = 1,
        PasswordSentInactive = 2,
        PasswordSent = 3
    }

    /// <summary>
    /// Wrapper around an IContact or IRemisierEmployee.
    /// Used as a common interface to them, when generating login names/passwords on the client site.
    /// </summary>
    public abstract class LoginPerson
    {
        public abstract IExternalLogin Login { get; set; }

        public abstract int PersonKey { get; }

        public abstract string PersonType { get; }
        public abstract string PdfSubfolder { get; }

        public abstract bool IsActive { get; }
        public abstract bool HasLogin { get; }

        public abstract string Email { get; }
        
        public abstract string ShortName { get; }
        public abstract string FullName { get; }
        
        public abstract string DearSirForm { get; }
        public abstract string AddressFirstLine { get; }
        public abstract string AddressSecondLine { get; }
        public abstract Address Address { get; }
        public abstract string FullAddress { get; }

        public abstract IAssetManager AssetManager { get; }
        public abstract string[] InitialClientUserRoleList { get; }

        public void AssertHasEmail()
        {
            if (string.IsNullOrEmpty(Email))
                throw new ApplicationException(string.Format("{0} does not have an e-mail address.", PersonType));
        }

        public abstract void AssertAddressIsComplete();

        public bool IsAddressComplete
        {
            get { return Address != null && Address.IsComplete; }
        }

        public virtual string AccountNumbers { get { return ""; } }

        public string ShortNameAlphaCharsOnly
        {
            get { return Regex.Replace(ShortName, @"[^a-zA-Z]", ""); }
        }

        public string LoginUserName
        {
            get { return HasLogin ? Login.UserName : ""; }
        }

        public bool PasswordSent
        {
            get { return HasLogin && Login.PasswordSent; }
        }

        public bool IsLoginActive
        {
            get { return HasLogin && Login.IsActive; }
        }

        public virtual bool NeedsLoginSending
        {
            get { return IsActive && !HasLogin; }
        }

        public virtual bool NeedsPasswordSending
        {
            get { return IsActive && HasLogin && !PasswordSent; }
        }

        public bool NeedsHandling
        {
            get { return NeedsLoginSending || NeedsPasswordSending; }
        }

        public LoginPersonStatus Status
        {
            get
            {
                return !HasLogin           ? LoginPersonStatus.NoLogin :
                       !Login.PasswordSent ? LoginPersonStatus.NoPassword :
                       !Login.IsActive     ? LoginPersonStatus.PasswordSentInactive :
                                             LoginPersonStatus.PasswordSent;
            }
        }
    }
}
