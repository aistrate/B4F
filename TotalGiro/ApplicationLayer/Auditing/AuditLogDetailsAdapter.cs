using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace B4F.TotalGiro.ApplicationLayer.Auditing
{
    public static class AuditLogDetailsAdapter
    {
        public static DataSet GetAuditLogEvents(string entityClass, int entityKey)
        {
            return AuditLogAdapter.GetSqlQueryResults(
                @"SELECT AuditLogEntryId, DateCreated, EventName, UserName FROM AuditLogEntries 
                  WHERE EntityClass = @entityClass AND EntityKey = @entityKey ORDER BY AuditLogEntryId DESC",
                new SqlParameter[] { new SqlParameter("@entityClass", entityClass), 
                                     new SqlParameter("@entityKey", entityKey) });
        }

        public static DataSet GetAuditLogFields(int auditLogEntryId)
        {
            return AuditLogAdapter.GetSqlQueryResults(
                "SELECT FieldName, OldValue, NewValue FROM AuditLogEntryLines WHERE AuditLogEntryId = @auditLogEntryId",
                new SqlParameter[] { new SqlParameter("@auditLogEntryId", auditLogEntryId) });
        }
    }
}
