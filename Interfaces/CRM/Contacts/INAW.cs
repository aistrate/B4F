using System;
namespace B4F.TotalGiro.CRM
{
    public interface INAW
    {
        string ContactTitle { get; set; }
        DateTime CreationDate { get; set; }
        int Key { get; set; }
        Address PostalAddress { get; set; }
        Address ResidentialAddress { get; set; }
    }
}
