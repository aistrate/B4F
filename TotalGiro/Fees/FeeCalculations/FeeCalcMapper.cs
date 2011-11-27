using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Fees.FeeCalculations
{ 
    public class FeeCalcMapper
	{


		/// <summary>
		/// Retrieves a list of all <b>FeeCalc</b> objects in the system.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>FeeCalc</b> objects in the system.</returns>
        public static IList GetFeeCalculations(IDalSession session)
		{
            List<ICriterion> expressions = new List<ICriterion>();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", company.Key));

			return session.GetList(typeof(FeeCalc), expressions);
		}

        /// <summary>
        /// Retrieves a list of the <b>FeeCalc</b> objects in the system with name calcname.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <returns>A list of all <b>FeeCalc</b> objects in the system.</returns>
        public static IList GetFeeCalculations(IDalSession session, string calcname)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            if (calcname != null && calcname.Length > 0)
                expressions.Add(Expression.Like("Name", calcname, MatchMode.Anywhere));

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (!company.IsStichting)
                expressions.Add(Expression.Eq("AssetManager.Key", company.Key));

            return session.GetList(typeof(FeeCalc), expressions);
        }

        /// <summary>
        /// Retrieves a <b>FeeCalc</b> object by ID.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="ID">The ID of the <b>FeeCalc</b> object to be retrieved.</param>
        /// <returns>The <b>FeeCalc</b> object with the given ID, retrieved from the database.</returns>
		public static IFeeCalc GetFeeCalculation(IDalSession session, int ID)
		{
			return (IFeeCalc)session.GetObjectInstance(typeof(FeeCalc), ID);
		}

        /// <summary>
        /// Retrieves a <b>FeeCalcVersion</b> object by ID.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="ID">The ID of the <b>FeeCalcVersion</b> object to be retrieved.</param>
        /// <returns>The <b>FeeCalcVersion</b> object with the given ID, retrieved from the database.</returns>
		public static IFeeCalcVersion GetFeeCalcVersion(IDalSession session, int ID)
		{
            IList<IFeeCalcVersion> list = session.GetTypedList<FeeCalcVersion, IFeeCalcVersion>(ID);
            if (list != null)
                return list.FirstOrDefault();
            else
                return null;
		}

		/// <summary>
        /// Retrieves a <b>FeeCalcLine</b> object by ID.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="ID">The ID of the <b>FeeCalcLine</b> object to be retrieved.</param>
        /// <returns>The <b>FeeCalcLine</b> object with the given ID, retrieved from the database.</returns>
        public static FeeCalcLine GetCalculationLine(IDalSession session, int ID)
		{
			return (FeeCalcLine)session.GetObjectInstance(typeof(FeeCalcLine), ID);
		}

		/// <summary>
        /// Inserts a <b>FeeCalc</b> object into the database.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>FeeCalc</b> object to insert into the database.</param>
        public static bool Insert(IDalSession session, IFeeCalc obj)
		{
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.AssetManager = (IAssetManager) emp.Employer;
			return session.InsertOrUpdate(obj);
		}

		/// <summary>
        /// Updates a <b>FeeCalc</b> object to the database.
		/// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>FeeCalc</b> object to update into the database.</param>
        public static bool Update(IDalSession session, IFeeCalc obj)
		{
            InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            obj.AssetManager = (IAssetManager)emp.Employer;
            return session.InsertOrUpdate(obj);
		}

        /// <summary>
        /// Deletes a <b>FeeCalc</b> object from the database.
        /// </summary>
        /// <param name="session">An instance of the Data Access Library (see class <see cref="B4F.TotalGiro.DAL.NHSession">NHSession</see>).</param>
        /// <param name="obj">The <b>FeeCalc</b> object to delete from the database.</param>
        public static bool Delete(IDalSession session, IFeeCalc obj)
		{
            return session.Delete(obj);
		}
	}
}
