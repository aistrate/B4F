using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments.Classification
{
    public class SectorClass : ISectorClass
    {
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual string SectorName
        {
            get { return this.sector; }
            set { this.sector = value; }
        }

        #region Overrides

        public override string ToString()
        {
            return SectorName;
        }

        public override int GetHashCode()
        {
            return Key;
        }

        #endregion

        #region Privates

        private int key;
        private string sector;

        #endregion
    }
}
