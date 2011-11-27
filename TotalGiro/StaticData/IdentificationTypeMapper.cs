using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// This class is used to instantiate Identification objects
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class IdentificationTypeMapper
    {
        /// <summary>
        /// Get Identification object by ID
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="IdentificationTypeID">Unique identifier</param>
        /// <returns>Identification object</returns>
        public static IdentificationType GetIdentificationType(IDalSession session, int IdentificationTypeID)
        {
            return (IdentificationType)session.GetObjectInstance(typeof(IdentificationType), IdentificationTypeID);
        }

        /// <summary>
        /// Get collection of all system identification objects
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <returns>Collection of identification objects</returns>
        public static IList GetIdentificationType(IDalSession session)
        {
            return session.GetList(typeof(IdentificationType));
        }
    }
}
