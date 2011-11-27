using System;
namespace B4F.TotalGiro.Instruments
{
    public enum OptionTypes
    {
        Call,
        Put
    }
    
    public interface IOption : IDerivative
    {
        OptionTypes OptionType { get; set; }
        Price StrikePrice { get; set; }
        DateTime ExpiryDate { get; set; }
        string SortOrder { get; }
    }
}
