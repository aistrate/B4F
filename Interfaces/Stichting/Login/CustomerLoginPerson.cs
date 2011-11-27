using System;
using System.Linq;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Stichting.Login
{
    public class CustomerLoginPerson : LoginPerson
    {
        public CustomerLoginPerson(IContact contact)
        {
            Contact = contact;
        }

        public IContact Contact { get; private set; }

        public IContactsNAW CurrentNAW
        {
            get
            {
                if (Contact.CurrentNAW != null)
                    return Contact.CurrentNAW;
                else
                    throw new ApplicationException("Contact does not have an associated NAW.");
            }
        }

        public override IExternalLogin Login
        {
            get
            {
                if (HasLogin)
                    return Contact.Login;
                else
                    throw new ApplicationException("Contact does not have an associated login.");
            }
            set
            {
                ICustomerLogin customerLogin = (ICustomerLogin)value;
                customerLogin.Contact = Contact;
                Contact.Login = customerLogin;
            }
        }

        public override int PersonKey
        {
            get { return Contact.Key; }
        }

        public override string PersonType
        {
            get { return "Contact"; }
        }

        public override string PdfSubfolder
        {
            get { return "Clients"; }
        }

        public override bool IsActive
        {
            get { return Contact.IsActive; }
        }

        public override bool HasLogin
        {
            get { return Contact.Login != null && !string.IsNullOrEmpty(Contact.Login.UserName); }
        }

        public override string Email
        {
            get { return Contact.Email; }
        }

        public override string ShortName
        {
            get { return Contact.CurrentNAW.GetS(naw => naw.Name); }
        }

        public override string FullName
        {
            get { return Contact.FullName; }
        }

        public override string DearSirForm
        {
            get { return CurrentNAW.Formatter.DearSirForm; }
        }

        public override string AddressFirstLine
        {
            get { return CurrentNAW.Formatter.AddressFirstLine; }
        }

        public override string AddressSecondLine
        {
            get { return CurrentNAW.Formatter.AddressSecondLine; }
        }

        public override Address Address
        {
            get { return CurrentNAW.Formatter.Address; }
        }

        public override string FullAddress
        {
            get { return Contact.FullAddress; }
        }

        public override IAssetManager AssetManager
        {
            get { return Contact.AssetManager; }
        }

        public override string[] InitialClientUserRoleList
        {
            get
            {
                if (Contact.AssetManager.InitialClientUserRoleList.Length > 0)
                    return Contact.AssetManager.InitialClientUserRoleList;
                else
                    throw new ApplicationException(string.Format("Asset Manager '{0}' does not have any assigned InitialClientUserRoles.",
                                                                 Contact.AssetManager.CompanyName));
            }
        }

        public override void AssertAddressIsComplete()
        {
            CurrentNAW.Formatter.AssertAddressIsComplete();
        }

        public override string AccountNumbers
        {
            get { return Contact.ActiveAccounts.Select(a => a.Number).JoinStrings(", "); }
        }

        public override bool NeedsLoginSending
        {
            get { return base.NeedsLoginSending && Contact.ActiveAccounts.Count > 0; }
        }

        public override bool NeedsPasswordSending
        {
            get { return base.NeedsPasswordSending && Contact.ActiveAccounts.Count > 0; }
        }
    }
}
