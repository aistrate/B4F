using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class holds collection of instruments that changed into the parent instrument (through corporate actions.
    /// </summary>
    public class InstrumentCollection : GenericCollection<IInstrument>, IInstrumentCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentCollection">InstrumentCollection</see> class.
        /// </summary>
        internal InstrumentCollection(IList instruments, IInstrument parent)
            : base(instruments)
        {
            this.parent = parent;
        }

        /// <summary>
        /// The parent instrument
        /// </summary>
        public IInstrument Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }


        #region Private Variables

        private IInstrument parent;

        #endregion
    }
}
