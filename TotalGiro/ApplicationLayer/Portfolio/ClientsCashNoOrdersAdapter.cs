using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class ClientsCashNoOrdersAdapter
    {

        public static DataSet GetAccountsWithCashNoOrders()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<ICustomerAccount> accounts = AccountMapper.GetAccountsWithCashNoOrders(session);

                return accounts.Select( a => new 
                {
                    a.Key,
                    a.Number,
                    a.ShortName,
                    a.TotalCash,
                    TotalCashDisplay = a.TotalCash.DisplayString
                    ,a.TotalCashFund,
                    TotalCashFundDisplay = a.TotalCashFund.DisplayString
                }).ToDataSet();
            }
        }
    }
}
