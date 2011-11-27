using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters
{
    internal abstract class SecurityFilter
    {
        #region Factory method

        public static SecurityFilter GetCurrent(IDalSession session)
        {
            LoginTypes currentLoginType = GetCurrentLoginType(session);
            SecurityFilter currentFilter = null;

            switch (currentLoginType)
            {
                case LoginTypes.StichtingEmployee:
                    currentFilter = new StichtingEmployeeFilter();
                    break;
                case LoginTypes.AssetManagerEmployee:
                    currentFilter = new AssetManagerEmployeeFilter();
                    break;
                case LoginTypes.ComplianceEmployee:
                    currentFilter = new ComplianceEmployeeFilter();
                    break;
                case LoginTypes.RemisierEmployee:
                    currentFilter = new RemisierEmployeeFilter();
                    break;
                case LoginTypes.Customer:
                    currentFilter = new CustomerFilter();
                    break;
                default:
                    throw new SecurityLayerException();
            }

            currentFilter.Session = session;
            currentFilter.ExpectedLoginType = currentLoginType;

            return currentFilter;
        }

        #endregion


        #region Owned Contacts

        public abstract List<IContact> GetOwnedContacts(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive);

        public virtual IContact GetOwnedContact(int contactId)
        {
            IContact contact = ContactMapper.GetContact(Session, contactId);
            if (contact != null && contact.IsActive && IsContactOwned(contact))
                return contact;
            else
                throw new ContactNotFoundException(contactId);
        }

        #endregion


        #region Owned Accounts

        public abstract List<ICustomerAccount> GetOwnedContactAccounts(int contactId, bool activeOnly);

        public virtual ICustomerAccount GetOwnedActiveAccount(int accountId)
        {
            IAccount account = AccountMapper.GetAccount(Session, accountId);

            if (account != null && account.Status == AccountStati.Active)
            {
                if (account.AccountType == AccountTypes.Customer)
                {
                    if (IsAccountOwned((ICustomerAccount)account))
                        return (ICustomerAccount)account;
                }
                else
                    throw new SecurityLayerException("Account not authorized because of non-Customer type.");
            }
            
            throw new SecurityLayerException("Account not authorized or not found.");
        }

        #endregion


        #region Owned Asset Managers

        public virtual List<IAssetManager> GetOwnedAssetManagers()
        {
            return new List<IAssetManager>() { CurrentAssetManager };
        }

        public abstract IAssetManager CurrentAssetManager { get; }

        public virtual ICurrency BaseCurrency { get { return CurrentAssetManager.BaseCurrency; } }

        #endregion


        #region Owned Remisiers

        public virtual List<IRemisier> GetOwnedRemisiers(int assetManagerId)
        {
            GetForcedKey(CurrentRemisier.AssetManager, assetManagerId);

            return new List<IRemisier>() { CurrentRemisier };
        }

        public abstract IRemisier CurrentRemisier { get; }

        #endregion


        #region Owned Remisier Employees

        public abstract List<IRemisierEmployee> GetOwnedRemisierEmployees(
            int assetManagerId, int remisierId, string remisierEmployeeName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive);

        public abstract List<IRemisierEmployee> GetOwnedRemisierEmployees(int remisierId);

        public abstract IRemisierEmployee GetOwnedRemisierEmployee(int remisierEmployeeId);

        public abstract IRemisierEmployee CurrentRemisierEmployee { get; }

        #endregion

        
        #region Owned Models

        public virtual List<IPortfolioModel> GetOwnedModels()
        {
            return GetOwnedModels(0);
        }

        public virtual List<IPortfolioModel> GetOwnedModels(int assetManagerId)
        {
            assetManagerId = GetForcedKey(CurrentAssetManager, assetManagerId);

            return ModelMapper.GetModelsSorted(Session, assetManagerId, true, false, ActivityReturnFilter.Active, null);
        }

        public virtual IPortfolioModel GetOwnedModel(int modelId)
        {
            IPortfolioModel model = GetModel(modelId);
            GetForcedKey(CurrentAssetManager, model.AssetManager.Key);
            return model;
        }

        protected IPortfolioModel GetModel(int modelId)
        {
            IPortfolioModel model = ModelMapper.GetModel(Session, modelId);
            if (model != null)
                return model;
            else
                throw new SecurityLayerException("Model not authorized or not found.");
        }

        #endregion


        #region Current Login

        public static LoginTypes GetCurrentLoginType(IDalSession session)
        {
            return getCurrentLogin(session).LoginType;
        }

        public virtual LoginPerson CurrentLoginPerson
        {
            get { return null; }
        }

        #endregion


        #region Internals

        protected IDalSession Session { get; private set; }
        protected LoginTypes ExpectedLoginType { get; private set; }

        protected ILogin CurrentLogin
        {
            get
            {
                ILogin currentLogin = getCurrentLogin(Session);
                if (currentLogin.LoginType == ExpectedLoginType)
                    return currentLogin;
                else
                    throw new SecurityLayerException();
            }
        }
        
        private static ILogin getCurrentLogin(IDalSession session)
        {
            ILogin currentLogin = LoginMapper.GetCurrentLogin(session);
            if (currentLogin != null)
                return currentLogin;
            else
                // TODO: throw a dedicated exception, which will be caught in AppErrors.aspx and cause a redirect to Login.aspx
                throw new SecurityLayerException("No user is currently logged in.");
        }

        protected abstract bool IsContactOwned(IContact contact);

        protected abstract bool IsAccountOwned(ICustomerAccount account);

        protected List<IContact> GetContactsUnchecked(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive)
        {
            return ContactMapper.GetContacts(Session,
                        assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName, true, false,
                        null, emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive, accountActive, accountInactive, null);
        }

        #endregion


        #region Object Checker

        protected static T GetNotNull<T>(T obj)
        {
            return ObjectChecker.GetNotNull<T>(obj);
        }

        protected static int GetForcedKey<T>(T obj, int requestedId)
        {
            return ObjectChecker.GetForcedKey<T>(obj, requestedId);
        }

        #endregion
    }
}
