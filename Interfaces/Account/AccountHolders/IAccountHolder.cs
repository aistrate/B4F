using System;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Accounts
{
    public interface IAccountHolder
    {
        int Key { get; set; }
        IContact Contact { get; set; }
        ICustomerAccount GiroAccount { get; set; }
        bool IsPrimaryAccountHolder { get; set; }
        DateTime CreationDate { get; }
    }
}
