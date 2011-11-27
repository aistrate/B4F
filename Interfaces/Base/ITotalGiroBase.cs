using System;

namespace B4F.TotalGiro.Base
{
    public interface ITotalGiroBase<IType> : IEquatable<ITotalGiroBase<IType>>
    {
        int Key { get; }
    }
}
