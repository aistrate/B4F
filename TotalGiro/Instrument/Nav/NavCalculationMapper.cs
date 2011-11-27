using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;

namespace B4F.TotalGiro.Instruments.Nav
{
    public static class NavCalculationMapper
    {
        public static INavCalculation GetNavCalculation(IDalSession session, int navCalculationID)
        {
            return (INavCalculation)session.GetObjectInstance(typeof(NavCalculation), navCalculationID);
        }



        public static void Insert(IDalSession session, INavCalculation obj)
        {
            session.Insert(obj); ;
        }

        public static void Update(IDalSession session, INavCalculation obj)
        {
            session.InsertOrUpdate(obj); ;
        }

        public static IList GetNavCalculations(IDalSession session, IVirtualFund fund, DateTime navDateFrom,
                                              DateTime navDateTo)
        {
            Hashtable parameters = new Hashtable();
            string hql = string.Format("FROM NavCalculation S WHERE (1 = 1");

            if (navDateFrom != DateTime.MinValue)
            {
                parameters.Add("NavDateFrom", navDateFrom);
                hql += " AND S.ValuationDate >= :NavDateFrom";
            }

            if (navDateTo != DateTime.MinValue)
            {
                parameters.Add("NavDateTo", navDateTo);
                hql += " AND S.ValuationDate <= :NavDateTo";
            }

            hql += ")";
           
            return session.GetListByHQL(hql, parameters);
        }

        public static INavCalculation GetLastNavCalculation(IDalSession session, IVirtualFund fund)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("fund", fund);

            IList navCalculations = session.GetListByNamedQuery("B4F.TotalGiro.Instruments.Nav.GetLastNavCalculation", parameters);
            return (navCalculations.Count > 0 ? (INavCalculation)navCalculations[0] : null);
        }
    }
}
