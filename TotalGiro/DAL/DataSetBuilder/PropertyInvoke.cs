using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace B4F.TotalGiro.Dal
{
	internal class PropertyInvoke
	{
		private const string levelSeparator = ".";
		private static PropertyCache propertyCache = new PropertyCache();

		private PropertyInfo propertyInfo;
		private PropertyInvoke subProperty;

		public PropertyInfo PropertyInfo
		{
			get { return propertyInfo; }
		}

		public PropertyInvoke SubProperty
		{
			get { return subProperty; }
		}

		public Type FinalType
		{
			get { return (subProperty == null ? propertyInfo.PropertyType : subProperty.FinalType); }
		}

		// constructor
		public PropertyInvoke(PropertyInfo propertyInfo, PropertyInvoke subProperty)
		{
			this.propertyInfo = propertyInfo;
			this.subProperty = subProperty;
		}

		// invocation method
		public object Invoke(object obj)
		{
			object value;

			try
			{
				value = propertyInfo.GetValue(obj, null);
			}
            catch (Exception ex)
			{
				throw new ApplicationException(string.Format(
					"[PropertyInvoke] Could not call property {0}.{1} on type {2}. {3}",
					propertyInfo.DeclaringType.Name, propertyInfo.Name, obj.GetType().Name, ex.Message),
                    ex.InnerException);
			}

			if (subProperty == null)
				return value;
			else
			{
                if (value != null)
                    return subProperty.Invoke(value);
                else
                    return System.DBNull.Value;
			}
		}

		public static PropertyInvoke CreatePropertyInvoke(Type type, string multipleLevelProperty)
		{
			if (!multipleLevelProperty.Contains(levelSeparator))
				return new PropertyInvoke(propertyCache.GetPropertyInfo(type, multipleLevelProperty), null);
			else
			{
				int firstDotIndex = multipleLevelProperty.IndexOf(levelSeparator);
				string propertyName = multipleLevelProperty.Substring(0, firstDotIndex);
				string subPropertyName = multipleLevelProperty.Substring(firstDotIndex + 1);

				PropertyInfo pi = propertyCache.GetPropertyInfo(type, propertyName);
				return new PropertyInvoke(pi, CreatePropertyInvoke(pi.PropertyType, subPropertyName));
			}
		}
    }
}
