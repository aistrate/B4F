using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Notifications;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Class holds common data about a contact object
    /// </summary>
	public abstract class Contact : TotalGiroBase<IContact>, IContact 
	{
        public Contact()
        {
            contactSendingOptions = new ContactSendingOptionCollection(this);
            accountHolders = new ContactAccountHoldersCollection(this);
        }

        public Contact(string name, Address postalAddress, Address residentialAddress, IContactDetails contactDetails) 
            : this()
        {
            if ((postalAddress == null || !postalAddress.ContainsData) && residentialAddress != null && residentialAddress.ContainsData)
                postalAddress = residentialAddress;
            IContactsNAW contactNaw = new ContactsNAW(name, postalAddress, residentialAddress);
            this.ContactsNAWs.Add(contactNaw);
            this.CurrentNAW = contactNaw;
            this.ContactDetails = contactDetails;
        }

        /// <summary>
        /// Get/set gender
        /// </summary>
        public virtual EnumStatusNAR StatusNAR
        {
            get { return statusNAR; }
            set { statusNAR = value; }
        }

        public virtual IAssetManager AssetManager
        {
            get { return assetManager; }
            set { assetManager = value; }
        }

        public virtual IContactDetails ContactDetails
        {
            get { return contactDetails; }
            set { contactDetails = value; }
        }

        public string Email
        {
            get { return ContactDetails.GetS(d => d.Email).Trim(); }
        }

        public bool SendNewsItem
        {
            get { return ContactDetails.GetV(d => d.SendNewsItem) ?? false; }
        }

        public virtual IContactAccountHoldersCollection AccountHolders
        {
            get
            {
                ContactAccountHoldersCollection ahs = (ContactAccountHoldersCollection)accountHolders.AsList();
                if (ahs.Parent == null)
                    ahs.Parent = this;
                return ahs;
            }
        }

        public List<ICustomerAccount> GetAccounts(bool activeOnly)
        {
            IEnumerable<ICustomerAccount> accounts = accountHolders.Where(ah => ah.GiroAccount != null)
                                                                   .Select(ah => ah.GiroAccount)
                                                                   .OrderBy(a => a.Number);
            
            if (activeOnly)
                accounts = accounts.Where(a => a.Status == AccountStati.Active);

            return accounts.ToList();
        }

        public List<ICustomerAccount> ActiveAccounts
        {
            get { return GetAccounts(true); }
        }

        public virtual IContactsIntroducersCollection ContactsIntroducers
        {
            get
            {
                if (this.contactsIntroducers == null)
                {
                    this.contactsIntroducers = new ContactsIntroducersCollection(this, bagOfContactsIntroducers);
                }
                return contactsIntroducers;
            }
            internal set { contactsIntroducers = value; }
        }
        

        /// <summary>
        /// The collection of Naam-Adres-Woonplaats belonging to contact
        /// </summary>
        public virtual IContactsNAWCollection ContactsNAWs
        {
            get
            {
                if (this.contactsNAWs == null)
                {
                    this.contactsNAWs = new ContactsNAWCollection(this, bagOfContactsNAWs);
                }
                return contactsNAWs;
            }
            internal set { contactsNAWs = value; }
        }

        /// <summary>
        /// The collection of CounterAccounts belonging to contact
        /// </summary>
        public virtual ICounterAccountCollection CounterAccounts
        {
            get
            {
                if (this.counterAccounts == null)
                {
                    this.counterAccounts = new CounterAccountCollection(this, bagOfCounterAccounts);
                }
                return counterAccounts;
            }
        }

        /// <summary>
        /// Flag if contact has internet provider
        /// </summary>
        public virtual InternetEnabled InternetEnabled
        {
            get { return internetEnabled; }
            set { internetEnabled = value; }
        }

        /// <summary>
        /// Get Contact Type (class)
        /// </summary>
        public abstract ContactTypeEnum ContactType { get; }

        public abstract string GetBSN { get; }

        public abstract DateTime GetBirthFounding { get; }

        public virtual ResidentStatus ResidentialState  { get; set; }

        public virtual ICustomerLogin Login { get; set; }

        public CustomerLoginPerson LoginPerson
        {
            get
            {
                if (loginPerson == null)
                    loginPerson = new CustomerLoginPerson(this);
                return loginPerson;
            }
        }
        private CustomerLoginPerson loginPerson;

        /// <summary>
        /// Is contact an active participant of the Effectengiro
        /// </summary>
        public virtual bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// The date that the contact was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The date that the contact was last modified.
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        public virtual IContactsNAW CurrentNAW
        {
            get { return currentNAW; }
            set { currentNAW = value; }
        }

        public string FullName
        {
            get { return CurrentNAW.GetS(naw => naw.Formatter.FullName); }
        }

        public string FullAddress
        {
            get { return CurrentNAW.GetS(naw => naw.Formatter.FullAddress); }
        }

        public virtual IContactsIntroducer CurrentIntroducer
        {
            get { return currentIntroducer; }
            set { currentIntroducer = value; }
        }

        /// <summary>
        /// This is the collection of active withdrawal instructions that belong to the account.
        /// </summary>
        public virtual IContactNotificationsCollection Notifications
        {
            get
            {
                ContactNotificationsCollection col = (ContactNotificationsCollection)notifications.AsList();
                if (col.Parent == null) col.Parent = this;
                return col;
            }
        }


        public virtual IContactSendingOptionCollection ContactSendingOptions
        {
            get
            {
                ContactSendingOptionCollection sendingOptions = (ContactSendingOptionCollection)contactSendingOptions.AsList();
                if (sendingOptions.Parent == null)
                    sendingOptions.Parent = this;
                return sendingOptions;
            }
        }

        public bool SendByPost
        {
            get { return ContactSendingOptions.GetValueOrDefault(SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByPost); }
        }

        public bool SendByEmail
        {
            get { return ContactSendingOptions.GetValueOrDefault(SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByEmail); }
        }

        #region Private Variables

        private EnumStatusNAR statusNAR = EnumStatusNAR.Incomplete;
        private IContactsNAWCollection contactsNAWs;
        internal IDomainCollection<IAccountHolder> accountHolders;
        private IList bagOfContactsNAWs = new ArrayList();
        private IList bagOfContactsIntroducers = new ArrayList();
        private IContactsIntroducersCollection contactsIntroducers;
        private ICounterAccountCollection counterAccounts;
        private IList bagOfCounterAccounts = new ArrayList();
        private IAssetManager assetManager;
        private InternetEnabled internetEnabled = InternetEnabled.Unknown;
        private bool isActive = true;
        private IContactDetails contactDetails;
        private DateTime lastUpdated;
        private IContactsNAW currentNAW;
        private IContactsIntroducer currentIntroducer;
        private IDomainCollection<Notification> notifications;
        private IDomainCollection<IContactSendingOption> contactSendingOptions;

		#endregion
	}
}
