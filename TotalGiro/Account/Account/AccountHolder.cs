using System;
using System.Collections.Generic;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Dal;
using System.Collections;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// The AccountHodler is the Contact (Person or Company) that holds an Account
    /// in the System. There is a many-to-many relationship bewteen accounts and Accountholders.
    /// </summary>
    public class AccountHolder : IAccountHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.AccountHolder">AccountHolder</see> class.
        /// </summary>
        public AccountHolder(ICustomerAccount GiroAccount, IContact Contact)
        {
            this.GiroAccount = GiroAccount;
            this.Contact = Contact;
            this.CreationDate = DateTime.Now;
        }

        public AccountHolder() { }

        public IContact Contact
        {
            get { return contact; }
            set { contact = value; }
        }

        public ICustomerAccount GiroAccount
        {
            get { return giroAccount; }
            set { giroAccount = value; }
        }        

        public int Key
        {
            get { return key; }
            set { key = value; }
        }        

        public int Aanvraag
        {
            get { return aanvraag; }
            set { aanvraag = value; }
        }        

        public bool IsPrimaryAccountHolder
        {
            get { return primaryAccountHolder; }
            set { primaryAccountHolder = value; }
        }        

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        public override bool Equals(object obj)
        {
            try
            {
                AccountHolder obje = obj as AccountHolder;
                return this.Contact.Equals(obje.Contact) && this.GiroAccount.Equals(obje.GiroAccount);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// Overridden creation of a hashcode.
        /// Returns the hashcode of the AccountHolder to be able of comparing different account instances for equality
        /// </summary>
        /// <returns>Returns the Key property of the account instance</returns>
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public static IList findByStatus(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("IsActive", 1));
            return session.GetList(typeof(AccountHolder), expressions);
           // session.Session.Find("from AccountHolder ah where ah.IsValue = 1");
        }

        #region Private Variables

        private ICustomerAccount giroAccount;
        private IContact contact;
        private int key;
        private int aanvraag;
        private bool primaryAccountHolder;
        private DateTime creationDate = DateTime.Now;
        private bool isActive = true;

        #endregion

    }
}
