using System;
namespace B4F.TotalGiro.StaticData
{
    public interface IBankDetails
    {
        string AccountNumber { get; set; }
        IBank Bank { get; set; }
        string BankName { get; set; }
        string BankAccountName { get; set; }
        Address BankAddress { get; set; }
        Address BankPostalAddress { get; set; }
    }
}
