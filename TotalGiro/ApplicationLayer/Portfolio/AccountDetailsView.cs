using System;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Notifications;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Accounts.Instructions;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public class AccountDetailsView
    {
        internal AccountDetailsView() { }
        
        public AccountDetailsView(ICurrency baseCurrency)
        {
            Money zeroAmount = new Money(0m, baseCurrency);
            TotalAll = zeroAmount.DisplayString;
            TotalPositions = TotalAll;
            TotalCash = TotalAll;

            Status = AccountStati.Active;
            StatusIsOpen = true;
        }

        public AccountDetailsView(IAccountTypeCustomer account)
        {
            AccountNumberWithName = account.DisplayNumberWithName;
            AccountName = account.ShortName;
            ManagementStartDate = account.ManagementStartDate;

            // Set the flag for Crumble account
            IsCrumbleAccount = account.AccountType == AccountTypes.Crumble;

            ICustomerAccount customerAccount = null;
            if (account.AccountType == AccountTypes.Customer)
            {
                customerAccount = (ICustomerAccount)account;
                if (customerAccount.AccountHolders.PrimaryAccountHolder != null)
                {
                    PrimaryAccountHolderName = customerAccount.Formatter.AddressFirstLine;
                    SecondaryAccountHolderName = customerAccount.Formatter.AddressSecondLine;
                    StreetAddressLine = customerAccount.Formatter.Address.Get(a => a.StreetAddressLine);
                    CityAddressLine = customerAccount.Formatter.Address.Get(a => a.CityAddressLine);
                    CountryAddressLine = customerAccount.Formatter.Address.Get(a => a.CountryAddressLine);
                }

                if (customerAccount.RemisierEmployee != null)
                {
                    RemisierEmployee = customerAccount.RemisierEmployee.LoginPerson.FullName;
                    Remisier = customerAccount.RemisierEmployee.Remisier.Name;
                }

                if (customerAccount.VerpandSoort != null)
                    VerpandSoort = Util.Capitalize(Util.SplitCamelCase(customerAccount.VerpandSoort.Description).ToLower());
                
                if (customerAccount.PandHouder != null)
                    Pandhouder = customerAccount.PandHouder.Name;
            }

            ModelName = account.ModelPortfolio != null ? account.ModelPortfolio.ModelName : "";
            AccountNumber = account.Number;
            Status = account.Status;
            StatusIsOpen = account.Status == AccountStati.Active;
            IsTradeable = account.TradeableStatus == Tradeability.Tradeable;

            Money totalAll = account.TotalPositionAmount(PositionAmountReturnValue.All);
            Money totalCash = account.TotalCashAmount;
            Money totalPositions = totalAll - totalCash;

            TotalAll = totalAll.DisplayString;
            TotalCash = totalCash.DisplayString;
            TotalCashQuantity = totalCash.Quantity;
            TotalPositions = totalPositions.DisplayString;

            LastRebalanceDate = account.LastRebalanceDate;
            CurrentRebalanceDate = account.CurrentRebalanceDate;
            if (account.IsDeparting)
            {
                IsDeparting = true;
                FutureWithdrawalAmount = totalAll.Quantity;
                DisplayFutureWithdrawalAmount = TotalAll + " (Portfolio Liquidation)";
            }
            else if (account.ActiveWithdrawalInstructions != null)
            {
                Money withdrawalAmount = account.ActiveWithdrawalInstructions.TotalAmount;
                if (withdrawalAmount != null && withdrawalAmount.IsNotZero)
                {
                    FutureWithdrawalAmount = withdrawalAmount.Quantity;
                    DisplayFutureWithdrawalAmount = withdrawalAmount.DisplayString;
                }
            }
            IsUnderRebalance = account.IsUnderRebalance;

            if (customerAccount != null && !(IsDeparting || IsUnderRebalance))
                ActiveOrderCount = customerAccount.OpenOrdersForAccount.Count;

            // Notifications
            Notification = account.Notifications.DisplayMessages;
            NotificationType = account.Notifications.DisplayNotificationType;
        }

        public string AccountNumberWithName { get; internal set; }
        public string AccountName { get; internal set; }
        public DateTime ManagementStartDate { get; internal set; }
        public string PrimaryAccountHolderName { get; internal set; }
        public string SecondaryAccountHolderName { get; internal set; }
        public string StreetAddressLine { get; internal set; }
        public string CityAddressLine { get; internal set; }
        public string CountryAddressLine { get; internal set; }
        public string ModelName { get; internal set; }
        public string AccountNumber { get; internal set; }
        public AccountStati Status { get; internal set; }
        public bool StatusIsOpen { get; internal set; }
        public string TotalPositions { get; internal set; }
        public string TotalCash { get; internal set; }
        public decimal TotalCashQuantity { get; internal set; }
        public string TotalAll { get; internal set; }
        public DateTime LastRebalanceDate { get; internal set; }
        public DateTime CurrentRebalanceDate { get; internal set; }
        public string Remisier { get; internal set; }
        public string RemisierEmployee { get; internal set; }
        public bool IsCrumbleAccount { get; internal set; }
        public decimal FutureWithdrawalAmount { get; internal set; }
        public string DisplayFutureWithdrawalAmount { get; internal set; }
        public bool IsTradeable { get; internal set; }
        public string VerpandSoort { get; internal set; }
        public string Pandhouder { get; internal set; }
        public string Notification { get; internal set; }
        public NotificationTypes NotificationType { get; internal set; }
        public bool IsDeparting { get; internal set; }
        public bool IsUnderRebalance { get; internal set; }
        public int ActiveOrderCount { get; internal set; }
        public string DepositFeeInfo { get; internal set; }
    }
}
