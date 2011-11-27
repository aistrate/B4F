using System;
using System.Collections;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public class DividWepRecordCollection : TransientDomainCollection<IDividWepRecord>, IDividWepRecordCollection
    {
        public DividWepRecordCollection()
            : base() { }

        public DividWepRecordCollection(IDividWepFile parentFile)
            : base()
        {
            ParentFile = parentFile;
        }

        public IDividWepFile ParentFile { get; set; }

        public void AddDividWepRecord(IDividWepRecord record)
        {
            record.ParentFile = ParentFile;            
            base.Add(record);
            ParentFile.TotalWep += record.WepValue;
            ParentFile.TotalDividend += record.DivrentebedragValue;
            ParentFile.TotalTax += record.BedragbronbelastingValue;
            ParentFile.CreateCloseRecord();
        }

    }
}
