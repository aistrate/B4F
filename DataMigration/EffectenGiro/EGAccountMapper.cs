using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.DataMigration.EffectenGiro
{
    public static class EGAccountMapper
    {
        public static IList GetUnMappedAccounts(IDalSession session)
        {
            Hashtable parameters = new Hashtable();

            string hql = @"from EGAccount ega 
                join fetch ega.AccountRequest T
                where (ega.TGAccount is null)
                and (T.FormOntvangen = 1)";

            IList result = session.GetListByHQL(hql,parameters);
            return result;
        }

        public static IList GetEGAccounts(IDalSession session)
        {
            return session.GetList(typeof(EGAccount));
        }
    }
}
