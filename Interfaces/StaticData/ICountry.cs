using System;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.StaticData.Country">Country</see> class
    /// </summary>
    public interface ICountry
	{
        string CountryName { get; }
        string InternationalName { get; }
		string Iso2 { get; }
		string Iso3 { get; }
		string Iso3NumCode { get; }
		int Key { set; get; }
        ICountryHolidayCollection CountryHolidays { get; }
	}
}
