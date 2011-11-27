using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.StaticData
{
    public class CountryHolidayCollection : GenericCollection<DateTime>, ICountryHolidayCollection
    {
        public CountryHolidayCollection(IList bagOfHolidays) 
            : base(bagOfHolidays)
        {

        }
    }
}
