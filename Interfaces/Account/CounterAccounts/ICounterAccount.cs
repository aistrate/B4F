using System;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Accounts
{
    public interface ICounterAccount : IAuditable
    {
        int Key { get; set; }
        string Number { get; set; }
        string AccountName { get; set; }
        IBank Bank { get; set; }
        string BankName { get; set; }
        Address BankAddress { get; set; }
        Address BeneficiaryAddress { get; set; }
        bool IsPublic { get; set; }
        bool IsValid { get; }
        IManagementCompany ManagementCompany { get; }
        string DisplayName { get; }
        string DisplayNameShort { get; }
    }
}
