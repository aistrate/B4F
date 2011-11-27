using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class is used to instantiate Category objects of securities
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class SecCategoryMapper
    {
        /// <summary>
        /// Get Category by ID
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="SecCategoryID">Identifier</param>
        /// <returns>Category</returns>
        public static SecCategory GetSecCategory(IDalSession session, SecCategories SecCategoryID)
        {
            return (SecCategory)session.GetObjectInstance(typeof(SecCategory), (int)SecCategoryID);
        }

        /// <summary>
        /// Get all system categories
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <returns>Collection of categories</returns>
        public static IList<ISecCategory> GetSecCategories(IDalSession session)
        {
            return GetSecCategories(session, SecCategoryFilterOptions.All, false);
        }

        /// <summary>
        /// Get all system categories
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="secCategoryFilter">Type of sec category to return</param>
        /// <returns>Collection of categories</returns>
        public static IList<ISecCategory> GetSecCategories(IDalSession session, SecCategoryFilterOptions secCategoryFilter)
        {
            return GetSecCategories(session, secCategoryFilter, false);
        }


        /// <summary>
        /// Get all system categories
        /// </summary>
        /// <param name="session">Data access object</param>
        /// <param name="secCategoryFilter">Type of sec category to return</param>
        /// <param name="includeNotSupported">include Not Supported sec categories</param>
        /// <returns>Collection of categories</returns>
        public static IList<ISecCategory> GetSecCategories(IDalSession session, SecCategoryFilterOptions secCategoryFilter, bool? includeNotSupported)
        {
            Hashtable parameters = new Hashtable();

            if (secCategoryFilter != SecCategoryFilterOptions.All)
                parameters.Add("secCategoryFilter", (int)secCategoryFilter);
            if (includeNotSupported.HasValue && !includeNotSupported.Value)
                parameters.Add("isSupported", true);
            IList<ISecCategory> list = session.GetTypedListByNamedQuery<ISecCategory>(
                "B4F.TotalGiro.Instruments.SecCategories",
                parameters);
            return list.ToList();


            // 
        }
    }
}
