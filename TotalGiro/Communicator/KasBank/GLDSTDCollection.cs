using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;

namespace B4F.TotalGiro.Communicator.KasBank
{
    public class GLDSTDCollection : GenericCollection<IGLDSTD>, IGLDSTDCollection
    {
        public GLDSTDCollection() : base() { }

        public GLDSTDCollection(IGLDSTDFile parent, IList bagOfRecords)
            : base(bagOfRecords)
        {
            this.Parent = parent;
        }

        public IGLDSTDFile Parent { get; set; }

        public override void Add(IGLDSTD item)
        {
            base.Add(item);
            item.ParentFile = Parent;
        }

        public override int Count
        {
            get
            {
                return base.Count;
            }
        }


    }
}
