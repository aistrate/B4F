using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate;
using System.Data;
using System.Data.SqlClient;

namespace B4F.TotalGiro.Communicator.Reconciliation
{
    public class CashReconciliationMapper
    {

        public static void CreateCashPositions(IDalSession DataSession)
        {
            string connectionString = ((ISession)DataSession.Session).Connection.ConnectionString;
            connectionString += ";Asynchronous Processing=true;Application Name=Reconciliation Module";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlCommand mySelect = new SqlCommand("TG_MappingBOStockTX", conn);
            mySelect.CommandType = CommandType.StoredProcedure;

            int i= mySelect.ExecuteNonQuery();


            conn.Close();
        }
    }
}
