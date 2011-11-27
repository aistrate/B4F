using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate;
using System.Data;
using System.Data.SqlClient;

namespace B4F.TotalGiro.Dal.AdoNet
{
    /// <exclude/>
    public class AdoDataConnect
    {
        /// <exclude/>
        public AdoDataConnect(IDalSession DataSession)
        {
            string connectionString = ((ISession)DataSession.Session).Connection.ConnectionString;
            //connectionString += ";Asynchronous Processing=true;Application Name=Reconciliation Module";
            this.CurrentConnection = new SqlConnection(connectionString);

            this.CurrentConnection.Open();
        }

        /// <exclude/>
        public AdoDataConnect(IDalSession DataSession, int ConnectTimeout)
        {
            string connectionString = ((ISession)DataSession.Session).Connection.ConnectionString;
            //connectionString += ";Asynchronous Processing=true;Application Name=Reconciliation Module";
            connectionString += ";Connection Timeout=" + ConnectTimeout.ToString();
            this.CurrentConnection = new SqlConnection(connectionString);

            this.CurrentConnection.Open();
        }

        /// <exclude/>
        public SqlConnection CurrentConnection
        {
            get { return currentConnection; }
            set { currentConnection = value; }
        }

        #region Private Variables

        private SqlConnection currentConnection;
        
        #endregion
    }
}
