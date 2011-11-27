using System.Collections.Generic;

namespace B4F.TotalGiro.Utils.Linq
{
    /// <summary>
    /// Equality Comparer for KeyValuePair objects. It ignores the Value and uses only the Key in equality tests.
    /// </summary>
    public class KeyValueEqualityComparer<TKey, TValue> : EqualityComparer<KeyValuePair<TKey, TValue>>
    {
        public override bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return keyEqualityComparer.Equals(x.Key, y.Key);
        }

        public override int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return obj.Key.GetHashCode();
        }

        public static KeyValueEqualityComparer<TKey, TValue> Default
        {
            get { return new KeyValueEqualityComparer<TKey, TValue>(); }
        }

        private EqualityComparer<TKey> keyEqualityComparer = EqualityComparer<TKey>.Default;
    }
}
