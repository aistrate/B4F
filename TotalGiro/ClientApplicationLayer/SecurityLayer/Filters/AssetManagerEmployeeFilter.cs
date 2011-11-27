using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    internal class AssetManagerEmployeeFilter : InternalEmployeeFilter
    {
        public override List<IContact> GetOwnedContacts(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive)
        {
            assetManagerId = GetForcedKey(CurrentAssetManager, assetManagerId);

            return GetContactsUnchecked(assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName,
                                        emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive, accountActive, accountInactive);
        }
        
        public override List<ICustomerAccount> GetOwnedContactAccounts(int contactId, bool activeOnly)
        {
            int currentAssetManagerId = CurrentAssetManager.Key;
            return GetOwnedContact(contactId).GetAccounts(activeOnly)
                                             .Where(a => a.AccountOwner != null && a.AccountOwner.Key == currentAssetManagerId)
                                             .ToList();
        }

        public override List<IRemisier> GetOwnedRemisiers(int assetManagerId)
        {
            assetManagerId = GetForcedKey(CurrentAssetManager, assetManagerId);

            return RemisierMapper.GetRemisiers(Session, assetManagerId);
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(
            int assetManagerId, int remisierId, string remisierEmployeeName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            assetManagerId = GetForcedKey(CurrentAssetManager, assetManagerId);

            return RemisierEmployeeMapper.GetRemisierEmployees(Session, assetManagerId, remisierId, remisierEmployeeName,
                                                               emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive);
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(int remisierId)
        {
            assertRemisierIsOwned(remisierId);
            return RemisierEmployeeMapper.GetRemisierEmployees(Session, remisierId);
        }

        public override IRemisierEmployee GetOwnedRemisierEmployee(int remisierEmployeeId)
        {
            IRemisierEmployee remisierEmployee = GetRemisierEmployee(remisierEmployeeId);
            assertRemisierIsOwned(remisierEmployee.Remisier);
            return remisierEmployee;
        }
        
        public override IAssetManager CurrentAssetManager
        {
            get { return (IAssetManager)CurrentManagementCompany; }
        }

        protected override bool IsContactOwned(IContact contact)
        {
            return contact.AssetManager.Key == CurrentAssetManager.Key;
        }

        protected override bool IsAccountOwned(ICustomerAccount account)
        {
            return account.AccountOwner.Key == CurrentAssetManager.Key;
        }

        private void assertRemisierIsOwned(int remisierId)
        {
            assertRemisierIsOwned(RemisierMapper.GetRemisier(Session, remisierId));
        }

        private void assertRemisierIsOwned(IRemisier remisier)
        {
            if (remisier == null || !remisier.IsActive ||
                remisier.AssetManager == null || remisier.AssetManager.Key != CurrentAssetManager.Key)
                throw new SecurityLayerException("Remisier (company) not authorized or not found.");
        }
    }
}
