using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    internal class RemisierEmployeeFilter : SecurityFilter
    {
        public override List<IContact> GetOwnedContacts(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive)
        {
            remisierId     = GetForcedKey(CurrentRemisier, remisierId);
            assetManagerId = GetForcedKey(CurrentAssetManager, assetManagerId);
            if (remisierEmployeeId != 0)
                assertRemisierEmployeeIsOwned(remisierEmployeeId);

            return GetContactsUnchecked(assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName,
                                        emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive, accountActive, accountInactive);
        }

        public override List<ICustomerAccount> GetOwnedContactAccounts(int contactId, bool activeOnly)
        {
            return GetOwnedContact(contactId).GetAccounts(activeOnly)
                                             .Where(a => IsAccountOwned(a)).ToList();
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(
                    int assetManagerId, int remisierId, string remisierEmployeeName,
                    bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            remisierId = GetForcedKey(CurrentRemisier, remisierId);
            assetManagerId = GetForcedKey(CurrentAssetManager, assetManagerId);

            return RemisierEmployeeMapper.GetRemisierEmployees(Session, assetManagerId, remisierId, remisierEmployeeName,
                                                               emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive);
        }
        
        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(int remisierId)
        {
            remisierId = GetForcedKey(CurrentRemisier, remisierId);
            return RemisierEmployeeMapper.GetRemisierEmployees(Session, remisierId);
        }

        public override IRemisierEmployee GetOwnedRemisierEmployee(int remisierEmployeeId)
        {
            assertRemisierEmployeeIsOwned(remisierEmployeeId);
            return CurrentRemisier.Employees.FirstOrDefault(e => e.Key == remisierEmployeeId);
        }

        public override IAssetManager CurrentAssetManager
        {
            get { return GetNotNull(CurrentRemisier.AssetManager); }
        }

        public override IRemisier CurrentRemisier
        {
            get { return GetNotNull(CurrentRemisierEmployee.Remisier); }
        }

        public override IRemisierEmployee CurrentRemisierEmployee
        {
            get { return GetNotNull(CurrentRemisierEmployeeLogin.RemisierEmployee); }
        }

        public override LoginPerson CurrentLoginPerson
        {
            get { return CurrentRemisierEmployee.LoginPerson; }
        }

        protected IRemisierEmployeeLogin CurrentRemisierEmployeeLogin
        {
            get { return (IRemisierEmployeeLogin)CurrentLogin; }
        }

        protected override bool IsContactOwned(IContact contact)
        {
            return contact.ActiveAccounts.Any(a => IsAccountOwned(a));
        }

        protected override bool IsAccountOwned(ICustomerAccount account)
        {
            return account.RemisierEmployee != null &&
                   account.RemisierEmployee.Remisier != null &&
                   account.RemisierEmployee.Remisier.Key == CurrentRemisier.Key;
        }

        private bool isRemisierEmployeeOwned(int remisierEmployeeId)
        {
            return remisierEmployeeId != 0 &&
                   CurrentRemisier.Employees != null &&
                   CurrentRemisier.Employees.Select(e => e.Key).Contains(remisierEmployeeId);
        }

        private void assertRemisierEmployeeIsOwned(int remisierEmployeeId)
        {
            if (!isRemisierEmployeeOwned(remisierEmployeeId))
                throw new SecurityLayerException("Remisier Employee not authorized or not found.");
        }
    }
}
