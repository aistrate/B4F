using System;
using System.Collections.Generic;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Stichting.Remisier
{
    /// <summary>
    /// Class holds collection of Remisiers
    /// </summary>
    public class RemisierCollection : TransientDomainCollection<IRemisier>, IRemisierCollection
    {
        public RemisierCollection()
            : base() { }

        /// <summary>
        /// Initializes collection of Remisiers
        /// </summary>
        /// <param name="assetManager">Associated Asset Manager</param>
        internal RemisierCollection(IAssetManager assetManager)
            : base()
        {
            this.Parent = assetManager;
        }

        /// <summary>
        /// Get/set associated Asset Manager
        /// </summary>
        public IAssetManager Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        #region Private Variables

        private IAssetManager parent;

        #endregion

    }
}
