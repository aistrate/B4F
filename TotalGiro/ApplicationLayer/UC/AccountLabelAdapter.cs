using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.Portfolio;
using B4F.TotalGiro.Accounts.RemisierHistory;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class AccountLabelAdapter
    {
        public static AccountDetailsView GetAccountDetails(int id, bool showFeeDetails)
        {
            AccountDetailsView view = null;
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IAccountTypeCustomer account = (IAccountTypeCustomer)AccountMapper.GetAccount(session, id);
                if (account != null)
                {
                    view = new AccountDetailsView();
                    view.AccountNumber = account.Number;
                    view.AccountName = account.ShortName;
                    view.Notification = account.Notifications.DisplayMessages;
                    view.NotificationType = account.Notifications.DisplayNotificationType;
                    view.Status = account.Status;
                    view.IsDeparting = account.IsDeparting;
                    view.IsUnderRebalance = account.IsUnderRebalance;

                    if (showFeeDetails && account.AccountType == AccountTypes.Customer)
                    {
                        ICustomerAccount customer = (ICustomerAccount)account;
                        view.DepositFeeInfo = customer.CurrentRemisierDetails.GetS(x => x.DepositFeeInfo);
                    }
                }
            }
            return view;
        }


    }
}
