using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Fees.Calculations
{
    public class CommValueBreakupLineCollection : GenericCollection<ICommValueBreakupLine>, ICommValueBreakupLineCollection
    {
        //protected CommValueBreakupLineCollection() { }
        internal CommValueBreakupLineCollection(CommValueDetails parent, IList bagOfLines)
            : base(bagOfLines)
        {
            this.parent = parent;
        }

        public ICommValueDetails Parent
        {
            get { return parent; }
            //set { parent = value; }
        }

        public ICommValueBreakupLine GetItemByType(CommValueBreakupType breakupType)
        {
            foreach (ICommValueBreakupLine line in this)
            {
                if (line.CalcType == breakupType)
                    return line;
            }
            return null;
        }

        private CommValueDetails parent;
    }

}
