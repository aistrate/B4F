using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Fees
{
    internal static class RuleComparer
    {
        public static bool CalculateWeightForBitWiseEnum<TEnum>(TEnum val1, TEnum val2, int ruleWeighting, ref int theWeight)
            where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            bool theResult = false;
            if (Util.IsEnumInValue<TEnum>(val2, val1))
            {
                theWeight += (int)ruleWeighting;
                theResult = true;
            }
            return theResult;
        }

        public static bool CalculateWeight<T>(object obj1, object obj2, int ruleWeighting, ref int theWeight)
        {
            bool theResult = false;
            if (obj2 != null)
            {
                if (((T)obj1).GetHashCode().Equals(((T)obj2).GetHashCode()))
                {
                    theWeight += (int)ruleWeighting;
                    theResult = true;
                }
            }
            else
            {
                theResult = true;
            }
            return theResult;
        }

        public static bool CalculateWeight<T>(object obj1, object obj2, string keyField, int ruleWeighting, ref int theWeight)
        {
            bool theResult = false;
            if (obj2 != null)
            {
                if (getPropertyValue(obj1, typeof(T), keyField) == getPropertyValue(obj2, typeof(T), keyField))
                {
                    theWeight += (int)ruleWeighting;
                    theResult = true;
                }
            }
            else
            {
                theResult = true;
            }
            return theResult;
        }

        private static int getPropertyValue(object obj, Type type, string keyField)
        {
            PropertyInfo pi = obj.GetType().GetProperty(keyField, BindingFlags.Instance | BindingFlags.Public |BindingFlags.DeclaredOnly);
            if (pi != null)
                return (int)pi.GetValue(obj, null);
            else
                return int.MinValue;
        }
    }
}
