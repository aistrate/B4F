using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Stichting.Remisier
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.RemisierCollection">RemisierCollection</see> class
    /// </summary>
    public interface IRemisierCollection : IList<IRemisier>
    {
        IAssetManager Parent { get; }
    }
}
