using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Orders.Transfers.Reporting
{
    public class PositionTransferReportPortfolio : IPositionTransferReportPortfolio
    {
        public PositionTransferReportPortfolio()
        {
            this.positions = new PositionTransferReportPositionCollection(this);
        }

        public PositionTransferReportPortfolio(IAccountTypeInternal account, DateTime positionDate,
            IPositionTransfer parentTransfer, IPositionTransferPortfolio beforePortfolio,
            IPositionTransferPortfolio afterPortfolio)
            : this()
        {
            this.Account = account;
            this.PositionDate = positionDate;
            this.ParentTransfer = parentTransfer;
            this.BeforePortfolio = beforePortfolio;
            this.AfterPortfolio = afterPortfolio;
            assembleReportPositions(parentTransfer);
        }

        public IPositionTransfer ParentTransfer { get; set; }
        public IAccountTypeInternal Account { get; set; }
        public DateTime PositionDate { get; set; }
        public IPositionTransferPortfolio BeforePortfolio { get; set; }
        public IPositionTransferPortfolio AfterPortfolio { get; set; }
        

        public virtual IPositionTransferReportPositionCollection Positions
        {
            get
            {
                IPositionTransferReportPositionCollection temp = (IPositionTransferReportPositionCollection)positions.AsList();
                if (temp.ParentPortfolio == null) temp.ParentPortfolio = this;
                return temp;
            }
        }

        private void assembleReportPositions(IPositionTransfer parentTransfer)
        {
            IPositionTransferPositionCollection afterPortfolio = AfterPortfolio != null ? AfterPortfolio.Positions : new PositionTransferPositionCollection();

            if (BeforePortfolio != null && BeforePortfolio.Positions != null && BeforePortfolio.Positions.Count > 0)
            {
                var uniqueInstruments = BeforePortfolio.Positions.Select(p => new { Before = true, Position = p })
                        .Concat(afterPortfolio.Select(p => new { Before = false, Position = p }))
                        .GroupBy(i => i.Position.PositionSize.Underlying.Key)
                        .Select(g =>
                        new
                        {

                            Before = g.FirstOrDefault(p => p.Before),
                            After = g.FirstOrDefault(p => !p.Before),
                            g.First().Position.ActualPrice,
                            g.First().Position.ExchangeRate,
                            UnderLying = g.First().Position.PositionSize.Underlying
                        })
                        .ToArray();

                foreach (var line in uniqueInstruments)
                {
                    IPositionTransferReportPosition pos = new PositionTransferReportPosition();
                    pos.ExchangeRate = line.ExchangeRate;
                    pos.ActualPrice = line.ActualPrice;
                    pos.InstrumentOfPosition = line.UnderLying;

                    if (line.Before != null)
                    {
                        IPositionTransferPosition before = line.Before.Position;
                        pos.BeforePositionSize = before.Get(p => p.PositionSize);
                        pos.ValueinEuroBefore = before.Get(p => p.ValueinEuro);
                        pos.PercentageOfPortfolioBefore = before.GetV(p => p.PercentageOfPortfolio).HasValue ? before.GetV(p => p.PercentageOfPortfolio).Value : 0m;
                    }
                    else
                    {
                        pos.BeforePositionSize = new InstrumentSize(0m, line.UnderLying);
                        pos.ValueinEuroBefore = new Money(0m, getCurrencyFromInstrument(line.UnderLying));
                        pos.PercentageOfPortfolioBefore = 0m;
                    }

                    if (line.After != null)
                    {
                        IPositionTransferPosition after = line.After.Position;
                        pos.AfterPositionSize = after.Get(p => p.PositionSize);
                        pos.ValueinEuroAfter = after.Get(p => p.ValueinEuro);
                        pos.PercentageOfPortfolioAfter = after.GetV(p => p.PercentageOfPortfolio).HasValue ? after.GetV(p => p.PercentageOfPortfolio).Value : 0m;
                    }
                    else
                    {
                        pos.AfterPositionSize = new InstrumentSize(0m, line.UnderLying);
                        pos.ValueinEuroAfter = new Money(0m, getCurrencyFromInstrument(line.UnderLying));
                        pos.PercentageOfPortfolioAfter = 0m;
                    }

                    //IPositionTransferReportPosition pos = new PositionTransferReportPosition();
                    //pos.ExchangeRate = line.ExchangeRate;
                    //pos.TransferPrice = line.TransferPrice;
                    //IPositionTransferPosition before = BeforePortfolio.Positions.Where(b => b.PositionSize.Underlying.Key == line.Key).ToList().FirstOrDefault();
                    //IPositionTransferPosition after = AfterPortfolio.Positions.Where(b => b.PositionSize.Underlying.Key == line.Key).ToList().FirstOrDefault();
                    //if (before != null)
                    //{
                    //    pos.BeforePositionSize = before.PositionSize;
                    //    pos.ValueinEuroBefore = before.ValueinEuro;
                    //    pos.PercentageOfPortfolioBefore = before.PercentageOfPortfolio;
                    //}
                    //else
                    //{
                    //    //pos.BeforePositionSize = new InstrumentSize(0m, line.Underlying);
                    //    //pos.ValueinEuroBefore = new Money(0m, getCurrencyFromInstrument(line.Underlying));
                    //    //pos.PercentageOfPortfolioBefore = 0m;
                    //}

                    //if (after != null)
                    //{
                    //    pos.AfterPositionSize = after.PositionSize;
                    //    pos.ValueinEuroAfter = after.ValueinEuro;
                    //    pos.PercentageOfPortfolioAfter = after.PercentageOfPortfolio;
                    //}
                    //else
                    //{
                    //    //pos.AfterPositionSize = new InstrumentSize(0m, line.Underlying);
                    //    //pos.ValueinEuroAfter = new Money(0m, getCurrencyFromInstrument(line.Underlying));
                    //    pos.PercentageOfPortfolioAfter = 0m;
                    //}

                    this.Positions.AddPosition(pos);
                }
            }
        }

        private ICurrency getCurrencyFromInstrument(IInstrument instrument)
        {
            if (instrument.IsCash)
                return instrument.ToCurrency;
            else
                return ((ITradeableInstrument)instrument).CurrencyNominal;
        }



        private IDomainCollection<IPositionTransferReportPosition> positions;
    }
}
