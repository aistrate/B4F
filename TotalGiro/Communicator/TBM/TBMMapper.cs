using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Communicator.TBM
{
    class TBMMapper
    {
        /// <summary>
        /// Gets the details of a TBM issue by ISIN code.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <param name="IssueId">ISIN code</param>
        /// <returns>A list of issue details</returns>
        public static IList GetIssueDetailsByIsin(IDalSession DataSession, string isin)
		{
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Sql("ISINCode like '%" + isin.ToString() + "%'"));
            return DataSession.GetList(typeof(TBMIssueDetails), expressions);
		}

        /// <summary>
        /// Gets the details of a TBM issue by issueid.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <param name="IssueId">Issue Id (TBM Key value for an instrument)</param>
        /// <returns></returns>
        public static TBMIssueDetails GetIssueDetails(IDalSession DataSession, Int32 IssueId)
        {
            return (TBMIssueDetails)DataSession.GetObjectInstance(typeof(TBMIssueDetails), IssueId);
        }

        /// <summary>
        /// Gets a list of TBMIssueDetails objects.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <returns>A list of type <seealso cref="TBMIssueDetails">TBMIssueDetails</seealso>.</returns>
		public static IList GetIssuesDetailsList(IDalSession DataSession)
		{
            return DataSession.GetList(typeof(TBMIssueDetails));
		}


        /// <summary>
        /// Gets a list of MissingHistoricalPrice objects.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <returns>A list of type <seealso cref="MissingHistoricalPrice">MissingHistoricalPrice</seealso>.</returns>
        public static IList GetMissingHistoricalPriceList(IDalSession DataSession)
        {
            return DataSession.GetList(typeof(MissingHistoricalPrice));
        }
    }
}

