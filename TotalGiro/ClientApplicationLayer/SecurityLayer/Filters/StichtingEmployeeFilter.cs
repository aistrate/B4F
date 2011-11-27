using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    internal class StichtingEmployeeFilter : InternalEmployeeFilter
    {
        public override List<IContact> GetOwnedContacts(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive)
        {
            assertIsStichting();
            return GetContactsUnchecked(assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName,
                                        emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive, accountActive, accountInactive);
        }

        public override List<ICustomerAccount> GetOwnedContactAccounts(int contactId, bool activeOnly)
        {
            assertIsStichting();
            return GetOwnedContact(contactId).GetAccounts(activeOnly);
        }

        public override List<IAssetManager> GetOwnedAssetManagers()
        {
            assertIsStichting();
            return ManagementCompanyMapper.GetAssetManagers(Session);
        }

        public override ICurrency BaseCurrency { get { return CurrentManagementCompany.BaseCurrency; } }

        public override List<IRemisier> GetOwnedRemisiers(int assetManagerId)
        {
            assertIsStichting();
            return RemisierMapper.GetRemisiers(Session, assetManagerId);
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(
            int assetManagerId, int remisierId, string remisierEmployeeName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            assertIsStichting();
            return RemisierEmployeeMapper.GetRemisierEmployees(Session, assetManagerId, remisierId, remisierEmployeeName,
                                                               emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive);
        }

        public override List<IRemisierEmployee> GetOwnedRemisierEmployees(int remisierId)
        {
            assertIsStichting();
            return RemisierEmployeeMapper.GetRemisierEmployees(Session, remisierId);
        }

        public override IRemisierEmployee GetOwnedRemisierEmployee(int remisierEmployeeId)
        {
            assertIsStichting();
            return GetRemisierEmployee(remisierEmployeeId);
        }

        public override IAssetManager CurrentAssetManager
        {
            get { throw new SecurityLayerException("Could not determine current asset manager."); }
        }

        public override List<IPortfolioModel> GetOwnedModels(int assetManagerId)
        {
            assertIsStichting();
            return ModelMapper.GetModelsSorted(Session, assetManagerId, true, false, ActivityReturnFilter.Active, null);
        }

        public override IPortfolioModel GetOwnedModel(int modelId)
        {
            assertIsStichting();
            return GetModel(modelId);
        }

        protected override bool IsContactOwned(IContact contact)
        {
            return CurrentManagementCompany.IsStichting;
        }

        protected override bool IsAccountOwned(ICustomerAccount account)
        {
            return CurrentManagementCompany.IsStichting;
        }

        private void assertIsStichting()
        {
            if (!CurrentManagementCompany.IsStichting)
                throw new SecurityLayerException();
        }
    }
}
