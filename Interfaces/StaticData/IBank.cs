using System;

namespace B4F.TotalGiro.StaticData
{
    public interface IBank
    {
        int Key { get; set; }
        string Name { get; }
        Address Address { get; }
        bool UseElfProef { get; }
    }
}
