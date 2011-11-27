using System;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.ApplicationLayer.Orders.AssetManager
{
    public static class ApproveOrdersAdapter
    {
        public static DataSet GetUnapprovedGroupedOrders(string accountNumber, string accountName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return OrderMapper.GetGroupedOrders(session, accountNumber, accountName, false)
                                  .Cast<object[]>()
                                  .Select(a => new
                                  {
                                      AccountId = a[0],
                                      AccountName = a[1],
                                      AccountNumber = a[2],
                                      Commission = a[3] ?? 0m,
                                      NrOfOrders = a[4]
                                  })
                                  .ToDataSet();
            }
        }
        
        public static void ApproveOrdersPerAccount(int[] accountKeys)
        {
            if (accountKeys == null || accountKeys.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();
            
            IDalSession session = NHSessionFactory.CreateSession();
            OrderMapper.ApproveOrdersPerAccount(session, accountKeys);
            session.Close();
        }
    }
}
