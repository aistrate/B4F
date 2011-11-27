using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// This class is used to instantiate Country objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class CountryMapper
    {
        /// <summary>
        /// Get country object
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="countryID">Unique identifier</param>
        /// <returns>Country object</returns>
        public static ICountry GetCountry(IDalSession session, int countryID)
        {
            return (ICountry)session.GetObjectInstance(typeof(Country), countryID);
        }

        /// <summary>
        /// Get all countries in system
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <returns>collection of countries</returns>
        public static IList GetCountries(IDalSession session)
        {
            return session.GetList(typeof(Country));
        }

        #region CRUD

        public static void Insert(IDalSession session, Country obj)
        {
            session.Insert(obj);
        }

        public static void Insert(IDalSession session, IList list)
        {
            session.Insert(list);
        }

        public static void Update(IDalSession session, Country obj)
        {
            session.Update(obj);
        }

        public static void Update(IDalSession session, IList list)
        {
            session.Update(list);
        }

        internal static void Delete(IDalSession session, Country obj)
        {
            session.Delete(obj);
        }

        internal static void Delete(IDalSession session, IList list)
        {
            session.Delete(list);
        }

        #endregion
    }
}
