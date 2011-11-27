using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashMutationViewTX : CashMutationView, ICashMutationViewTX
    {

        public CashMutationViewTX(ITransaction trade, ICashSubPosition position)
        {
            this.Trade = trade;
            this.Position = position;
            setTransactionType();
            this.SearchKey = "T" + Trade.Key.ToString();
            CreationDate = Trade.CreationDate;
            TransactionDate = Trade.TransactionDate;
            Money amount = null;
            if (position != null && position.SettledFlag == CashPositionSettleStatus.UnSettled &&
                ((ICashSubPositionUnSettled)position).UnSettledType != null && !((ICashSubPositionUnSettled)position).UnSettledType.IsDefault)
                amount = trade.TradingJournalEntry.Lines.Where(x => x.ParentSubPosition != null && x.ParentSubPosition.Key == position.Key).Select(x => x.Balance).Sum();
            else
                amount = trade.TotalAmount;
            this.Amount = amount;
        }

        public override CashMutationViewTypes CashMutationViewType
        {
            get { return CashMutationViewTypes.Transaction; }
        }

        public ITransaction Trade { get; set; }
        public virtual Price Price { get { return Trade.Price; } }
        public string PriceShortDisplayString { get { return (Price != null ? Price.ShortDisplayString : "0"); } }
        public InstrumentSize Size { get { return Trade.ValueSize; } }
        public decimal SizeQuantity { get { return (Size != null ? Size.Quantity : 0m); } }
        public string TradedInstrumentName { get { return (Trade.ValueSize != null ? Trade.ValueSize.Underlying.Name : ""); } }
        public override bool IsTransaction
        {
            get { return true; }
        }        

        public virtual Side SideOfTX
        {
            get
            {
                if (Trade.AccountA == Account)
                    return Trade.TxSide;
                else
                    return (Side) Enum.ToObject(Trade.TxSide.GetType(), (-1 * ((int)Trade.TxSide)));
            }
        }

        public override IAccountTypeInternal Account
        {
            get { return Trade.Account; }
        }

        public override string FullDescription
        {
            get
            {
                StringBuilder fullDescription = new StringBuilder();

                if (!Trade.IsStorno)
                {
                    if (string.IsNullOrEmpty(Trade.Description))
                    {
                        fullDescription.Append(string.Format("{0} {1:#,###,##0.00####} {2} @ {3}",
                                                            (SideOfTX == Side.Buy ? "Aankoop" : (SideOfTX == Side.Sell ? "Verkoop" : "?")),
                                                            Math.Abs(SizeQuantity),
                                                            TradedInstrumentName,
                                                            PriceShortDisplayString));
                        if (Trade.AccruedInterest != null && Trade.AccruedInterest.IsNotZero)
                            fullDescription.Append(string.Format(" Accrued Interest {0}", Trade.AccruedInterest.DisplayString));

                        if (TotalCommissionQuantity != 0m)
                            fullDescription.Append(string.Format(" (Provisie {0})", TotalCommission.Abs().DisplayString));
                    }
                    else
                        fullDescription.Append(Trade.Description);
                }
                else
                {
                    fullDescription.Append(string.Format("Storno of Trade {0}{1}", Trade.OriginalTransaction.Key, Trade.StornoReason != null ? " (" + Trade.StornoReason + ")" : ""));
                }
                return fullDescription.ToString();
            }
        }

        public Money TotalCommission
        {
            get
            {
                if (Trade.Commission != null)
                {
                    if (Trade.ServiceCharge != null)
                        return Trade.Commission + Trade.ServiceCharge;
                    else
                        return Trade.Commission;
                }
                else
                {
                    if (Trade.ServiceCharge != null)
                        return Trade.ServiceCharge;
                    else
                        return null;
                }
            }
        }

        public decimal TotalCommissionQuantity
        {
            get { return (TotalCommission != null ? TotalCommission.Quantity : 0m); }
        }

        public override void setTransactionType()
        {
            if(Trade.IsStorno)
                TransactionType = "Storno";
            else
                TransactionType = "Allocation";
        }

        public override string TypeID
        {
            get
            {
                string id = "666";
                if (Trade.TradeType != null)
                    id = Trade.TradeType.Key.ToString();
                return "T" + id;
            }
        }

        public override string TypeDescription
        {
            get
            {
                string description = "Onbekend";
                if (Trade.TradeType != null)
                    description = Trade.TradeType.Description;
                return description;
            }
        }
    }
}
