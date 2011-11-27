using System;

namespace B4F.TotalGiro.Instruments
{
    public interface ILifecycleLine
    {
        int Key { get; set; }
        ILifecycle Parent { get; set; }
        short SerialNo { get; set; }
        int AgeFrom { get; set; }
        int AgeTo { get; set;  }
        IPortfolioModel Model { get; set; }
        string CreatedBy { get; set; }
        DateTime CreationDate { get; }
        DateTime LastUpdated { get; set; }

    }
}
