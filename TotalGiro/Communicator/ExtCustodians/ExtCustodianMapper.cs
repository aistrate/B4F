using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Collections;
using NHibernate.Criterion;
using B4F.TotalGiro.Communicator.ExtCustodians.Positions;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Communicator.ExtCustodians
{

    /// <summary>
    /// This class is used to instantiate the ExtCustodian objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class ExtCustodianMapper
    {
        /// <summary>
        /// Gets the ExtCustodian
        /// </summary>
        /// <param name="session">Data session object</param>
        /// <param name="key">The unique identifier of the extCustodian</param>
        /// <returns>A list of type <seealso cref="ExtCustodian">ExtCustodian</seealso></returns>
        public static ExtCustodian GetExtCustodian(IDalSession session, int key)
        {
            ExtCustodian custodian = null;
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Eq("Key", key));
            IList list = session.GetList(typeof(ExtCustodian), expressions);
            if (list != null && list.Count == 1)
            {
                custodian = (ExtCustodian)list[0];
            }
            return custodian;
        }

        
        /// <summary>
        /// Gets the ExtCustodians
        /// </summary>
        /// <param name="session">Data session object</param>
        /// <returns>A list of type <seealso cref="ExtCustodian">ExtCustodian</seealso></returns>
        public static IList GetExtCustodians(IDalSession session)
        {
            return session.GetList(typeof(ExtCustodian));
        }

        /// <summary>
        /// Gets the Position for the ExtCustodian and instrument for the specified date
        /// </summary>
        /// <param name="session">Data session object</param>
        /// <param name="custodian">The specified ExtCustodians</param>
        /// <param name="instrument">The specified Instrument</param>
        /// <param name="balanceDate">The specified date</param>
        /// <returns>An ExtCustodian Position</returns>
        public static ExtPosition GetExtCustodianPosition(IDalSession session, ExtCustodian custodian, IInstrument instrument, DateTime balanceDate)
        {
            ExtPosition pos = null;
            List<NHibernate.Criterion.ICriterion> expressions = new List<NHibernate.Criterion.ICriterion>();
            expressions.Add(NHibernate.Criterion.Expression.Eq("Custodian.Key", custodian.Key));
            expressions.Add(NHibernate.Criterion.Expression.Eq("Size.Underlying.Key", instrument.Key));
            expressions.Add(NHibernate.Criterion.Expression.Eq("BalanceDate", balanceDate));
            IList list = session.GetList(typeof(ExtPosition), expressions);
            if (list != null && list.Count == 1)
                pos = (ExtPosition)list[0];
            return pos;
        }



        /// <summary>
        /// Gets the Positions for the ExtCustodian for the specified date
        /// </summary>
        /// <param name="session">Data session object</param>
        /// <param name="custodian">The specified ExtCustodians</param>
        /// <param name="balanceDate">The specified date</param>
        /// <returns>A list ExtCustodian Positions</returns>
        public static IList GetExtCustodianPositions(IDalSession session, ExtCustodian custodian, DateTime balanceDate)
        {
            List<NHibernate.Criterion.ICriterion> expressions = new List<NHibernate.Criterion.ICriterion>();
            expressions.Add(NHibernate.Criterion.Expression.Eq("Custodian.Key", custodian.Key));
            expressions.Add(NHibernate.Criterion.Expression.Eq("BalanceDate", balanceDate));
            return session.GetList(typeof(ExtPosition), expressions);
        }

        /// <summary>
        /// Creates/Updates a new object in the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Object of type ExtPosition</param>
        public static void InsertOrUpdate(IDalSession session, ExtPosition obj)
        {
            session.InsertOrUpdate(obj);
        }

    }
}
