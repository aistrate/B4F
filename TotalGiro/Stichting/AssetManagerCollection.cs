using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Stichting
{
    /// <summary>
    /// Class holds collection of asset managers
    /// </summary>
    /// <moduleiscollection>
    /// </moduleiscollection>
    public class AssetManagerCollection : TransientDomainCollection<IAssetManager>, IAssetManagerCollection
    {
        public AssetManagerCollection()
            : base() { }

        /// <summary>
        /// Instantiates collection of Asset Managers of a Stichting Effectengiro
        /// </summary>
        /// <param name="parentEffectenGiro">Effectengiro object</param>
        public AssetManagerCollection(IEffectenGiro parentEffectenGiro)
            : base()
        {
            this.ParentEffectenGiro = parentEffectenGiro;
        }

        ///// <summary>
        ///// Get/set Remisier object belonging to a collection of Asset Managers
        ///// </summary>
        //public virtual IRemisier ParentRemisier { get; set; }

        /// <summary>
        /// Get/set an Effectengiro object belonging to a collection of Asset Managers
        /// </summary>
        public virtual IEffectenGiro ParentEffectenGiro { get; set; }
    }
}
