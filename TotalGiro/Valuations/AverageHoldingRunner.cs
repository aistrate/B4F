using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Timers;
using System.Diagnostics;
using B4F.TotalGiro.Dal;
using System.ComponentModel;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Valuations
{
    internal class AverageHoldingRunner
    {
        public delegate void ReportProgressEventHandler(object sender, ReportProgressEventArgs e);

        public event ReportProgressEventHandler ReportProgress;
        public event EventHandler HasFinished;

        #region Constructor

        public AverageHoldingRunner(DateTime maxAvgHldDate, int concurrentSPCount, IDalSession session, out bool success)
        {
            this.maxAvgHldDate = maxAvgHldDate;
            this.concurrentSPCount = concurrentSPCount;
            this.session = session;
            this.connectionString = session.ConnectionString;
            this.timer = new Timer(2000);
            timer.Elapsed += timer_Elapsed;

            success = createAverageHoldingRun();
            // check if necessary
            if (success)
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
            if (this.averageHoldingRunID > 0 && totalNrRecordsToProcess > 0)
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

        private bool createAverageHoldingRun()
        {
            bool retVal = false;
            SqlConnection connection = (SqlConnection)session.Connection;
            SqlCommand cmd = new SqlCommand("TG_CreateAverageHoldingRun", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter prmValDate = new SqlParameter("maxAvgHldDate", this.maxAvgHldDate);
            prmValDate.SqlDbType = SqlDbType.DateTime;
            cmd.Parameters.Add(prmValDate);

            SqlParameter prmMaxRetroDate = new SqlParameter("maxRetroDate", SqlDbType.DateTime);
            DateTime maxRetroDate = Util.GetFirstDayOfMonth(DateTime.Today).AddYears(-1);
            DateTime.TryParse(ConfigSettingsInfo.GetInfo("MgtFeeMaxRetroDate"), out maxRetroDate);
            prmMaxRetroDate.Value = maxRetroDate;
            cmd.Parameters.Add(prmMaxRetroDate);

            SqlParameter prmRunID = new SqlParameter("AverageHoldingRunID", SqlDbType.Int);
            prmRunID.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmRunID);

            SqlParameter prmRecords = new SqlParameter("records", SqlDbType.Int);
            prmRecords.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmRecords);

            SqlParameter prmErrMessage = new SqlParameter("errMessage", SqlDbType.VarChar, 1000);
            prmErrMessage.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(prmErrMessage);

            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();

            if (!((System.Data.SqlTypes.SqlString)prmErrMessage.SqlValue).IsNull)
                this.errorMessage = ((System.Data.SqlTypes.SqlString)prmErrMessage.SqlValue).Value;

            if (string.IsNullOrEmpty(this.errorMessage) && !((System.Data.SqlTypes.SqlInt32)prmRecords.SqlValue).IsNull)
            {
                totalNrRecordsToProcess = ((System.Data.SqlTypes.SqlInt32)prmRecords.SqlValue).Value;
                if (totalNrRecordsToProcess > 0 && !((System.Data.SqlTypes.SqlInt32)prmRunID.SqlValue).IsNull)
                {
                    averageHoldingRunID = ((System.Data.SqlTypes.SqlInt32)prmRunID.SqlValue).Value;
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
                SqlCommand cmd = new SqlCommand("dbo.TG_CreateAverageHoldings");
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("averageHoldingRunID", this.averageHoldingRunID);
                cmd.Parameters.Add(param1);
                SqlParameter param3 = new SqlParameter("Process", key);
                cmd.Parameters.Add(param3);
                SqlParameter param4 = new SqlParameter("RetryFailures", retryFailures);
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
                string query1 = string.Format(@"select count(*) from dbo.AverageHoldingRunUnits (nolock) where AverageHoldingRunID = {0} and Processed is null or success = 0", this.averageHoldingRunID);
                string query2 = string.Format(@"select count(*) from dbo.AverageHoldingRunUnits (nolock) where AverageHoldingRunID = {0} and success = 0", this.averageHoldingRunID);

                this.nrAccountsToProcessLeft = getDataFromDB(query1);
                int errCount = getDataFromDB(query2);

                if (progress != ((TotalNrAccountsProcessed * 100) / totalNrRecordsToProcess) && progress < 99)
                {
                    progress = ((TotalNrAccountsProcessed * 100) / totalNrRecordsToProcess);
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

        public int TotalNrRecordsToProcess
        {
            get { return this.totalNrRecordsToProcess; }
        }

        public int TotalNrAccountsProcessed
        {
            get { return (this.totalNrRecordsToProcess - this.nrAccountsToProcessLeft); }
        }

        public int ErrorCount
        {
            get { return this.errorCount; }
        }
 
        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        #endregion

        #region Privates

        private BackgroundWorker worker;
        private DoWorkEventArgs workerEvent;
        private IDalSession session;
        private DateTime maxAvgHldDate;
        private int averageHoldingRunID;
        private int totalNrRecordsToProcess;
        private int nrAccountsToProcessLeft;
        private int errorCount;
        private string errorMessage;
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
