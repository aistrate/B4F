using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Dal
{
    public enum LockModes
    {
        None,
        Normal
    }

    /// <summary>
    /// The main runtime interface between a GUI application and DAL. 
    /// This is the central API class abstracting the notion of a persistence service.  
    /// </summary>
    public interface IDalSession : IDisposable
    {
        /// <summary>
        /// Is the ISession currently connected? 
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Is the ISession still open? 
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// End the Session by disconnecting from the ADO.NET connection and cleaning up.  
        /// </summary>
        void Close();

        /// <summary>
        /// Disconnect the ISession from the current ADO.NET connection.  
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Obtain a new ADO.NET connection.  
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Re-read the state of the given instance from the underlying database.  
        /// </summary>
        void Refresh(object obj);

        void Refresh(IList list);

        ISession Session { get; }
        IDbConnection Connection { get; }
        string Database { get; }
        string ConnectionString { get; }
        int? TimeOut { get; set; }
        LockModes LockMode { get; set; }
        DateTime GetServerTime();

        DataSet GetDataSet(string hql, string propertyList);
        Object GetObjectInstance(Type type, Object ID);

        IList GetList(Type type);
        IList GetList(Type type, int id);
        IList GetList(Type type, string property, object value);
        IList GetList(Type type, List<ICriterion> expressions);
        IList GetList(Type type, List<ICriterion> expressions, int? maximumRows, int? startRowIndex);
        IList GetList(Type type, List<ICriterion> expressions, List<Order> orderings);
        IList GetList(Type type, List<ICriterion> expressions, List<Order> orderings, int? maximumRows, int? startRowIndex);

        List<T> GetTypedList<T>();
        List<T> GetTypedList<T>(int id);
        List<T> GetTypedList<T>(string property, object value);
        List<T> GetTypedList<T>(List<ICriterion> expressions);
        List<T> GetTypedList<T>(List<ICriterion> expressions, List<Order> orderings);
        List<T> GetTypedList<T>(List<ICriterion> expressions, List<Order> orderings, int? maximumRows, int? startRowIndex);

        List<I> GetTypedList<T, I>();
        List<I> GetTypedList<T, I>(int id);
        List<I> GetTypedList<T, I>(string property, object value);
        List<I> GetTypedList<T, I>(List<ICriterion> expressions);
        List<I> GetTypedList<T, I>(List<ICriterion> expressions, List<Order> orderings);
        List<I> GetTypedList<T, I>(List<ICriterion> expressions, List<Order> orderings, int? maximumRows, int? startRowIndex);

        IList GetListByHQL(string hql);
        IList GetListByHQL(string hql, Hashtable parameters);
        IList GetListByHQL(string hql, Hashtable parameters, Hashtable parameterLists);
        IList GetListByHQL(string hql, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex);

        List<T> GetTypedListByHQL<T>(string hql);
        List<T> GetTypedListByHQL<T>(string hql, Hashtable parameters);
        List<T> GetTypedListByHQL<T>(string hql, Hashtable parameters, Hashtable parameterLists);
        List<T> GetTypedListByHQL<T>(string hql, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex);

        List<I> GetTypedListByHQL<T, I>(string hql);
        List<I> GetTypedListByHQL<T, I>(string hql, Hashtable parameters);
        List<I> GetTypedListByHQL<T, I>(string hql, Hashtable parameters, Hashtable parameterLists);
        List<I> GetTypedListByHQL<T, I>(string hql, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex);

        IList GetListByNamedQuery(string queryName);
        IList GetListByNamedQuery(string queryName, string where);
        IList GetListByNamedQuery(string queryName, Hashtable parameters);
        IList GetListByNamedQuery(string queryName, string where, Hashtable parameters);
        IList GetListByNamedQuery(string queryName, Hashtable parameters, Hashtable parameterLists);
        IList GetListByNamedQuery(string queryName, string where, Hashtable parameters, Hashtable parameterLists);
        IList GetListByNamedQuery(string queryName, string where, Hashtable parameters, Hashtable parameterLists,
                                  int? maximumRows, int? startRowIndex);

        List<T> GetTypedListByNamedQuery<T>(string queryName);
        List<T> GetTypedListByNamedQuery<T>(string queryName, string where);
        List<T> GetTypedListByNamedQuery<T>(string queryName, Hashtable parameters);
        List<T> GetTypedListByNamedQuery<T>(string queryName, string where, Hashtable parameters);
        List<T> GetTypedListByNamedQuery<T>(string queryName, Hashtable parameters, Hashtable parameterLists);
        List<T> GetTypedListByNamedQuery<T>(string queryName, string where, Hashtable parameters, Hashtable parameterLists);
        List<T> GetTypedListByNamedQuery<T>(string queryName, string where, Hashtable parameters, Hashtable parameterLists,
                                            int? maximumRows, int? startRowIndex);

        bool ExecuteStoredProcedure(string sqlStoredProc, Hashtable parameters);
        IList ExecuteStoredProcedureParam(string sqlStoredProc, ArrayList parameters);
        DataSet GetDataFromSP(string sqlStoredProc, Hashtable parameters);
        DataSet GetDataFromQuery(string sqlQuery);
        int ExecuteNonQuery(string sqlStoredProc, Hashtable parameters);
        IList GetListbySQLQuery(string Query, string TableAlias, Type type);

        bool BeginTransaction();
        bool CommitTransaction();

        bool Insert(Object obj);
        bool Insert(IEnumerable list);
        bool Update(Object obj);
        bool Update(IEnumerable list);
        bool Delete(Object obj);
        bool Delete(IEnumerable list);
        bool InsertOrUpdate(Object obj);
        bool InsertOrUpdate(IEnumerable list);
        bool Flush();

        List<ICriterion> GetExpression(string property, object value);
    }
}
