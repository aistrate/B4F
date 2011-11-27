using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using System;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    internal class CustomerFilter : SecurityFilter
    {
        public override List<IContact> GetOwnedContacts(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive)
        {
            assetManagerId     = GetForcedKey(CurrentCustomerContact.AssetManager, assetManagerId);
            remisierEmployeeId = GetForcedKey(CurrentRemisierEmployee, remisierEmployeeId);
            remisierId = GetForcedKey(CurrentRemisierEmployee.Remisier, remisierId);

            return new List<IContact>() { CurrentCustomerContact };
        }

        public override IContact GetOwnedContact(int contactId)
        {
            GetForcedKey(CurrentCustomerLogin.Contact, contactId);
            if (CurrentCustomerLogin.Contact.IsActive)
                return CurrentCustomerLogin.Contact;
            else
                throw new ContactNotFoundException(contactId);
        }

        public override List<ICustomerAccount> GetOwnedContactAccounts(int contactId, bool activeOnly)
        {
            return GetOwnedContact(contactId).GetAccounts(activeOnly);
        }

        public override ICustomerAccount GetOwnedActiveAccount(int accountId)
        {
            ICustomerAccount account = CurrentCustomerContact.ActiveAccounts.FirstOrDefault(a => a.Key == accountId);
            if (account != null)
                return account;
            else
                throw new SecurityLayerException("Account not authorized or not found.");
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(
                    int assetManagerId, int remisierId, string remisierEmployeeName,
                    bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            throw new SecurityLayerException();
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(int remisierId)
        {
            throw new SecurityLayerException("Remisier (company) not authorized or not found.");
        }

        public override IRemisierEmployee GetOwnedRemisierEmployee(int remisierEmployeeId)
        {
            throw new SecurityLayerException("Remisier Employee not authorized or not found.");
        }

        public override IAssetManager CurrentAssetManager
        {
            get { return GetNotNull(CurrentCustomerContact.AssetManager); }
        }
        
        public override IRemisier CurrentRemisier
        {
            get { return GetNotNull(CurrentRemisierEmployee.Remisier); }
        }

        public override IRemisierEmployee CurrentRemisierEmployee
        {
            get
            {
                var accounts = CurrentCustomerContact.ActiveAccounts.Where(a => a.RemisierEmployee != null)
                                                                    .ToList();

                if (accounts.Count > 0 && accounts.Select(a => a.RemisierEmployee.Key).Distinct().Count() == 1)
                    return accounts.First().RemisierEmployee;
                else
                    throw new SecurityLayerException("Could not determine current remisier employee (there are either more than one, or none).");
            }
        }

        public override LoginPerson CurrentLoginPerson
        {
            get { return CurrentCustomerContact.LoginPerson; }
        }

        protected IContact CurrentCustomerContact
        {
            get { return GetNotNull(CurrentCustomerLogin.Contact); }
        }

        protected ICustomerLogin CurrentCustomerLogin
        {
            get { return (ICustomerLogin)CurrentLogin; }
        }

        protected override bool IsContactOwned(IContact contact)
        {
            return contact.Key == CurrentCustomerContact.Key;
        }

        protected override bool IsAccountOwned(ICustomerAccount account)
        {
            return CurrentCustomerContact.ActiveAccounts.Any(a => a.Key == account.Key);
        }
    }
}
