using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace B4F.TotalGiro.Utils
{
    public static class EnumExtensions
    {
        public static bool ContainsValue<TEnum>(this TEnum flags, TEnum singleFlag)
            where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            if (typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
                return EnumComparer<TEnum>.Instance.ContainsValue(flags, singleFlag);
            else
                return (flags.Equals(singleFlag));
        }

        public static bool IsValueWithin<TEnum>(this TEnum flags, TEnum[] singleFlags)
            where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            foreach (TEnum singleFlag in singleFlags)
            {
                if (ContainsValue(flags, singleFlag))
                    return true;
            }
            return false;
        }

        public static TEnum[] ToEnumArray<TEnum>(this TEnum flags)
            where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Where(x => flags.ContainsValue(x)).ToArray<TEnum>();
        }




    }
}
