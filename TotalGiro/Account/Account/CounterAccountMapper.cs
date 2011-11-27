using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This class is used to instantiate CounterAccount objects. 
    /// </summary>
    public static class CounterAccountMapper
    {
        /// <summary>
        /// This method retrieves an CounterAccount instance via it's database identifier.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="id">The unique identifier</param>
        /// <returns>A specific instance of a accountholder class</returns>
        public static ICounterAccount GetCounterAccount(IDalSession session, int id)
        {

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList<ICounterAccount> list = session.GetTypedList<ICounterAccount>(expressions);
            if (list != null && list.Count == 1)
                return list[0];
            else
                return null;
        }

        public static ICounterAccount GetCounterAccount(IDalSession session, string tegenRekening)
        {

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Number", tegenRekening));
            IList<ICounterAccount> list = session.GetTypedList<ICounterAccount>(expressions);
            if (list != null && list.Count == 1)
                return list[0];
            else
                return null;
        }

        /// <summary>
        /// This method retrieves a list of counter account instances that meet the passed in arguments. 
        /// When an argument is ommitted it is not used to filter the counter accounts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="assetManager">The asset manager the customer belongs to</param>
        /// <param name="counterAccountNumber">Number of the counter account</param>
        /// <param name="counterAccountName">Name of the counter account</param>
        /// <param name="contactName">The name of the contact</param>
        /// <param name="accountNumber">The account's number of the account</param>
        /// <param name="showActive">Show for active contacts</param>
        /// <param name="showInactive">Show for inactive contacts</param>
        /// <param name="isPublic">Show public contacts</param>
        /// <returns>A list of counter account instances</returns>
        public static IList<ICounterAccount> GetCounterAccounts(IDalSession session, IAssetManager assetManager,
            string counterAccountNumber, string counterAccountName, 
            string contactName, string accountNumber, bool showActive, 
            bool showInactive, bool isPublic)
        {
            Hashtable parameters = new Hashtable();

            if (assetManager != null)
                parameters.Add("managementCompanyId", assetManager.Key);
            if (counterAccountNumber != null && counterAccountNumber.Length > 0)
                parameters.Add("counterAccountNumber", Util.PrepareNamedParameterWithWildcard(counterAccountNumber, MatchModes.Anywhere));
            if (counterAccountName != null && counterAccountName.Length > 0)
                parameters.Add("counterAccountName", Util.PrepareNamedParameterWithWildcard(counterAccountName, MatchModes.Anywhere));
            if (contactName != null && contactName.Length > 0)
                parameters.Add("contactName", Util.PrepareNamedParameterWithWildcard(contactName, MatchModes.Anywhere));
            parameters.Add("isPublic", isPublic);
            if (showActive && !showInactive)
                parameters.Add("isActive", true);
            if (!showActive && showInactive)
                parameters.Add("isActive", false);

            if (accountNumber != null && accountNumber.Length > 0)
                parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber, MatchModes.Anywhere));

            return session.GetTypedListByNamedQuery<ICounterAccount>(
                "B4F.TotalGiro.Accounts.CounterAccount.GetCounterAccounts",
                parameters);
        }

        /// <summary>
        /// This method retrieves a list of counter account instances that belong at least to one of the accountholders. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="account">The relevant account</param>
        /// <returns>A list of counter account instances</returns>
        public static IList<ICounterAccount> GetCounterAccounts(IDalSession session, IAccount account)
        {
            return CounterAccountMapper.GetCounterAccounts(session, account, false);
        }

        /// <summary>
        /// This method retrieves a list of counter account instances that belong at least to one of the accountholders. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="account">The relevant account</param>
        /// <param name="addPublicAccounts">Show public accounts as well</param>
        /// <returns>A list of counter account instances</returns>
        public static IList<ICounterAccount> GetCounterAccounts(IDalSession session, IAccount account, bool addPublicAccounts)
        {
            Hashtable parameters = new Hashtable();

            parameters.Add("accountId", account.Key);
            parameters.Add("addPublicAccounts", addPublicAccounts ? 1 : 0);
            
            return session.GetTypedListByNamedQuery<ICounterAccount>(
                "B4F.TotalGiro.Accounts.CounterAccount.GetCounterAccountsByAccount",
                parameters);
        }

        /// <summary>
        /// This method retrieves a list of counter account instances that belong to the contact. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="contact">The relevant contact</param>
        /// <returns>A list of counter account instances</returns>
        public static IList<ICounterAccount> GetCounterAccounts(IDalSession session, IContact contact)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("contactId", contact.Key);

            return session.GetTypedListByNamedQuery<ICounterAccount>(
                "B4F.TotalGiro.Accounts.CounterAccount.GetCounterAccountsByContact",
                parameters);
        }

        /// <summary>
        /// This method retrieves a list of counter account instances that have the flag IsPublic = true. 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <returns>A list of counter account instances</returns>
        public static IList<ICounterAccount> GetPublicCounterAccounts(IDalSession session)
        {
            Hashtable parameters = new Hashtable();

            IManagementCompany mgtCompany = LoginMapper.GetCurrentManagmentCompany(session);
            if (mgtCompany != null && !mgtCompany.IsStichting)
                parameters.Add("managementCompanyId", mgtCompany.Key);

            return session.GetTypedListByNamedQuery<ICounterAccount>(
                "B4F.TotalGiro.Accounts.CounterAccount.GetPublicCounterAccounts",
                parameters);
        }


        #region CRUD

        public static void Insert(IDalSession session, ICounterAccount obj)
        {
            session.Insert(obj);
        }

        public static void Update(IDalSession session, ICounterAccount obj)
        {
            session.Update(obj);
        }

        public static void Update(IDalSession session, IList list)
        {
            session.Update(list);
        }

        #endregion
    }
}
