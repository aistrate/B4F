using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Jobs.Manager.Worker
{
    public class ADOCommandWorker: AgentWorker
    {

        #region Props

        public string CommandName
        {
            get { return query; }
            set { query = value; }
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
                if (session == null)
                    throw new ApplicationException("The session can not be null");

                if (CommandName == string.Empty)
                    throw new ApplicationException("The command name can not be null");

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
                int i = session.ExecuteNonQuery(CommandName, parameters);
                string result = string.Format("{0} records were affected by sp {1}", i.ToString(), CommandName);
                e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, result, result);
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured in the command worker", "", ex);
            }
            finally
            {
                worker.ReportProgress(100);
            }
            return (WorkerResult)e.Result;
        }

        #endregion

        #region Privates

        private string query;
        private ADOCommandWorkerParamCollection commandParams = new ADOCommandWorkerParamCollection();

        #endregion
    }

    /// <summary>
    /// ADOCommandWorkerParam holds information for calling stored procuderes
    /// </summary>
    public class ADOCommandWorkerParam
    {
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Value
        {
            get { return paramValue; }
            set { paramValue = value; }
        }

        public ADOCommandWorkerParam() { }

        public ADOCommandWorkerParam(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        #region Privates

        private string name;
        private string paramValue;

        #endregion
    }

    /// <summary>
    /// This is the collection class for the ADOCommandWorkerParam.</summary>
    [Serializable]
    public class ADOCommandWorkerParamCollection : CollectionBase
    {
        /// <summary>
        /// Add a param to the collection.</summary>
        public void Add(ADOCommandWorkerParam param)
        {
            List.Add(param);
        }

        /// <summary>
        /// Remove a param from the collection.</summary>
        public void Remove(ADOCommandWorkerParam param)
        {
            List.Remove(param);
        }

        /// <summary>
        /// Set or retrieve a param at the specific index in the collection.</summary>
        public ADOCommandWorkerParam this[int index]
        {
            get
            {
                return (ADOCommandWorkerParam)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
    }

}
