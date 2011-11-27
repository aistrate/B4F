using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Instruments
{
    public static class ModelPerformanceMapper
    {
        public static IModelPerformance GetModelBenchmarkPerformance(IDalSession session, int modelperformanceID)
        {
            return (IModelPerformance)session.GetObjectInstance(typeof(ModelPerformance), modelperformanceID);
        }

        public static IList<IModelPerformance> GetModelBenchmarkPerformances(IDalSession session, int modelID, int quarter, int yyyy)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            List<Order> orderings = new List<Order>();
            expressions.Add(Expression.Eq("ModelPortfolio.Key", modelID));

            if (quarter > 0)
                expressions.Add(Expression.Eq("Quarter", quarter));
            if (yyyy > 0)
                expressions.Add(Expression.Eq("PerformanceYear", yyyy));

            return session.GetTypedList<ModelPerformance, IModelPerformance>(expressions);
        }

        public static bool Insert(IDalSession session, IModelPerformance obj)
        {
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.EmployeeID = emp.Key;   //employer id;
            return session.InsertOrUpdate(obj);
        }

        public static bool Delete(IDalSession session, IModelPerformance obj)
        {
            return session.Delete(obj);
        }

    }
}



//X-Working
//public static IList GetModelBenchmarkPerformances(IDalSession session, int modelID)
//{
//    Hashtable parameters = new Hashtable(1);
//    parameters.Add("modelID", modelID);
//    return session.GetListByNamedQuery(
//        "B4F.TotalGiro.Instruments.ModelBechmarkPerformances",
//        parameters);
//}