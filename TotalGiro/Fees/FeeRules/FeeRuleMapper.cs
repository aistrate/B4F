using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Fees.FeeRules
{
    /// <summary>
    /// Class used to instantiate and persist <b>FeeRule</b> objects.
    /// Data is retrieved from the database using an instance of the Data Access Library 
    /// (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).
    /// </summary>
    public static class FeeRuleMapper
    {
        /// <summary>
        /// Retrieve a <b>FeeRule</b> object.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="feeRuleID">The unique id.</param>
        /// <returns>A <b>FeeRule</b> object.</returns>
        public static IFeeRule GetFeeRule(IDalSession session, int feeRuleID)
        {
            return (IFeeRule)session.GetObjectInstance(typeof(FeeRule), feeRuleID);
        }
        
        /// <summary>
        /// Retrieves a list of all <b>FeeRule</b> objects in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>FeeRule</b> objects in the system.</returns>
        public static IList GetFeeRules(IDalSession session)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", company.Key));
            return session.GetList(typeof(FeeRule), expressions);
        }

        /// <summary>
        /// Retrieves a list of all <b>FeeRule</b> objects in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>CommRule</b> objects in the system.</returns>
        public static IList<IFeeRule> GetFeeRules(IDalSession session,
            int feeCalcId, int modelId, int accountId, int startPeriod, int endPeriod,
            bool isDefault, bool hasEmployerRelation, bool executionOnly, bool sendByPost)
        {
            Hashtable parameters = new Hashtable();

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                parameters.Add("companyId", company.Key);
            if (feeCalcId != 0 && (int)feeCalcId != int.MinValue)
                parameters.Add("feeCalcId", feeCalcId);
            if (accountId != 0 && accountId != int.MinValue)
                parameters.Add("accountId", accountId);
            if (modelId != 0 && modelId != int.MinValue)
                parameters.Add("modelId", modelId);
            if (startPeriod != 0)
                parameters.Add("startPeriod", startPeriod);
            if (endPeriod != 0)
                parameters.Add("endPeriod", endPeriod);
            if (isDefault)
                parameters.Add("isDefault", isDefault);
            if (hasEmployerRelation)
                parameters.Add("hasEmployerRelation", hasEmployerRelation);
            if (executionOnly)
                parameters.Add("executionOnly", executionOnly);
            if (sendByPost)
                parameters.Add("sendByPost", sendByPost);

            return session.GetTypedListByNamedQuery<IFeeRule>(
                "B4F.TotalGiro.Fees.FeeRules.GetFeeRules",
                parameters);
        }


        public static IList GetModelFeeRules(IDalSession session, int modelID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("modelID", modelID);
            return session.GetListByNamedQuery(
                "B4F.TotalGiro.Fees.FeeRules.ModelFeeRules",
                parameters);
        }

        public static IList GetAccountFeeRules(IDalSession session, int accountID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("accountID", accountID);
            return session.GetListByNamedQuery(
                "B4F.TotalGiro.Fees.FeeRules.AccountFeeRules",
                parameters);
        }

        /// <summary>
        /// Retrieves a list of all <b>FeeType</b> objects in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>FeeType</b> objects in the system.</returns>
        public static IList GetFeeTypes(IDalSession session)
        {
            return session.GetList(typeof(FeeType));
        }

        /// <summary>
        /// Retrieves a <b>FeeType</b> object in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A <b>FeeType</b> object in the system.</returns>
        public static FeeType GetFeeType(IDalSession session, FeeTypes key)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("key", (int)key));
            IList result = session.GetList(typeof(FeeType), expressions);
            if ((result != null) && (result.Count > 0))
                return (FeeType)result[0];
            else
                return null;
        }

        /// <summary>
        /// Inserts a <b>FeeRule</b> object into the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>FeeRule</b> object to insert into the database.</param>
        public static bool Insert(IDalSession session, IFeeRule obj)
        {
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.AssetManager = (IAssetManager)emp.Employer;
            return session.InsertOrUpdate(obj);
        }

        /// <summary>
        /// Updates a <b>FeeRule</b> object to the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>FeeRule</b> object to update into the database.</param>
        public static bool Update(IDalSession session, IFeeRule obj)
        {
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.AssetManager = (IAssetManager)emp.Employer;
            return session.InsertOrUpdate(obj);
        }

    }
}
