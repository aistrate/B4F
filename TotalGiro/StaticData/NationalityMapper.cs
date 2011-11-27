using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// This class is used to instantiate Nationality objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class NationalityMapper
    {
        /// <summary>
        /// Gets Nationality by unique identifier
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="NationalityID">Unique identifier</param>
        /// <returns>Nationality object</returns>
        public static INationality GetNationality(IDalSession session, int NationalityID)
        {
            return (INationality)session.GetObjectInstance(typeof(Nationality), NationalityID);
        }

        /// <summary>
        /// Get collection of all system nationalities
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <returns></returns>
        public static IList GetNationalities(IDalSession session)
        {
            List<Order> orderings = new List<Order>();
            orderings.Add(Order.Asc("Description"));
            return session.GetList(typeof(Nationality));
        }

    }
}
