using System;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;

namespace B4F.ReportingServices.DataExtensions
{
    /// <summary>
    /// Represents a connection.
    /// </summary>
    public class DataSetConnection : IDbConnection, IDbConnectionExtension, IExtension
    {
        private string connectionString;
        private System.Data.ConnectionState state = System.Data.ConnectionState.Closed;
        private string localizedName = "ADO.NET DataSet";       // as it appears in the reporting Dataset dropdown
        private bool integratedSecurity;
        private string impersonate;
        private string userName;
        private string password;

        public DataSetConnection()
        {
        }

        public DataSetConnection(string connString)
        {
            this.connectionString = connString;
        }

        public System.Data.ConnectionState State
        {
            get { return this.state; }
        }

        #region IDbConnection Members

        public string ConnectionString
        {
            get { return this.connectionString; }
            set { this.connectionString = value; }
        }

        /// <summary>
        /// Returns the connection time-out value set in the connection string. Zero indicates an indefinite time-out period.
        /// </summary>
        public int ConnectionTimeout
        {
            get { return 0; }
        }

        public void Open()
        {
            System.Diagnostics.Trace.WriteLine("DataSetConnection Open");
            
            // nothing to open, just set the state
            this.state = System.Data.ConnectionState.Open;
        }

        public void Close()
        {
            this.state = System.Data.ConnectionState.Closed;
        }

        public IDbCommand CreateCommand()
        {
            return new DataSetCommand(this);
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IExtension Members

        /// <summary>
        /// Gets the localized name of the extension to be displayed in a user interface.
        /// </summary>
        public string LocalizedName
        {
            get { return this.localizedName; }
            set { this.localizedName = value; }
        }

        /// <summary>
        /// Used to pass custom configuration data from an XML file to an extension.
        /// </summary>
        public void SetConfiguration(string configuration)
        {
        }

        #endregion

        #region IDbConnectionExtension Members

        /*
          * For data sources that require credentials, these properties
          * add support for storing secure credentials while designing
          * reports with Report Designer. The Data Source dialog will
          * include support for the Integrated checkbox as well as
          * text boxes for username and password.
          */

        public bool IntegratedSecurity
        {
            get { return this.integratedSecurity; }
            set { this.integratedSecurity = value; }
        }

        public string Impersonate
        {
            set { this.impersonate = value; }
        }

        public string UserName
        {
            set { this.userName = value; }
        }

        public string Password
        {
            set { this.password = value; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of the object and perform any cleanup.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}
