using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments.Classification;

namespace B4F.TotalGiro.Instruments.Classification
{
    public class AssetClass : IAssetClass
    {
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual string AssetName
        {
            get { return this.asset; }
            set { this.asset = value; }
        }

        #region Overrides

        public override string ToString()
        {
            return AssetName;
        }

        public override int GetHashCode()
        {
            return Key;
        }

        #endregion

        #region Privates

        private int key;
        private string asset;

        #endregion
    }
}
