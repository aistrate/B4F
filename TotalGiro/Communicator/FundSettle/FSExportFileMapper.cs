using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;


namespace B4F.TotalGiro.Communicator.FSInterface
{
    /// <summary>
    /// Class that implements different methods to access Fund Settle export file functionality.
    /// </summary>
	public class FSExportFileMapper
	{
        /// <summary>
        /// Gets an FS export file by id.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <param name="FSExportFileid">File Id (Key)</param>
        /// <returns></returns>
		public static FSExportFile GetExportFile(IDalSession DataSession, Int32 FSExportFileid)
		{
			return (FSExportFile)DataSession.GetObjectInstance(typeof(FSExportFile), FSExportFileid);
		}

        /// <summary>
        /// Gets a list of all FS export files.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <returns>A list of type <seealso cref="FSExportFile">FSExportFile</seealso>.</returns>
        public static IList GetExportFiles(IDalSession DataSession)
        {
            return DataSession.GetList(typeof(FSExportFile));
        }

        /// <summary>
        /// Gets a list of FS export files that match supplied criteria.
        /// Criteria can be either:     creationdate between startdate and enddate (inclusive)
        ///                 or:         fileid = FileID
        ///                 or:         FS Number = FsNumber
        /// Parameter vars start with p_
        /// Vars local to the method start with l_
        /// </summary>
        /// <param name="p_objDataSession">Data session object</param>
        /// <param name="p_datStartDate">Date object</param>
        /// <param name="p_datEndDate">Date object</param>
        /// <param name="p_intFileID">Integer object</param>
        /// <param name="p_strFsNumber">String object</param>
        /// <returns>A list of type <seealso cref="FSExportFile">FSExportFile</seealso>.</returns>
        public static IList GetExportFilesByCriteria(IDalSession p_objDataSession, DateTime p_datStartDate, DateTime p_datEndDate, int p_intFileID, string p_strFsNumber)
        {

            //prep an expressionlist
            List<NHibernate.Criterion.ICriterion> l_objExpressions = new List<NHibernate.Criterion.ICriterion>();
            
            //check if any of the criteria have to be met
            //do we have a fileid?
            if (p_intFileID != 0)
            {
                l_objExpressions.Add(NHibernate.Criterion.Expression.Eq("Key", p_intFileID));
            }
            //do we have a fsnumber
            if (p_strFsNumber != null && p_strFsNumber != "" && l_objExpressions.Count == 0)
            {
                l_objExpressions.Add(NHibernate.Criterion.Expression.Eq("FSNumber", p_strFsNumber));
            }

            //only add date criteria when no other criteria are used
            if (l_objExpressions.Count == 0)
            {
                l_objExpressions.Add(NHibernate.Criterion.Expression.Ge("CreationDate", p_datStartDate));
                l_objExpressions.Add(NHibernate.Criterion.Expression.Le("CreationDate", p_datEndDate.AddDays(1)));
            }
            return p_objDataSession.GetList(typeof(FSExportFile), l_objExpressions);

        }

        /// <summary>
        /// Gets the orders that have been written to the FS export file with a specific id.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <param name="fileId">File Id (Key)</param>
        /// <returns>A list of type <seealso cref="Order">Order</seealso></returns>
		public static IList GetOrders(IDalSession DataSession, int fileId)
		{
			List<NHibernate.Criterion.ICriterion> expressions = new List<NHibernate.Criterion.ICriterion>();
			expressions.Add(NHibernate.Criterion.Expression.Eq("ExportFileID", fileId));
			return DataSession.GetList(typeof(Order), expressions);
		}

        /// <summary>
        /// Deletes an FS export file, the object as well as the file on disk.
        /// </summary>
        /// <param name="DataSession">Data session object</param>
        /// <param name="fileId">File Id (Key)</param>
        public static void Delete(IDalSession dataSession, int fileId)
		{
			FSExportFile fileToDelete = GetExportFile(dataSession, fileId);

			// Set order stati back to routed
			foreach (B4F.TotalGiro.Orders.Order order in fileToDelete.ExportedOrders)
			{
				order.SetNew();
				order.ExportFile = null;
				OrderMapper.Update(dataSession, order);
			}

			fileToDelete.DeleteExportFile();
			Delete(dataSession, fileToDelete);
		}

		#region CRUD

        /// <summary>
        /// Makes newly created FSExportFile persistent.
        /// </summary>
        /// <param name="DataSession">data session object</param>
        /// <param name="obj">newly created object</param>
		public static void Insert(IDalSession DataSession, FSExportFile obj)
		{
			DataSession.Insert(obj);
		}

        /// <summary>
        /// Makes newly created list of FSExportFile persistent.
        /// </summary>
        /// <param name="DataSession">data session object</param>
        /// <param name="obj">list of newly created objects</param>
        public static void Insert(IDalSession DataSession, IList list)
		{
			DataSession.Insert(list);
		}

        /// <summary>
        /// Saves the data of an existing FSExportFile.
        /// </summary>
        /// <param name="DataSession">data session object</param>
        /// <param name="obj">existing object</param>
        public static bool Update(IDalSession DataSession, FSExportFile obj)
		{
			NHibernate.ISession session = ((NHSession)DataSession).Session;
			bool transCommitt = true;

			NHibernate.ITransaction nTrans = null;
			try
			{
				 //Get a transaction
				if (session.Transaction == null)
				{
				    nTrans = session.BeginTransaction();
				}
				else
				{
				    nTrans = session.Transaction;
					transCommitt = false;
				}

				session.Save(obj);

				if (transCommitt)
					nTrans.Commit();
				return true;
			}
			catch (Exception ex)
			{
				nTrans.Rollback();
				throw new ApplicationException(ex.InnerException.ToString());
			}
		}

        /// <summary>
        /// Saves the data of an existing list of FSExportFile objects.
        /// </summary>
        /// <param name="DataSession">data session object</param>
        /// <param name="obj">list of existing FSExportFile objects</param>
        public static void Update(IDalSession DataSession, IList list)
		{
			DataSession.Update(list);
		}

        /// <summary>
        /// Deletes an existing FSExportFile.
        /// </summary>
        /// <param name="DataSession">data session object</param>
        /// <param name="obj">existing object</param>
        public static void Delete(IDalSession DataSession, FSExportFile obj)
		{
			DataSession.Delete(obj);
		}

        /// <summary>
        /// Deletes a list of FSExportFile objects.
        /// </summary>
        /// <param name="DataSession">data session object</param>
        /// <param name="obj">list of existing FSExportFile objects</param>
        public static void Delete(IDalSession DataSession, IList list)
		{
			DataSession.Delete(list);
		}

		#endregion
	}
}
