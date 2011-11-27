using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class is used to instantiate Lifecycle objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public abstract class LifecycleMapper
    {
        /// <summary>
        /// Get lifecycle by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="assetManagerId">Identifier</param>
        /// <returns>List of Lifecycle objects</returns>
        public static IList<ILifecycle> GetLifecycles(IDalSession session, int assetManagerId)
        {
            return GetLifecycles(session, assetManagerId, ActivityReturnFilter.Active);
        }

        
        /// <summary>
        /// Get lifecycle by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="assetManagerId">Identifier</param>
        /// <param name="activityFilter">Identifier</param>
        /// <returns>List of Lifecycle objects</returns>
        public static IList<ILifecycle> GetLifecycles(IDalSession session, int assetManagerId, ActivityReturnFilter activityFilter)
		{
            Hashtable parameters = new Hashtable();
            if (assetManagerId != 0)
                parameters.Add("managementCompanyID", assetManagerId);

            if (activityFilter != ActivityReturnFilter.All)
                parameters.Add("isActive", (activityFilter == ActivityReturnFilter.Active ? true : false));

            return session.GetTypedListByNamedQuery<ILifecycle>(
                "B4F.TotalGiro.Instruments.Lifecycles",
                parameters);
        }

        /// <summary>
        /// Get lifecycle by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="id">Identifier</param>
        /// <returns>Lifecycle object</returns>
        public static ILifecycle GetLifecycle(IDalSession session, int id)
		{
            ILifecycle lifecycle = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", id));
            List<ILifecycle> list = session.GetTypedList<Lifecycle, ILifecycle>(expressions);
            if (list != null && list.Count > 0)
                lifecycle = list[0];
            return lifecycle;
		}

        public static bool Update(IDalSession session, ILifecycle obj)
        {
            return session.InsertOrUpdate(obj);
        }

        public static bool Update(IDalSession session, IList list)
        {
            return session.InsertOrUpdate(list);
        }
    }
}
