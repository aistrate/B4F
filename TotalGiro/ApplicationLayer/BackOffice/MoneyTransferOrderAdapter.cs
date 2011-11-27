using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice
{
    public struct StichtingDetails
    {
        public string NarDebet1;
        public string NarDebet2;
        public string NarDebet3;
        public string NarDebet4;
        public int DefaultJournalID;
    }

    public class MoneyTransferOrderDetails
    {
        public int MoneyOrderID { get; set; }
        public string Reference { get; set; }
        public int FromJournal { get; set; }
        public Decimal AmountQty { get; set; }
        public int AmountID { get; set; }
        public int Accountid { get; set; }
        public int CounterAccountID { get; set; }
        public string NarBenef1 { get; set; }
        public string NarBenef2 { get; set; }
        public string NarBenef3 { get; set; }
        public string NarBenef4 { get; set; }
        public string NarDebet1 { get; set; }
        public string NarDebet2 { get; set; }
        public string NarDebet3 { get; set; }
        public string NarDebet4 { get; set; }
        public string SwiftAddress { get; set; }
        public string BenefBankAcctNr { get; set; }
        public string TransferDescription1 { get; set; }
        public string TransferDescription2 { get; set; }
        public string TransferDescription3 { get; set; }
        public string TransferDescription4 { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime CreationDate { get; set; }
        public MoneyTransferOrderStati Status { get; set; }
        public bool IsEditable { get; set; }
        public IndicationOfCosts CostIndication { get; set; }
    }

    public static class MoneyTransferOrderAdapter
    {
        public static DataSet GetWithdrawalJournals()
        {
            //"Key, BankAccountNumber, BankAccountDescription, ManagementCompany.CompanyName, Currency.Symbol"
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return JournalMapper.GetJournals(session, JournalTypes.BankStatement)
                    .Select(c => new
                    {
                        c.Key,
                        c.BankAccountNumber,
                        c.BankAccountDescription,
                        ManagementCompany_CompanyName = 
                            c.ManagementCompany.CompanyName,
                        Currency_Symbol =
                            c.Currency.Symbol
                    })
                    .ToDataSet();
            }
        }

        public static IJournal GetJournal(int journalID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return JournalMapper.GetJournal(session, journalID);
        }


        public static StichtingDetails GetStichtingDetails()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IEffectenGiro detail = ManagementCompanyMapper.GetEffectenGiroCompany(session);

            StichtingDetails returnValue = new StichtingDetails();
            returnValue.DefaultJournalID = detail.DefaultWithdrawJournal.Key;
            returnValue.NarDebet1 = detail.StichtingName;
            returnValue.NarDebet2 = detail.ResidentialAddress.AddressLine1;
            returnValue.NarDebet3 = detail.ResidentialAddress.AddressLine2;
            returnValue.NarDebet4 = detail.ResidentialAddress.Country.CountryName;

            return returnValue;

        }

        public static DataSet GetPredefinedBeneficiaries()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                            PredefinedBeneficiaryMapper.GetPredefinedBeneficiaries(session),
                                            "Key, BenefBankAcctNr, NarBenef1, NarBenef2, NarBenef3, ,NarBenef4, Description1");

            session.Close();

            return ds;
        }

        public static PredefinedBeneficiary GetPredefinedBeneficiary(int predefinedKey)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return PredefinedBeneficiaryMapper.GetPredefinedBeneficiary(session, predefinedKey);
        }

        public static DataSet GetCurrencies()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetCurrenciesSortedByCurrency(session)
                    .Select(c => new
                    {
                        c.Key,
                        c.Symbol
                    })
                    .ToDataSet();
            }
        }

        public static ICurrency GetCurrency(int currencyID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return InstrumentMapper.GetCurrency(session, currencyID);
        }

        public static ICustomerAccount GetCustomerAccount(int accountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return (ICustomerAccount)AccountMapper.GetAccount(session, accountID);
        }

        public static ICounterAccount GetCounterAccount(int counterAccountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            return CounterAccountMapper.GetCounterAccount(session, counterAccountID);
        }

        public static MoneyTransferOrderDetails LoadMoneyTransferOrder(int moneyTransferOrderID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            MoneyTransferOrderDetails returnValue = new MoneyTransferOrderDetails();

            IMoneyTransferOrder result = MoneyTransferOrderMapper.GetMoneyTransferOrder(session, moneyTransferOrderID);

            if (result != null)
            {
                returnValue.MoneyOrderID = result.Key;
                returnValue.Reference = result.Reference;
                returnValue.FromJournal = result.TransferorJournal.Key;
                returnValue.AmountQty = result.Amount.Quantity;
                returnValue.AmountID = result.Amount.Underlying.Key;
                if(result.TransfereeAccount != null) returnValue.Accountid = result.TransfereeAccount.Key;
                if (result.TransfereeCounterAccount != null) returnValue.CounterAccountID = result.TransfereeCounterAccount.Key;
                returnValue.NarBenef1 = result.NarBenef1;
                returnValue.NarBenef2 = result.NarBenef2;
                returnValue.NarBenef3 = result.NarBenef3;
                returnValue.NarBenef4 = result.NarBenef4;
                returnValue.NarDebet1 = result.NarDebet1;
                returnValue.NarDebet2 = result.NarDebet2;
                returnValue.NarDebet3 = result.NarDebet3;
                returnValue.NarDebet4 = result.NarDebet4;
                returnValue.SwiftAddress = result.SwiftAddress;
                returnValue.BenefBankAcctNr = result.BenefBankAcctNr;
                returnValue.TransferDescription1 = result.TransferDescription1;
                returnValue.TransferDescription2 = result.TransferDescription2;
                returnValue.TransferDescription3 = result.TransferDescription3;
                returnValue.TransferDescription4 = result.TransferDescription4;
                returnValue.ProcessDate = result.ProcessDate;
                returnValue.CreationDate = result.CreationDate;
                returnValue.Status = result.Status;
                returnValue.IsEditable = result.IsEditable;
                returnValue.CostIndication = result.CostIndication;
            }
            session.Close();
            return returnValue;
        }


        public static Tuple<int, bool, string> SaveMoneyTransferOrder(MoneyTransferOrderDetails moneyTransferOrderDetails, bool ignoreWarning)
        {
            if (!SecurityManager.IsCurrentUserInRole("Back Office: Money Transfer.Money Transfer Order"))
                throw new System.Security.SecurityException("You are not authorized to create/edit Money Orders.");

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                int MoneyOrderID = moneyTransferOrderDetails.MoneyOrderID;
                bool success = true;
                string errMessage = "";

                IMoneyTransferOrder moneyOrder = null;
                if (MoneyOrderID != 0)
                    moneyOrder = MoneyTransferOrderMapper.GetMoneyTransferOrder(session, MoneyOrderID);
                else
                    moneyOrder = new MoneyTransferOrder();

                moneyOrder.BenefBankAcctNr = moneyTransferOrderDetails.BenefBankAcctNr;
                moneyOrder.NarBenef1 = moneyTransferOrderDetails.NarBenef1;
                moneyOrder.NarBenef2 = moneyTransferOrderDetails.NarBenef2;
                moneyOrder.NarBenef3 = moneyTransferOrderDetails.NarBenef3;
                moneyOrder.NarBenef4 = moneyTransferOrderDetails.NarBenef4;
                moneyOrder.NarDebet1 = moneyTransferOrderDetails.NarDebet1;
                moneyOrder.NarDebet2 = moneyTransferOrderDetails.NarDebet2;
                moneyOrder.NarDebet3 = moneyTransferOrderDetails.NarDebet3;
                moneyOrder.NarDebet4 = moneyTransferOrderDetails.NarDebet4;
                moneyOrder.SwiftAddress = moneyTransferOrderDetails.SwiftAddress;
                moneyOrder.TransferDescription1 = moneyTransferOrderDetails.TransferDescription1;
                moneyOrder.TransferDescription2 = moneyTransferOrderDetails.TransferDescription2;
                moneyOrder.TransferDescription3 = moneyTransferOrderDetails.TransferDescription3;
                moneyOrder.TransferDescription4 = moneyTransferOrderDetails.TransferDescription4;
                moneyOrder.ProcessDate = moneyTransferOrderDetails.ProcessDate;
                moneyOrder.CreatedBy = Security.SecurityManager.CurrentUser;
                moneyOrder.Approved = false;
                moneyOrder.ApprovedBy = null;
                moneyOrder.ApprovalDate = DateTime.MinValue;
                moneyOrder.CostIndication = moneyTransferOrderDetails.CostIndication;

                ICurrency amtCurrency = GetCurrency(moneyTransferOrderDetails.AmountID);
                moneyOrder.Amount = new Money(moneyTransferOrderDetails.AmountQty, amtCurrency);

                moneyOrder.TransferorJournal = GetJournal(moneyTransferOrderDetails.FromJournal);
                if (moneyTransferOrderDetails.Accountid != 0)
                    moneyOrder.TransfereeAccount = GetCustomerAccount(moneyTransferOrderDetails.Accountid);

                if (moneyTransferOrderDetails.CounterAccountID != 0)
                    moneyOrder.TransfereeCounterAccount = GetCounterAccount(moneyTransferOrderDetails.CounterAccountID);

                if (!ignoreWarning)
                {
                    var result = moneyOrder.Validate();
                    success = result.Item1;
                    errMessage = result.Item2;
                }

                if (success)
                    success = MoneyTransferOrderMapper.Update(session, moneyOrder);
                return new Tuple<int, bool, string>(moneyOrder.Key, success, errMessage);
            }
        }


    }
}
