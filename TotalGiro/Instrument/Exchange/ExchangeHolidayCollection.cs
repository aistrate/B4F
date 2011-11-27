using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Instruments
{
    public class ExchangeHolidayCollection : GenericCollection<DateTime>, IExchangeHolidayCollection
    {
        public ExchangeHolidayCollection(IList bagOfHolidays) 
            : base(bagOfHolidays)
        {

        }
    }
}
