using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Communicator.ExtCustodians
{
    /// <summary>
    /// Gets the orders that have been written to the FS export file with a specific id.
    /// </summary>
    public class ExtCustodian
    {
        public ExtCustodian() { }

        public ExtCustodian(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Gets the orders that have been written to the FS export file with a specific id.
        /// </summary>
        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }
        /// <summary>
        /// Gets the orders that have been written to the FS export file with a specific id.
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// Gets the orders that have been written to the FS export file with a specific id.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        #region Privates

        private int key;
        private string name;

        #endregion
    }
}
