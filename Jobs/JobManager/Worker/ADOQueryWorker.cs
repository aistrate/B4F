using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Jobs.Manager.Worker
{
    /// <summary>
    /// This enumeration defines the possible options for notification.</summary>
    [Serializable]
    [Flags]
    public enum ADOQueryWorkerOptions
    {
        NotifyOnData,
        NotifyOnNoData
    }
    
    public class ADOQueryWorker : AgentWorker
    {
        #region Props

        public string QueryString
        {
            get { return queryString; }
            set { queryString = value; }
        }

        public ADOQueryWorkerOptions NotificationOption
        {
            get { return notificationOption; }
            set { notificationOption = value; }
        }

        public bool UseSP
        {
            get { return useSP; }
            set { useSP = value; }
        }

        public ADOCommandWorkerParamCollection CommandParams
        {
            get { return commandParams; }
        }

        #endregion

        #region Overrides

        public override WorkerResult Run(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                IDalSession session = factory.CreateSession(); 
                DataSet ds;

                if (session == null)
                    throw new ApplicationException("The session can not be null");

                if (QueryString == string.Empty)
                    throw new ApplicationException("The query string can not be null");

                if (UseSP)
                {
                    Hashtable parameters = null;
                    if (CommandParams != null && CommandParams.Count > 0)
                    {
                        foreach (ADOCommandWorkerParam p in CommandParams)
                        {
                            if (parameters == null)
                                parameters = new Hashtable(CommandParams.Count);
                            if (p.Value == string.Empty || p.Value == "NULL")
                                parameters.Add(p.Name, null);
                            else
                                parameters.Add(p.Name, p.Value);
                        }
                    }
                    ds = session.GetDataFromSP(QueryString, parameters);
                }
                else
                    ds = session.GetDataFromQuery(QueryString);

                if (ds == null)
                    throw new ApplicationException(string.Format("The query ({0}) could not be executed.", QueryString));

                if ((NotificationOption == ADOQueryWorkerOptions.NotifyOnData) && (ds.Tables[0].Rows.Count > 0))
                {
                    string detailedMessage = string.Format("{0} transactions were found that are approved but did not allocate.", ds.Tables[0].Rows.Count.ToString());
                    e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Warning, "Data was found for: " + Name, detailedMessage);
                }
                else if ((NotificationOption == ADOQueryWorkerOptions.NotifyOnNoData) && (ds.Tables[0].Rows.Count == 0))
                {
                    e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Warning, "No Data was found for: " + Name);
                }
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured in the query worker", "", ex);
            }
            finally
            {
                worker.ReportProgress(100);
            }
            return (WorkerResult)e.Result;
        }

        #endregion

        #region Privates

        private string queryString;
        private bool useSP = false;
        private ADOQueryWorkerOptions notificationOption;
        private ADOCommandWorkerParamCollection commandParams = new ADOCommandWorkerParamCollection();

        #endregion
    }
}
