using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public static class InstrumentSizeExtensions
    {
        public static InstrumentSize Sum(this IEnumerable<InstrumentSize> source)
        {
            int sourceCount = source.Count();
            if (sourceCount == 0)
                return null;
            else if (sourceCount > 0)
                return source.Sum(null);
            else
                throw new ApplicationException("Cannot sum an empty collection of Values.");
        }

        public static InstrumentSize Sum(this IEnumerable<InstrumentSize> source, IInstrument defaultInstrument)
        {
            if (source.Count() == 1 && source.FirstOrDefault() == null)
                return null;
            else if (source.Count() > 0 && source.FirstOrDefault() != null)
            {
                //var instruments = from values in source group values by values.Underlying.Key into g select g.Key;
                var instruments = source.
                    GroupBy(x => x.Underlying.Key)
                    .Select(g => g.FirstOrDefault().Underlying);

                if (instruments.Count() == 1)
                    return new InstrumentSize(source.Sum(m => m.Quantity), (IInstrument)instruments.ElementAt(0));
                else
                    throw new ApplicationException("Cannot sum more than one Instrument.");
            }
            else
                return new InstrumentSize(0m, defaultInstrument);
        }
    }
}
