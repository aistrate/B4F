using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.ReportingServices.DataProcessing;

namespace B4F.ReportingServices.DataExtensions
{
    /// <summary>
    /// Represents a data reader.
    /// </summary>
    public class DataSetDataReader : Microsoft.ReportingServices.DataProcessing.IDataReader
    {
        private DataSetConnection connection = null;
        private string commandText = null;
        private DataSetDataParameterCollection parameters = null;
        private DataSet dataSet = null;
        private DataTable dataTable = null;
        internal IEnumerator enumerator;

        // Because the user should not be able to directly create a 
        // DataReader object, the constructors are marked as internal.
        internal DataSetDataReader()
        {
        }

        internal DataSetDataReader(string cmdText)
        {
            this.commandText = cmdText;
        }

        internal DataSetDataReader(DataSetConnection connection, string cmdText)
        {
            this.connection = connection;
            this.commandText = cmdText;
        }

        internal DataSetDataReader(DataSetConnection connection, string cmdText, DataSetDataParameterCollection parameters)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("DataSetConnection: {0}, cmdText: {1}, WsParameterCollection: {2}", connection.ConnectionString, cmdText, parameters.Count.ToString()));
            this.connection = connection;
            this.commandText = cmdText;
            this.parameters = parameters;
        }

        /// <summary>
        /// Loads the DataSet.
        /// </summary>
        /// <remarks>
        /// During design time (query and layout tab) returns an empty DataSet.
        /// During runtime, it binds to whatever is passed to the @DataSource parameter.
        /// </remarks>
        internal void LoadDataSet()
        {
            string dataSource = null;

            // attempt to get the DataSource parameter
            DataSetDataParameter parameter = this.parameters.GetByName(Util.DataSourceParameterName) as DataSetDataParameter;
            Debug.Assert(parameter != null, "DataSource parameter not specified.");
            if (parameter == null)
                throw new ArgumentException("DataSource parameter not specified.");
            
            dataSource = parameter.Value.ToString();
            Debug.Assert(dataSource != null, "DataSource parameter not specified.");
            if (dataSource == null)
                throw new ArgumentException("DataSource parameter not specified.");

            // load the DataSet
            this.dataSet = GetDataSet(dataSource);

            Trace.WriteLine(string.Format("Referencing DataSet table: {0}", 
                (this.commandText.Trim() != string.Empty ? this.commandText.Trim() : "<first>")));

            // get the DataSet table
            if (this.commandText.Trim() == "")
                this.dataTable = this.dataSet.Tables[0];
            else
            {
                // get the specified table
                Debug.Assert(this.dataSet.Tables[this.commandText] != null, string.Format("Data table {0} not found.", this.commandText));
                if (this.dataSet.Tables[this.commandText] == null)
                    throw new ArgumentException(string.Format("Data table {0} not found.", this.commandText));
                this.dataTable = this.dataSet.Tables[this.commandText];
            }

            // set the row enumerator
            this.enumerator = this.dataTable.Rows.GetEnumerator();
        }

        /// <summary>
        /// Loads the DataSet from its XML-serialized copy or file.
        /// </summary>
        /// <param name="dataSource">The XML-serialized copy or file path.</param>
        private DataSet GetDataSet(string dataSource)
        {
            DataSet dataSet = new DataSet();

            // is this XML or file path?
            if (dataSource.IndexOf("<") >= 0)
            {
                // XML
                StringReader reader = new StringReader(dataSource);
                dataSet.ReadXml(reader);
            }
            else
            {
                // file path
                FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Read, dataSource);
                permission.Assert();

                dataSet.ReadXml(dataSource);
            }

            return dataSet;
        }

        #region IDataReader Members

        /// <summary>
        /// Returns the number of columns in the table.
        /// </summary>
        public int FieldCount
        {
            get { return (this.dataTable.Columns.Count); }
        }

        /// <summary>
        /// Called repeatedly when the report is processed for each row.
        /// </summary>
        /// <returns>A value of true if the enumerator was successfully advanced to the next record,
        /// or a value of false if the enumerator has passed the end of the result set.</returns>
        public bool Read()
        {
            if (this.enumerator != null)
                return this.enumerator.MoveNext();
            
            return false;
        }

        /// <summary>
        /// Returns the name of the column.
        /// </summary>
        public string GetName(int fieldIndex)
        {
            return (this.dataTable.Columns[fieldIndex].ColumnName);
        }

        /// <summary>
        /// Returns the column type.
        /// </summary>
        public Type GetFieldType(int fieldIndex)
        {
            return (this.dataTable.Columns[fieldIndex].DataType);
        }

        /// <summary>
        /// Returns the column value.
        /// </summary>
        public Object GetValue(int fieldIndex)
        {
            return ((DataRow)this.enumerator.Current)[fieldIndex];
        }

        /// <summary>
        /// Returns the column ordinal position.
        /// </summary>
        /// <returns>-1 if not found.</returns>
        public int GetOrdinal(string fieldName)
        {
            return this.dataTable.Columns[fieldName].Ordinal;
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
