using System;

namespace B4F.TotalGiro.CRM
{
    public interface IContactDelegate : IContact
    {
        DateTime DateOfFounding { get; set; }
        string KvKNumber { get; set; }
    }
}
