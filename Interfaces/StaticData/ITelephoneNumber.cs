using System;
namespace B4F.TotalGiro.StaticData
{
    public interface ITelephoneNumber
    {
        string AreaCode { get; set; }
        string CountryCode { get; set; }
        string LocalNumber { get; set; }
        string Number { get;}
    }
}
