using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Communicator.BelastingDienst;
namespace B4F.TotalGiro.Valuations.ReportedData.Reports
{
    public interface IEndTermDividWepComparison
    {

        IAccountTypeInternal Account { get; set; }
        IEndTermValue EndTermValue { get; set; }
        IDividWepRecord DividWep { get; set; }
        bool IncludedinDividWep { get; set; }
        IReportEndTermDividWep Parent { get; set; }
        string AccoutNumber { get; }
        string AccoutShortName { get; }
        decimal CashValue { get; }
        decimal FundValue { get; }
        Decimal FullValue { get; }
        decimal DividendValue { get; }
        decimal DividendTaxValue { get; }
        int WEP { get; }
        decimal RoundingError { get; }
        decimal FullValueForDividWep { get; }
        decimal ValuesNotIncludedinWEP { get; }
    }
}
