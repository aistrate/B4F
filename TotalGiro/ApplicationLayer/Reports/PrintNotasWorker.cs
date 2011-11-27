using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Jobs;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public class PrintNotasWorker: AgentWorker
    {
        #region Props

        public int NotaType
        {
            get { return notaType; }
            set { notaType = value; }
        }

        #endregion

        #region Overrides

        public override WorkerResult Run(IDalSessionFactory factory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                IDalSession session = factory.CreateSession(); 
                if (session == null)
                    throw new ApplicationException("The session cannot be null");
                session.Close();

                BatchExecutionResults results = new BatchExecutionResults();
                if (NotaType == -1)
                    PrintNotasAdapter.PrintNotas(results, ManagementCompanyID);
                else
                    PrintNotasAdapter.PrintNotas(results, ManagementCompanyID, (NotaReturnClass)NotaType);

                string result = PrintNotasAdapter.FormatErrorsForPrintNotas(results, ManagementCompanyID.ToString());
                e.Result = new WorkerResult(WorkerResult.STATE_NORMAL, WorkerResultStatus.Ok, result, result);
            }
            catch (Exception ex)
            {
                e.Result = new WorkerResult(WorkerResult.STATE_EXCEPTION, WorkerResultStatus.Exception, "An error occured in the print notas worker", "", ex);
            }
            finally
            {
                worker.ReportProgress(100);
            }
            return (WorkerResult)e.Result;
        }

        #endregion

        #region Privates

        private int notaType = -1;

        #endregion
    }
}
