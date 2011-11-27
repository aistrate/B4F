using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.Dal
{
	internal class BusinessObjectListPropertyTable : PropertyTable
	{
		private Hashtable propertyInvokeTable = new Hashtable();

        public override void LoadProperties(object obj)
		{
            base.LoadProperties(obj);

            // Fill propertyInvokeTable with PropertyInvoke objects (including multi-level properties, e.g. property Account.Name on class Order)
			foreach (string propertyName in PropertyNames)
                propertyInvokeTable[propertyName] = PropertyInvoke.CreatePropertyInvoke(obj.GetType(), propertyName.Trim());
		}

        protected override Type GetRealPropertyDataType(string propertyName)
		{
			return ((PropertyInvoke)propertyInvokeTable[propertyName]).FinalType;
		}

		public override object GetPropertyValue(object obj, string propertyName)
		{
            try
            {
                return ((PropertyInvoke)propertyInvokeTable[propertyName]).Invoke(obj);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException(
                    string.Format("[DataSetBuilder, BusinessObjectListPropertyTable] Error retrieving property {0}. {1}", 
                                  propertyName, ex.Message),
                    ex.InnerException);
            }
		}
	}
}
