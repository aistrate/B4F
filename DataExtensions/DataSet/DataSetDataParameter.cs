using System;
using Microsoft.ReportingServices.DataProcessing;

namespace B4F.ReportingServices.DataExtensions
{
    /// <summary>
    /// Represents a command parameter.
    /// </summary>
    public class DataSetDataParameter : IDataParameter
    {
        string parameterName;
        object value;

        public DataSetDataParameter()
        {
        }

        public DataSetDataParameter(string parameterName, object value)
        {
            this.parameterName = parameterName;
            this.Value = value;
        }

        public string ParameterName
        {
            get { return this.parameterName; }
            set { this.parameterName = value; }
        }

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
