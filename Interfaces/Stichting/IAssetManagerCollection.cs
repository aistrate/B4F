using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.AssetManagerCollection">AssetManagerCollection</see> class
    /// </summary>
    public interface IAssetManagerCollection : IList<IAssetManager>
    {
        //IRemisier ParentRemisier { get; set; }
        IEffectenGiro ParentEffectenGiro { get; set; }
    }
}
