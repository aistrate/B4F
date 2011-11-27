using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.Dal
{
	internal class HibernateListPropertyTable : PropertyTable
	{
		Hashtable propertyIndexes = new Hashtable();
		Type[] propertyDataTypes;

        public override void LoadProperties(object obj)
		{
            base.LoadProperties(obj);

            object[] objectArray = (object[])obj;

			if (PropertyNames.Length > objectArray.Length)
				throw new ApplicationException(
					"DataSetBuilder (HibernateListPropertyTable): parameter propertyList contains more properties than the first object in objectList");

			propertyDataTypes = new Type[PropertyNames.Length];

			int i = 0;
			foreach (string propertyName in PropertyNames)
			{
				propertyIndexes[propertyName] = i;
                if (objectArray[i] != null)
                    propertyDataTypes[i] = objectArray[i].GetType();
                else
                    propertyDataTypes[i] = typeof(string);
				i++;
			}
		}

        protected override Type GetRealPropertyDataType(string propertyName)
		{
			return propertyDataTypes[(int)propertyIndexes[propertyName]];
		}

		public override object GetPropertyValue(object obj, string propertyName)
		{
			return ((object[])obj)[(int)propertyIndexes[propertyName]];
		}
	}
}
