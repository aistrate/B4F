using System;
using Microsoft.ReportingServices.DataProcessing;

namespace B4F.ReportingServices.DataExtensions
{
    /// <summary>
    /// Represents a database transaction.
    /// </summary>
    public class DataSetTransaction : IDbTransaction
    {
        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public IDbConnection Connection
        {
            get { return this.Connection; }
        }

        public void Dispose()
        {
        }
    }
}
