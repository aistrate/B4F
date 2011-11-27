using System;
using System.Collections;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments.Classification;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.CommRules;


namespace B4F.TotalGiro.Instruments
{
    #region Helper Class

    public class TransactionFillDetails
    {
        public TransactionFillDetails() { }

        public TransactionFillDetails(InstrumentSize size, Money amount,
            Money accruedInterest, Money serviceCharge, decimal serviceChargePercentage,
            Money commission, Money totalOrderAmount, Side side)
        {
            this.Size = size;
            this.Amount = amount;
            this.AccruedInterest = accruedInterest;
            this.ServiceCharge = serviceCharge;
            this.Commission = commission;
            this.TotalOrderAmount = totalOrderAmount;
            this.Side = side;
        }

        public InstrumentSize Size { get; set; }
        public Money Amount { get; set; }
        public Money AccruedInterest { get; set; }
        public Money ServiceCharge { get; set; }
        public decimal ServiceChargePercentage { get; set; }
        public Money Commission { get; set; }
        public Money TotalOrderAmount { get; set; }
        public Side Side { get; set; }
        public string Info { get; set; }

        public Money Total
        {
            get 
            { 
                if (Side == Side.Sell)
                    return Amount + AccruedInterest + ServiceCharge;
                else
                    return Amount + AccruedInterest + ServiceCharge + Commission;
            }

        }

        public Money Diff
        {
            get 
            {
                return Total - TotalOrderAmount; 
            }
        }

        public bool IsOK
        {
            get { return Size != null && Size.IsNotZero; }
        }

        public bool IsDiff
        {
            get { return Diff.IsNotZero; }
        }

        public bool FixUp(IOrderAmountBased order)
        {
            bool success = false;
            Money diff = Diff;
            if (diff.IsNotZero && diff.IsWithinTolerance(0.09M))
            {
                // Only adjust for saved orders (at trade fill) -> otherwise adjust the commission
                if (order.Key != 0 && (order.Amount.Abs() - Amount - ServiceCharge).IsWithinTolerance(0.03M))
                {
                    Amount = order.Amount.Abs() - ServiceCharge;
                    Commission = order.Commission.Abs();
                }
                else
                    Commission -= diff;
                success = true;
            }
            return success;
        }

        public void SetSign(Side side)
        {
            if (side == Side.Buy)
            {
                Size = Size.Abs();
                Amount = Amount.Abs().Negate();
                if (AccruedInterest != null && AccruedInterest.IsNotZero)
                    AccruedInterest = AccruedInterest.Abs().Negate();
            }
            else
            {
                Size = Size.Abs().Negate();
                Amount = Amount.Abs();
                if (AccruedInterest != null && AccruedInterest.IsNotZero)
                    AccruedInterest = AccruedInterest.Abs();
            }
            if (ServiceCharge != null && ServiceCharge.IsNotZero)
                ServiceCharge = ServiceCharge.Abs().Negate();
            if (Commission != null && Commission.IsNotZero)
                Commission = Commission.Abs().Negate();
        }

        public override string ToString()
        {
            if (!IsOK)
                return "Not OK";
            else
            {
                string s = String.Format("Size {0} Amount {1} Commission {2}", Size.Quantity, Amount.ToString(), Commission.ToString());
                if (AccruedInterest != null && AccruedInterest.IsNotZero)
                    s += String.Format(" AccrInt {0}", AccruedInterest.ToString());
                if (ServiceCharge != null && ServiceCharge.IsNotZero)
                    s += String.Format(" ServCharge {0}", ServiceCharge.ToString());
                if (!string.IsNullOrEmpty(Info))
                    s += String.Format(" Info {0}", Info);
                return s;
            }
        }

    }

    #endregion

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.TradeableInstrument">TradeableInstrument</see> class
    /// </summary>
    public interface ITradeableInstrument : IInstrumentsWithPrices
	{
		string CompanyName { get; set; }
        IExchange DefaultExchange { get; set; }
        IExchange HomeExchange { get; set; }
        string DefaultExchangeName { get; }
        IRoute DefaultRoute { get; }
        bool AllowNetting { get; }

        DateTime IssueDate { get; set; }
        IInstrumentExchangeCollection InstrumentExchanges { get; }

        DateTime GetSettlementDate(DateTime tradeDate, IExchange exchange);
        DateTime GetSettlementDate(DateTime tradeDate, IExchange exchange, Int16 settlementDays);
        Money GetServiceChargeForOrder(IOrder order);
        Money GetServiceChargeForOrder(IOrder order, IExchange exchange);
        bool IsGreenFund { get; set; }
        bool IsCultureFund { get; set; }
        bool IsCommissionLinear { get; }
        IGLAccount SettlementDifferenceAccount { get; set; }
        int ContractSize { get; }

        TransactionFillDetails GetTransactionFillDetails(IOrder order, Price price, DateTime settlementDate, IFeeFactory feeFactory, decimal fillRatio, IExchange exchange);
        TransactionFillDetails GetTransactionFillDetails(IOrderSizeBased order, Price price, DateTime settlementDate, IFeeFactory feeFactory, decimal fillRatio, IExchange exchange);
        TransactionFillDetails GetTransactionFillDetails(IOrderAmountBased order, Price price, DateTime settlementDate, IFeeFactory feeFactory, decimal fillRatio, IExchange exchange);
	}
}
