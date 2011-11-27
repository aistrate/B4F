using System;
using System.Text;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class CashMutationViewGop : CashMutationView, ICashMutationViewGop
    {
        public CashMutationViewGop(IGeneralOperationsBooking booking, ICashSubPosition position, bool isSettled)
        {
            this.Booking = booking;
            this.Position = position;
            setTransactionType();
            this.SearchKey = "G" + Booking.Key.ToString();
            CreationDate = Booking.CreationDate;
            TransactionDate = Booking.BookDate;
            this.IsSettled = isSettled;
            setGopValue();
        }

        private void setGopValue()
        {
            switch (this.Booking.BookingType)
            {
                case GeneralOperationsBookingTypes.ManagementFee:
                    this.Amount = ((IManagementFee)this.Booking).TotalAmount;
                    cashMutationViewType = CashMutationViewTypes.ManagementFee;
                    break;
                case GeneralOperationsBookingTypes.CashDividend:
                    this.Amount = ((ICashDividend)this.Booking).TotalAmount;
                    cashMutationViewType = CashMutationViewTypes.CashDividend;
                    break;
                case GeneralOperationsBookingTypes.CashTransfer:
                    this.Amount = ((ICashTransfer)this.Booking).TransferAmount;
                    cashMutationViewType = CashMutationViewTypes.CashTransfer;
                    break;
                case GeneralOperationsBookingTypes.ForeignExchange:
                    this.Amount = ((IForeignExchange)this.Booking).AmountinBaseCurrency;
                    cashMutationViewType = CashMutationViewTypes.ForeignExchange;
                    break;
                case GeneralOperationsBookingTypes.BondCouponPayment:
                    if (this.IsSettled)
                        this.Amount = ((IBondCouponPayment)this.Booking).TotalAmount;
                    else
                        this.Amount = ((IBondCouponPayment)this.Booking).TotalAmountUnSettled;
                    cashMutationViewType = CashMutationViewTypes.BondCouponPayment;
                    break;
                default:
                    break;
            }
        }

        public override CashMutationViewTypes CashMutationViewType
        {
            get { return cashMutationViewType; }
        }

        public virtual IGeneralOperationsBooking Booking { get; set; }

        public override IAccountTypeInternal Account
        {
            get { return Booking.Account; }
        }

        public override string FullDescription
        {
            get
            {
                StringBuilder fullDescription = new StringBuilder();

                switch (this.Booking.BookingType)
                {
                    case GeneralOperationsBookingTypes.ManagementFee:
                        fullDescription.Append(getMgmtFeeDescription());
                        break;
                    case GeneralOperationsBookingTypes.CashDividend:
                        fullDescription.Append(((ICashDividend)this.Booking).DividendDetails.Instrument.DisplayIsinWithName);
                        break;
                    case GeneralOperationsBookingTypes.CashTransfer:
                        fullDescription.Append(setCashTransferDescription());
                        break;
                    case GeneralOperationsBookingTypes.ForeignExchange:
                        fullDescription.Append("Vreemd Valuta Transactie");
                        break;
                    case GeneralOperationsBookingTypes.BondCouponPayment:
                        fullDescription.Append(this.Booking.Description);
                        break;
                    default:                        
                        break;
                }
                return fullDescription.ToString();
            }
        }

        private string getMgmtFeeDescription()
        {
            //Beheervergoeding (incl. btw) Q3 2009 

            if (this.Booking != null && !string.IsNullOrEmpty(this.Booking.Description))
                return this.Booking.Description;
            else
            {
                string returnValue = "";
                if ((TransactionDate.Day == 1) && (TransactionDate.Month == 1))
                    returnValue = "Beheervergoeding (incl. btw) Q4 " + TransactionDate.AddYears(-1).Year.ToString();
                else if ((TransactionDate.Day == 1) && (TransactionDate.Month == 4))
                    returnValue = "Beheervergoeding (incl. btw) Q1 " + TransactionDate.Year.ToString();
                else if ((TransactionDate.Day == 1) && (TransactionDate.Month == 7))
                    returnValue = "Beheervergoeding (incl. btw) Q2 " + TransactionDate.Year.ToString();
                else if ((TransactionDate.Day == 1) && (TransactionDate.Month == 10))
                    returnValue = "Beheervergoeding (incl. btw) Q3 " + TransactionDate.Year.ToString();
                else
                    returnValue = "Eindafrekening " + TransactionDate.ToShortDateString();
                return returnValue;
            }
        }

        private string setCashTransferDescription()
        {
            string returnValue = "";
            IJournalEntryLine line = ((ICashTransfer)Booking).MainTransferLine;
            if ((line.Description != null) && (line.Description.Length > 0))
                returnValue = line.Description;
            else if (line.GLAccount.CashTransferType == B4F.TotalGiro.GeneralLedger.Static.CashTransferTypes.Deposit)
                returnValue = "Storting";
            else if (line.GLAccount.CashTransferType == B4F.TotalGiro.GeneralLedger.Static.CashTransferTypes.Withdrawal)
                returnValue = "Onttrekking";
            else
                returnValue = "Onbekend";

            Money transferFee = ((ICashTransfer)Booking).Components.ReturnComponentValue(BookingComponentTypes.CashTransferFee);
            if (transferFee != null)
                returnValue += string.Format(" (afsluitprovisie {0})", transferFee.Abs().DisplayString);

            return returnValue;
        }

        private string setForexDescription()
        {
            string returnValue = "";

            IJournalEntryLine line = ((ICashTransfer)Booking).MainTransferLine;
            if ((line.Description != null) && (line.Description.Length > 0))
                returnValue = line.Description;
            else if (line.GLAccount.CashTransferType == B4F.TotalGiro.GeneralLedger.Static.CashTransferTypes.Deposit)
                returnValue = "Storting";
            else if (line.GLAccount.CashTransferType == B4F.TotalGiro.GeneralLedger.Static.CashTransferTypes.Withdrawal)
                returnValue = "Ontrekking";
            else
                returnValue = "Onbekend";

            return returnValue;
        }

        public override bool IsTransaction
        {
            get { return false; }
        } 

        public override void setTransactionType()
        {
            this.TransactionType = this.Booking.BookingType.ToString();
        }

        public override string TypeID
        {
            get
            {
                string id = "666";
                if (Booking.GLBookingType != null)
                    id = Booking.GLBookingType.Key.ToString();
                return "GL" + id;
            }
        }

        public override string TypeDescription
        {
            get
            {
                string description = "Onbekend";
                if (Booking.GLBookingType != null)
                    description = Booking.GLBookingType.Description;
                return description;
            }
        }

        private CashMutationViewTypes cashMutationViewType;
    }
}
