using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public interface IDividWepRecordCollection : IList<IDividWepRecord>
    {
        IDividWepFile ParentFile { get; set; }
        void AddDividWepRecord(IDividWepRecord record);
    }
}
