using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using B4F.TotalGiro.Jobs.Manager;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using log4net;
using log4net.Config;
using B4F.TotalGiro.Utils;

namespace B4F.WindowsService.JobService
{
    public partial class JobWindowsService : ServiceBase
    {
        public JobWindowsService()
        {
            InitializeComponent();

            string configFile = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".config";
            FileInfo fileInfo = new FileInfo(configFile);
            if (fileInfo.Exists)
                // remoting configuration in order to allow other programs to retrieve status
                RemotingConfiguration.Configure(configFile, false);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // start the manager
                if (manager == null)
                    manager = JobManager.GetInstance();
                manager.Start();

                // for remoting
                RemoteJobManager.Manager = manager;
            }
            catch (Exception ex)
            {
                string errDescription = string.Format("{0} {1}", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : ""));
                log.Error(errDescription);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (manager != null)
                    manager.Stop();

                // for remoting
                RemoteJobManager.Manager = null;
            }
            catch (Exception ex)
            {
                string errDescription = string.Format("{0} {1}", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : ""));
                log.Error(errDescription, ex);
            }
        }

        //public JobManager JobManager
        //{
        //    get { return manager; }
        //}

        #region Privates

        private static readonly ILog log = LogManager.GetLogger("App");
        private JobManager manager;

        #endregion
    }

    //public class RemoteJobManager : MarshalByRefObject, IJobManager
    //{
    //    #region IJobManager Members

    //    public void Start()
    //    {
    //        RemoteJobManager.Manager.Start();
    //    }

    //    public void Stop()
    //    {
    //        RemoteJobManager.Manager.Stop();
    //    }

    //    public System.Data.DataSet GetData()
    //    {
    //        return RemoteJobManager.Manager.GetData();
    //    }

    //    #endregion

    //    public static IJobManager Manager;
    //}

    //[WebService(Namespace = "http://B4F.Totalgiro.com/Jobs/JobService")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    ////[Policy("ServerPolicy")]
    //public class JobService : System.Web.Services.WebService
    //{
    //    [WebMethod]
    //    [SoapDocumentMethod(ResponseElementName = "JobReport")]
    //    [return: XmlElement("JobReportData")]
    //    public DataSet JobReportRequest()
    //    {
    //        DataSet report = null;
    //        if (JobService.service != null && JobService.service.JobManager != null)
    //            report = JobService.service.JobManager.GetData();
    //            return report;
    //    }

    //    internal static JobWindowsService service;
    //}
}