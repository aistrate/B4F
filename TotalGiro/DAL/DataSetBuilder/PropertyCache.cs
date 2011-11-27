using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace B4F.TotalGiro.Dal
{
	internal class PropertyCache
	{
		Hashtable cache = new Hashtable();

		public PropertyInfo GetPropertyInfo(Type type, string propertyName)
		{
			Hashtable table = ((Hashtable)cache[type]);

			if (table == null)
				table = addTypeEntry(type);

			PropertyInfo pi = (PropertyInfo)table[propertyName];

			if (pi == null)
				throw new ApplicationException(string.Format("DataSetBuilder (PropertyCache): property {0} does not exist on type {1}",
					propertyName, type));

			return pi;
		}

		private Hashtable addTypeEntry(Type type)
		{
			lock (cache)
			{
				Hashtable table = new Hashtable();

				addMembersToTypeEntry(table, type);

				// If 'type' is an interface, properties of its parent interfaces need to be added separately.
				// If a class, properties implementing interfaces need to be replaced in 'table' by properties of those interfaces, 
				// so that heterogenous lists (of business objects) can be read.
				foreach (Type parentInterface in type.GetInterfaces())
					addMembersToTypeEntry(table, parentInterface);

				// For each 'override' property, replace it with the property of the base class where originally declared (as 'abstract' or 'virtual')
				string[] keys = new string[table.Keys.Count];
				table.Keys.CopyTo(keys, 0);
				foreach (string key in keys)
				{
					PropertyInfo propertyInfo = (PropertyInfo)table[key];
					if (propertyInfo.DeclaringType.IsClass)
					{
						MethodInfo getMethod = propertyInfo.GetGetMethod();

                        if (getMethod != null)      // null means write-only property
                        {
                            // check if it's an override:
                            if (getMethod.IsVirtual &&      // declared as 'abstract', 'virtual' or 'override' ...
                               (getMethod.Attributes & MethodAttributes.VtableLayoutMask) != MethodAttributes.NewSlot && // ... but NOT as 'new' and/or 'virtual';
                               propertyInfo.DeclaringType.BaseType != typeof(object))       // if base type is System.Object then it cannot be an 'override'
                                table[key] = GetPropertyInfo(propertyInfo.DeclaringType.BaseType, key);
                        }
					}
				}

				cache[type] = table;
				return table;
			}
		}

		private void addMembersToTypeEntry(Hashtable table, Type type)
		{
			PropertyInfo[] propertyInfoList = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetProperty);

			foreach (PropertyInfo propertyInfo in propertyInfoList)
				table[propertyInfo.Name] = propertyInfo;
		}
	}
}
