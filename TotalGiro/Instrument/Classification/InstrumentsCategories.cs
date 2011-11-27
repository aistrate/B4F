using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments.Classification
{
    public class InstrumentsCategories : IInstrumentsCategories
    {
        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual string InstrumentsCategoryName
        {
            get { return this.instrumentsCategoryName; }
            set { this.instrumentsCategoryName = value; }
        }

        public virtual bool IsDefault
        {
            get { return this.isDefault; }
            set { this.isDefault = value; }
        }

        #region Overrides

        public override string ToString()
        {
            return InstrumentsCategoryName;
        }

        public override int GetHashCode()
        {
            return Key;
        }

        #endregion

        #region Privates

        private int key;
        private string instrumentsCategoryName;
        private bool isDefault;

        #endregion
    }
}
