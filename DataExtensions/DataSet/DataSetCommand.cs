using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.ReportingServices.DataProcessing;

namespace B4F.ReportingServices.DataExtensions
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public class DataSetCommand : IDbCommand, IDbCommandAnalysis
    {
        DataSetConnection connection;
        DataSetTransaction transaction;
        string commandText;
        DataSetDataParameterCollection parameters = new DataSetDataParameterCollection();

        public DataSetCommand()
        {
        }

        public DataSetCommand(DataSetConnection connection)
        {
            this.connection = connection;
        }

        public DataSetCommand(string cmdText)
        {
            this.commandText = cmdText;
        }

        public DataSetCommand(string cmdText, DataSetConnection connection)
        {
            this.commandText = cmdText;
            this.connection = connection;
        }

        public DataSetCommand(string cmdText, DataSetConnection connection, DataSetTransaction transaction)
        {
            this.commandText = cmdText;
            this.connection = connection;
            this.transaction = transaction;
        }

        #region IDbCommandAnalysis Members

        /// <summary>
        /// GetParameters is called by the Report Designer.
        /// </summary>
        /// <returns>List of parameters used in the query.</returns>
        public IDataParameterCollection GetParameters()
        {
            DataSetDataParameterCollection parameters = new DataSetDataParameterCollection();
            // inform the designer that @DataSource parameter is needed
            parameters.Add(new DataSetDataParameter(Util.DataSourceParameterName, this.connection.ConnectionString));
            return parameters;
        }

        #endregion

        #region IDbCommand Members

        public string CommandText
        {
            get { return this.commandText; }
            set { this.commandText = value; }
        }

        /// <summary>
        /// Time-out not supported.
        /// </summary>
        public int CommandTimeout
        {
            get { return 30; }
            set { }
        }

        /// <summary>
        /// Only CommandType.Text is supported.
        /// </summary>
        public CommandType CommandType
        {
            get { return CommandType.Text; }
            set { if (value != CommandType.Text) throw new NotSupportedException(); }
        }

        public DataSetDataParameterCollection Parameters
        {
            get { return this.parameters; }
        }

        IDataParameterCollection IDbCommand.Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Set the transaction. Consider additional steps to ensure that the transaction
        /// is compatible with the connection, because the two are usually linked.
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return this.transaction; }
            set { this.transaction = (DataSetTransaction)value; }
        }

        /// <summary>
        /// Canceling a Command not supported once it has been initiated.
        /// </summary>
        public void Cancel()
        {
            throw new NotSupportedException();
        }

        public IDataParameter CreateParameter()
        {
            return (IDataParameter)(new DataSetDataParameter());
        }

        /// <summary>
        /// Retrieve results from the data source and return a DataReader that allows the user to process the results.
        /// </summary>
        public IDataReader ExecuteReader()
        {
            // parameter not used
            return ExecuteReader(CommandBehavior.SingleResult);
        }

        /// <summary>
        /// Retrieve results from the data source and return a DataReader that allows the user to process the results.
        /// </summary>
        /// <param name="behavior">Not used.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            Trace.WriteLine("ExecuteReader");

            // There must be a valid and open connection.
            if (this.connection == null || this.connection.State != System.Data.ConnectionState.Open)
                throw new InvalidOperationException("Connection must be valid and open.");

            // Execute the command.      
            DataSetDataReader reader = new DataSetDataReader(this.connection, this.commandText, this.parameters);
            reader.LoadDataSet();

            return reader;
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
