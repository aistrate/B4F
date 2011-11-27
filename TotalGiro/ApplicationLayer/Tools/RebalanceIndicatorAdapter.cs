using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.Tools
{
    public static class RebalanceIndicatorAdapter
    {

        #region Helper Class

        public class RebalanceIndicationLine
        {
            public RebalanceIndicationLine(
                IModelVersion version, IModelVersion nextVersion,
                IInstrumentsWithPrices instrument, DateTime startDate, 
                decimal allocation, Price buyPrice, Price sellPrice)
            {
                this.Version = version;
                this.NextVersion = nextVersion;
                if (this.NextVersion != null)
                    this.EndDate = this.NextVersion.LatestVersionDate;
                this.Instrument = instrument;
                this.StartDate = startDate;
                this.Allocation = allocation;
                this.BuyPrice = buyPrice;
                this.SellPrice = sellPrice;
            }

            public IModelVersion Version { get; set; }
            public IModelVersion NextVersion { get; set; }
            public string VersionInfo { get; set; }
            public IInstrumentsWithPrices Instrument { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Allocation { get; set; }
            public Price BuyPrice { get; set; }
            public Price SellPrice { get; set; }
            public InstrumentSize Size { get; set; }
            public Money StartAmount { get; set; }
            public Money NewAmount { get; set; }
            public bool IsConverted { get; set; }
            public RebalanceIndicationLine PrevLine { get; set; }
        }

        #endregion

        
        public static DataSet GetModelRebalanceIndications(
            string modelName, DateTime depositDate, DateTime endDate,
            decimal startBalance, decimal maxDeviation, bool includeModelChanges,
            bool includeInActiveModels, out string message)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Money amount = new Money(startBalance, InstrumentMapper.GetBaseCurrency(session));
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                Dictionary<int, string> modelIdsToSkip = new Dictionary<int, string>();
                message = "";

                int[] modelIds = ModelMapper.GetModelsSorted(session, company.IsStichting ? null : (IAssetManager)company, true, false, includeInActiveModels ? ActivityReturnFilter.All : ActivityReturnFilter.Active, modelName).
                    Where(x => !x.IsSubModel && !x.ModelName.Contains("Opheffing")).
                    Select(x => x.Key).ToArray();
                IList<IModelVersion> versions = ModelMapper.GetModelVersionsForDate(session, modelIds, depositDate);
                List<DateTime> dates = new List<DateTime>();

                if (includeModelChanges && versions.Count > 0)
                {
                    Hashtable parameters = new Hashtable(1);
                    Hashtable parameterLists = new Hashtable(1);
                    parameters.Add("date", depositDate.Date);
                    if (Util.IsNotNullDate(endDate))
                        parameters.Add("endDate", endDate.Date);
                    parameterLists.Add("modelIds", versions.Select(x => x.ParentModel.Key).Distinct().ToArray());
                    IList<IModelVersion> versionsLater = session.GetTypedListByNamedQuery<IModelVersion>(
                        "B4F.TotalGiro.Instruments.ModelVersion.GetModelVersionsForModelsFromDate",
                        parameters, parameterLists);

                    versions = versions.Union(versionsLater).ToList();
                    dates = versions.Select(x => x.LatestVersionDate).Distinct().ToList();
                }
                if (!dates.Contains(depositDate))
                    dates.Add(depositDate);
                if (Util.IsNotNullDate(endDate) && !dates.Contains(endDate))
                    dates.Add(endDate);

                List<IModelInstrument> dists = versions.SelectMany(x => x.ModelInstruments).ToList<IModelInstrument>();

                IList<IHistoricalPrice> prices = HistoricalPriceMapper.GetHistoricalPrices(session, dates.ToArray());
                IList<IHistoricalExRate> exrates = HistoricalExRateMapper.GetHistoricalExRates(session, 0, dates.ToArray());


                var modelsWithCurrency = (from x in dists
                                          where x.Component.IsCash
                                          select x.ParentVersion.ParentModel).
                                          Distinct().ToList();

                foreach (var item in modelsWithCurrency)
                    dists.RemoveAll(x => x.ParentVersion.ParentModel.Key == item.Key);

                var allLines = (from x in dists
                                join y in prices on 
                                    new { InstrumentID = x.Component.Key , Date = (x.ParentVersion.LatestVersionDate > depositDate ? x.ParentVersion.LatestVersionDate : depositDate) } equals
                                    new { InstrumentID = y.Instrument.Key, Date = y.Date } into z
                                from y in z.DefaultIfEmpty()
                                join a in dists on 
                                    new { ModelID = x.ParentVersion.ParentModel.Key, InstrumentID = x.Component.Key,  VersionNumber = x.ParentVersion.VersionNumber } equals
                                    new { ModelID = a.ParentVersion.ParentModel.Key, InstrumentID = a.Component.Key, VersionNumber = a.ParentVersion.VersionNumber - 1 } into b
                                from a in b.DefaultIfEmpty()
                                join c in prices on
                                    new { InstrumentID = a != null ? a.Component.Key : 0, Date = a != null ? ((Util.IsNotNullDate(endDate) && a.ParentVersion.LatestVersionDate > endDate) ? endDate : a.ParentVersion.LatestVersionDate) : DateTime.MinValue } equals
                                    new { InstrumentID = c.Instrument.Key, Date = c.Date } into d
                                from c in d.DefaultIfEmpty()
                                select new RebalanceIndicationLine
                                (
                                    x.ParentVersion,
                                    a.Get(e => e.ParentVersion),
                                    (IInstrumentsWithPrices)x.Component,
                                    x.ParentVersion.LatestVersionDate,
                                    x.Allocation,
                                    y.Get(e => e.Price),
                                    c.Get(e => e.Price)
                                )
                         ).OrderBy(x => x.Version.VersionNumber).ToList();

                // check for modelversions on same day
                var doubleDate = (from x in allLines
                                 group x by new { x.Version.ParentModel, x.Version.LatestVersionDate } into g
                                 where g.Select(y => y.Version).Distinct().Count() > 1
                                 select g.Select(y => y.Version).Distinct().OrderBy(y => y.VersionNumber).First()).ToList();

                var versionsThatDoNotAddUp = (from x in allLines
                                  group x by x.Version into g
                                  where g.Select(y => y.Allocation).Sum() != 1M
                                  select g.Key).ToList();

                foreach (var item in doubleDate.Union(versionsThatDoNotAddUp))
                {
                    foreach (var line in allLines.Where(x => x.NextVersion != null && x.NextVersion.Key == item.Key))
                    {
                        var delLine = allLines.Where(x => x.Version.Key == item.Key && x.Instrument.Key == line.Instrument.Key).First();
                        if (delLine == null && line.Instrument.ParentInstrument != null)
                            delLine = allLines.Where(x => x.Version.Key == item.Key && x.Instrument.Key == line.Instrument.ParentInstrument.Key).First();
                        line.NextVersion = delLine.Get(e => e.NextVersion);
                        line.SellPrice = delLine.Get(e => e.SellPrice);
                        line.EndDate = delLine != null ? delLine.EndDate : DateTime.MinValue;
                    }
                    allLines.RemoveAll(x => x.Version.Key == item.Key);
                }

                // set all nextversion
                foreach (var model in allLines.Select(x => x.Version.ParentModel).Distinct().OrderByDescending(x => x.ModelName))
                {
                    IModelVersion lastVersion = null;
                    foreach (IModelVersion version in allLines.Where(x => x.Version.ParentModel.Key == model.Key).Select(x => x.Version).Distinct().OrderByDescending(x => x.VersionNumber))
                    {
                        foreach (var line in allLines.Where(x => x.Version.Key == version.Key))
                        {
                            if (lastVersion != null)
                            {
                                if (line.NextVersion == null)
                                {
                                    line.NextVersion = lastVersion;
                                    line.EndDate = lastVersion.LatestVersionDate;
                                }
                            }
                            if (line.StartDate < depositDate)
                                line.StartDate = depositDate;
                        }
                        lastVersion = version;
                    }
                }

                var linesWithoutPrice = (from y in
                                        (from x in allLines
                                         where x.BuyPrice == null
                                         select new
                                         {
                                            x.Instrument,
                                            Period = Util.GetPeriodFromDate(x.StartDate),
                                            Line = x
                                         }).Union
                                        (from x in allLines
                                         where x.SellPrice == null && (Util.IsNotNullDate(x.EndDate) || x.Instrument.ParentInstrument != null)
                                         select new 
                                         { 
                                             x.Instrument, 
                                             Period = Util.GetPeriodFromDate(x.EndDate),
                                             Line = x
                                         })
                                        group y by new { y.Instrument, y.Period } into g
                                         select new
                                         {
                                             g.Key,
                                             Lines = g.Select(x => x.Line),
                                         }
                                        ).ToList();

                foreach (var lineNoPriceCol in linesWithoutPrice.Where(x => x.Key.Period != 101))
                {
                    bool skip = false;
                    DateTime begin;
                    DateTime end;
                    Util.GetDatesFromPeriod(lineNoPriceCol.Key.Period, out begin, out end);
                    IList<IHistoricalPrice> tempPrices = HistoricalPriceMapper.GetHistoricalPrices(session, lineNoPriceCol.Key.Instrument.Key, begin, end.AddDays(60));
                    if (tempPrices.Count == 0)
                    {
                        if (lineNoPriceCol.Key.Instrument.ParentInstrument != null)
                            tempPrices = HistoricalPriceMapper.GetHistoricalPrices(session, lineNoPriceCol.Key.Instrument.ParentInstrument.Key, begin, end.AddDays(60));
                        if (tempPrices.Count == 0)
                        {
                            string error = string.Format("It is not possible to do a indication since prices are missing for instrument {0} on {1} used by {2}", 
                                lineNoPriceCol.Key.Instrument.DisplayNameWithIsin, 
                                begin.ToString("dd-MM-yyyy"),
                                lineNoPriceCol.Lines.Select(x => x.Version.ToString()).JoinStrings(", "));
                            foreach (var line in lineNoPriceCol.Lines)
                            {
                                if (!modelIdsToSkip.Keys.Contains(line.Version.ParentModel.Key))
                                    modelIdsToSkip.Add(line.Version.ParentModel.Key, error);
                                skip = true;
                            }
                        }
                    }

                    if (!skip)
                    {
                        foreach (var line in lineNoPriceCol.Lines)
                        {
                            if (line.BuyPrice == null && Util.DateBetween(begin, end, line.StartDate))
                            {
                                Price newPrice = tempPrices.Where(x => x.Date >= line.StartDate).OrderBy(x => x.Date).First().Price;
                                line.BuyPrice = newPrice;
                                prices.Add(new HistoricalPrice(newPrice, line.StartDate));
                            }
                            if (line.SellPrice == null && Util.DateBetween(begin, end, line.EndDate))
                            {
                                Price newPrice = tempPrices.Where(x => x.Date >= line.EndDate).OrderBy(x => x.Date).FirstOrDefault().Get(e => e.Price);
                                if (newPrice != null)
                                {
                                    line.SellPrice = newPrice;
                                    prices.Add(new HistoricalPrice(newPrice, line.EndDate));
                                }
                            }
                        }
                    }
                }

                if (modelIdsToSkip.Count > 0)
                {
                    foreach (int modelId in modelIdsToSkip.Keys)
                        allLines.RemoveAll(x => x.Version.ParentModel.Key == modelId);

                    message = modelIdsToSkip.Values.JoinStrings("<br/>");
                }

                var linesPerModel = (from x in allLines
                          group x by x.Version.ParentModel into g
                          select new
                          {
                              Model = g.Key,
                              Versions = g,
                              VersionInfo = g.OrderBy(x => x.Version.VersionNumber).Select(x => x.Version.VersionNumber.ToString()).Distinct().JoinStrings(",")
                          }
                        ).ToList();

                foreach (var modelVersion in linesPerModel)
                {
                    DateTime prevDate = DateTime.MinValue;
                    IModelVersion mvprev = null;
                    foreach (IModelVersion mv in modelVersion.Versions.OrderBy(x => x.StartDate).ThenBy(x => x.Version.VersionNumber).Select(x => x.Version).Distinct())
                    {
                        DateTime startDate = mv.LatestVersionDate < depositDate ? depositDate : mv.LatestVersionDate;
                        DateTime? nextDate = modelVersion.Versions.Where(x => x.StartDate > startDate).OrderBy(x => x.StartDate).FirstOrDefault().GetV(e => e.StartDate);
                        Money portfolioAmount = amount;
                        if (Util.IsNotNullDate(prevDate))
                            portfolioAmount = modelVersion.Versions.Where(x => x.Version.Key == mvprev.Key && x.NewAmount != null).Select(x => x.NewAmount).Sum();
                        foreach (RebalanceIndicationLine version in modelVersion.Versions.Where(x => x.Version.Key == mv.Key))
                        {
                            if (nextDate.HasValue)
                            {
                                if (version.SellPrice == null && version.Instrument.ParentInstrument != null)
                                {
                                    version.SellPrice = prices.Where(x => x.Instrument.Key == version.Instrument.ParentInstrument.Key && x.Date == nextDate).FirstOrDefault().Get(e => e.Price);
                                    version.IsConverted = true;
                                }
                            }
                            else
                            {
                                if (Util.IsNullDate(endDate))
                                    version.SellPrice = version.Instrument.CurrentPrice.Price;
                                else
                                    version.SellPrice = prices.Where(x => x.Instrument.Key == version.Instrument.Key && x.Date >= endDate).Take(60).OrderBy(x => x.Date >= endDate).FirstOrDefault().Get(e => e.Price);
                            }

                            if (Util.IsNotNullDate(prevDate))
                            {
                                version.PrevLine = modelVersion.Versions.Where(x => x.Instrument.Key == version.Instrument.Key && x.StartDate == prevDate).FirstOrDefault();
                                if (version.PrevLine == null && version.IsConverted)
                                    version.PrevLine = modelVersion.Versions.Where(x => x.Version.Key == mvprev.Key && x.StartDate == prevDate && x.Instrument.ParentInstrument != null && x.Instrument.ParentInstrument.Key == version.Instrument.Key).FirstOrDefault();
                            }

                            // eerste inleg + nieuwe instrumenten
                            if (version.PrevLine == null)
                            {
                                version.Size = calculateSize(portfolioAmount * version.Allocation, version.BuyPrice, exrates, version.StartDate);
                            }
                            else
                            {
                                Money oldAmount = version.PrevLine.NewAmount;
                                Money modelAmount = portfolioAmount * version.Allocation;
                                InstrumentSize prevSize = version.PrevLine.Size;
                                if (version.IsConverted)
                                    prevSize = new InstrumentSize(version.PrevLine.Size.Quantity, version.Instrument);
                                version.Size = prevSize + calculateSize((modelAmount - oldAmount), version.BuyPrice, exrates, version.StartDate);
                            }

                            // Calculate the new amount (on enddate)
                            if (version.IsConverted || !(version.Size.Underlying.Equals(version.SellPrice.Instrument)))
                                version.NewAmount = calculateBaseAmount(new InstrumentSize(version.Size.Quantity, version.Instrument.ParentInstrument) * version.SellPrice, exrates, version.EndDate);
                            else
                                version.NewAmount = calculateBaseAmount(version.Size * version.SellPrice, exrates, version.EndDate);

                            // Only when it was in the previous version
                            version.StartAmount = calculateBaseAmount(version.Size * version.BuyPrice, exrates, version.StartDate);

                            version.VersionInfo = modelVersion.VersionInfo;
                        }
                        mvprev = mv;
                        prevDate = startDate;
                        Money checkAmt = modelVersion.Versions.Where(x => x.Version.Key == mv.Key && x.Size != null).Select(x => x.StartAmount).Sum();
                        if (((checkAmt - portfolioAmount).Quantity / portfolioAmount.Quantity * startBalance) > 0.1M)
                            message += string.Format("There is a problem with the data (prices) during the rebalance indication calculation for model {0} for {1}.",
                                mv.ToString(),
                                startDate.ToString("dd-MM-yyyy")) + "<br/>";
                    }
                }


                var q = (from x in allLines
                         where Util.IsNullDate(x.EndDate)
                         group x by x.Version.ParentModel into g
                         select new
                         {
                             Model = g.Key,
                             TotalAmount = g.Select(m => m.NewAmount).Sum()
                         }
                        ).ToList();

                var oldPrices = (from x in prices
                         group x by x.Instrument into g
                         select new
                         {
                             Instrument = g.Key,
                             g.OrderBy(x => x.Date).First().Price
                         }
                        ).ToList();

                var r = (from x in allLines
                         join y in q on x.Version.ParentModel.Key equals y.Model.Key
                         join z in oldPrices on x.Instrument.Key equals z.Instrument.Key
                         where Util.IsNullDate(x.EndDate) && y.TotalAmount != null &&  y.TotalAmount.IsNotZero
                         select new
                         {
                             Model = x.Version.ParentModel,
                             x.VersionInfo,
                             x.Instrument,
                             ModelAllocation = x.Allocation,
                             OldPrice = z.Price,
                             x.Size,
                             x.NewAmount,
                             ActualAllocation = Math.Round(x.NewAmount.Quantity / y.TotalAmount.Quantity, 4),
                             LastPrice = (Util.IsNullDate(endDate) ? 
                                                x.Instrument.CurrentPrice.Price :
                                                prices.Where(a => a.Instrument.Key == x.Instrument.Key && a.Date == endDate).FirstOrDefault().Get(e => e.Price)),
                             LastPriceDate = (Util.IsNullDate(endDate) ? x.Instrument.CurrentPrice.Date : endDate),
                             WarningLevel = determineWarningLevel(x.Allocation, Math.Round(x.NewAmount.Quantity / y.TotalAmount.Quantity, 6), maxDeviation)
                         }
                         ).ToList();

                var s = (from x in r
                         group x by x.Model into g
                         select new
                         {
                             Model = g.Key,
                             VersionInfo = g.First().VersionInfo,
                             TotalAmount = g.Select(m => m.NewAmount).Sum(),
                             WarningLevel = g.Max(m => m.WarningLevel)
                         }
                        ).ToList();


                DataSet ds = r
                    .Select(c => new
                    {
                        Key = c.Instrument.Key,
                        ModelID = c.Model.Key,
                        InstrumentName = c.Instrument.Name,
                        Size = c.Size.DisplayString,
                        SizeQuantity = c.Size.Quantity,
                        ModelAllocation = c.ModelAllocation * 100M,
                        OldPrice = c.OldPrice.ShortDisplayString,
                        OldPriceQuantity = c.OldPrice.Quantity,
                        LastPrice = c.LastPrice.ShortDisplayString,
                        LastPriceQuantity = c.LastPrice.Quantity,
                        c.LastPriceDate,
                        NewAmount = c.NewAmount.DisplayString,
                        NewAmountQuantity = c.NewAmount.Quantity,
                        ActualAllocation = c.ActualAllocation * 100M,
                        c.WarningLevel
                    })
                    .ToDataSet();
                ds.Tables[0].TableName = "Details";

                DataTable dt = s
                    .Select(c => new
                    {
                        c.Model.Key,
                        ModelName = string.Format("{0} v.({1})", c.Model.ModelName,c.VersionInfo),
                        TotalAmount = c.TotalAmount.DisplayString,
                        TotalAmountQuantity = c.TotalAmount.Quantity,
                        c.WarningLevel
                    })
                    .OrderBy(c => c.ModelName)
                    .ToDataTable("Overview");
                ds.Tables.Add(dt);
                return ds;
            }
        }

        private static InstrumentSize calculateSize(Money amount, Price price, IList<IHistoricalExRate> exrates, DateTime date)
        {
            if (!price.Underlying.IsBase)
            {
                IHistoricalExRate exrate = exrates.Where(x => x.Currency.Key == price.Underlying.Key && x.RateDate == date).FirstOrDefault();
                if (exrate == null)
                    throw new ApplicationException(string.Format("Exchange rate for {0} on {1} is missing.", price.Underlying.Symbol, date.ToShortDateString()));
                Money convAmt = amount.Convert(exrate.Rate, price.Underlying);
                return convAmt / price;
            }
            else
                return amount / price;
        }

        private static Money calculateBaseAmount(Money amount, IList<IHistoricalExRate> exrates, DateTime date)
        {
            if (!((ICurrency)amount.Underlying).IsBase)
            {
                if (Util.IsNotNullDate(date))
                {
                    IHistoricalExRate exrate = exrates.Where(x => x.Currency.Key == amount.Underlying.Key && x.RateDate == date).FirstOrDefault();
                    if (exrate == null)
                        throw new ApplicationException(string.Format("Exchange rate for {0} on {1} is missing.", ((ICurrency)amount.Underlying).Symbol, date.ToShortDateString()));
                    Money convAmt = amount.ConvertToBase(1M/exrate.Rate);
                    return convAmt;
                }
                else
                    return amount.CurrentBaseAmount;
            }
            else
                return amount;
        }

        //private static InstrumentSize calculateCurrentBaseAmount(Money amount, Price price, IList<IHistoricalExRate> exrates, DateTime date)
        //{
        //    InstrumentSize size = calculateSize(amount, price, exrates, date);
        //    IInstrumentsWithPrices instrument = (IInstrumentsWithPrices)price.Instrument;
        //    Money currentAmount = size.CalculateAmount(instrument.CurrentPrice.Price);
        //    return currentAmount.BaseAmount;
        //}

        private static byte determineWarningLevel(decimal originalValue, decimal newValue, decimal maxDeviation)
        {
            decimal deviation = (Math.Abs(newValue - originalValue) / originalValue) * 100M;

            if (deviation >= maxDeviation)
                return 2;
            else if (deviation >= (maxDeviation) / 2)
                return 1;
            else
                return 0;
        }

    }
}
