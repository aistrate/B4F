using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Timers;
using System.Diagnostics;
using B4F.TotalGiro.Dal;
using System.ComponentModel;

namespace B4F.TotalGiro.Valuations
{
    #region ReportProgressEventArgs

    internal class ReportProgressEventArgs : EventArgs
    {
        public ReportProgressEventArgs(int progress, int errCount)
        {
            this.progress = progress;
            this.errCount = errCount;
        }

        private int progress;
        public int Progress
        {
            get { return progress; }
        }

        private int errCount;
        public int ErrCount
        {
            get { return errCount; }
        }
    }

    #endregion

    internal class ValuationRunner
    {
        public delegate void ReportProgressEventHandler(object sender, ReportProgressEventArgs e);

        public event ReportProgressEventHandler ReportProgress;
        public event EventHandler HasFinished;

        #region Constructor

        public ValuationRunner(DateTime valuationDate, int concurrentSPCount, IDalSession session)
        {
            this.valuationDate = valuationDate;
            this.concurrentSPCount = concurrentSPCount;
            this.session = session;
            this.connectionString = session.ConnectionString;
            this.timer = new Timer(2000);
            timer.Elapsed += timer_Elapsed;

            // check if necessary
            if (createValuationRun())
            {
                if (concurrentSPCount > 0)
                {
                    commands = new Dictionary<int, SqlCommand>(concurrentSPCount);
                    runners = new Dictionary<int, bool>(concurrentSPCount);
                }
            }
        }

        #endregion

        #region Methods

        public void Run()
        {
            if (this.valuationRunID > 0 && TotalNrAccountsToProcess > 0)
            {
                for (int i = 0; i < this.concurrentSPCount; i++)
                    addSP(i);

                if (commands.Count > 0 && HasFinished != null)
                    timer.Start();
            }
            else
                hasFinished = true;
        }

        public void Stop()
        {
            isCancelled = true;
            foreach (int key in commands.Keys)
            {
                SqlCommand cmd = commands[key];
                cmd.Cancel();
            }
        }

        public void Dispose()
        {
            if (commands != null)
            {
                foreach (SqlCommand cmd in commands.Values)
                {
                    if (cmd != null && cmd.Connection != null)
                    {
                        cmd.Connection.Close();
                        Debug.Print("Closed Connection");
                    }
                }
            }
            
            commands = null;
            runners = null;
            Debug.Print(string.Format("{0} - Disposed", DateTime.Now));
        }

        #endregion

        #region TG_RunValuations

        private bool createValuationRun()
        {
            bool retVal = false;
            SqlConnection connection = (SqlConnection)session.Connection;
            SqlCommand cmd = new SqlCommand("TG_CreateValuationRun", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter prmValDate = new SqlParameter("ValuationDate", this.valuationDate);
            cmd.Parameters.Add(prmValDate);

            SqlParameter prmRunID = new SqlParameter("ValuationRunID", SqlDbType.Int);
            prmRunID.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmRunID);

            SqlParameter prmUsedDate = new SqlParameter("UsedDate", SqlDbType.DateTime);
            prmUsedDate.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmUsedDate);

            SqlParameter prmRecords = new SqlParameter("records", SqlDbType.Int);
            prmRecords.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmRecords);

            cmd.ExecuteNonQuery();

            if (!((System.Data.SqlTypes.SqlDateTime)prmUsedDate.SqlValue).IsNull)
                this.valuationDate = ((System.Data.SqlTypes.SqlDateTime)prmUsedDate.SqlValue).Value;

            if (!((System.Data.SqlTypes.SqlInt32)prmRecords.SqlValue).IsNull)
            {
                totalNrAccountsToProcess = ((System.Data.SqlTypes.SqlInt32)prmRecords.SqlValue).Value;
                if (totalNrAccountsToProcess > 0 && !((System.Data.SqlTypes.SqlInt32)prmRunID.SqlValue).IsNull)
                {
                    valuationRunID = ((System.Data.SqlTypes.SqlInt32)prmRunID.SqlValue).Value;
                    retVal = true;
                }
            }
            return retVal;
        }

        private void addSP(int key)
        {
            addSP(key, false);
        }

        private void addSP(int key, bool retryFailures)
        {
            if (!hasFinished || isCancelled)
            {
                SqlConnection connection = new SqlConnection(this.connectionString + ";Asynchronous Processing=true");
                SqlCommand cmd = new SqlCommand("dbo.TG_CreateValuations");
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("ValuationRunID", this.valuationRunID);
                param1.SqlDbType = SqlDbType.Int;
                cmd.Parameters.Add(param1);
                SqlParameter param2 = new SqlParameter("ValuationDate", this.valuationDate);
                param2.SqlDbType = SqlDbType.DateTime;
                cmd.Parameters.Add(param2);
                SqlParameter param3 = new SqlParameter("Process", key);
                param3.SqlDbType = SqlDbType.Int;
                cmd.Parameters.Add(param3);
                SqlParameter param4 = new SqlParameter("RetryFailures", retryFailures);
                param4.SqlDbType = SqlDbType.Bit;
                cmd.Parameters.Add(param4);
                SqlParameter param5 = new SqlParameter("ErrorCount", SqlDbType.Int);
                param5.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param5);
                cmd.CommandTimeout = 0;

                commands[key] = cmd;

                connection.Open();

                if (runners != null)
                {
                    AsyncCallback callback = new AsyncCallback(runValuations_Callback);
                    cmd.BeginExecuteNonQuery(callback, key);

                    runners[key] = true;
                    Debug.Print(string.Format("{0} - {1} started", DateTime.Now, key));
                }
            }
        }

        private void runValuations_Callback(IAsyncResult result)
        {
            int key = 0;
            try
            {
                // retrieve the key.
                key = (int)result.AsyncState;

                Debug.Print(string.Format("{0} - {1} callback", DateTime.Now, key));
                if (commands != null)
                {
                    SqlCommand cmd = commands[key];
                    if (cmd != null)
                    {
                        cmd.EndExecuteNonQuery(result);

                        if (retriedFailures)
                        {
                            SqlParameter prmOut = cmd.Parameters["ErrorCount"];
                            if (prmOut != null && !((System.Data.SqlTypes.SqlInt32)prmOut.SqlValue).IsNull)
                            {
                                this.errorCount = ((System.Data.SqlTypes.SqlInt32)prmOut.SqlValue).Value;
                                Debug.Print(string.Format("{0} command: errorCount {1}", key, this.errorCount));
                            }
                        }

                        runners[key] = false;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (!ex.Message.Contains("Operation cancelled by user"))
                    throw new ApplicationException(ex.ToString());
                else
                {
                    if (runners != null && runners.ContainsKey(key))
                        runners[key] = false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString());
            }
            finally
            {
                checkFinished();
            }
        }

        protected void checkFinished() 
        {
            try
            {
                // check if ready
                if (HasFinished != null)
                {
                    if (runners != null)
                    {
                        foreach (bool isRunning in runners.Values)
                        {
                            if (isRunning)
                                return;
                        }
                    }
                    if (!isCancelled && !retriedFailures)
                        retryFailures();
                    else
                    {
                        if (!hasFinished)
                        {
                            hasFinished = true;
                            if (this.timer != null)
                                this.timer.Stop();
                            HasFinished(this, new EventArgs());
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        protected void retryFailures() 
        {
            addSP(0, true);
            retriedFailures = true;
        }

        #endregion

        #region Timer

        protected void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ReportProgress != null)
            {
                string query1 = string.Format(@"select count(*) from dbo.ValuationRunsAccounts (nolock) where ValuationRunID = {0} and Processed is null or success = 0", this.valuationRunID);
                string query2 = string.Format(@"select count(*) from dbo.ValuationRunsAccounts (nolock) where ValuationRunID = {0} and success = 0", this.valuationRunID);

                this.nrAccountsToProcessLeft = getDataFromDB(query1);
                int errCount = getDataFromDB(query2);

                if (progress != ((TotalNrAccountsProcessed * 100) / totalNrAccountsToProcess) && progress < 99)
                {
                    progress = ((TotalNrAccountsProcessed * 100) / totalNrAccountsToProcess);
                    if (progress < 0)
                        progress = 0;
                    ReportProgress(this, new ReportProgressEventArgs(progress, errCount));
                }
            }
        }

        #endregion

        #region Helper

        protected int getDataFromDB(string query)
        {
            int retVal = 0;
            try
            {
                object obj = null;
                if (this.session.Connection != null && ((SqlConnection)this.session.Connection).State == ConnectionState.Open)
                {
                    SqlCommand cmd = new SqlCommand(query, (SqlConnection)this.session.Connection);
                    obj = cmd.ExecuteScalar();
                }
                if (obj != null)
                    retVal = (int)obj;
            }
            catch (Exception)
            {
                // Do Nothing
            }
            return retVal;
        }

        #endregion

        #region Props

        public BackgroundWorker Worker
        {
            get { return this.worker; }
            set { this.worker = value; }
        }

        public DoWorkEventArgs WorkerEvent
        {
            get { return this.workerEvent; }
            set { this.workerEvent = value; }
        }

        public bool IsRunning
        {
            get { return (!this.hasFinished && !this.isCancelled); }
        }

        public int TotalNrAccountsToProcess
        {
            get { return this.totalNrAccountsToProcess; }
        }

        public int TotalNrAccountsProcessed
        {
            get { return (this.totalNrAccountsToProcess - this.nrAccountsToProcessLeft); }
        }

        public int ErrorCount
        {
            get { return this.errorCount; }
        }

        #endregion

        #region Privates

        private BackgroundWorker worker;
        private DoWorkEventArgs workerEvent;
        private IDalSession session;
        private DateTime valuationDate;
        private int valuationRunID;
        private int totalNrAccountsToProcess;
        private int nrAccountsToProcessLeft;
        private int errorCount;
        private int progress = 0;
        private int concurrentSPCount;
        string connectionString;
        private Dictionary<int, SqlCommand> commands;
        private Dictionary<int, bool> runners;
        private Timer timer;
        private bool isCancelled;
        private bool retriedFailures;
        private bool hasFinished;

        #endregion

    }
}
