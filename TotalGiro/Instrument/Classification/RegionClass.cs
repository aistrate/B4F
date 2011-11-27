using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments.Classification
{
    public class RegionClass : IRegionClass
    {
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual string RegionName
        {
            get { return this.region; }
            set { this.region = value; }
        }

        #region Overrides

        public override string ToString()
        {
            return RegionName;
        }

        public override int GetHashCode()
        {
            return Key;
        }

        #endregion

        #region Privates

        private int key;
        private string region;

        #endregion
    }
}
