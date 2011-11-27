using System;
namespace B4F.TotalGiro.GeneralLedger.Static
{
    public interface IGLClass
    {
        string GLClassDescription { get; set; }
        string GLClassNumber { get; set; }
        int Key { get; set; }
    }
}
