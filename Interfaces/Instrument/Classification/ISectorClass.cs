using System;

namespace B4F.TotalGiro.Instruments.Classification
{
    public interface ISectorClass
    {
        int Key { get; set; }
        string SectorName { get; }
    }
}
