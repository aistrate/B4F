using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;


namespace B4F.TotalGiro.Fees.CommRules
{
    /// <summary>
    /// Class used to instantiate and persist <b>CommRule</b> objects.
    /// Data is retrieved from the database using an instance of the Data Access Library 
    /// (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).
    /// </summary>
    public static class CommRuleMapper
	{
        /// <summary>
        /// Retrieves a list of all <b>CommRule</b> objects in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="company">The company that owns these rules.</param>
        /// <returns>A list of all <b>CommRule</b> objects in the system.</returns>
        public static IList<ICommRule> GetCommissionRules(IDalSession session)
		{
            List<ICriterion> expressions = new List<ICriterion>();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", company.Key));
            return session.GetTypedList<CommRule, ICommRule>(expressions);
		}

        public static IList GetAccountCommissionRules(IDalSession session, int accountID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("accountID", accountID);
            return session.GetListByNamedQuery(
                "B4F.TotalGiro.Fees.CommRules.AccountCommissionRules",
                parameters);
        }

        public static IList GetModelCommissionRules(IDalSession session, int modelID)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("modelID", modelID);
            return session.GetListByNamedQuery(
                "B4F.TotalGiro.Fees.CommRules.ModelCommissionRules",
                parameters);
        }

        /// <summary>
        /// Retrieves a <b>CommRule</b> object by ID.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="ID">The ID of the <b>CommRule</b> object to be retrieved.</param>
        /// <returns>The <b>CommRule</b> object with the given ID, retrieved from the database.</returns>
        public static ICommRule GetCommissionRule(IDalSession session, int ID)
		{
			return (ICommRule)session.GetObjectInstance(typeof(CommRule), ID);
		}

        /// <summary>
        /// Retrieves a list of all <b>CommRule</b> objects in the system.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="account">The number of the account property.</param>
        /// <returns>A list of all <b>CommRule</b> objects in the system.</returns>
        public static IList<ICommRule> GetCommissionRules(IDalSession session,
            string commRuleName, int commRuleTypeId, int modelId, int accountId, int instrumentId, int buySell,
            SecCategories secCategoryId, int exchangeId, DateTime startDate, DateTime endDate,
            int commCalcId, int orderActionType, int additionalCalcId, Boolean applytoAllAccounts)
        {
            Hashtable parameters = new Hashtable();

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                parameters.Add("companyId", company.Key);
            if (!string.IsNullOrEmpty(commRuleName))
                parameters.Add("comruleName", Util.PrepareNamedParameterWithWildcard(commRuleName, MatchModes.Anywhere));
            if (commRuleTypeId != 0 && commRuleTypeId != int.MinValue)
                parameters.Add("commRuleTypeId", commRuleTypeId);
            if (accountId != 0 && accountId != int.MinValue)
                parameters.Add("accountId", accountId);
            if (modelId != 0 && modelId != int.MinValue)
                parameters.Add("modelId", modelId);
            if (secCategoryId != 0 && (int)secCategoryId != int.MinValue)
                parameters.Add("secCategoryId", secCategoryId);
            if (instrumentId != 0 && instrumentId != int.MinValue)
                parameters.Add("instrumentId", instrumentId);
            if (buySell != 0 && buySell != int.MinValue)
                parameters.Add("buySell", buySell);
            if (exchangeId != 0 && exchangeId != int.MinValue)
                parameters.Add("exchangeId", exchangeId);
            if (commCalcId != 0 && commCalcId != int.MinValue)
                parameters.Add("commCalcId", commCalcId);
            if (additionalCalcId != 0 && additionalCalcId != int.MinValue)
                parameters.Add("additionalCalcId", additionalCalcId);
            if (Util.IsNotNullDate(startDate))
                parameters.Add("startDate", startDate);
            if (Util.IsNotNullDate(endDate))
                parameters.Add("endDate", endDate);
            if (orderActionType != 0 && orderActionType != int.MinValue)
                parameters.Add("orderActionTypeId", orderActionType);
            if (applytoAllAccounts)
                parameters.Add("applytoAllAccounts", true);

            return session.GetTypedListByNamedQuery<ICommRule>(
                "B4F.TotalGiro.Fees.CommRules.GetCommissionRules",
                parameters);
        }

        /// <summary>
        /// Retrieves a list of all <b>CommRule</b> objects of a certain asset manager.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="assetmanager">Asset Manager we want the set of rules for.</param>
        /// <returns>A list of all <b>CommRule</b> objects in the system.</returns>
        public static IList GetCommissionRulesByAssetManager(IDalSession session, IAssetManager assetmanager)
        {
            List<ICriterion> expressions = new List<ICriterion>();

            expressions.Add(Expression.Eq("AssetManager.Key", assetmanager.Key));

            return session.GetList(typeof(CommRule));
        }

        /// <summary>
        /// Inserts a <b>CommRule</b> object into the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>CommRule</b> object to insert into the database.</param>
        public static void InsertOrUpdate(IDalSession session, ICommRule obj)
		{
			session.InsertOrUpdate(obj);
		}

        /// <summary>
        /// Updates a <b>CommRule</b> object to the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>CommRule</b> object to update into the database.</param>
        public static void Update(IDalSession session, ICommRule obj)
		{
			session.InsertOrUpdate(obj);
		}

        /// <summary>
        /// Deletes a <b>CommRule</b> object from the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>CommRule</b> object to delete from the database.</param>
        public static void Delete(IDalSession session, ICommRule obj)
		{
			session.Delete(obj);
		}
	}
}
