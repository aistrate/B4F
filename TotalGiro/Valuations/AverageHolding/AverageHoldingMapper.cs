using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Valuations.AverageHoldings
{
    public class AverageHoldingMapper
    {
        public static IList<IAverageHolding> GetAverageHoldings(IDalSession session, int[] averageHoldingIds)
        {
            Hashtable parameterLists = new Hashtable(1);
            parameterLists.Add("averageHoldingIds", averageHoldingIds);

            return session.GetTypedListByNamedQuery<IAverageHolding>(
                "B4F.TotalGiro.Valuations.AverageHoldings.GetAverageHoldings", 
                new Hashtable(), parameterLists);
        }
        
        public static IList GetAverageHoldings(IDalSession session, IAccountTypeInternal account, DateTime startDate, DateTime endDate, ValuationTypesFilterOptions filterOption)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("account", account);
            parameters.Add("startDate", startDate);
            parameters.Add("endDate", endDate);
            string instrumentFilter = "";

            switch (filterOption)
            {
                case ValuationTypesFilterOptions.Security:
                    instrumentFilter = "and I.Key in (select S.Key from TradeableInstrument S) ";
                    break;
                case ValuationTypesFilterOptions.Monetary:
                    instrumentFilter = "and I.Key in (select C.Key from Currency C) ";
                    break;
            }

            string hql = string.Format(@"from AverageHolding A 
                left join fetch A.Instrument I
                where A.Account = :account 
                and (A.BeginDate between :startDate 
                and :endDate ) {0}
                order by I.Key, A.BeginDate", instrumentFilter);
            return session.GetListByHQL(hql, parameters);
        }

        public static IList GetAverageHoldingsForManagementFee(IDalSession session, IAccountTypeInternal account, DateTime startDate, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(2);
            parameters.Add("account", account);
            parameters.Add("endDate", endDate);

            string hql = @"from AverageHolding A 
                left join fetch A.Instrument I
                left join fetch A.Transaction T
                where A.Account = :account  
                and IsNull(A.SkipFees, 0) = 0
                and (A.BeginDate <= :endDate)
                and IsNull(A.IsInValid, 0) = 0
                and (T is null or T.StornoTransaction is not null)
                and A.Key not in (
                    select A.PreviousHolding.Key
                    from AverageHolding A
                    where A.Account = :account  
                    and IsNull(A.SkipFees, 0) = 0
                    and (A.BeginDate <= :endDate)
                    and A.PreviousHolding.Key is not null)
                order by I.Key, A.BeginDate";
            return session.GetListByHQL(hql, parameters);
        }
    }
}
