using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ApplicationLayer.Auditing
{
    public static class AuditLogAdapter
    {
        public static DataSet GetAuditLogEntities(
            string entityType, int? key, DateTime createdFrom, DateTime createdTo, string createdBy, 
            DateTime lastUpdatedFrom, DateTime lastUpdatedTo, string lastUpdatedBy)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string firstWhere = "", lastWhere = "";

            if (entityType != string.Empty)
            {
                firstWhere += " AND FA.EntityClass LIKE @entityType";
                lastWhere += " AND LA.EntityClass LIKE @entityType";
                parameters.Add(new SqlParameter("@entityType", string.Format("%{0}%", entityType)));
            }

            if (key != null)
            {
                firstWhere += " AND FA.EntityKey = @entityKey";
                lastWhere += " AND LA.EntityKey = @entityKey";
                parameters.Add(new SqlParameter("@entityKey", key));
            }

            if (createdFrom != DateTime.MinValue)
            {
                firstWhere += " AND FA.DateCreated >= @createdFrom";
                parameters.Add(new SqlParameter("@createdFrom", createdFrom));
            }

            if (createdTo != DateTime.MinValue)
            {
                firstWhere += " AND FA.DateCreated < @createdTo";
                parameters.Add(new SqlParameter("@createdTo", createdTo.AddDays(1)));
            }

            if (createdBy != string.Empty)
            {
                firstWhere += " AND FA.UserName LIKE @createdBy";
                parameters.Add(new SqlParameter("@createdBy", string.Format("%{0}%", createdBy)));
            }

            if (lastUpdatedFrom != DateTime.MinValue)
            {
                lastWhere += " AND LA.DateCreated >= @lastUpdatedFrom";
                parameters.Add(new SqlParameter("@lastUpdatedFrom", lastUpdatedFrom));
            }

            if (lastUpdatedTo != DateTime.MinValue)
            {
                lastWhere += " AND LA.DateCreated < @lastUpdatedTo";
                parameters.Add(new SqlParameter("@lastUpdatedTo", lastUpdatedTo.AddDays(1)));
            }

            if (lastUpdatedBy != string.Empty)
            {
                lastWhere += " AND LA.UserName LIKE @lastUpdatedBy";
                parameters.Add(new SqlParameter("@lastUpdatedBy", string.Format("%{0}%", lastUpdatedBy)));
            }
            

            string sqlQuery = string.Format(
              @"SELECT A.AuditLogEntryId, A.EntityClass, A.EntityKey, FA.DateCreated AS Created, FA.UserName AS CreatedBy, 
                       A.DateCreated AS LastUpdated, A.UserName AS LastUpdatedBy, XA.EventCount 
                FROM AuditLogEntries FA 
                INNER JOIN (SELECT MIN(AA.AuditLogEntryId) AS FirstId, MAX(AA.AuditLogEntryId) AS LastId, COUNT(*) AS EventCount
	                        FROM AuditLogEntries AA
	                        INNER JOIN (SELECT LA.EntityClass, LA.EntityKey FROM AuditLogEntries LA
				                        WHERE 1 = 1
                                        {0}
                                        GROUP BY LA.EntityClass, LA.EntityKey) UA ON UA.EntityClass = AA.EntityClass AND UA.EntityKey = AA.EntityKey
	                        GROUP BY AA.EntityClass, AA.EntityKey) XA ON XA.FirstId = FA.AuditLogEntryId 
                INNER JOIN AuditLogEntries A ON A.AuditLogEntryId = XA.LastId 
                WHERE 1 = 1
                {1}
                {2}
                ORDER BY A.AuditLogEntryId DESC", lastWhere, firstWhere, lastWhere.Replace("LA.", "A."));
            
            return GetSqlQueryResults(sqlQuery, parameters);
        }

        internal static DataSet GetSqlQueryResults(string query)
        {
            return GetSqlQueryResults(query, null);
        }

        internal static DataSet GetSqlQueryResults(string query, IEnumerable<SqlParameter> parameters)
        {
            return GetSqlQueryResults(query, parameters.ToArray());
        }
        
        internal static DataSet GetSqlQueryResults(string query, SqlParameter[] parameters)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                SqlCommand command = new SqlCommand(query, (SqlConnection)session.Session.Connection);
                command.CommandTimeout = 300;

                if (parameters != null && parameters.Length > 0)
                    command.Parameters.AddRange(parameters);

                SqlDataAdapter da = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds, "Results");

                return ds;
            }
        }
    }
}
