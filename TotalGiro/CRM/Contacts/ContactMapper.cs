using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using NHibernate.Criterion;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// This class is used to instantiate Contact objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class ContactMapper
    {
        /// <summary>
        /// Retrieves list of Contacts
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="assetManager">AssetManager</param>
        /// <param name="model">Model</param>
        /// <param name="accountNumber">AccountNumber</param>
        /// <param name="accountName">AccountName</param>
        /// <param name="accountsWithModelOnly">Flag for Accounts with models only</param>
        /// <returns></returns>
        public static IList GetContacts(IDalSession session, IAssetManager assetManager, string accountNumber, string contactName)
        {
            return GetContacts(session, assetManager, accountNumber, contactName, true, false, null);
        }

        public static IList GetContacts(IDalSession session, IAssetManager assetManager, string accountNumber, string contactName,
                    bool contactActive, bool contactInactive)
        {
            return GetContacts(session, assetManager, accountNumber, contactName, contactActive, contactInactive, null);
        }

        public static IList GetContacts(IDalSession session, IAssetManager assetManager, string accountNumber, string contactName,
            bool contactActive, bool contactInactive, ContactTypeEnum? contactTypeEnum)
        {
            return GetContacts(session, assetManager, accountNumber, contactName, contactActive, contactInactive, contactTypeEnum,
                               true, true, null, null, null, true, true, null);
        }


        public static IList GetContacts(IDalSession session, IAssetManager assetManager, string accountNumber, string contactName,
             string bsN_KvK, bool contactActive, bool contactInactive)
        {
            return GetContacts(session, assetManager, accountNumber, contactName, contactActive, contactInactive, null,
                   true, true, null, null, null, true, true, bsN_KvK);
        }

        public static List<IContact> GetContacts(IDalSession session, IAssetManager assetManager, string accountNumber, string contactName,
            bool contactActive, bool contactInactive, ContactTypeEnum? contactTypeEnum, bool emailStatusYes, bool emailStatusNo,
            bool? hasLogin, bool? passwordSent, bool? isLoginActive, bool accountActive, bool accountInactive, string bsN_KvK)
        {
            int assetManagerId = assetManager != null ? assetManager.Key : 0;

            return GetContacts(session, assetManagerId, 0, 0, accountNumber, contactName, contactActive, contactInactive, contactTypeEnum, 
                               emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive, accountActive, accountInactive, bsN_KvK);
        }

        public static List<IContact> GetContacts(IDalSession session,
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool contactActive, bool contactInactive, ContactTypeEnum? contactTypeEnum, bool emailStatusYes, bool emailStatusNo,
            bool? hasLogin, bool? passwordSent, bool? isLoginActive, bool accountActive, bool accountInactive, string bsN_KvK)
        {
            Hashtable parameters = new Hashtable();
            StringBuilder sb = new StringBuilder();

            string className = (contactTypeEnum == ContactTypeEnum.Person ? "ContactPerson" :
                               (contactTypeEnum == ContactTypeEnum.Company ? "Company" : "Contact"));

            sb.AppendFormat(@"SELECT C.Key FROM {0} C 
                              LEFT OUTER JOIN C.Login L WHERE 1=1", className);

            if (assetManagerId != 0)
            {
                parameters.Add("AssetManagerId", assetManagerId);
                sb.Append(" AND C.AssetManager.Key = :AssetManagerId");
            }

            string accountHolderWhereHql = "";

            if (remisierId != 0)
            {
                parameters.Add("RemisierId", remisierId);
                accountHolderWhereHql += " AND AccH.GiroAccount.RemisierEmployee.Remisier.Key = :RemisierId";
            }

            if (remisierEmployeeId != 0)
            {
                parameters.Add("RemisierEmployeeId", remisierEmployeeId);
                accountHolderWhereHql += " AND AccH.GiroAccount.RemisierEmployee.Key = :RemisierEmployeeId";
            }

            if (!string.IsNullOrEmpty(accountNumber))
            {
                parameters.Add("AccountNumber", accountNumber + "%");
                accountHolderWhereHql += " AND AccH.GiroAccount.Number LIKE :AccountNumber";
            }

            AccountStati? accountStatus = null;
            if (accountActive && !accountInactive)
                accountStatus = AccountStati.Active;
            else if (!accountActive && accountInactive)
                accountStatus = AccountStati.Inactive;

            if (accountStatus != null)
            {
                parameters.Add("AccountStatus", (int)accountStatus);
                accountHolderWhereHql += " AND AccH.GiroAccount.Status = :AccountStatus";
            }
            
            if (accountHolderWhereHql != "")
            {
                if (assetManagerId != 0)
                    accountHolderWhereHql += " AND AccH.GiroAccount.AccountOwner.Key = :AssetManagerId";

                sb.AppendFormat(" AND C.Key IN (SELECT AccH.Contact.Key FROM AccountHolder AccH WHERE 1=1 {0})", accountHolderWhereHql);
            }

            if (!string.IsNullOrEmpty(contactName))
            {
                parameters.Add("ContactName", "%" + contactName + "%");
                sb.Append(" AND C.CurrentNAW.Name LIKE :ContactName");
            }

            sb.Append(" AND C.IsActive ");
            if (contactActive && !contactInactive)
                sb.Append("= true");
            else if (!contactActive && contactInactive)
                sb.Append("= false");
            else if (!contactActive && !contactInactive)
                sb.Append("IS NULL");
            else
                sb.Append("IS NOT NULL");

            if (!string.IsNullOrEmpty(bsN_KvK))
            {
                parameters.Add("BsN_KvK", "%" + bsN_KvK + "%");
                sb.Append(@" AND (C.Key IN (SELECT CP.Key FROM ContactPerson CP WHERE CP.BurgerServiceNummer LIKE :BsN_KvK) OR 
                                  C.Key IN (SELECT CC.Key FROM ContactCompany CC WHERE CC.KvKNumber LIKE :BsN_KvK))");
            }

            string hasEmail = "(C.ContactDetails.Email IS NOT NULL AND C.ContactDetails.Email != '')";
            if (!emailStatusYes)
                sb.Append(" AND NOT " + hasEmail);
            if (!emailStatusNo)
                sb.Append(" AND " + hasEmail);

            if (hasLogin != null)
                sb.AppendFormat(" AND L IS {0} NULL", (bool)hasLogin ? "NOT" : "");
            if (passwordSent != null)
                sb.AppendFormat(" AND L.PasswordSent = {0}", passwordSent.ToString().ToLower());
            if (isLoginActive != null)
                sb.AppendFormat(" AND L.IsActive = {0}", isLoginActive.ToString().ToLower());

            string hql = string.Format("FROM {0} CX WHERE CX.Key IN ({1})", className, sb);

            return session.GetTypedListByHQL<IContact>(hql, parameters);
        }

        // Used only for testing (for now)
        public static List<IContact> GetContacts(IDalSession session, SendableDocumentCategories category, SendingOptions sendingOption, bool value)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("Category", (int)category);
            parameters.Add("SendingOption", (int)sendingOption);
            parameters.Add("Value", value);

            string hql = @"from Contact C
                           left join fetch C.contactSendingOptions SO
                           where SO.SendableDocumentCategory = :Category and SO.SendingOption = :SendingOption and SO.Value = :Value
                           order by C.CurrentNAW.Name";

            return session.GetTypedListByHQL<IContact>(hql, parameters);
        }

        /// <summary>
        /// Retrieves a list of all <b>Contact</b> objects in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="accountName">AccountName</param>
        /// <returns>A list of all <b>Contacts</b> objects in the system.</returns>
        public static IList GetContacts(IDalSession session, string accountName)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (accountName != null && accountName.Length > 0)
            {
                expressions.Add(Expression.Like("Name", accountName, MatchMode.Anywhere));
            }
            return session.GetList(typeof(Contact), expressions);
        }

        public static List<IContact> GetContacts(IDalSession session, int[] contactIds)
        {
            Hashtable parameters = new Hashtable();
            Hashtable parameterLists = new Hashtable();

            string where = "1 = 2";
            if (contactIds.Length > 0)
            {
                parameterLists.Add("keys", contactIds);
                where = "C.Key in (:keys)";
            }

            string hql = string.Format("from Contact C where {0} order by C.CurrentNAW.Name", where);
            return session.GetTypedListByHQL<IContact>(hql, parameters, parameterLists);
        }

        /// <summary>
        /// Get Contact by ID
        /// </summary>
        /// <param name="Session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="contactId">ID</param>
        /// <returns>Contact object</returns>
        public static IContact GetContact(IDalSession session, int contactId)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", contactId));
            //LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.Both);
            IList contacts =  session.GetList(typeof(Contact), expressions);
            if (contacts != null && contacts.Count == 1)
                return (IContact)contacts[0];
            else
                return null;
        }

        public static IContactPerson GetContactbyBSN(IDalSession session, string BurgerServiceNummer)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("BurgerServiceNummer", BurgerServiceNummer));
            IList contacts = session.GetList(typeof(ContactPerson), expressions);
            if (contacts != null && contacts.Count == 1)
                return (IContactPerson)contacts[0];
            else
                return null;
        }

        public static IContactCompany GetContactbyKVK(IDalSession session, string KvKNummer)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("KvKNumber", KvKNummer));
            IList contacts = session.GetList(typeof(ContactCompany), expressions);
            if (contacts != null && contacts.Count == 1)
                return (IContactCompany)contacts[0];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="contactId">Identifier contact object</param>
        /// <returns>Description of type contact</returns>
        public static ContactTypeEnum? GetContactType(IDalSession session, int contactId)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", contactId));
            //LoginMapper.GetSecurityInfo(session, expressions, SecurityInfoOptions.Both);
            IList contacts = session.GetList(typeof(Contact), expressions);
            if (contacts != null)
            {
                IContact contact = (IContact)contacts[0];
                return contact.ContactType;
            }
            else
                return null;
        }

         /// <summary>
        /// Inserts a <b>Contact</b> object into the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>Contact</b> object to insert into the database.</param>
        public static bool Insert(IDalSession session, IContact obj)
        {
           return session.Insert(obj);
        }

        /// <summary>
        /// Updates a <b>Contact</b> object to the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>Contact</b> object to update into the database.</param>
        public static bool Update(IDalSession session, IContact obj)
        {
            return session.InsertOrUpdate(obj);
        }

        /// <summary>
        /// Deletes a <b>Contact</b> object from the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>Contact</b> object to delete from the database.</param>
        public static bool Delete(IDalSession session, IContact obj)
        {
            return session.Delete(obj);
        }

        public static void DelAccountHolder(int accountKey, int contactKey)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountKey);
            IContact contact = ContactMapper.GetContact(session, contactKey);

            if (acc != null && contact != null)
            {
                IAccountHolder ah = (IAccountHolder)AccountHolderMapper.GetAccountHolder(session, contact, acc);
                contact.AccountHolders.Remove(ah);
                ContactMapper.Update(session, contact);
            }
            session.Close();
        }
    }
}
