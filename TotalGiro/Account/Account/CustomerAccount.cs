using System;
using System.Collections;
using B4F.TotalGiro.Accounts.RemisierHistory;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Accounts
{
	/// <summary>
	/// This is a account that belongs to the customers of the asset managing companies.
	/// </summary>
    public class CustomerAccount : AccountTypeCustomer, ICustomerAccount
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.CustomerAccount">CustomerAccount</see> class.
        /// </summary>
        private CustomerAccount() { }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.CustomerAccount">CustomerAccount</see> class.
        ///// </summary>
        ///// <param name="number">The Account's number</param>
        ///// <param name="shortName">Shortname of the account</param>
        ///// <param name="accountOwner">The owner of the account</param>
        ///// <param name="modelPortfolio">The model portfolio the account is tied to</param>
        ///// <param name="ah">An accountholder</param>
        //public CustomerAccount(string number, string shortName, IManagementCompany accountOwner, IPortfolioModel modelPortfolio, IAccountHolder ah)
        //    : base(number, shortName, accountOwner, modelPortfolio)
        //{
        //    this.UseManagementFee = true;
        //    if (ah != null)
        //    {
        //        this.AccountHolders.Add(ah);
        //    }
        //}

        public CustomerAccount(string number, string shortName, IManagementCompany accountOwner, DateTime creationDate)
            : this(number, shortName, accountOwner, null, creationDate)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.CustomerAccount">CustomerAccount</see> class.
        /// </summary>
        /// <param name="number">The Account's number</param>
        /// <param name="shortName">Shortname of the account</param>
        /// <param name="accountOwner">The owner of the account</param>
        /// <param name="modelPortfolio">The model portfolio the account is tied to</param>
        public CustomerAccount(string number, string shortName, IManagementCompany accountOwner, IPortfolioModel modelPortfolio, DateTime creationDate)
            : base(number, shortName, accountOwner, modelPortfolio)
        {
            this.CreationDate = creationDate;
            this.Status = AccountStati.Active;
            this.LastDateStatusChanged = creationDate;
            this.TradeableStatus = Tradeability.NonTradeable;
            this.UseManagementFee = true;
            this.DateTradeabilityStatusChanged = creationDate;

        }

        #endregion

        #region Properties

        public virtual bool IsJointAccount { get; set; }

        public virtual bool ContactContractsValidated
        {
            get
            {
                if (PrimaryAccountHolder != null)
                    return this.PrimaryAccountHolder.Contact.StatusNAR == EnumStatusNAR.Complete;
                else
                    return false;
            }
        }

        /// <summary>
        /// The account where the Withdrawals belongs to
        /// </summary>
        public virtual IWithdrawalRuleCollection WithdrawalRules
        {
            get
            {
                if (this.withdrawalRules == null)
                    this.withdrawalRules = new WithdrawalRuleCollection(this, bagOfWithdrawalRules);
                return withdrawalRules;
            }
            set { withdrawalRules = value; }
        }

        public virtual IRemisierEmployee RemisierEmployee
        {
            get { return remisierEmployee; }
            set { remisierEmployee = value; }
        }

        public virtual IRemisierHistory CurrentRemisierDetails
        {
            get { return currentRemisierDetails; }
            set { currentRemisierDetails = value; }
        }
        
        /// <summary>
        /// This is the collection of Remisier History changes that belong to the account.
        /// The <see cref="T:B4F.TotalGiro.Accounts.RemisierHistory.RemisierHistoryCollection">Remisier History changes</see> that belong to the current account.
        /// </summary>
        public virtual IRemisierHistoryCollection RemisierDetailChanges
        {
            get
            {
                if (this.remisierDetailChanges == null)
                    this.remisierDetailChanges = new RemisierHistoryCollection(this, bagOfRemisierHistoryItems);
                return remisierDetailChanges;
            }
        }

        /// <summary>
        /// The employee that has a relationship with this account
        /// </summary>
        public virtual IInternalEmployeeLogin RelatedEmployee
        {
            get { return relatedEmployee; }
            set { relatedEmployee = value; }
        }

        public IAccountFinancialTarget CurrentFinancialTarget { get; set; }

        /// <summary>
        /// Is there a relationship to the employer and the account
        /// </summary>
        public virtual AccountEmployerRelationship EmployerRelationship
        {
            get { return employerRelationship; }
            set { employerRelationship = value; }
        }

        /// <summary>
        /// The contact address as it appears on reports.
        /// </summary>
        public virtual ContactsFormatter Formatter
        {
            get
            {
                if (formatter == null)
                    formatter = ContactsFormatter.CreateContactsFormatter(this);
                return formatter;
            }
        }

        /// <summary>
        /// Fee rules attached to the Account
        /// </summary>
        public virtual IFeeRuleCollection FeeRules
        {
            get
            {
                if (feeRules == null)
                    feeRules = new FeeRuleCollection(this, bagOfFeeRules);
                return feeRules;
            }
        }

        public virtual IAccountFinancialTargetCollection FinancialTargetHistory
        {
            get
            {
                IAccountFinancialTargetCollection history = (IAccountFinancialTargetCollection)financialTargetHistory.AsList();
                if (history.Parent == null) history.Parent = this;
                return history;
            }
        }
        
        public Money FirstPromisedDeposit { get; set; }
        public IPandHouder PandHouder { get; set; }
        public IVerpandSoort VerpandSoort { get; set; }
        public IAccountFamily Family { get; set; }

        #endregion

        #region Overides

        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Customer; }
        }

        /// <summary>
        /// Does the account need commission calculation
        /// </summary>
        public override bool CommissionCalcReqd
        {
            get { return true; }
        }

        #endregion

        #region PrivateVariables

        private IWithdrawalRuleCollection withdrawalRules;
        private IList bagOfWithdrawalRules = new ArrayList();
        private ContactsFormatter formatter;
        private IRemisierEmployee remisierEmployee;
        private IRemisierHistory currentRemisierDetails;
        private IRemisierHistoryCollection remisierDetailChanges;
        private IList bagOfRemisierHistoryItems = new ArrayList();
        private AccountEmployerRelationship employerRelationship = AccountEmployerRelationship.None;
        private IInternalEmployeeLogin relatedEmployee;
        private IList bagOfFeeRules;
        private IFeeRuleCollection feeRules;
        private IDomainCollection<IAccountFinancialTarget> financialTargetHistory;

        #endregion

        #region ContactCollection

        //public class ContactCollection : GenericCollection<IContact>, IContactCollection
        //{
        //    public ContactCollection(ICustomerAccount parent, IList bagOfContacts)
        //        : base(bagOfContacts)
        //    {
        //        this.Parent = parent;
        //    }

        //    public ICustomerAccount Parent
        //    {
        //        get { return parent; }
        //        set { parent = value; }
        //    }


        //    #region Private Variables

        //    private ICustomerAccount parent;

        //    #endregion
        //}

        #endregion

    }
}
