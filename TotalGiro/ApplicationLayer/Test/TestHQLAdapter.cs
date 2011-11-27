using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.ApplicationLayer.Test
{
    public static class TestHQLAdapter
    {
        public static DataSet GetData(string hql, string fields, bool fromBusinessObjects, out int objectCount)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            IList list = session.GetListByHQL(hql);

            if (fromBusinessObjects)
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                    list,
                    fields);
            else
                ds = DataSetBuilder.CreateDataSetFromHibernateList(
                    list,
                    fields);
            
            session.Close();

            objectCount = ds.Tables[0].Rows.Count;
            return ds;
        }

    }
}
