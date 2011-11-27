using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using System.Collections;
using System.Data;
using NHibernate.Type;
using NHibernate.SqlCommand;

namespace B4F.TotalGiro.Dal.Auditing
{
    public class AuditLogInterceptor : IInterceptor
    {
        private ISession session;
        private string userName;
        private ArrayList changes = new ArrayList();

        public ISession Session
        {
            set { this.session = value; }
        }

        public string UserName
        {
            set { this.userName = value; }
        }

        private class EntityChange
        {
            public string EventName;
            public object Entity;
            public object[] CurrentState;
            public object[] PreviousState;
            public string[] PropertyNames;

            public EntityChange(string eventName, object entity, object[] currentState, object[] previousState, string[] propertyNames)
            {
                this.EventName = eventName;
                this.Entity = entity;
                this.CurrentState = currentState;
                this.PreviousState = previousState;
                this.PropertyNames = propertyNames;
            }
        }

        #region IInterceptor Members

        bool IInterceptor.OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity is IAuditable)
                changes.Add(new EntityChange("Create", entity, state, null, propertyNames));

            return false;
        }

        bool IInterceptor.OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            if (entity is IAuditable)
                changes.Add(new EntityChange("Update", entity, currentState, previousState, propertyNames));

            return false;
        }

        void IInterceptor.OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity is IAuditable)
                changes.Add(new EntityChange("Delete", entity, null, state, propertyNames));
        }

        void IInterceptor.PostFlush(System.Collections.ICollection entities)
        {
            try
            {
                foreach (EntityChange entityChange in changes)
                {
                    AuditLog.LogEvent(session.Connection, session.Transaction, entityChange.Entity.GetType().Name, 
                        ((IAuditable)entityChange.Entity).Key, entityChange.EventName, userName, entityChange.PropertyNames, 
                        entityChange.PreviousState, entityChange.CurrentState);
                }
            }
            catch (HibernateException ex)
            {
                throw new CallbackException("Audit Log entry could not be created.", ex);
            }
            finally
            {
                changes.Clear();
            }
        }


        // Unimplemented methods -- default actions:

        int[] IInterceptor.FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            return null;
        }

        object IInterceptor.Instantiate(string entityName, EntityMode entityMode, object id)
        {
            return null;
        }

        bool? IInterceptor.IsTransient(object entity)
        {
            return null;
        }

        bool IInterceptor.OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return false;
        }

        void IInterceptor.PreFlush(System.Collections.ICollection entities)
        {
        }

        void IInterceptor.OnCollectionRecreate(object collection, object key)
        {
        }

        void IInterceptor.OnCollectionRemove(object collection, object key)
        {
        }

        void IInterceptor.OnCollectionUpdate(object collection, object key)
        {
        }

        SqlString IInterceptor.OnPrepareStatement(SqlString sql)
        {
            return sql;
        }

        object IInterceptor.GetEntity(string entityName, object id)
        {
            return null;
        }

        string IInterceptor.GetEntityName(object entity)
        {
            return null;
        }

        void IInterceptor.AfterTransactionBegin(ITransaction tx)
        {
        }

        void IInterceptor.AfterTransactionCompletion(ITransaction tx)
        {
        }

        void IInterceptor.BeforeTransactionCompletion(ITransaction tx)
        {
        }

        void IInterceptor.SetSession(ISession session)
        {
        }

        #endregion
    }
}
