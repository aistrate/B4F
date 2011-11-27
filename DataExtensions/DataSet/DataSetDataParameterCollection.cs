using System;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using Microsoft.ReportingServices.DataProcessing;

namespace B4F.ReportingServices.DataExtensions
{
    /// <summary>
    /// Represents a collection of query parameters.
    /// </summary>
    /// <remarks>
    /// Because IDataParameterCollection is primarily an IList,
    /// we can use an existing class for most of the implementation, i.e. ArrayList.
    /// </remarks>
    public class DataSetDataParameterCollection : ArrayList, IDataParameterCollection
    {
        public object this[string index]
        {

            get { return this[IndexOf(index)]; }
            set { this[IndexOf(index)] = value; }
        }

        /// <summary>
        /// Helper wrapper to get a parameter by name.
        /// </summary>
        public DataSetDataParameter GetByName(string parameterName)
        {
            DataSetDataParameter parameter = null;
            IEnumerator enumerator = this.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                DataSetDataParameter tempParameter = (DataSetDataParameter)enumerator.Current;
                if (tempParameter.ParameterName == parameterName)
                {
                    parameter = tempParameter;
                    break;
                }
            }

            return parameter;
        }

        public int Add(IDataParameter value)
        {
            Trace.WriteLine("DataSet Extension: Add(IDataParameter value).");
            
            if (((DataSetDataParameter)value).ParameterName != null)
                return base.Add(value);
            else
                throw new ArgumentException("Parameter must be named.");
        }

        public override int Add(object value)
        {
            Trace.WriteLine("Add parameter value " + value.ToString());
            
            return Add((IDataParameter)value);
        }
    }
}
