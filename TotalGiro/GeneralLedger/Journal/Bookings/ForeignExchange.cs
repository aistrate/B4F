using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Static;

namespace B4F.TotalGiro.GeneralLedger.Journal.Bookings
{
    public class ForeignExchange : GeneralOperationsBooking, IForeignExchange
    {
        public ForeignExchange() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dividendDetails">The details of the cash dividend (date, price)</param>
        /// <param name="units">The total number of units over which dividend is paid</param>
        public ForeignExchange(IJournalEntryLine line)
            : base((ICustomerAccount)line.GiroAccount, line.Parent, getDescription(line))
        {
            if (!(line.Status == JournalEntryLineStati.Booked && line.BookComponent == null &&
                (line.GLAccount.CashTransferType == CashTransferTypes.ForexBaseCurrency ||
                line.GLAccount.CashTransferType == CashTransferTypes.ForexNonBaseCurrency)))
                throw new ApplicationException("This journal entry line is not a valid Foreign Exchange.");

            BookingComponentTypes bookingComponentType = BookingComponentTypes.ForexBaseCurrency;
            switch (line.GLAccount.CashTransferType)
            {
                case CashTransferTypes.ForexNonBaseCurrency:
                    bookingComponentType = BookingComponentTypes.ForexNonBaseCurrency;
                    break;
                case CashTransferTypes.ForexBaseCurrency:
                    bookingComponentType = BookingComponentTypes.ForexBaseCurrency;
                    break;
            }

            IForeignExchangeComponent newComponent = new ForeignExchangeComponent(this, bookingComponentType, this.CreationDate);
            line.BookComponent = newComponent.Component;
            newComponent.JournalLines.Add(line);
            line.BookComponent.MainLine = line;
            newComponent.Component.SetDescription(this.Description);
            this.Components.Add(newComponent);
        }

        public override GeneralOperationsBookingTypes BookingType
        {
            get { return GeneralOperationsBookingTypes.ForeignExchange; }
        }

        public override IGeneralOperationsBooking Storno(B4F.TotalGiro.Stichting.Login.IInternalEmployeeLogin employee, string reason, IMemorialBooking journalEntry)
        {
            ForeignExchange newStorno = new ForeignExchange();
            return this.storno(employee, reason, journalEntry, newStorno); ;
        }

        public override B4F.TotalGiro.Notas.INota CreateNota()
        {
            throw new NotImplementedException();
        }

        public virtual Money AmountinBaseCurrency
        {
            get
            {
                return this.Components.ReturnComponentValue(BookingComponentTypes.ForexBaseCurrency);
            }
        }

        public virtual Money AmountinForeignCurrency
        {
            get
            {
                return this.Components.ReturnComponentValue(BookingComponentTypes.ForexNonBaseCurrency);
            }
        }

        protected static string getDescription(IJournalEntryLine line)
        {
            string description = "";
            if (!string.IsNullOrEmpty(line.Description))
                description = line.Description;
            else
                description = line.GLAccount.CashTransferType.ToString();
            return description;
        }
    }
}
