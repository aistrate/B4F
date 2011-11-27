using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;
using NHibernate;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// This class is used to instantiate Management companies objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class ManagementCompanyMapper
    {
        /// <summary>
        /// Get Asset Manager by ID
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="id">Unique number</param>
        /// <returns>Asset Manager object</returns>
        public static IAssetManager GetAssetManager(IDalSession session, int id)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList result = session.GetList(typeof(AssetManager), expressions);
            if ((result != null) && (result.Count > 0))
                return (IAssetManager)result[0];
            else
                return null;
        }

        public static List<IAssetManager> GetAssetManagers(IDalSession session)
        {
            return session.GetTypedList<AssetManager, IAssetManager>(null, 
                                            new List<Order>() { Order.Asc("CompanyName") });
        }

        public static IManagementCompany GetManagementCompany(IDalSession session, int id)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            IList result = session.GetList(typeof(ManagementCompany), expressions);
            if ((result != null) && (result.Count > 0))
                return (IManagementCompany)result[0];
            else
                return null;
        }

        public static IList GetManagementCompanies(IDalSession session)
        {
            return session.GetList(typeof(ManagementCompany));
        }

        public static IEffectenGiro GetEffectenGiroCompany(IDalSession session)
        {
            IList list = session.GetList(typeof(EffectenGiro));
            if (list.Count == 1)
                return (IEffectenGiro)list[0];
            else
                throw new ApplicationException("There should be exactly one EffectenGiro company in the system.");
        }

        #region CRUD

        /// <summary>
        /// Updates a ManagementCompany
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="list">The ManagementCompany</param>
        public static void Update(IDalSession session, IManagementCompany obj)
        {
            session.Update(obj);
        }


        #endregion
    }
}
