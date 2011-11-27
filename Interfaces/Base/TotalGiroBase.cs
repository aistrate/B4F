using System;

namespace B4F.TotalGiro.Base
{
    public abstract class TotalGiroBase<IType> : ITotalGiroBase<IType>
    {
        public virtual int Key { get; private set; }

        bool IEquatable<ITotalGiroBase<IType>>.Equals(ITotalGiroBase<IType> other)
        {
            return other != null && this.Key == other.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<ITotalGiroBase<IType>>)this).Equals(obj as ITotalGiroBase<IType>);
        }
    }
}
