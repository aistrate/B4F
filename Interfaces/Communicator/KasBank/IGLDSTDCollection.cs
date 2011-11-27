using System;
using B4F.TotalGiro.Collections;
namespace B4F.TotalGiro.Communicator.KasBank
{
    public interface IGLDSTDCollection : IGenericCollection<IGLDSTD>
    {
        IGLDSTDFile Parent { get; set; }
    }
}
