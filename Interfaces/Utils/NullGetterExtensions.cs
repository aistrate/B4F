using System;

namespace B4F.TotalGiro.Utils
{
    public static class NullGetterExtensions
    {
        /// <summary>
        /// If 'a' is NOT null, it returns the result of applying 'func' to it; if 'a' is null, it returns null.
        /// To be used when the result type is a <i>reference type</i>.
        /// </summary>
        public static B Get<A, B>(this A a, Func<A, B> func)
            where B : class
        {
            return a != null ? func(a) : null;
        }

        /// <summary>
        /// If 'a' is NOT null, it returns the result of applying 'func' to it; if 'a' is null, it returns null.
        /// To be used when the desired result type is a <i>value type</i>.
        /// (The actual type will be the Nullable variant of the value type.)
        /// </summary>
        public static B? GetV<A, B>(this A a, Func<A, B> func)
            where B : struct
        {
            return a != null ? func(a) : (B?)null;
        }

        /// <summary>
        /// If 'a' is NOT null, it returns the result of applying 'func' to it, or the empty string if the result is null; 
        /// if 'a' is null, it returns the empty string. (So the result will never be null.)
        /// To be used when the result type is <i>string</i>.
        /// </summary>
        public static string GetS<A>(this A a, Func<A, string> func)
        {
            return a != null ? (func(a) ?? "") : "";
        }
    }
}
