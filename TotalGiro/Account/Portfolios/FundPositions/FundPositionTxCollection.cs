using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Accounts.Portfolios.FundPositions
{
    public class FundPositionTxCollection : TransientDomainCollection<IFundPositionTx>, IFundPositionTxCollection
    {
        public FundPositionTxCollection()
            : base() { }


        public FundPositionTxCollection(IFundPosition parentPosition)
            : base()
        {
            ParentPosition = parentPosition;
        }

        public void AddPositionTX(IFundPositionTx item)
        {
            Add(item);
            item.ParentPosition = ParentPosition;

            if (Util.IsNullDate(ParentPosition.OpenDate))
                ParentPosition.OpenDate = item.TransactionDate;
            else if (item.TransactionDate < ParentPosition.OpenDate)
                ParentPosition.OpenDate = item.TransactionDate;

            if (!item.ParentTransaction.IsStorno && item.Instrument.SecCategory.Key == SecCategories.Bond && Util.IsNotNullDate(ParentPosition.LastBondCouponCalcDate))
            {
                if (item.TransactionDate <= ParentPosition.LastBondCouponCalcDate)
                {
                    ParentPosition.LastBondCouponCalcDate = item.TransactionDate.AddDays(-1);

                    // get the settlement date
                    DateTime settlementDate = DateTime.MinValue;
                    switch (item.ParentTransaction.TransactionType)
                    {
                        case TransactionTypes.Allocation:
                        case TransactionTypes.Crumble:
                            settlementDate = ((IOrderExecutionChild)item.ParentTransaction).ParentExecution.ContractualSettlementDate;
                            break;
                        case TransactionTypes.Execution:
                            settlementDate = ((IOrderExecution)item.ParentTransaction).ContractualSettlementDate;
                            break;
                    }
                    if (Util.IsNullDate(settlementDate))
                    {
                        if (item.Instrument.IsTradeable)
                        {
                            IExchange exchange = ((ITradeableInstrument)item.Instrument).HomeExchange ?? ((ITradeableInstrument)item.Instrument).DefaultExchange;
                            settlementDate = ((ITradeableInstrument)item.Instrument).GetSettlementDate(item.TransactionDate, exchange);
                        }
                        else
                            settlementDate = item.TransactionDate;
                    }

                    IBondCouponPayment bcp = ParentPosition.BondCouponPayments.ActivePayment;
                    if (bcp != null && bcp.Status == BondCouponPaymentStati.Active && bcp.CouponHistory.StartAccrualDate > settlementDate)
                    {
                        bcp.Cancel(true);

                        var bcpsToBeSettled = ParentPosition.BondCouponPayments
                            .Where(x => x.Status == BondCouponPaymentStati.ToBeSettled && (
                                Util.DateBetween(x.CouponHistory.StartAccrualDate, x.CouponHistory.EndAccrualDate, settlementDate) ||
                                x.CouponHistory.StartAccrualDate <  settlementDate))
                            .ToList();
                        if (bcpsToBeSettled.Count() > 0)
                        {
                            foreach (var bcpToBeSettled in bcpsToBeSettled)
                            {
                                IBondCouponPaymentDailyCalculation lastCalc = bcpToBeSettled.DailyCalculations.LastCalculation;
                                InstrumentSize newSize = this.Where(x => x.TransactionDate <= item.TransactionDate).Select(x => x.Size).Sum();
                                if (lastCalc.PositionSize != newSize)
                                    bcpToBeSettled.Cancel();
                            }
                        }
                    }
                }
            }

            ParentPosition.Size += item.Size;
            if ((item.ParentTransaction.TxSide == B4F.TotalGiro.Orders.Side.Buy) || (item.ParentTransaction.TxSide == B4F.TotalGiro.Orders.Side.XI))
                ParentPosition.TotalOpenSize += item.Size;
        }

        public IFundPosition ParentPosition { get; set; }

    }
}
