using System;
using System.Collections;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Accounts.ModelHistory;
using B4F.TotalGiro.Accounts.RemisierHistory;
using B4F.TotalGiro.Accounts.Withdrawals;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Utils;
using System.Collections.Generic;
using B4F.TotalGiro.Notifications;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    #region Helper Class

    public class AccountDetails
    {
        public int AccountNrID { get; set; }
        public string AccountNumber { get; set; }
        public bool IsJointAccount { get; set; }
        public bool IsCustomer { get; set; }
        public int FamilyID { get; set; }
        public int AssetManagerKey { get; set; }
        public string AssetManagerName { get; set; }
        public bool AssetManagerSupportLifecycles { get; set; }
        public int LifecycleId { get; set; }
        public int Modelid { get; set; }
        public bool ModelAllowExecOnlyCustomers { get; set; }
        public bool IsExecOnlyCustomer { get; set; }
        public string AccountShortName { get; set; }
        public string AccountFullName { get; set; }
        public int VerpandSoortID { get; set; }
        public int PandHouderID { get; set; }
        public int CounterAccountID { get; set; }
        public AccountStati Status { get; set; }
        public bool StatusIsOpen { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpDated { get; set; }
        public DateTime LastDateStatusChanged { get; set; }
        public DateTime FirstManagementStartDate { get; set; }
        public DateTime FinalManagementEndDate { get; set; }
        public Tradeability TradeableStatus { get; set; }
        public DateTime DateTradeabilityStatusChanged { get; set; }
        public bool ContactContractsValidated { get; set; }
        public int ExitFeePayingAccountID { get; set; }
        public int RemisierID { get; set; }
        public int RemisierEmployeeID { get; set; }
        public bool UseManagementFee { get; set; }
        public bool UseKickBack { get; set; }
        public decimal KickBack { get; set; }
        public decimal IntroductionFee { get; set; }
        public decimal SubsequentDepositFee { get; set; }
        public decimal IntroductionFeeReduction { get; set; }
        public decimal SubsequentDepositFeeReduction { get; set; }
        public AccountEmployerRelationship EmployerRelationship { get; set; }
        public int RelatedEmployeeID { get; set; }
        public Decimal FirstPromisedDeposit { get; set; }
        public AccountFinancialTargetHelper AccountFinancialTarget { get; set; }
        public string Notification { get; internal set; }
        public NotificationTypes NotificationType { get; internal set; }
        public bool IsDeparting { get; internal set; }
        public bool IsUnderRebalance { get; internal set; }
    }

    public class AccountFinancialTargetHelper
    {
        public AccountFinancialTargetHelper() { }
        public AccountFinancialTargetHelper(AccountFinancialTarget parent)
        {
            this.Key = parent.Key;
            this.ParentAccountID = parent.ParentAccount.Key;
            this.TargetAmountSize = parent.TargetAmount.Quantity;
            this.DepositPerYearSize = parent.DepositPerYear.Quantity;
            this.CurrencyID = parent.TargetAmount.Underlying.Key;
            this.CreatedByID = parent.CreatedBy.Key;
            this.TargetEndDate = parent.TargetEndDate;
        }

        public int Key { get; set; }
        public int ParentAccountID { get; set; }
        public decimal TargetAmountSize { get; set; }
        public decimal DepositPerYearSize { get; set; }
        public int CurrencyID { get; set; }
        public int CreatedByID { get; set; }
        public DateTime TargetEndDate { get; set; }

    }


    #endregion

    public static class AccountEditAdapter
    {
        public static AccountDetails GetAccountDetails(int accountNrID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAccountTypeCustomer acc = (IAccountTypeCustomer)AccountMapper.GetAccount(session, accountNrID);
            AccountDetails returnValue = new AccountDetails()
            {
                AccountNrID = accountNrID,
                AccountNumber = String.Empty,
                FamilyID = int.MinValue,
                AssetManagerKey = int.MinValue,
                Modelid = int.MinValue,
                LifecycleId = int.MinValue,
                AccountShortName = string.Empty,
                AccountFullName = string.Empty,
                VerpandSoortID = int.MinValue,
                PandHouderID = int.MinValue,
                CounterAccountID = int.MinValue,
                CreationDate = DateTime.MinValue,
                LastUpDated = DateTime.MinValue,
                LastDateStatusChanged = DateTime.MinValue,
                FirstManagementStartDate = DateTime.MinValue,
                FinalManagementEndDate = DateTime.MinValue,
                DateTradeabilityStatusChanged = DateTime.MinValue,
                ExitFeePayingAccountID = int.MinValue,
                RemisierID = int.MinValue,
                RemisierEmployeeID = int.MinValue,
                EmployerRelationship = AccountEmployerRelationship.None,
                RelatedEmployeeID = int.MinValue,
                FirstPromisedDeposit = 0.00m,
                UseManagementFee = true,
                IsCustomer = true
            };


            if (acc != null)
            {
                returnValue.AccountNrID = acc.Key;
                returnValue.AccountNumber = acc.Number;
                if (acc.AccountOwner != null)
                {
                    returnValue.AssetManagerKey = acc.AccountOwner.Key;
                    returnValue.AssetManagerName = acc.AccountOwner.CompanyName;
                    if (acc.AccountOwner.CompanyType == ManagementCompanyType.AssetManager)
                        returnValue.AssetManagerSupportLifecycles = ((IAssetManager)acc.AccountOwner).SupportLifecycles;
                }
                if (acc.ModelPortfolio != null)
                {
                    returnValue.Modelid = acc.ModelPortfolio.Key;
                    returnValue.ModelAllowExecOnlyCustomers = acc.ModelPortfolio.AllowExecOnlyCustomers;
                }
                if (acc.Lifecycle != null)
                    returnValue.LifecycleId = acc.Lifecycle.Key;
                returnValue.IsExecOnlyCustomer = acc.IsExecOnlyCustomer;

                if (acc.ShortName != null)
                    returnValue.AccountShortName = acc.ShortName;

                if (acc.FullName != null)
                    returnValue.AccountFullName = acc.FullName;

                if (acc.CounterAccount != null)
                    returnValue.CounterAccountID = acc.CounterAccount.Key;

                returnValue.TradeableStatus = acc.TradeableStatus;

                returnValue.Status = acc.Status;

                AccountStatus statusObj = AccountMapper.GetAccountStatus(session, acc.Status);
                returnValue.StatusIsOpen = (statusObj != null ? statusObj.IsOpen : true);

                returnValue.CreationDate = acc.CreationDate;
                returnValue.LastUpDated = acc.LastUpdated;
                returnValue.LastDateStatusChanged = acc.LastDateStatusChanged;
                returnValue.DateTradeabilityStatusChanged = acc.DateTradeabilityStatusChanged;

                returnValue.FirstManagementStartDate = acc.FirstManagementStartDate;
                returnValue.FinalManagementEndDate = acc.FinalManagementEndDate;

                if (!acc.ExitFeePayingAccount.Equals(acc))
                    returnValue.ExitFeePayingAccountID = acc.ExitFeePayingAccount.Key;

                returnValue.UseManagementFee = acc.UseManagementFee;
                returnValue.UseKickBack = acc.UseKickback;

                returnValue.Notification = acc.Notifications.DisplayMessages;
                returnValue.NotificationType = acc.Notifications.DisplayNotificationType;
                returnValue.IsDeparting = acc.IsDeparting;
                returnValue.IsUnderRebalance = acc.IsUnderRebalance;

                if (acc.AccountType == AccountTypes.Customer)
                {
                    ICustomerAccount cust = (ICustomerAccount)acc;
                    returnValue.IsJointAccount = cust.IsJointAccount;
                    if (cust.Family != null)
                        returnValue.FamilyID = cust.Family.Key;
                    if (cust.VerpandSoort != null)
                        returnValue.VerpandSoortID = cust.VerpandSoort.Key;

                    if (cust.PandHouder != null)
                        returnValue.PandHouderID = cust.PandHouder.Key;

                    returnValue.ContactContractsValidated = cust.ContactContractsValidated;

                    if (cust.RemisierEmployee != null)
                    {
                        returnValue.RemisierID = cust.RemisierEmployee.Remisier.Key;
                        returnValue.RemisierEmployeeID = cust.RemisierEmployee.Key;
                    }

                    if (cust.CurrentRemisierDetails != null)
                    {
                        returnValue.KickBack = cust.CurrentRemisierDetails.KickBack;
                        returnValue.IntroductionFee = cust.CurrentRemisierDetails.IntroductionFee;
                        returnValue.SubsequentDepositFee = cust.CurrentRemisierDetails.SubsequentDepositFee;
                        returnValue.IntroductionFeeReduction = cust.CurrentRemisierDetails.IntroductionFeeReduction;
                        returnValue.SubsequentDepositFeeReduction = cust.CurrentRemisierDetails.SubsequentDepositFeeReduction;
                    }

                    returnValue.EmployerRelationship = cust.EmployerRelationship;
                    if (cust.RelatedEmployee != null)
                        returnValue.RelatedEmployeeID = cust.RelatedEmployee.Key;

                    if (cust.FirstPromisedDeposit != null)
                        returnValue.FirstPromisedDeposit = cust.FirstPromisedDeposit.Quantity;
                }
                else
                    returnValue.IsCustomer = false;
            }
            session.Close();
            return returnValue;
        }

        public static string GetEnumAccountTypeCustomer()
        {
            return AccountTypes.Customer.ToString();
        }

        public static string GetContactName(int contactID)
        {
            string contactName = "";
            IDalSession session = NHSessionFactory.CreateSession();
            IContact contact = ContactMapper.GetContact(session, contactID);
            contactName = contact.CurrentNAW.Name;
            session.Close();
            return contactName;
        }

        public static void AddAccountHolder(int newContactID, int accountNrID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IContact newContact = ContactMapper.GetContact(session, newContactID);
            ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountNrID);
            IAccountHolder ah = new AccountHolder(acc, newContact);

            if (!acc.AccountHolders.Contains(ah))
            {
                acc.AccountHolders.Add(ah);
                if (acc.AccountHolders.Count == 1) acc.AccountHolders.SetPrimaryAccountHolder(newContact);
                AccountHolderMapper.Insert(session, ah);
            }
            session.Close();
        }

        public static void DetachAccountHolder(int accountKey, int contactKey)
        {
            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountKey);
                IContact contact = ContactMapper.GetContact(session, contactKey);

                if (acc != null && contact != null)
                {
                    IAccountHolder ah = acc.AccountHolders.GetItemByContact(contact);
                    if (ah != null)
                    {
                        acc.AccountHolders.Remove(ah);
                        AccountHolderMapper.Delete(session, ah);
                        if (acc.AccountHolders != null && ah.IsPrimaryAccountHolder)
                        {
                            acc.ShortName = "";
                            foreach (IAccountHolder otherAH in acc.AccountHolders)
                            {
                                if (!otherAH.Equals(ah))
                                {
                                    IContact otherContact = otherAH.Contact;
                                    acc.AccountHolders.SetPrimaryAccountHolder(otherContact);
                                    acc.ShortName = otherContact.FullName;
                                }
                            }
                        }
                        AccountMapper.Update(session, acc);
                    }
                    session.Close();
                }
            }
        }

        public static void SetPrimaryAccountHolder(int accountKey, int contactKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountKey);
                IContact contact = ContactMapper.GetContact(session, contactKey);

                if (acc != null && contact != null)
                {
                    acc.AccountHolders.SetPrimaryAccountHolder(contact);
                    AccountHolderMapper.Update(session, acc.AccountHolders.ToList());
                }
            }
        }

        public static DataSet GetAttachedCounterAccounts(int accountId, int contactID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                IList<ICounterAccount> counterAccounts = null;

                IAccount account = AccountMapper.GetAccount(session, accountId);
                if (account != null)
                    counterAccounts = CounterAccountMapper.GetCounterAccounts(session, account);
                else if (contactID != 0 && contactID != int.MinValue)
                {
                    IContact contact = ContactMapper.GetContact(session, contactID);
                    if (contact != null)
                        counterAccounts = CounterAccountMapper.GetCounterAccounts(session, contact);
                }

                if (counterAccounts != null)
                {
                    ds = counterAccounts
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayName
                    })
                    .ToDataSet();
                }
                else
                {
                    ds = new DataSet("CounterAccounts");
                    DataTable dt = new DataTable("CounterAccounts");
                    ds.Tables.Add(dt);
                    dt.Columns.Add("Key", typeof(int));
                    dt.Columns.Add("DisplayName", typeof(string));
                }
                Utility.AddEmptyFirstRow(ds);
                return ds;
            }
        }

        public static DataSet GetAccountAccountHolders(int accountId)
        {
            DataSet ds = ucAccountsEditAdapter.GetAccountAccountHolders(accountId);
            return ds;
        }

        public static void GetCurrentManagmentCompany(ref string name, ref int id)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            name = "";
            id = 0;

            if (company.CompanyName != null)
                name = company.CompanyName;

            id = company.Key;

            session.Close();
        }

        public static DataSet GetRemisiers()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            IList list = null;

            if (company != null && !company.IsStichting)
                list = RemisierMapper.GetRemisiers(session, (IAssetManager)company);
            else
                list = RemisierMapper.GetRemisiers(session);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                        list,
                        "Key, DisplayName");
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();
            return ds;
        }

        public static DataSet GetRemisierEmployees(int remisierID)
        {
            DataSet ds = null;

            if (remisierID != int.MinValue)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IRemisier remisier = RemisierMapper.GetRemisier(session, remisierID);

                if (remisier != null && remisier.Employees != null && remisier.Employees.Count > 0)
                {
                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                remisier.Employees.ToList(),
                                "Key, Employee.FullNameLastNameFirst, IsDefault");
                    if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        Util.SortDataTable(ds.Tables[0], "IsDefault DESC, Employee_FullNameLastNameFirst ASC");
                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                }
                session.Close();
            }
            return ds;
        }

        public static DataSet GetIntroducers(int companyID)
        {
            IDalSession session;
            DataSet ds = null;

            session = NHSessionFactory.CreateSession();
            ds = null;

            if (companyID != 0)
            {

                IAssetManager assetMan = ManagementCompanyMapper.GetAssetManager(session, companyID);
                if (assetMan.Remisiers != null)
                {
                    var list = from a in  assetMan.Remisiers
                               select a;
                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                               list.ToList(), "Key, Name");
                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                }
            }
            session.Close();
            return ds;
        }

        public static void SaveCustomerAccount(AccountDetails saveValue, bool forceClosedStatus, out bool askConfirmation, out string message)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
                throw new System.Security.SecurityException("You are not authorized to update account details.");

            IDalSession session = NHSessionFactory.CreateSession();
            message = "";
            try
            {
                ICustomerAccount acc = null;
                bool isNewAccount = false;
                IPortfolioModel model = null;
                IPandHouder pandhouder = null;
                IVerpandSoort verpandSoort = null;
                ICounterAccount counterAccount = null;
                IInternalEmployeeLogin relatedEmployee = null;
                IRemisierEmployee remisierEmployee = null;
                IAccountTypeCustomer exitFeePayer = null;
                Money firstPromisedDeposit = null;
                DateTime upDateTime = session.GetServerTime();
                IInternalEmployeeLogin employee = (IInternalEmployeeLogin)LoginMapper.GetCurrentLogin(session);

                if (saveValue.AccountNrID == int.MinValue)
                    isNewAccount = true;

                if (saveValue.PandHouderID != int.MinValue)
                    pandhouder = PandHouderMapper.GetPandHouder(session, saveValue.PandHouderID);

                if (saveValue.VerpandSoortID != int.MinValue)
                    verpandSoort = VerpandSoortMapper.GetVerpandSoort(session, saveValue.VerpandSoortID);

                if (saveValue.CounterAccountID != int.MinValue)
                    counterAccount = CounterAccountMapper.GetCounterAccount(session, saveValue.CounterAccountID);

                if (saveValue.Modelid != int.MinValue)
                    model = ModelMapper.GetModel(session, saveValue.Modelid);

                if (saveValue.ExitFeePayingAccountID != int.MinValue)
                    exitFeePayer = (IAccountTypeCustomer)AccountMapper.GetAccount(session, saveValue.ExitFeePayingAccountID);

                if (saveValue.RemisierEmployeeID != int.MinValue)
                    remisierEmployee = RemisierEmployeeMapper.GetRemisierEmployee(session, saveValue.RemisierEmployeeID);

                if (saveValue.RelatedEmployeeID != int.MinValue)
                    relatedEmployee = (IInternalEmployeeLogin)LoginMapper.GetLogin(session, saveValue.RelatedEmployeeID);

                if (saveValue.FirstPromisedDeposit > 0M)
                {
                    ICurrency baseCurrency = InstrumentMapper.GetBaseCurrency(session);
                    firstPromisedDeposit = new Money(saveValue.FirstPromisedDeposit, baseCurrency);
                }

                IAssetManager am = ManagementCompanyMapper.GetAssetManager(session, saveValue.AssetManagerKey);

                if (isNewAccount)
                {
                    //string newAccountNr = am.GenerateAccountNumber();
                    //acc = new CustomerAccount(newAccountNr, "", am, model);
                }
                else
                {
                    acc = (ICustomerAccount)AccountMapper.GetAccount(session, saveValue.AccountNrID);

                    acc.Family = AccountFamilyMapper.GetAccountFamily(session, saveValue.FamilyID);

                    if (!AccountMapper.AccountStatusIsOpen(session, acc.Status))
                        throw new ApplicationException("Could not save account because its status is already closed.");

                    acc.AccountOwner = am;
                }

                askConfirmation = false;

                if (!isNewAccount && !AccountMapper.AccountStatusIsOpen(session, saveValue.Status))
                {
                    //IList positions = AccountMapper.GetPositions(session, acc, PositionReturnClass.AllPositions, PositionsView.NotZero);
                    //if (positions.Count > 0)
                    //    throw new ApplicationException(
                    //        string.Format("Could not close account because it has {0} open position{1} ({2} total portfolio value).",
                    //                      positions.Count, (positions.Count > 1 ? "s" : ""), acc.TotalPositionAmount(PositionAmountReturnValue.All)));

                    //if (!forceClosedStatus)
                    //{
                    //    askConfirmation = true;
                    //    return;
                    //}
                }

                if (am.SupportLifecycles && ((acc.Lifecycle != null ? acc.Lifecycle.Key : int.MinValue) != saveValue.LifecycleId))
                {
                    if (saveValue.LifecycleId != int.MinValue)
                        AccountOverviewAdapter.CheckLifecycleForAccount(acc, acc.PrimaryAccountHolder);

                    acc.Lifecycle = saveValue.LifecycleId != int.MinValue ? LifecycleMapper.GetLifecycle(session, saveValue.LifecycleId) : null;
                }

                if ((acc.ModelPortfolio == null && saveValue.Modelid != int.MinValue ) 
                    || (acc.ModelPortfolio != null && acc.ModelPortfolio.Key != saveValue.Modelid)
                    || acc.IsExecOnlyCustomer != saveValue.IsExecOnlyCustomer 
                    || acc.EmployerRelationship != saveValue.EmployerRelationship
                    || isNewAccount)
                {
                    if (acc.ModelPortfolio != null && model == null && acc.TradeableStatus == Tradeability.Tradeable)
                        throw new ApplicationException("The Model is mandatory when the account is tradeable.");
                    
                    if (acc.ActiveRebalanceInstructions != null && acc.ActiveRebalanceInstructions.Count > 0)
                    {
                        foreach (IInstruction instruction in acc.ActiveRebalanceInstructions)
                        {
                            if (instruction.Status > 1)
                                throw new ApplicationException(string.Format("It is currently not possible to change the Model since an active rebalance instruction exists for account {0}.", acc.DisplayNumberWithName));
                        }
                    }
                    acc.SetModelPortfolio(acc.Lifecycle, model, saveValue.IsExecOnlyCustomer, saveValue.EmployerRelationship, employee, DateTime.Now);
                }

                acc.IsJointAccount = saveValue.IsJointAccount;
                acc.ShortName = saveValue.AccountShortName;
                acc.VerpandSoort = verpandSoort;
                acc.PandHouder = pandhouder;
                acc.CounterAccount = counterAccount;
                acc.ExitFeePayingAccount = exitFeePayer;
                acc.EmployerRelationship = saveValue.EmployerRelationship;
                acc.RelatedEmployee = relatedEmployee;
                acc.FirstPromisedDeposit = firstPromisedDeposit;
                DateTime originalMgtEndDate = acc.ManagementEndDate;
                acc.FinalManagementEndDate = saveValue.FinalManagementEndDate;
                if (acc.TradeableStatus != saveValue.TradeableStatus)
                {
                    // check
                    if (saveValue.TradeableStatus == Tradeability.Tradeable && acc.ModelPortfolio == null)
                        throw new ApplicationException("A Model is mandatory on the account to be tradeable.");
                    
                    acc.DateTradeabilityStatusChanged = upDateTime;
                    acc.TradeableStatus = saveValue.TradeableStatus;
                }

                if (acc.Status != saveValue.Status)
                {
                    acc.LastDateStatusChanged = upDateTime;
                    acc.Status = saveValue.Status;

                    if (!AccountMapper.AccountStatusIsOpen(session, acc.Status))
                    {
                        // Set Closed Model
                        if (acc.AccountOwner.CompanyType == ManagementCompanyType.AssetManager)
                        {
                            IPortfolioModel closedModel = ((IAssetManager)acc.AccountOwner).ClosedModelPortfolio;
                            if (closedModel != null)
                                acc.SetModelPortfolio(acc.Lifecycle, closedModel, acc.IsExecOnlyCustomer, acc.EmployerRelationship, employee, acc.LastDateStatusChanged.Date);
                        }

                        DateTime finalEndDate = Util.IsNotNullDate(acc.ManagementEndDate) ? acc.ManagementEndDate : acc.LastDateStatusChanged.Date;
                        acc.ValuationsEndDate = finalEndDate;
                        if (Util.IsNullDate(acc.FinalManagementEndDate))
                            acc.FinalManagementEndDate = finalEndDate;
                    }
                }

                if (Util.IsNullDate(originalMgtEndDate) && Util.IsNotNullDate(saveValue.FinalManagementEndDate))
                {
                    if (acc.ActiveWithdrawalInstructions.Count > 0)
                    {
                        InstructionEngine engine = new InstructionEngine();
                        int instructionsCount = 0;
                        foreach (ICashWithdrawalInstruction instruction in acc.ActiveWithdrawalInstructions)
                        {
                            if (instruction.WithdrawalDate >= saveValue.FinalManagementEndDate)
                            {
                                if (engine.CancelInstruction(instruction))
                                    instructionsCount++;
                                else
                                    throw new ApplicationException(
                                        string.Format("Could not set the Management End since withdrawal instruction {0} could not be cancelled, please cancel the instruction in the 'Withdrawal Management Console'.", instruction.Key));
                            }
                        }
                        message = string.Format("{0} withdrawal instruction(s) were cancelled", instructionsCount);
                    }

                    if (acc.WithdrawalRules.Count > 0)
                    {
                        int rulesCount = 0;
                        foreach (IWithdrawalRule rule in acc.WithdrawalRules)
                        {
                            if (rule.IsActive && (Util.IsNullDate(rule.EndDateWithdrawal) || rule.EndDateWithdrawal > saveValue.FinalManagementEndDate))
                            {
                                rule.EndDateWithdrawal = saveValue.FinalManagementEndDate;
                                rulesCount++;
                            }
                        }
                        message += Environment.NewLine + string.Format("{0} withdrawal rule(s) were cancelled", rulesCount);
                    }
                }

                // if FinalManagementEndDate > ValuationsEndDate -> update ValuationsEndDate
                if (Util.IsNotNullDate(acc.ValuationsEndDate) && acc.FinalManagementEndDate > acc.ValuationsEndDate)
                    acc.ValuationsEndDate = acc.FinalManagementEndDate;

                if (acc.UseManagementFee != saveValue.UseManagementFee)
                    acc.UseManagementFee = saveValue.UseManagementFee;
                if (acc.UseKickback != saveValue.UseKickBack)
                    acc.UseKickback = saveValue.UseKickBack;
                IRemisierHistory rh = new RemisierHistory(remisierEmployee, saveValue.KickBack, saveValue.IntroductionFee, saveValue.IntroductionFeeReduction, saveValue.SubsequentDepositFee, saveValue.SubsequentDepositFeeReduction, employee, DateTime.Now.Date);
                if (acc.CurrentRemisierDetails == null || !acc.CurrentRemisierDetails.Equals(rh))
                    acc.RemisierDetailChanges.Add(rh);

                if (isNewAccount)
                    AccountMapper.Insert(session, acc);
                else
                    AccountMapper.Update(session, acc);
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet GetPandHouders()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                PandHouderMapper.GetPandHouders(session), "Key, Name");
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();
            return ds;
        }

        public static DataSet GetAccountHolders(int accountId)
        {
            return ucAccountsEditAdapter.GetAccountAccountHolders(accountId);
        }

        public static DataSet GetWithdrawalRules(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<IWithdrawalRule> list = WithdrawalRuleMapper.GetWithdrawalRules(session, accountId, ActivityReturnFilter.All);
                if (list != null)
                {
                    return list
                    .Select(c => new
                    {
                        c.Key,
                        Amount_Quantity = c.Amount != null ? c.Amount.Quantity : 0M,
                        Regularity_Description = c.Regularity.GetS(e => e.Description),
                        c.FirstDateWithdrawal,
                        c.EndDateWithdrawal,
                        CounterAccount_DisplayNameShort = c.CounterAccount.GetS(e => e.DisplayNameShort),
                        c.DoNotChargeCommission,
                        c.IsActive,
                        c.TransferDescription,
                        c.CreationDate,
                        c.CreatedBy
                    })
                    .ToDataSet();
                }
                else
                    return null;
            }
        }

        public static DataSet GetVerpandSoort()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                VerpandSoortMapper.GetVerpandSoorten(session), "Key, Description");
            Util.AddSpacesBetweenCapitalsInDataColumn(ds.Tables[0], 1);
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();
            return ds;
        }

        public static DataSet GetEmployerRelationshipList()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(AccountEmployerRelationship), string.Empty, string.Empty, SortingDirection.Ascending, true);
            return ds;
        }

        public static DataSet GetEmployees()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList employees = LoginMapper.GetEmployees(session, ActivityReturnFilter.Active);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                employees, "Key, UserName");
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetAccountCategories(int assetManagerID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAssetManager am = ManagementCompanyMapper.GetAssetManager(session, assetManagerID);
            IList collAccountCat = AccountCategoryMapper.GetAccountCategories(session, am);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                collAccountCat, "Key, CustomerType");

            if (collAccountCat != null && collAccountCat.Count > 1)
                Utility.AddEmptyFirstRow(ds.Tables[0]);
            session.Close();
            return ds;
        }

        public static DataSet GetAccountStatuses()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(AccountMapper.GetAccountStatuses(session), "Key, Name, IsOpen");
            session.Close();
            return ds;
        }

        public static DataSet GetAccountTradeabilityStatuses()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetAccountTradeabilityStatuses(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Name
                    })
                    .ToDataSet();
            }
        }

        public static ExecutionOnlyOptions GetModelExecutionOnlyOptions(int modelId)
        {
            if (modelId == int.MinValue)
                return ExecutionOnlyOptions.NotAllowed;
            
            IDalSession session = NHSessionFactory.CreateSession();
            IPortfolioModel model = ModelMapper.GetModel(session, modelId);
            if (model != null)
                return model.ExecutionOptions;
            else
                return ExecutionOnlyOptions.NotAllowed;
        }

        public static DataSet GetModelHistory(int accountId)
        {
            string hql = string.Format(@"from ModelHistory H
                left join fetch H.ModelPortfolio M
                left join fetch H.Employee E
                where H.Account.Key = {0}
                order by H.ChangeDate", accountId);

            IDalSession session = NHSessionFactory.CreateSession();
            IList list = session.GetListByHQL(hql);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                list,
                "Key, ModelPortfolio.ModelName, ModelPortfolioKey, IsExecOnlyCustomer, EmployerRelationship, ChangeDate, EndDate, Employee.UserName");
            session.Close();
            return ds;
        }

        public static DataSet GetAccountFeeRules(int accountId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                FeeRuleMapper.GetAccountFeeRules(session, accountId),
                "Key, FeeCalculation.Name, ExecutionOnly, SendByPost, StartPeriod, EndPeriod");
            session.Close();
            return ds;
        }

        public static DataSet GetActiveFeeCalculations(int accountId)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();

            ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
            if (account != null)
            {
                if (account.AccountOwner != null)
                    parameters.Add("managementCompanyID", account.AccountOwner.Key);
                IList list = session.GetListByNamedQuery(
                    "B4F.TotalGiro.Fees.FeeCalculations.ActiveFeeCalculations",
                    parameters);

                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list,
                    "Key, Name");
                Utility.AddEmptyFirstRow(ds.Tables[0]);
            }

            session.Close();
            return ds;
        }

        public static void UpdateAccountFeeRule(int endPeriod, int original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IFeeRule rule = FeeRuleMapper.GetFeeRule(session, original_Key);
            if (rule != null)
            {
                rule.EndPeriod = endPeriod;
                session.Update(rule);
            }

            session.Close();
        }

        public static bool CreateAccountFeeRule(int accountId, int feeCalculationId,
            bool executionOnly, bool sendByPost, int startPeriod, int endPeriod)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
            IFeeCalc calc = FeeCalcMapper.GetFeeCalculation(session, feeCalculationId);

            FeeRule rule = new FeeRule(calc, null, account, false, executionOnly, false, sendByPost, startPeriod);
            if (endPeriod > 0)
                rule.EndPeriod = endPeriod;
            bool success = FeeRuleMapper.Insert(session, rule);
            session.Close();
            return success;
        }

        public static bool DeleteModelHistoryItem(int modelHistoryID, int accountId)
        {
            bool retVal = false;
            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ICustomerAccount acc = null;
                bool saveAccount = false;

                IModelHistory mh = (IModelHistory)session.GetObjectInstance(typeof(ModelHistory), modelHistoryID);
                if (mh != null)
                {
                    if (mh.EndDate == DateTime.MinValue)
                    {
                        acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
                        if (acc.ModelPortfolioChanges != null && acc.ModelPortfolioChanges.Count > 1)
                        {
                            IModelHistory[] sortedHistory = acc.ModelPortfolioChanges.OrderByDescending(a => a.ChangeDate).ToArray();
                            acc.ModelPortfolio = sortedHistory[1].ModelPortfolio;
                            //acc.ModelPortfolioChanges.Remove(mh);
                            saveAccount = true;
                        }
                        else
                            throw new ApplicationException("It is not possible to delete the latest model history item. Otherwise the data would no longer be in sync.");
                    }

                    if (saveAccount && acc != null)
                    {
                        session.Update(acc);
                        session = NHSessionFactory.CreateSession();
                    }
                    retVal = session.Delete(mh);
                }
                else
                    throw new ApplicationException("Could not find model history item " + modelHistoryID.ToString());
                session.Close();
            }
            return retVal;
        }

        public static void UpdateModelHistoryItem(DateTime ChangeDate, bool IsExecOnlyCustomer,
            int modelID, int employerRelationship, int original_Key)
        {
            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IModelHistory mh = (IModelHistory)session.GetObjectInstance(typeof(ModelHistory), original_Key);
                if (mh != null)
                {
                    IPortfolioModel model = ModelMapper.GetModel(session, modelID);
                    AccountEmployerRelationship aer = (AccountEmployerRelationship)employerRelationship;
                    if (mh.Edit(model, IsExecOnlyCustomer, aer, (IInternalEmployeeLogin)LoginMapper.GetCurrentLogin(session), ChangeDate))
                        session.Update(mh);
                }
                else
                    throw new ApplicationException("Could not find model history item " + original_Key.ToString());
                session.Close();
            }
        }

        public static DataSet GetManagementPeriods(int accountId)
        {
            string hql = string.Format(@"from ManagementPeriod M
                where M.Account.Key = {0}
                order by M.StartDate, M.ManagementType", accountId);

            IDalSession session = NHSessionFactory.CreateSession();
            IList list = session.GetListByHQL(hql);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                list,
                "Key, ManagementType, StartDate, EndDateDisplayString, Employee, CreationDate, IsEditable");
            session.Close();
            return ds;
        }

        public static void CreateManagementPeriod(int accountId, int managementType, DateTime startDate)
        {
            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountId);
                IManagementPeriod mp = new ManagementPeriod((ManagementTypes)managementType, startDate);
                if (acc != null && mp != null)
                {
                    acc.ManagementPeriods.AddManagementPeriod(mp);
                    AccountMapper.Update(session, acc);
                }
                else
                    throw new ApplicationException("Error during creating a new management period");
                session.Close();
            }
        }

        public static void GetManagementPeriodDetails(int managementPeriodID, out int managementTypeId, out DateTime startDate , out DateTime endDate)
        {
            managementTypeId = 0;
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IManagementPeriod mp = (IManagementPeriod)session.GetObjectInstance(typeof(ManagementPeriod), managementPeriodID);
                if (mp != null)
                {
                    managementTypeId = (int)mp.ManagementType;
                    startDate = mp.StartDate;
                    if (mp.EndDate.HasValue)
                        endDate = mp.EndDate.Value;
                }
                else
                    throw new ApplicationException("Could not find management period " + managementPeriodID.ToString());
                session.Close();
            }
        }

        public static void UpdateManagementPeriod(DateTime StartDate , DateTime EndDate, int original_Key)
        {
            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IManagementPeriod mp = (IManagementPeriod)session.GetObjectInstance(typeof(ManagementPeriod), original_Key);
                if (mp != null)
                {
                    if (mp.Edit(StartDate, EndDate))
                        session.Update(mp);
                }
                else
                    throw new ApplicationException("Could not find management period " + original_Key.ToString());
                session.Close();
            }
        }

        public static bool DeleteManagementPeriod(int managementPeriodID)
        {
            bool retVal = false;
            bool saveAcc = false;


            if (SecurityManager.IsCurrentUserInRole("Data Mtce: Account Edit"))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IManagementPeriod mp = (IManagementPeriod)session.GetObjectInstance(typeof(ManagementPeriod), managementPeriodID);
                IAccountTypeCustomer account = null;
                if (mp != null)
                {
                    account = mp.Account;
                    if (mp.ManagementPeriodUnits != null && mp.ManagementPeriodUnits.Count > 0)
                        throw new ApplicationException("It is not possible to delete the selected management period. Contact your system administrator.");
                    else
                    {
                        if (mp.Equals(account.CurrentManagementFeePeriod))
                        {
                            account.CurrentManagementFeePeriod = null;
                            for (int i = account.ManagementPeriods.Count; i > 0; i--)
                            {
                                IManagementPeriod period = account.ManagementPeriods[i - 1];
                                if (!mp.Equals(period) && mp.ManagementType == period.ManagementType)
                                {
                                    account.CurrentManagementFeePeriod = period;
                                    break;
                                }
                            }
                            account.ManagementPeriods.Remove(mp);
                            saveAcc = true;
                        }
                    }
                    session.BeginTransaction();
                    if (saveAcc && account != null)
                        AccountMapper.Update(session, account);
                    retVal = session.Delete(mp);
                    session.CommitTransaction();
                }
                else
                    throw new ApplicationException("Could not find management period " + managementPeriodID.ToString());
                session.Close();
            }
            return retVal;
        }

        public static DataSet GetManagementTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(ManagementTypes), string.Empty, string.Empty, SortingDirection.Ascending, true);
            return ds;
        }

        public static DataSet GetOtherOwnAccounts(int accountID)
        {
            string hql = string.Format(@"from CustomerAccount A
                where A.Key in (
                    select H.GiroAccount.Key
                    from AccountHolder H
                    where H.Contact.Key in (
                        select H.Contact.Key
                        from CustomerAccount A
                        left join A.bagOfAccountHolders H
                        where A.Key = {0})
                ) and A.Key != {0}", accountID);

            IDalSession session = NHSessionFactory.CreateSession();
            IList list = session.GetListByHQL(hql);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                list,
                "Key, DisplayNumberWithName");
            session.Close();
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static string CheckAccountsWithdrawals(int accountID, DateTime finalManagementEndDate)
        {
            string message = "";
            if (accountID != 0)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountID);

                if (acc != null && Util.IsNullDate(acc.FinalManagementEndDate))
                {
                    if (acc.ActiveWithdrawalInstructions.Count > 0)
                    {
                        int counter = 0;
                        foreach (ICashWithdrawalInstruction instruction in acc.ActiveWithdrawalInstructions)
                        {
                            if (instruction.IsActive && instruction.WithdrawalDate > finalManagementEndDate)
                                counter++;
                        }
                        if (counter > 0)
                            message = string.Format("The account has {0} active withdrawal instructions, are you sure to set the management end date?", counter);
                    }
                }
            }
            return message;
        }

        public static string CheckKickBackManagementPeriod(int accountID, bool useKickBack)
        {
            string message = "";
            if (accountID != 0 && !useKickBack)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ICustomerAccount acc = (ICustomerAccount)AccountMapper.GetAccount(session, accountID);

                if (acc != null && acc.CurrentKickBackFeePeriod != null)
                    message = "The account has an active kickback management period, do you need to set the end date?";

                session.Close();
            }
            return message;
        }

        public static bool AddAccountFinanicalTarget(AccountFinancialTargetHelper accountFinancialTarget)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                if (accountFinancialTarget.ParentAccountID != 0)
                {
                    ICustomerAccount account = (ICustomerAccount)AccountMapper.GetAccount(session, accountFinancialTarget.ParentAccountID);

                    ICurrency currency = InstrumentMapper.GetCurrency(session, accountFinancialTarget.CurrencyID);
                    ILogin createdBy = LoginMapper.GetCurrentLogin(session);

                    if (account.CurrentFinancialTarget != null &&
                        account.CurrentFinancialTarget.TargetAmount.Quantity == accountFinancialTarget.TargetAmountSize &&
                        account.CurrentFinancialTarget.TargetAmount.Underlying.Key == currency.Key &&
                        account.CurrentFinancialTarget.DepositPerYear.Quantity == accountFinancialTarget.DepositPerYearSize &&
                        account.CurrentFinancialTarget.DepositPerYear.Underlying.Key == currency.Key &&
                        account.CurrentFinancialTarget.TargetEndDate == accountFinancialTarget.TargetEndDate)
                        return false;

                    AccountFinancialTarget newTarget = new AccountFinancialTarget();
                    newTarget.ParentAccount = account;
                    newTarget.TargetAmount = new Money(accountFinancialTarget.TargetAmountSize, currency);
                    newTarget.DepositPerYear = new Money(accountFinancialTarget.DepositPerYearSize, currency);
                    newTarget.TargetEndDate = accountFinancialTarget.TargetEndDate;
                    newTarget.CreatedBy = createdBy;
                    newTarget.CreationDate = DateTime.Now;

                    account.CurrentFinancialTarget = newTarget;
                    session.Update(account);
                }
                else
                    throw new ApplicationException("Could not identify Account");

                return true;
            }
        }
    }
}
