using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.BackOffice.Orders
{
    public static class PredefinedBeneficiaryMapper
    {
        public static PredefinedBeneficiary GetPredefinedBeneficiary(IDalSession session, int PredefinedBeneficiaryID)
        {
            return (PredefinedBeneficiary)session.GetObjectInstance(typeof(PredefinedBeneficiary), PredefinedBeneficiaryID);
        }

        public static IList GetPredefinedBeneficiaries(IDalSession session)
        {
            return session.GetList(typeof(PredefinedBeneficiary));
        }

        public static bool Update(IDalSession session, PredefinedBeneficiary obj)
        {
            bool blnSuccess = session.InsertOrUpdate(obj);
            return blnSuccess;
        }

    }
}
