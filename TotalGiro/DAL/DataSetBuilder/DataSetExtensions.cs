using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace B4F.TotalGiro.Dal
{
    public static class DataSetBuilderExtensions
    {
        #region ToDataSet extensions for IEnumerable<T> that delegate to DataSetBuilder

        public static DataSet ToDataSet<T>(this IEnumerable<T> source, string propertyList)
        {
            return DataSetBuilder.CreateDataSetFromBusinessObjectList(toList(source), propertyList);
        }

        public static DataSet ToDataSet<T>(this IEnumerable<T> source, string propertyList, string dataTableName)
        {
            return DataSetBuilder.CreateDataSetFromBusinessObjectList(toList(source), propertyList, dataTableName);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> source, string propertyList, string dataTableName)
        {
            return DataSetBuilder.CreateDataTableFromBusinessObjectList(toList(source), propertyList, dataTableName);
        }

        private static List<T> toList<T>(IEnumerable<T> source)
        {
            return source is List<T> ? (List<T>)source : source.ToList();
        }

        #endregion


        #region ToDataSet extensions for IEnumerable<T> that do NOT use DataSetBuilder

        public static DataSet ToDataSet<T>(this IEnumerable<T> source)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(source.ToDataTable("Results"));
            return ds;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> source, string dataTableName)
        {
            Type objType = (typeof(T) == typeof(object) && source.Count() > 0) ? source.First().GetType() : typeof(T);

            DataTable dt = new DataTable(dataTableName);
            PropertyInfo[] properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | 
                                                              BindingFlags.GetProperty);

            foreach (PropertyInfo property in properties)
                dt.Columns.Add(property.Name, property.PropertyType);

            foreach (T businessObject in source)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);

                foreach (PropertyInfo property in properties)
                    dr[property.Name] = property.GetValue(businessObject, null);
            }

            return dt;
        }

        #endregion


        #region ToDataSetFromBusinessObjectList extensions for IList

        public static DataSet ToDataSetFromBusinessObjectList(this IList source, string propertyList)
        {
            return DataSetBuilder.CreateDataSetFromBusinessObjectList(source, propertyList);
        }

        public static DataSet ToDataSetFromBusinessObjectList(this IList source, string propertyList, string dataTableName)
        {
            return DataSetBuilder.CreateDataSetFromBusinessObjectList(source, propertyList, dataTableName);
        }

        public static DataTable ToDataTableFromBusinessObjectList(this IList source, string propertyList, string dataTableName)
        {
            return DataSetBuilder.CreateDataTableFromBusinessObjectList(source, propertyList, dataTableName);
        }

        #endregion


        #region ToDataSetFromHibernateList extensions for IList

        public static DataSet ToDataSetFromHibernateList(this IList source, string propertyList)
        {
            return DataSetBuilder.CreateDataSetFromHibernateList(source, propertyList);
        }

        public static DataSet ToDataSetFromHibernateList(this IList source, string propertyList, string dataTableName)
        {
            return DataSetBuilder.CreateDataSetFromHibernateList(source, propertyList, dataTableName);
        }

        public static DataTable ToDataTableFromHibernateList(this IList source, string propertyList, string dataTableName)
        {
            return DataSetBuilder.CreateDataTableFromHibernateList(source, propertyList, dataTableName);
        }

        #endregion


        #region Extensions for class DataSet

        public static DataSet AddEmptyFirstRow(this DataSet ds)
        {
            return ds.AddEmptyFirstRow("Key");
        }

        public static DataSet AddEmptyFirstRow(this DataSet ds, string keyFieldName)
        {
            DataTable dt = ds.Tables[0];
            int keyFieldIndex = dt.Columns.IndexOf(keyFieldName);

            DataRow row = dt.NewRow();
            row[keyFieldIndex] = int.MinValue;
            dt.Rows.InsertAt(row, 0);

            return ds;
        }

        #endregion
    }
}
