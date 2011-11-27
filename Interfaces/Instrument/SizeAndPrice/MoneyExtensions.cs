using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public static class MoneyExtensions
    {
        public static Money Sum(this IEnumerable<Money> source)
        {
            int sourceCount = source.Count();
            if (sourceCount == 0)
                return null;
            else if (sourceCount > 0)
                return source.Sum(null);
            else
                throw new ApplicationException("Cannot sum an empty collection of Money.");
        }

        public static Money Sum(this IEnumerable<Money> source, ICurrency defaultCurrency)
        {
            if (source.Count() == 1 && source.FirstOrDefault() == null)
                return null;
            else if (source.Count() > 0 && source.FirstOrDefault() != null)
            {
                //var currencies = from money in source group money by money.Underlying into g select g.Key;
                var currencies = source.
                    GroupBy(x => x.Underlying.Key)
                    .Select(g => g.FirstOrDefault().Underlying);

                if (currencies.Count() == 1)
                {
                    ICurrency currency = (ICurrency)currencies.ElementAt(0);
                    decimal weightedXRate = 1M;
                    if (!currency.IsBase)
                    {
                        decimal totalAbsQuantity = source.Sum(m => Math.Abs(m.Quantity));
                        weightedXRate = Math.Round(totalAbsQuantity != 0m ? source.Sum(m => Math.Abs(m.Quantity) * m.XRate) / totalAbsQuantity :
                                                                      source.Sum(m => m.XRate) / source.Count(), 7);
                    }
                    return new Money(source.Sum(m => m.Quantity), currency, weightedXRate);
                }
                else
                    throw new ApplicationException("Cannot sum more than one currency.");
            }
            else
                if (defaultCurrency != null)
                    return new Money(0, defaultCurrency);
                else
                    return null;
        }
    }
}
