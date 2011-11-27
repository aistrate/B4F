using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Remoting;
using B4F.TotalGiro.Jobs.Manager;
using System.Configuration;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;


namespace B4F.TotalGiro.Jobs.Console
{
    public partial class JobConsole : Form
    {
        public JobConsole()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
              //Trace.Listeners.Add( new TextWriterTraceListener() );

            //DynamicXmlObjectLoader loader = new DynamicXmlObjectLoader();
            manager = JobManager.GetInstance();
            //Debug.Print(manager.Jobs.Count.ToString());
              //AgentManager manager = (AgentManager)loader.Load(@"jobs.config");

            //if (manager == null)
            //    manager = JobManager.GetInstance();
            manager.Start();

            //// remoting configuration in order to allow other programs to retrieve status
            //RemotingConfiguration.Configure(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".config", false);


            //if (manager == null)
            //{
            //    //RemotingConfiguration.RegisterWellKnownClientType(typeof(IJobManager), "tcp://localhost:8085/jobservice/jobmanager");
            //    string reportUrl = ConfigurationSettings.AppSettings["RemoteJobManagerUrl"];
            //    manager = (RemoteJobManager)RemotingServices.Connect(typeof(RemoteJobManager), reportUrl);
            //}
            ////manager.Start();
            //DataSet ds = manager.GetData();

            //if (ds != null)
            //{
            //    dataGridView1.DataSource = ds.Tables[0].DefaultView;

            //    // Automatically resize the visible rows.
            //    dataGridView1.AutoSizeRowsMode =
            //        DataGridViewAutoSizeRowsMode.AllCells;
            //    dataGridView1.AutoResizeColumns(
            //        DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            //    // Set the DataGridView control's border.
            //    dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            //}
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (manager != null)
                manager.Stop();
        }

        private IJobManager manager;

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                if (manager != null)
                {
                    DataSet ds = manager.GetData(getCompanyDetails());
                    if (ds != null)
                    {
                        int tableIndex;
                        int.TryParse(txtTable.Text, out tableIndex);
                        dataGridView1.DataSource = ds.Tables[tableIndex].DefaultView;

                        // Automatically resize the visible rows.
                        dataGridView1.AutoSizeRowsMode =
                            DataGridViewAutoSizeRowsMode.AllCells;
                        dataGridView1.AutoResizeColumns(
                            DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
                        // Set the DataGridView control's border.
                        dataGridView1.BorderStyle = BorderStyle.Fixed3D;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStartJob_Click(object sender, EventArgs e)
        {
            if (manager != null)
                manager.StartJob(getCompanyDetails(), Convert.ToInt32(txtStartJob.Text));
        }

        private ManagementCompanyDetails getCompanyDetails()
        {
            IManagementCompany company = null;
            IDalSession session = NHSessionFactory.GetInstance().GetSession();
            company = ManagementCompanyMapper.GetManagementCompany(session, 1);
            return company.Details;
        }
    }
}