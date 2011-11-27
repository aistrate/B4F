using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using System.Security.Authentication;

namespace B4F.TotalGiro.Fees.CommCalculations
{
    /// <summary>
    /// Class used to instantiate and persist <b>CommCalc</b> and <b>CommCalcLine</b> objects.
    /// Data is retrieved from the database using an instance of the Data Access Library 
    /// (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).
    /// </summary>
    public class CommCalcMapper
	{

        /// <summary>
		/// Retrieves a list of all <b>CommCalc</b> objects in the system.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>CommCalc</b> objects in the system.</returns>
        public static IList<CommCalc> GetCommissionCalculations(IDalSession session)
		{
            List<ICriterion> expressions = new List<ICriterion>();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", company.Key));

			return session.GetTypedList<CommCalc>(expressions);
		}

        /// <summary>
        /// Retrieves a list of the <b>CommCalc</b> objects in the system with name calcname.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>CommCalc</b> objects in the system.</returns>
        public static IList<CommCalc> GetCommissionCalculations(IDalSession session, string calcname)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (calcname != null && calcname.Length > 0)
                expressions.Add(Expression.Like("Name", calcname, MatchMode.Anywhere));

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", company.Key));

            return session.GetTypedList<CommCalc>(expressions);
        }

        /// <summary>
        /// Retrieves a <b>CommCalc</b> object by ID.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="ID">The ID of the <b>CommCalc</b> object to be retrieved.</param>
        /// <returns>The <b>CommCalc</b> object with the given ID, retrieved from the database.</returns>
		public static CommCalc GetCommissionCalculation(IDalSession session, int ID)
		{
			return (CommCalc)session.GetObjectInstance(typeof(CommCalc), ID);
		}

		/// <summary>
        /// Retrieves a <b>CommCalcLine</b> object by ID.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="ID">The ID of the <b>CommCalcLine</b> object to be retrieved.</param>
        /// <returns>The <b>CommCalcLine</b> object with the given ID, retrieved from the database.</returns>
        public static CommCalcLine GetCalculationLine(IDalSession session, int ID)
		{
			return (CommCalcLine)session.GetObjectInstance(typeof(CommCalcLine), ID);
		}

		/// <summary>
        /// Inserts a <b>CommCalc</b> object into the database.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>CommCalc</b> object to insert into the database.</param>
        public static void Insert(IDalSession session, ICommCalc obj)
		{
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.AssetManager = (IAssetManager) emp.Employer;
			session.InsertOrUpdate(obj);
		}

		/// <summary>
        /// Updates a <b>CommCalc</b> object to the database.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>CommCalc</b> object to update into the database.</param>
        public static void Update(IDalSession session, ICommCalc obj)
		{
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.AssetManager = (IAssetManager)emp.Employer;
            session.InsertOrUpdate(obj);
		}

        /// <summary>
        /// Deletes a <b>CommCalc</b> object from the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>CommCalc</b> object to delete from the database.</param>
		public static void Delete(IDalSession session, ICommCalc obj)
		{
			session.Delete(obj);
		}
	}
}
