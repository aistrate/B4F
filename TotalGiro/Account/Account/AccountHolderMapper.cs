using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using System.Collections;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This class is used to instantiate AccountHolder objects. 
    /// </summary>
    public static class AccountHolderMapper
    {
        /// <summary>
        /// This method retrieves an accountholder instance via it's database identifier.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="contact">Contact object</param>
        /// <param name="account">AccountTypeInternal object</param>
        /// <returns>A specific instance of a accountholder class</returns>
        public static IAccountHolder GetAccountHolder(IDalSession session, IContact contact, ICustomerAccount account)
        {

            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Contact.Key", contact.Key));
            expressions.Add(Expression.Eq("GiroAccount.Key", account.Key));
            IList collAH = session.GetList(typeof(AccountHolder), expressions);
            if (collAH != null && collAH.Count == 1)
                return (IAccountHolder)collAH[0];
            else
                return null;
        }


        #region CRUD

        public static void Delete(IDalSession session, IAccountHolder obj)
        {
            session.Delete(obj);
        }

        public static void Insert(IDalSession session, IAccountHolder obj)
        {
            session.Insert(obj);
        }

        public static void Update(IDalSession session, IAccountHolder obj)
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
