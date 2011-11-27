using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Reflection;

namespace B4F.TotalGiro.Dal
{
	/// <summary>
    /// Class that creates a <b>DataSet</b> (both schema and data) from a list of objects.
	/// </summary>
    public class DataSetBuilder
	{
		private IList objectList;
		private string propertyList;
		private PropertyTable propertyTable;
        private DataTable dataTable = new DataTable();
        private const string defaultDataTableName = "Results";

		/// <summary>
        /// Creates a <b>DataSet</b> (both schema and data) from a list of <i>business objects</i>, that is, 
        /// objects from the Business Layer (orders, accounts, instruments etc.).
		/// </summary>
        /// <param name="objectList">A list of <i>business objects</i>, that is, 
        /// objects from the Business Layer (orders, accounts, instruments etc.).</param>
        /// <param name="propertyList">A list of property names, which will become table columns in the <b>DataSet</b>.</param>
        /// <returns>A <b>DataSet</b> containing a table with one column for each property in <i>propertyList</i>,
        /// and one row for each business object in <i>objectList</i>. 
        /// If <i>objectList</i> is empty, the table will have no rows and no columns.</returns>
        /// <remarks>
        /// All properties in <i>propertyList</i> should be available on every object in the business object list.
        /// A property name should represent the <i>same</i> property across all business objects, even if two objects may belong to 
        /// two different classes deriving from the same root class, or implementing a common interface -- 
        /// in those cases the property should belong the the root class, or to the common interface, respectively.
        /// Properties can be <i>nested</i>, e.g. <i>Value.Quantity</i> for an <i>objectList</i> that contains account objects.<br/><br/>
        /// For property names that contain dots (nested property names), the corresponding column names will be formed by replacing 
        /// the dots with underscore characters (e.g. property <i>Value.Quantity</i> becomes column <i>Value_Quantity</i> in the <b>DataSet</b>).
        /// </remarks>
		public static DataSet CreateDataSetFromBusinessObjectList(IList objectList, string propertyList)
		{
            return CreateDataSetFromBusinessObjectList(objectList, propertyList, defaultDataTableName);
		}

        /// <summary>
        /// Creates a <b>DataSet</b> (both schema and data) from a list of <i>business objects</i>, that is, 
        /// objects from the Business Layer (orders, accounts, instruments etc.).
        /// </summary>
        /// <param name="objectList">A list of <i>business objects</i>, that is, 
        /// objects from the Business Layer (orders, accounts, instruments etc.).</param>
        /// <param name="propertyList">A list of property names, which will become table columns in the <b>DataSet</b>.</param>
        /// <param name="dataTableName">The name of the first (and only) <b>DataTable</b> in the returned <b>DataSet</b>.</param>
        /// <returns>A <b>DataSet</b> containing a table with one column for each property in <i>propertyList</i>,
        /// and one row for each business object in <i>objectList</i>. 
        /// If <i>objectList</i> is empty, the table will have no rows and no columns.</returns>
        /// <remarks>
        /// All properties in <i>propertyList</i> should be available on every object in the business object list.
        /// A property name should represent the <i>same</i> property across all business objects, even if two objects may belong to 
        /// two different classes deriving from the same root class, or implementing a common interface -- 
        /// in those cases the property should belong the the root class, or to the common interface, respectively.
        /// Properties can be <i>nested</i>, e.g. <i>Value.Quantity</i> for an <i>objectList</i> that contains account objects.<br/><br/>
        /// For property names that contain dots (nested property names), the corresponding column names will be formed by replacing 
        /// the dots with underscore characters (e.g. property <i>Value.Quantity</i> becomes column <i>Value_Quantity</i> in the <b>DataSet</b>).
        /// </remarks>
        public static DataSet CreateDataSetFromBusinessObjectList(IList objectList, string propertyList, string dataTableName)
        {
            return createDataSet(CreateDataTableFromBusinessObjectList(objectList, propertyList, dataTableName));
        }

        /// <summary>
        /// Creates a <b>DataTable</b> (both schema and data) from a list of <i>business objects</i>, that is, 
        /// objects from the Business Layer (orders, accounts, instruments etc.).
        /// </summary>
        /// <param name="objectList">A list of <i>business objects</i>, that is, 
        /// objects from the Business Layer (orders, accounts, instruments etc.).</param>
        /// <param name="propertyList">A list of property names, which will become columns in the <b>DataTable</b>.</param>
        /// <param name="dataTableName">The name of the <b>DataTable</b>.</param>
        /// <returns>A <b>DataTable</b> with one column for each property in <i>propertyList</i>,
        /// and one row for each business object in <i>objectList</i>. 
        /// If <i>objectList</i> is empty, the table will have no rows and no columns.</returns>
        /// <remarks>
        /// All properties in <i>propertyList</i> should be available on every object in the business object list.
        /// A property name should represent the <i>same</i> property across all business objects, even if two objects may belong to 
        /// two different classes deriving from the same root class, or implementing a common interface -- 
        /// in those cases the property should belong the the root class, or to the common interface, respectively.
        /// Properties can be <i>nested</i>, e.g. <i>Value.Quantity</i> for an <i>objectList</i> that contains account objects.<br/><br/>
        /// For property names that contain dots (nested property names), the corresponding column names will be formed by replacing 
        /// the dots with underscore characters (e.g. property <i>Value.Quantity</i> becomes column <i>Value_Quantity</i> in the <b>DataTable</b>).
        /// </remarks>
        public static DataTable CreateDataTableFromBusinessObjectList(IList objectList, string propertyList, string dataTableName)
        {
            return (new DataSetBuilder(objectList, propertyList, new BusinessObjectListPropertyTable(), dataTableName)).dataTable;
        }

        private static DataSet createDataSet(DataTable dataTable)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dataTable);
            return ds;
        }

        /// <summary>
        /// Creates a <b>DataSet</b> (both schema and data) from a <i>Hibernate list</i>, which is a list of lists of primitive types (string,
        /// int, DateTime, etc.). Such a list is typically the result of calling 
        /// <see cref="B4F.TotalGiro.Dal.NHSession.GetList(System.String)">NHSession.GetList()</see> with an 
        /// HQL (Hibernate Query Language) query string as the first parameter.
		/// </summary>
        /// <param name="objectList">A <i>Hibernate list</i>, which is a list of lists of primitive types (string, int, DateTime, etc.).</param>
        /// <param name="propertyList">A list of property names, which will become table columns in the <b>DataSet</b>.</param>
        /// <returns>A <b>DataSet</b> containing a table with one column for each property in <i>propertyList</i>,
        /// and one row for each secondary-level list in <i>objectList</i>. 
        /// If <i>objectList</i> is empty, the table will have no rows and no columns.</returns>
        /// <remarks>
        /// A <i>Hibernate list</i>, or list of lists of primitive-type values, is the HQL (Hibernate Query Language) equivalent of an SQL result set. 
        /// Every list in the primary-level list represents a row in the record set, and the primitive-type values are field values 
        /// inside the row.<br/><br/>
        /// The property names in <i>propertyList</i> are just names that will be assigned to the columns in the <b>DataSet</b> 
        /// (they don't have to exist as properties in a class). 
        /// The maximum number of property names in <i>propertyList</i> is the number of primitive-type values in 
        /// secondary-level lists in <i>objectList</i>.
        /// </remarks>
		public static DataSet CreateDataSetFromHibernateList(IList objectList, string propertyList)
		{
            return CreateDataSetFromHibernateList(objectList, propertyList, defaultDataTableName);
		}

        /// <summary>
        /// Creates a <b>DataSet</b> (both schema and data) from a <i>Hibernate list</i>, which is a list of lists of primitive types (string,
        /// int, DateTime, etc.). Such a list is typically the result of calling 
        /// <see cref="B4F.TotalGiro.Dal.NHSession.GetList(System.String)">NHSession.GetList()</see> with an 
        /// HQL (Hibernate Query Language) query string as the first parameter.
        /// </summary>
        /// <param name="objectList">A <i>Hibernate list</i>, which is a list of lists of primitive types (string, int, DateTime, etc.).</param>
        /// <param name="propertyList">A list of property names, which will become table columns in the <b>DataSet</b>.</param>
        /// <param name="dataTableName">The name of the first (and only) <b>DataTable</b> in the returned <b>DataSet</b>.</param>
        /// <returns>A <b>DataSet</b> containing a table with one column for each property in <i>propertyList</i>,
        /// and one row for each secondary-level list in <i>objectList</i>. 
        /// If <i>objectList</i> is empty, the table will have no rows and no columns.</returns>
        /// <remarks>
        /// A <i>Hibernate list</i>, or list of lists of primitive-type values, is the HQL (Hibernate Query Language) equivalent of an SQL result set. 
        /// Every list in the primary-level list represents a row in the record set, and the primitive-type values are field values 
        /// inside the row.<br/><br/>
        /// The property names in <i>propertyList</i> are just names that will be assigned to the columns in the <b>DataSet</b> 
        /// (they don't have to exist as properties in a class). 
        /// The maximum number of property names in <i>propertyList</i> is the number of primitive-type values in 
        /// secondary-level lists in <i>objectList</i>.
        /// </remarks>
        public static DataSet CreateDataSetFromHibernateList(IList objectList, string propertyList, string dataTableName)
        {
            return createDataSet(CreateDataTableFromHibernateList(objectList, propertyList, dataTableName));
        }

        /// <summary>
        /// Creates a <b>DataTable</b> (both schema and data) from a <i>Hibernate list</i>, which is a list of lists of primitive types (string,
        /// int, DateTime, etc.). Such a list is typically the result of calling 
        /// <see cref="B4F.TotalGiro.Dal.NHSession.GetList(System.String)">NHSession.GetList()</see> with an 
        /// HQL (Hibernate Query Language) query string as the first parameter.
        /// </summary>
        /// <param name="objectList">A <i>Hibernate list</i>, which is a list of lists of primitive types (string, int, DateTime, etc.).</param>
        /// <param name="propertyList">A list of property names, which will become columns in the <b>DataTable</b>.</param>
        /// <param name="dataTableName">The name of the <b>DataTable</b>.</param>
        /// <returns>A <b>DataTable</b> with one column for each property in <i>propertyList</i>,
        /// and one row for each secondary-level list in <i>objectList</i>. 
        /// If <i>objectList</i> is empty, the table will have no rows and no columns.</returns>
        /// <remarks>
        /// A <i>Hibernate list</i>, or list of lists of primitive-type values, is the HQL (Hibernate Query Language) equivalent of an SQL result set. 
        /// Every list in the primary-level list represents a row in the record set, and the primitive-type values are field values 
        /// inside the row.<br/><br/>
        /// The property names in <i>propertyList</i> are just names that will be assigned to the columns in the <b>DataTable</b> 
        /// (they don't have to exist as properties in a class). 
        /// The maximum number of property names in <i>propertyList</i> is the number of primitive-type values in 
        /// secondary-level lists in <i>objectList</i>.
        /// </remarks>
        public static DataTable CreateDataTableFromHibernateList(IList objectList, string propertyList, string dataTableName)
        {
            if (objectList.Count > 0 && (objectList[0] as object[]) == null)
            {
                object[] newObjectList = new object[objectList.Count];
                int i = 0;
                foreach (object obj in objectList)
                    newObjectList[i++] = new object[] { obj };
                objectList = newObjectList;
            }
            return (new DataSetBuilder(objectList, propertyList, new HibernateListPropertyTable(), dataTableName)).dataTable;
        }

        public static void MergeDataTableWithBusinessObjectList(DataTable dt, IList objectList, string keyPropertyName, string propertyList)
        {
            mergeDataTablesByKey(dt, CreateDataTableFromBusinessObjectList(objectList, propertyList, "Source"), keyPropertyName);
        }

        public static void MergeDataTableWithHibernateList(DataTable dt, IList objectList, string keyPropertyName, string propertyList)
        {
            mergeDataTablesByKey(dt, CreateDataTableFromHibernateList(objectList, propertyList, "Source"), keyPropertyName);
        }

        private static void mergeDataTablesByKey(DataTable destTable, DataTable sourceTable, string keyPropertyName)
        {
            setPrimaryKey(destTable, keyPropertyName);
            setPrimaryKey(sourceTable, keyPropertyName);
            
            if (sourceTable.Rows.Count == 0)
                // by default, columns are created as type 'string'
                sourceTable.PrimaryKey[0].DataType = destTable.PrimaryKey[0].DataType;

            destTable.Merge(sourceTable, false, MissingSchemaAction.Add);
        }

        private static void setPrimaryKey(DataTable dt, string keyPropertyName)
        {
            if (!dt.Columns.Contains(keyPropertyName))
                throw new ApplicationException(
                    "DataSetBuilder: could not merge DataTables because the specified key property does not appear in the property list.");

            DataColumn[] keys = new DataColumn[1];
            keys[0] = dt.Columns[keyPropertyName];
            dt.PrimaryKey = keys;
        }

		// Constructor
        private DataSetBuilder(IList objectList, string propertyList, PropertyTable propertyTable, string dataTableName)
		{
			this.objectList = objectList;
			this.propertyList = propertyList;
			this.propertyTable = propertyTable;
            this.dataTable.TableName = dataTableName;
            
            loadSchema();
			loadData();

			dotsToUnderscore();
		}

        private void loadSchema()
		{
            propertyTable.LoadPropertyList(propertyList);

            // Create a table of properties
            if (objectList != null && objectList.Count > 0)
                propertyTable.LoadProperties(objectList[0]);
            
            // Add DataColumns
            foreach (string propertyName in propertyTable.PropertyNames)
                dataTable.Columns.Add(propertyName, propertyTable.GetPropertyDataType(propertyName));
		}

		private void loadData()
		{
            if (objectList != null && objectList.Count > 0)
			{
				foreach (object obj in objectList)
				{
					if (obj != null)
					{
                        DataRow row = dataTable.NewRow();
                        foreach (DataColumn col in dataTable.Columns)
							row[col] = propertyTable.GetPropertyValue(obj, col.ColumnName);

                        dataTable.Rows.Add(row);
					}
					else
						throw new ApplicationException("DataSetBuilder: parameter objectList contains null references");
				}
			}
		}

		private void dotsToUnderscore()
		{
			foreach (DataColumn col in dataTable.Columns)
				col.ColumnName = col.ColumnName.Replace('.', '_');
		}

        public static IList GetDistinctList(IList list)
        {
            ArrayList l = new ArrayList();
            if (list == null || list.Count <= 1)
                return list;
            else
            {
                foreach (object item in list)
                {
                    if (!l.Contains(item))
                        l.Add(item);
                }
                return l;
            }
        }
	}
}
