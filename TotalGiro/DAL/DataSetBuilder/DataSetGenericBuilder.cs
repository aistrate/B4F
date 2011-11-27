using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace B4F.TotalGiro.Dal
{
	/// <summary>
    /// Class that creates a <b>DataSet</b> (both schema and data) from a list of objects.
	/// </summary>
    public class DataSetGenericBuilder<T>
	{
        private IList<T> objectList;
        private string propertyList;
		private PropertyTable propertyTable;
		private DataSet dataSet = new DataSet();

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
		public static DataSet CreateDataSetFromBusinessObjectList(IList<T> objectList, string propertyList)
		{
			return (new DataSetGenericBuilder<T>(objectList, propertyList, new BusinessObjectListPropertyTable())).dataSet;
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
		public static DataSet CreateDataSetFromHibernateList(IList<T> objectList, string propertyList)
		{
			return (new DataSetGenericBuilder<T>(objectList, propertyList, new HibernateListPropertyTable())).dataSet;
		}

		// Constructor
        private DataSetGenericBuilder(IList<T> objectList, string propertyList, PropertyTable propertyTable)
		{
			this.objectList = objectList;
			this.propertyList = propertyList;
			this.propertyTable = propertyTable;

			loadSchema();
			loadData();

			dotsToUnderscore();
		}

		private void loadSchema()
		{
			// Add DataTable
			DataTable dt = new DataTable("Results");
			dataSet.Tables.Add(dt);

            propertyTable.LoadPropertyList(propertyList);

            // Create a table of properties
            if (objectList != null && objectList.Count > 0)
                propertyTable.LoadProperties(objectList[0]);
            
            // Add DataColumns
            foreach (string propertyName in propertyTable.PropertyNames)
                dt.Columns.Add(propertyName, propertyTable.GetPropertyDataType(propertyName));
		}

		private void loadData()
		{
			DataTable dt = dataSet.Tables[0];

            if (objectList != null && objectList.Count > 0)
			{
				foreach (object obj in objectList)
				{
					if (obj != null)
					{
						DataRow row = dt.NewRow();
						foreach (DataColumn col in dt.Columns)
							row[col] = propertyTable.GetPropertyValue(obj, col.ColumnName);

						dt.Rows.Add(row);
					}
					else
						throw new ApplicationException("DataSetBuilder: parameter objectList contains null references");
				}
			}
		}

		private void dotsToUnderscore()
		{
			foreach (DataColumn col in dataSet.Tables[0].Columns)
				col.ColumnName = col.ColumnName.Replace('.', '_');
		}
	}
}
