using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Dal
{
    /// <summary>
    /// Wrapper around an <i>NHibernate</i> session. An <i>NHibernate</i> session is the main runtime interface between 
    /// an application and <i>NHibernate</i>, the central API class abstracting the notion of a persistence service.
    /// </summary>
    public class NHSession : IDalSession
    {
        internal NHSession(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets the associated <i>NHibernate</i> session.
        /// </summary>
        public ISession Session
        {
            get { return this.session; }
        }

        #region Retrieve Data Members

        /// <summary>
        /// Returns the persistent instance of the given entity class with the given identifier, assuming that the instance exists.
        /// </summary>
        /// <param name="type">A persistent class.</param>
        /// <param name="ID">A valid identifier of an existing persistent instance of the class.</param>
        /// <returns>The persistent instance or proxy.</returns>
        /// <remarks>
        /// You should not use this method to determine if an instance exists 
        /// (use <see cref="B4F.TotalGiro.Dal.NHSession.GetListByHQL(System.String)">NHSession.GetList()</see> instead). 
        /// Use this only to retrieve an instance that you assume exists, where non-existence would be an actual error.
        /// </remarks>
        public object GetObjectInstance(Type type, object ID)
        {

            object obj = null;

            isSessionOK(true);
            if (!(session.IsConnected)) session.Reconnect();

            try
            {
                obj = session.Load(type, ID);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(string.Format("Key : {0} not found!!", ID.ToString()), ex);
            }
            finally
            {
                //session.Disconnect();
            }
            return obj;
        }

        #region GetList

        /// <summary>
        /// Gets a list of all objects of a given type.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type)
        {
            return GetList(type, null, null, null, null);
        }

        /// <summary>
        /// Gets a list of all objects of a given type.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <param name="id">Unique id</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type, int id)
        {
            return GetList(type, "Key", id);
        }

        /// <summary>
        /// Gets a list of all objects of a given type.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <param name="property">A property name</param>
        /// <param name="value">The property value</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type, string property, object value)
        {
            return GetList(type, GetExpression(property, value), null, null, null);
        }

        /// <summary>
        /// Gets a list of objects of a given type, filtered by the given criteria.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <param name="expressions">Filtering criteria.</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type, List<ICriterion> expressions)
        {
            return GetList(type, expressions, null, null, null);
        }

        /// <summary>
        /// Gets a list of objects of a given type, filtered by the given criteria, and sorted as specified.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <param name="expressions">Filtering criteria.</param>
        /// <param name="maximumRows">Maximum number of objects to return in the list.</param>
        /// <param name="startRowIndex">Start Row to return. Only when maximumRows is not null.</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type, List<ICriterion> expressions, int? maximumRows, int? startRowIndex)
        {
            return GetList(type, expressions, null, maximumRows, startRowIndex);
        }

        /// <summary>
        /// Gets a list of objects of a given type, filtered by the given criteria, and sorted as specified.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <param name="expressions">Filtering criteria.</param>
        /// <param name="orderings">Sorting specification.</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type, List<ICriterion> expressions, List<Order> orderings)
        {
            return GetList(type, expressions, orderings, null, null);
        }

        /// <summary>
        /// Gets a list of objects of a given type, filtered by the given criteria, and sorted as specified.
        /// </summary>
        /// <param name="type">The type of objects to get.</param>
        /// <param name="expressions">Filtering criteria.</param>
        /// <param name="orderings">Sorting specification.</param>
        /// <param name="maximumRows">Maximum number of objects to return in the list.</param>
        /// <param name="startRowIndex">Start Row to return. Only when maximumRows is not null.</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        public IList GetList(Type type, List<ICriterion> expressions, List<Order> orderings, int? maximumRows, int? startRowIndex)
        {
            try
            {
                if (!session.IsConnected)
                    session.Reconnect();

                ICriteria criteria = session.CreateCriteria(type);

                if (expressions != null)
                    expressions.ForEach(exp => criteria.Add(exp));

                if (orderings != null)
                    orderings.ForEach(ord => criteria.AddOrder(ord));

                if (maximumRows != null && maximumRows > 0)
                {
                    criteria.SetMaxResults((int)maximumRows);
                    if (startRowIndex != null && startRowIndex >= 0)
                        criteria.SetFirstResult((int)startRowIndex);
                }

                if (TimeOut != null)
                    criteria.SetTimeout(TimeOut.Value);

                return criteria.List();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Could not retrieve {0} objects.", type), ex);
            }
        }

        #endregion

        #region GetTypedList

        public List<T> GetTypedList<T>()
        {
            return ToList<T>(GetList(typeof(T)));
        }

        public List<T> GetTypedList<T>(int id)
        {
            return ToList<T>(GetList(typeof(T), id));
        }

        public List<T> GetTypedList<T>(string property, object value)
        {
            return ToList<T>(GetList(typeof(T), property, value));
        }

        public List<T> GetTypedList<T>(List<ICriterion> expressions)
        {
            return ToList<T>(GetList(typeof(T), expressions));
        }

        public List<T> GetTypedList<T>(List<ICriterion> expressions, List<Order> orderings)
        {
            return ToList<T>(GetList(typeof(T), expressions, orderings));
        }

        public List<T> GetTypedList<T>(List<ICriterion> expressions, List<Order> orderings, int? maximumRows, int? startRowIndex)
        {
            return ToList<T>(GetList(typeof(T), expressions, orderings, maximumRows, startRowIndex));
        }

        public List<I> GetTypedList<T, I>()
        {
            return ToList<I>(GetList(typeof(T)));
        }

        public List<I> GetTypedList<T, I>(int id)
        {
            return ToList<I>(GetList(typeof(T), id));
        }

        public List<I> GetTypedList<T, I>(string property, object value)
        {
            return ToList<I>(GetList(typeof(T), property, value));
        }

        public List<I> GetTypedList<T, I>(List<ICriterion> expressions)
        {
            return ToList<I>(GetList(typeof(T), expressions));
        }

        public List<I> GetTypedList<T, I>(List<ICriterion> expressions, List<Order> orderings)
        {
            return ToList<I>(GetList(typeof(T), expressions, orderings));
        }

        public List<I> GetTypedList<T, I>(List<ICriterion> expressions, List<Order> orderings, int? maximumRows, int? startRowIndex)
        {
            return ToList<I>(GetList(typeof(T), expressions, orderings, maximumRows, startRowIndex));
        }

        #endregion

        public static List<T> ToList<T>(IList list)
        {
            return list.Cast<T>().ToList();
        }

        #region GetListByHQL

        public IList GetListByHQL(string hql)
        {
            return GetListByHQL(hql, null, null, null, null);
        }

        public IList GetListByHQL(string hql, Hashtable parameters)
        {
            return GetListByHQL(hql, parameters, null, null, null);
        }

        public IList GetListByHQL(string hql, Hashtable parameters, Hashtable parameterLists)
        {
            return GetListByHQL(hql, parameters, parameterLists, null, null);
        }

        public IList GetListByHQL(string hql, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex)
        {
            IList list = null;
            IQuery query = null;

            try
            {
                if (!(session.IsConnected)) session.Reconnect();

                query = session.CreateQuery(hql);
                list = getListFromQuery(query, parameters, parameterLists, maximumRows, startRowIndex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while retrieving data.", ex);
            }
            return list;
        }

        #endregion

        #region GetTypedListByHQL

        public List<T> GetTypedListByHQL<T>(string hql)
        {
            return ToList<T>(GetListByHQL(hql));
        }

        public List<T> GetTypedListByHQL<T>(string hql, Hashtable parameters)
        {
            return ToList<T>(GetListByHQL(hql, parameters));
        }

        public List<T> GetTypedListByHQL<T>(string hql, Hashtable parameters, Hashtable parameterLists)
        {
            return ToList<T>(GetListByHQL(hql, parameters, parameterLists));
        }

        public List<T> GetTypedListByHQL<T>(string hql, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex)
        {
            return ToList<T>(GetListByHQL(hql, parameters, parameterLists, maximumRows, startRowIndex));
        }

        public List<I> GetTypedListByHQL<T, I>(string hql)
        {
            return ToList<I>(GetListByHQL(hql));
        }

        public List<I> GetTypedListByHQL<T, I>(string hql, Hashtable parameters)
        {
            return ToList<I>(GetListByHQL(hql, parameters));
        }

        public List<I> GetTypedListByHQL<T, I>(string hql, Hashtable parameters, Hashtable parameterLists)
        {
            return ToList<I>(GetListByHQL(hql, parameters, parameterLists));
        }

        public List<I> GetTypedListByHQL<T, I>(string hql, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex)
        {
            return ToList<I>(GetListByHQL(hql, parameters, parameterLists, maximumRows, startRowIndex));
        }

        #endregion

        #region GetListByNamedQuery

        public IList GetListByNamedQuery(string queryName)
        {
            return GetListByNamedQuery(queryName, null, null, null, null, null);
        }

        public IList GetListByNamedQuery(string queryName, string where)
        {
            return GetListByNamedQuery(queryName, where, null, null, null, null);
        }

        public IList GetListByNamedQuery(string queryName, Hashtable parameters)
        {
            return GetListByNamedQuery(queryName, null, parameters, null, null, null);
        }

        public IList GetListByNamedQuery(string queryName, string where, Hashtable parameters)
        {
            return GetListByNamedQuery(queryName, where, parameters, null, null, null);
        }

        public IList GetListByNamedQuery(string queryName, Hashtable parameters, Hashtable parameterLists)
        {
            return GetListByNamedQuery(queryName, null, parameters, parameterLists, null, null);
        }

        public IList GetListByNamedQuery(string queryName, string where, Hashtable parameters, Hashtable parameterLists)
        {
            return GetListByNamedQuery(queryName, where, parameters, parameterLists, null, null);
        }

        /// <summary>
        /// Executes a named query (HQL or SQL) defined in the mapping file.
        /// </summary>
        /// <param name="queryName">The name of a query defined in the mapping file.</param>
        /// <param name="where">Additional where statements.</param>
        /// <param name="parameters">Parameters for the named query (name/value pairs).</param>
        /// <param name="parameterLists">Parameters for the named query for collections (name/collection of values pairs).</param>
        /// <param name="maximumRows">Maximum row numbers to return.</param>
        /// <param name="startRowIndex">The row to start on.</param>
        /// <returns>An NHibernate result set, which is a list of lists of primitive types (string, int, DateTime, etc.).
        /// Every list in the primary-level list represents a row in the record set, and the primitive-type values are field values 
        /// inside the row.</returns>
        public IList GetListByNamedQuery(string queryName, string where, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex)
        {
            IList list = null;
            IQuery query = null;

            try
            {
                if (!(session.IsConnected)) session.Reconnect();

                query = session.GetNamedQuery(queryName);

                if (query == null)
                    throw new ApplicationException(string.Format("Named query {0} can not be found. Contact your system administrator.", queryName));

                if (query.QueryString.Contains("{0}"))
                    query = session.CreateQuery(string.Format(query.QueryString, " " + where));
                else if (!string.IsNullOrEmpty(where))
                    query = session.CreateQuery(query.QueryString + System.Environment.NewLine + " " + where);

                string[] queryLines = query.QueryString.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (string param in query.NamedParameters)
                {
                    if (!((parameters != null && parameters.ContainsKey(param) && parameters[param] != null) ||
                        (parameterLists != null && parameterLists.ContainsKey(param) && parameterLists[param] != null)))
                    {
                        for (int i = 0; i < queryLines.Length; i++)
                        {
                            if (queryLines[i] != null && queryLines[i].Contains(":" + param))
                                queryLines[i] = null;
                        }
                    }
                }

                string q = String.Join(System.Environment.NewLine, queryLines);
                query = session.CreateQuery(q);

                list = getListFromQuery(query, parameters, parameterLists, maximumRows, startRowIndex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while retrieving data.", ex);
            }

            return list;
        }

        #endregion

        #region GetTypedListByNamedQuery

        public List<T> GetTypedListByNamedQuery<T>(string queryName)
        {
            return GetTypedListByNamedQuery<T>(queryName, null, null, null, null, null);
        }

        public List<I> GetTypedListByNamedQuery<T, I>(string queryName)
        {
            return ToList<I>(GetTypedListByNamedQuery<T>(queryName, null, null, null, null, null));
        }

        public List<T> GetTypedListByNamedQuery<T>(string queryName, string where)
        {
            return GetTypedListByNamedQuery<T>(queryName, where, null, null, null, null);
        }

        public List<I> GetTypedListByNamedQuery<T, I>(string queryName, string where)
        {
            return ToList<I>(GetTypedListByNamedQuery<T>(queryName, where, null, null, null, null));
        }

        public List<T> GetTypedListByNamedQuery<T>(string queryName, Hashtable parameters)
        {
            return GetTypedListByNamedQuery<T>(queryName, null, parameters, null, null, null);
        }

        public List<I> GetTypedListByNamedQuery<T, I>(string queryName, Hashtable parameters)
        {
            return ToList<I>(GetTypedListByNamedQuery<T>(queryName, null, parameters, null, null, null));
        }

        public List<T> GetTypedListByNamedQuery<T>(string queryName, string where, Hashtable parameters)
        {
            return GetTypedListByNamedQuery<T>(queryName, where, parameters, null, null, null);
        }

        public List<I> GetTypedListByNamedQuery<T, I>(string queryName, string where, Hashtable parameters)
        {
            return ToList<I>(GetTypedListByNamedQuery<T>(queryName, where, parameters, null, null, null));
        }

        public List<T> GetTypedListByNamedQuery<T>(string queryName, Hashtable parameters, Hashtable parameterLists)
        {
            return GetTypedListByNamedQuery<T>(queryName, null, parameters, parameterLists, null, null);
        }

        public List<I> GetTypedListByNamedQuery<T, I>(string queryName, Hashtable parameters, Hashtable parameterLists)
        {
            return ToList<I>(GetTypedListByNamedQuery<T>(queryName, null, parameters, parameterLists, null, null));
        }

        public List<T> GetTypedListByNamedQuery<T>(string queryName, string where, Hashtable parameters, Hashtable parameterLists)
        {
            return GetTypedListByNamedQuery<T>(queryName, where, parameters, parameterLists, null, null);
        }

        public List<I> GetTypedListByNamedQuery<T, I>(string queryName, string where, Hashtable parameters, Hashtable parameterLists)
        {
            return ToList<I>(GetTypedListByNamedQuery<T>(queryName, where, parameters, parameterLists, null, null));
        }

        /// <summary>
        /// Executes a named query (HQL or SQL) defined in the mapping file.
        /// </summary>
        /// <param name="queryName">The name of a query defined in the mapping file.</param>
        /// <param name="where">Additional where statements.</param>
        /// <param name="parameters">Parameters for the named query (name/value pairs).</param>
        /// <param name="parameterLists">Parameters for the named query for collections (name/collection of values pairs).</param>
        /// <param name="maximumRows">Maximum row numbers to return.</param>
        /// <param name="startRowIndex">The row to start on.</param>
        /// <returns>A Typed list.</returns>
        public List<T> GetTypedListByNamedQuery<T>(string queryName, string where, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex)
        {
            return ToList<T>(GetListByNamedQuery(queryName, where, parameters, parameterLists, maximumRows, startRowIndex));
        }

        #endregion

        /// <summary>
        /// Creates a <b>DataSet</b> (both schema and data) by performing the HQL (Hibernate Query Language) query passed in as the first parameter.
        /// </summary>
        /// <param name="hql">The HQL (Hibernate Query Language) query to perform.</param>
        /// <param name="propertyList">A list of property names, which will become table columns in the <b>DataSet</b>.</param>
        /// <returns>A <b>DataSet</b> containing a table with one column for each property in <i>propertyList</i>,
        /// and one row for each row in the result set of the HQL (Hibernate Query Language) query.</returns>
        public DataSet GetDataSet(string hql, string propertyList)
        {
            IList list = null;
            DataSet dset = null;

            try
            {
                list = GetListByHQL(hql);
                if (list != null)
                {
                    dset = DataSetBuilder.CreateDataSetFromHibernateList(list, propertyList);
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(string.Format("An error occurred while parsing the following HQL statement: '{0}'", hql), ex);
            }
            return dset;
        }

        #region ADO Stuff

        /// <summary>
        /// ParamInfo holds information for calling stored procuderes
        /// </summary>
        public class ParamInfo
        {
            public string name;
            public object value;
            public NHibernate.Type.IType type;

            public ParamInfo()
            {
            }

            public ParamInfo(string name, object value)
            {
                this.name = name;
                this.value = value;
            }

            public ParamInfo(string name, object value, NHibernate.Type.IType type)
            {
                this.name = name;
                this.value = value;
                this.type = type;
            }
        }

        /// <summary>
        /// Executes a SQL query.
        /// </summary>
        /// <param name="query">A query expressed in SQL.</param>
        /// <param name="tableAlias">A table alias that appears inside {} in the SQL string.</param>
        /// <param name="type">The persistent class to be returned.</param>
        /// <returns>A list of objects of type <i>type</i>.</returns>
        /// <example> This examples shows how to call the GetListbySQLQuery method.
        /// <code>
        ///     1)
        ///     string szQuery = "SELECT CountryID {Country.Key}, CountryName {Country.CountryName}, Iso2 {Country.Iso2}, Iso3 {Country.Iso3}, Iso3NumCode {Country.Iso3NumCode}, CountryNameInternational {Country.InternationalName} FROM Countries"; 
        ///     IList result = session.GetListbySQLQuery(szQuery, "Country", typeof(Country)); 
        /// 
        ///     2)
        ///     string szQuery = "SELECT {Position.*} FROM vwePositions {Position} where AccountID = " + accountId.ToString();
        ///     IList positions = session.GetListbySQLQuery(szQuery, "Position", typeof(Position)); 
        /// </code>
        /// </example>
        public IList GetListbySQLQuery(string query, string tableAlias, Type type)
        {
            return session.CreateSQLQuery(query).AddEntity(tableAlias, type).List();
        }

        /// <summary>
        /// Call a stored procedure with optional type of param
        /// </summary>
        /// <param name="sqlStoredProc">
        /// The sql of the stored procedure.
        /// It can look like EXEC [uspGetEmployeeManagers] @EmployeeID = :employeeID
        /// </param>
        /// <param name="parameters">ArrayList of Paraminfo objects</param>
        /// <returns>IList containing the query results of the sp</returns>
        public IList ExecuteStoredProcedureParam(string sqlStoredProc, ArrayList parameters)
        {
            // eg sqlStoredProc -> EXEC [uspGetEmployeeManagers] @EmployeeID = :employeeID
            IQuery query = session.CreateSQLQuery(sqlStoredProc).AddEntity("return", typeof(Utils.spReturnValue));
            if (parameters != null && parameters.Count > 0)
            {
                foreach (ParamInfo param in parameters)
                {
                    query.SetParameter((String)param.name, param.value, param.type);
                }
            }
            query.SetTimeout(0);
            return query.List();
        }

        /// <summary>
        /// Call a stored procedure
        /// </summary>
        /// <param name="sqlStoredProc">
        /// The sql of the stored procedure.
        /// It can look like EXEC [uspGetEmployeeManagers] @EmployeeID = :employeeID
        /// </param>
        /// <param name="parameters">The optional parameters passed to the sp</param>
        /// <returns></returns>
        public bool ExecuteStoredProcedure(string sqlStoredProc, Hashtable parameters)
        {
            // eg sqlStoredProc -> EXEC [uspGetEmployeeManagers] @EmployeeID = :employeeID
            IQuery query = session.CreateSQLQuery(sqlStoredProc).AddEntity("return", typeof(Utils.spReturnValue));
            if (parameters != null && parameters.Count > 0)
            {
                foreach (DictionaryEntry param in parameters)
                {
                    query.SetParameter((String)param.Key, param.Value);
                }
            }
            query.SetTimeout(0);
            IList list = query.List();

            return true;
        }

        /// <summary>
        /// Call a stored procedure that does not return a result
        /// </summary>
        /// <param name="sqlStoredProc">The sql of the stored procedure.</param>
        /// <param name="parameters">The optional parameters passed to the sp</param>
        /// <returns>The number of rows affected</returns>
        public int ExecuteNonQuery(string sqlStoredProc, Hashtable parameters)
        {
            SqlCommand command = new SqlCommand(sqlStoredProc, (SqlConnection)this.Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            
            if (parameters != null && parameters.Count > 0)
                foreach (DictionaryEntry param in parameters)
                {
                    SqlParameter cmdParam = new SqlParameter((String)param.Key, param.Value);
                    command.Parameters.Add(cmdParam);
                }

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Call a stored procedure that returns data
        /// </summary>
        /// <param name="sqlStoredProc">The sql of the stored procedure.</param>
        /// <param name="parameters">The optional parameters passed to the sp</param>
        /// <returns>Dataset with the data</returns>
        public DataSet GetDataFromSP(string sqlStoredProc, Hashtable parameters)
        {
            SqlDataAdapter da = new SqlDataAdapter(sqlStoredProc, (SqlConnection)this.Connection);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.CommandTimeout = 0;

            if (parameters != null && parameters.Count > 0)
                foreach (DictionaryEntry param in parameters)
                {
                    SqlParameter cmdParam = new SqlParameter((String)param.Key, param.Value);
                    da.SelectCommand.Parameters.Add(cmdParam);
                }

            DataSet ds = new DataSet(sqlStoredProc + "Data");
            da.Fill(ds);

            return ds;
        }

        /// <summary>
        /// Returns data by a sql query
        /// </summary>
        /// <param name="sqlQuery">The query.</param>
        /// <returns>Dataset with the data</returns>
        public DataSet GetDataFromQuery(string sqlQuery)
        {
            SqlDataAdapter da = new SqlDataAdapter(sqlQuery, (SqlConnection)this.Connection);
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.CommandTimeout = 0;

            DataSet ds = new DataSet("Data");
            da.Fill(ds);

            return ds;
        }

        #endregion

        #region Database Properties

        /// <summary>
        /// The name of the Database
        /// </summary>
        public string Database
        {
            get
            {
                string db = "";
                if (session != null && session.Connection != null)
                    db = session.Connection.Database;

                return db;
            }
        }

        public string ConnectionString
        {
            get
            {
                string con = "";
                if (session != null && session.Connection != null)
                    con = session.Connection.ConnectionString;

                return con;
            }
        }

        public IDbConnection Connection
        {
            get { return session.Connection; }
        }

        public DateTime GetServerTime()
        {
            SqlCommand cmd = new SqlCommand(@"select getDate()", (SqlConnection)this.Connection);
            return (DateTime)cmd.ExecuteScalar();

        }

        public int? TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        public LockModes LockMode
        {
            get { return lockMode; }
            set { lockMode = value; }
        }

        #endregion

        #region Connection

        /// <summary>
        /// Gets a value indicating whether the <i>NHibernate</i> session is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                isSessionOK(true);
                return session.IsConnected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <i>NHibernate</i> session is still open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                isSessionOK(true);
                return session.IsOpen;
            }
        }

        /// <summary>
        /// Ends the <i>NHibernate</i> session by disconnecting from the ADO.NET connection and cleaning up.
        /// </summary>
        public void Close()
        {
            isSessionOK(true);
            session.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing &&
                    session != null &&
                    session.IsOpen)
                    session.Close();

                session = null;
                disposed = true;
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Disconnects the <i>NHibernate</i> session from the current ADO.NET connection.
        /// </summary>
        public void Disconnect()
        {
            isSessionOK(true);
            session.Disconnect();
        }

        /// <summary>
        /// Obtains a new ADO.NET connection.
        /// </summary>
        public void Reconnect()
        {
            isSessionOK(true);
            session.Reconnect();
        }

        /// <summary>
        /// Re-reads the state of the given object from the underlying database.
        /// </summary>
        /// <param name="obj">The object for which to re-read the state from the underlying database.</param>
        /// <remarks>
        /// It is inadvisable to use this to implement long-running sessions that span many business tasks. 
        /// This method is, however, useful in certain special circumstances. 
        /// For example, when a database trigger alters the object state upon insert or update;
        /// or, after executing direct SQL (eg. a mass update) in the same session; or, after inserting a BLOB or CLOB.
        /// </remarks>
        public void Refresh(object obj)
        {
            isSessionOK(true);

            if (obj != null)
                session.Refresh(obj);
        }

        public void Refresh(IList list)
        {
            isSessionOK(true);

            if (list != null)
                foreach (object obj in list)
                    Refresh(obj);
        }

        #endregion

        #endregion


        #region CRUD

        private enum ActionType
        {
            Insert,
            Update,
            Delete,
            InsertOrUpdate,
            Flush
        }

        /// <summary>
        /// Begins a unit of work.
        /// </summary>
        /// <returns><b>true</b> if transaction begun successfully, <b>false</b> if not.</returns>
        /// <remarks>
        /// If a new underlying transaction is required, begin the transaction. 
        /// Otherwise, continue the new work in the context of the existing underlying transaction.
        /// </remarks>
        public bool BeginTransaction()
        {
            if (session.Transaction == null ||
                !session.Transaction.IsActive)
            {
                ITransaction nTrans = session.BeginTransaction();
                inTrans = (nTrans != null);

                return inTrans;
            }
            else
                return true;
        }

        /// <summary>
        /// Flushes the associated <i>NHibernate</i> session and ends the unit of work.
        /// </summary>
        /// <returns><b>true</b> if the transaction was successfully commited, <b>false</b> if it was rolled back.</returns>
        public bool CommitTransaction()
        {
            ITransaction nTrans = session.Transaction;

            if (nTrans == null)
                throw new ApplicationException("It is impossible to commit the transaction since there is no current transaction.");

            try
            {
                nTrans.Commit();
                return true;
            }
            catch (Exception)
            {
                nTrans.Rollback();
                throw;
            }
            finally
            {
                inTrans = false;
            }
        }

        /// <summary>
        /// Persists the given transient instance, first assigning a generated identifier.
        /// </summary>
        /// <param name="obj">A transient instance of a persistent class.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        public bool Insert(object obj)
        {
            return Action(obj, null, ActionType.Insert);
        }

        /// <summary>
        /// Persists a given list of transient instances, first assigning each a generated identifier.
        /// </summary>
        /// <param name="list">A given list of transient instances of a persistent class.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        public bool Insert(IEnumerable list)
        {
            return Action(null, list, ActionType.Insert);
        }

        /// <summary>
        /// Updates the persistent instance having the identifier of the given transient instance.
        /// </summary>
        /// <param name="obj">A transient instance containing updated state.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        /// <remarks>
        /// If the given transient instance has a null identifier, an exception will be thrown.
        /// </remarks>
        public bool Update(object obj)
        {
            return Action(obj, null, ActionType.Update);
        }

        /// <summary>
        /// Updates the persistent instances having the identifiers of the given list of transient instances.
        /// </summary>
        /// <param name="list">A list of transient instances containing updated state.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        /// <remarks>
        /// If any of the given transient instances has a null identifier, an exception will be thrown.
        /// </remarks>
        public bool Update(IEnumerable list)
        {
            return Action(null, list, ActionType.Update);
        }

        /// <summary>
        /// Remove a persistent instance from the datastore.
        /// </summary>
        /// <param name="obj">The instance to be removed.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        /// <remarks>
        /// The argument may be a new transient instance, or a transient instance with an identifier associated with existing persistent state.
        /// </remarks>
        public bool Delete(object obj)
        {
            return Action(obj, null, ActionType.Delete);
        }

        /// <summary>
        /// Remove a given list of persistent instances from the datastore.
        /// </summary>
        /// <param name="list">A list of instances to be removed.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        /// <remarks>
        /// Objects in the list may be new transient instances, or transient instances with identifiers associated with existing persistent state.
        /// </remarks>
        public bool Delete(IEnumerable list)
        {
            return Action(null, list, ActionType.Delete);
        }

        /// <summary>
        /// Either insert or update the given instance, depending upon the value of its identifier property.
        /// </summary>
        /// <param name="obj">A transient instance containing new or updated state.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        public bool InsertOrUpdate(object obj)
        {
            return Action(obj, null, ActionType.InsertOrUpdate);
        }

        /// <summary>
        /// Either insert or update the instances in the given list, depending upon the values of their identifier property.
        /// </summary>
        /// <param name="list">A list of transient instances containing new or updated state.</param>
        /// <returns><b>true</b> if operation was successful, <b>false</b> if not.</returns>
        public bool InsertOrUpdate(IEnumerable list)
        {
            return Action(null, list, ActionType.InsertOrUpdate);
        }

        public bool Flush()
        {
            return Action(null, null, ActionType.Flush);
        }

        private bool Action(object obj, IEnumerable list, ActionType action)
        {
            ITransaction transaction = null;

            bool transCommitt = true;
            bool returnValue = false;

            try
            {
                if (!session.IsConnected)
                    session.Reconnect();

                //Get a transaction
                if (session.Transaction == null || !session.Transaction.IsActive || !inTrans)
                    transaction = session.BeginTransaction();
                else
                {
                    transaction = session.Transaction;

                    if (transaction.WasCommitted || transaction.WasRolledBack)
                        throw new ApplicationException("Can not use a transaction that already has been committed/rolled back");

                    transCommitt = false;
                }

                if (action == ActionType.Flush)
                    session.Flush();
                else
                {
                    if (actionToMethod == null)
                        actionToMethod = new Dictionary<ActionType, Action<object>>()
                        {
                            { ActionType.Insert, o => session.Save(o) },
                            { ActionType.Update, session.Update },
                            { ActionType.Delete, session.Delete },
                            { ActionType.InsertOrUpdate, session.SaveOrUpdate },
                        };

                    Action<object> method;
                    if (actionToMethod.TryGetValue(action, out method))
                    {
                        if (list == null)
                            method(obj);
                        else
                            foreach (object objL in list)
                                method(objL);
                    }
                }

                if (transCommitt)
                    transaction.Commit();

                returnValue = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ApplicationException(string.Format("Could not perform {0} action.", action.ToString()), ex);
            }
            finally
            {
                //session.Disconnect();
            }

            return returnValue;
        }

        #endregion


        #region Helpers

        public List<ICriterion> GetExpression(string property, object value)
        {
            return new List<ICriterion>() { Expression.Eq(property, value) };
        }

        private bool isSessionOK(bool raiseErr)
        {
            if (session != null)
            {
                return true;
            }
            else
            {
                if (raiseErr)
                    throw new ApplicationException("Cannot use a Null Session");
                else
                    return false;
            }
        }

        private IList getListFromQuery(IQuery query, Hashtable parameters, Hashtable parameterLists, int? maximumRows, int? startRowIndex)
        {
            IList list = null;

            try
            {
                if (!(session.IsConnected)) session.Reconnect();

                if (query == null)
                    throw new ApplicationException("Query can not be null. Contact your system administrator.");

                if (parameters != null && parameters.Count > 0)
                    foreach (DictionaryEntry param in parameters)
                        query.SetParameter((String)param.Key, param.Value);

                if (parameterLists != null && parameterLists.Count > 0)
                    foreach (DictionaryEntry param in parameterLists)
                        query.SetParameterList((String)param.Key, (ICollection)param.Value);

                if (maximumRows != null && maximumRows > 0)
                {
                    query.SetMaxResults((int)maximumRows);
                    if (startRowIndex != null && startRowIndex >= 0)
                        query.SetFirstResult((int)startRowIndex);
                }

                if (TimeOut != null)
                    query.SetTimeout(TimeOut.Value);

                //if (LockMode == LockModes.None)
                //    query.SetLockMode("myAlias", NHibernate.LockMode.None);

                list = query.List();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while retrieving data.", ex);
            }
            return list;
        }

        #endregion


        #region Private Variables

        private ISession session;
        private DalPageInfo pageInfo;
        private bool inTrans = false;
        private int? timeOut;
        private LockModes lockMode = LockModes.Normal;

        private Dictionary<ActionType, Action<object>> actionToMethod;

        #endregion
    }
}
