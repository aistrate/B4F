using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.Dal
{
	internal abstract class PropertyTable
	{
		// class variables
		private const char separator = ',';
		private static char[] separators = new char[] { separator };
		private const int maxPropertyCount = 100;

		// instance variables
		private string[] propertyNames = new string[maxPropertyCount];
        private bool isObjectListEmpty = true;

        public void LoadPropertyList(string propertyList)
		{
			// Split propertyList
            propertyList = propertyList.Replace(" ", "").Replace("\r\n", "");   // eliminate spaces and new line characters
			propertyNames = propertyList.Split(separators, maxPropertyCount, StringSplitOptions.RemoveEmptyEntries);

			// Check propertyList
			if (propertyNames.Length == 0)
				throw new ApplicationException("DataSetBuilder (PropertyTable): parameter propertyList is empty");
			if (propertyNames[propertyNames.Length - 1].Contains(separator.ToString()))
				throw new ApplicationException("DataSetBuilder (PropertyTable): parameter propertyList contains too many elements (" +
					maxPropertyCount + " allowed)");

			// Check for duplicated properties
			Hashtable t = new Hashtable();
			foreach (string propertyName in propertyNames)
				if (!t.Contains(propertyName))
					t[propertyName] = null;
				else
					throw new ApplicationException(string.Format(
						"DataSetBuilder: parameter propertyList contains duplicated property '{0}'", propertyName));
		}

        public virtual void LoadProperties(object obj)
        {
            if (obj == null)
                throw new ApplicationException("DataSetBuilder (PropertyTable): parameter objectList contains null references");

            isObjectListEmpty = false;
        }

        public string[] PropertyNames
		{
			get { return propertyNames; }
		}

        public Type GetPropertyDataType(string propertyName)
        {
            if (!isObjectListEmpty)
                return GetRealPropertyDataType(propertyName);
            else
                return typeof(string);
        }

        protected abstract Type GetRealPropertyDataType(string propertyName);

		public abstract object GetPropertyValue(object obj, string propertyName);
	}
}
