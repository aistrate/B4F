using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using System.Collections;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public class DividWepRecordMapper
    {
        public static IList<IDividWepRecord> GetRecordsforDividWepFile(IDalSession session, int fileID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("fileID", fileID);

            string hql = string.Format(
                @"from DividWepRecord D
                where D.ParentFile.Key = :fileID");
            return session.GetTypedListByHQL<IDividWepRecord>(hql, parameters);
        }



    }
}
