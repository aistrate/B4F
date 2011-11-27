using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NHibernate;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;

namespace B4F.TotalGiro.Dal.Auditing
{
    public static class AuditLog
    {
        private static IDbCommand insertEntryCommand = new SqlCommand();
        private static IDbCommand insertEntryLineCommand = new SqlCommand();
        private static KeyPropertyCache keyPropertyCache = new KeyPropertyCache();

        static AuditLog()
        {
            // AuditLogEntries
            insertEntryCommand.CommandType = CommandType.StoredProcedure;
            insertEntryCommand.CommandText = "TG_InsertAuditLogEntry";
            
            addParam(insertEntryCommand, "EntityClass", DbType.String);
            addParam(insertEntryCommand, "EntityKey", DbType.Int32);
            addParam(insertEntryCommand, "DateCreated", DbType.DateTime);
            addParam(insertEntryCommand, "EventName", DbType.String);
            addParam(insertEntryCommand, "UserName", DbType.String);


            // AuditLogEntryLines
            insertEntryLineCommand.CommandType = CommandType.StoredProcedure;
            insertEntryLineCommand.CommandText = "TG_InsertAuditLogEntryLine";

            addParam(insertEntryLineCommand, "AuditLogEntryId", DbType.Int32);
            addParam(insertEntryLineCommand, "FieldName", DbType.String);
            addParam(insertEntryLineCommand, "OldValue", DbType.String);
            addParam(insertEntryLineCommand, "NewValue", DbType.String);
        }

        private static IDbDataParameter addParam(IDbCommand command, string paramName, DbType dbType)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = paramName;
            param.DbType = dbType;
            param.Direction = ParameterDirection.Input;
            
            command.Parameters.Add(param);
            
            return param;
        }

        public static void LogEvent(IDbConnection connection, ITransaction transaction,
                                   string entityClass, int entityKey, string eventName, string userName,
                                   string[] propertyNames, object[] previousState, object[] currentState)
        {
            // AuditLogEntries
            if (insertEntryCommand.Connection != connection)
                insertEntryCommand.Connection = connection;
            if (transaction != null)
                transaction.Enlist(insertEntryCommand);

            ((IDbDataParameter)insertEntryCommand.Parameters[0]).Value = entityClass;
            ((IDbDataParameter)insertEntryCommand.Parameters[1]).Value = entityKey;
            ((IDbDataParameter)insertEntryCommand.Parameters[2]).Value = DateTime.Now;
            ((IDbDataParameter)insertEntryCommand.Parameters[3]).Value = eventName;
            ((IDbDataParameter)insertEntryCommand.Parameters[4]).Value = userName;

            int auditLogEntryId = Convert.ToInt32((decimal)insertEntryCommand.ExecuteScalar());


            // AuditLogEntryLines
            if (insertEntryLineCommand.Connection != connection)
                insertEntryLineCommand.Connection = connection;
            if (transaction != null)
                transaction.Enlist(insertEntryLineCommand);

            if (propertyNames != null)
                for (int i = 0; i < propertyNames.Length; i++)
                    if ((previousState == null || !(previousState[i] is ICollection)) &&
                        (currentState == null || !(currentState[i] is ICollection)))
                    {
                        string previousStateString = (previousState != null ? getPropertyValue(previousState[i]) : "");
                        string currentStateString = (currentState != null ? getPropertyValue(currentState[i]) : "");

                        if (previousStateString != currentStateString)
                        {
                            ((IDbDataParameter)insertEntryLineCommand.Parameters[0]).Value = auditLogEntryId;
                            ((IDbDataParameter)insertEntryLineCommand.Parameters[1]).Value = propertyNames[i];
                            ((IDbDataParameter)insertEntryLineCommand.Parameters[2]).Value = previousStateString;
                            ((IDbDataParameter)insertEntryLineCommand.Parameters[3]).Value = currentStateString;

                            insertEntryLineCommand.ExecuteNonQuery();
                        }
                    }
        }

        private static string getPropertyValue(object property)
        {
            if (property == null)
                return "";

            if (property.GetType() == typeof(DateTime))
                return Utils.Util.DateTimeToString((DateTime)property);

            int? key = keyPropertyCache.GetKeyValue(property);
            return key != null ? key.ToString() : property.ToString();
        }

        private class KeyPropertyCache
        {
            Dictionary<string, PropertyInfo> keyProperties = new Dictionary<string, PropertyInfo>();

            public int? GetKeyValue(object obj)
            {
                Type type = obj.GetType();
                string typeName = type.FullName;

                if (!keyProperties.ContainsKey(typeName))
                    keyProperties[typeName] = type.GetProperty("Key", typeof(int));

                return (keyProperties[typeName] != null ? (int?)keyProperties[typeName].GetValue(obj, null) : null);
            }
        }
    }
}
