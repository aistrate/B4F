using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.CRM.Contacts;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Reports.Documents;

namespace B4F.TotalGiro.Notas
{
    [Flags]
    public enum NotaTypes
    {
        All = 0,
        NotaTransaction = 1,
        NotaDeposit = 2,
        NotaInstrumentConversion = 4,
        NotaDividend = 8,
        NotaFees = 16,
        NotaTransfer = 32,
        NotaTransactionBase = 256,
        NotaGeneralOperationsBooking = 512,
        NotaGeneralOperationsBookingTaxeable = 1024
    }
    
    public interface INota : IAuditable
    {
        int Key { get; set; }
        string NotaNumber { get; }
        DateTime CreationDate { get; }
        INotaDocument Document { get; set; }
        string Title { get; }
        NotaTypes NotaType { get; }
        IContactsNAW ContactsNAW { get; }
        IContactsNAW SecondContactsNAW { get; }
        int PrintCount { get; }
        bool IsStorno { get; }
        ICustomerAccount Account { get; }
        ContactsFormatter Formatter { get; }
        string ModelPortfolioName { get; }
        DateTime ModelDate { get; }
        Money TotalValue { get; }
        Money GrandTotalValue { get; }
        ICurrency InstrumentCurrency { get; }
        DateTime TransactionDate { get; }
        decimal ExchangeRate { get; }
        int IncrementPrintCount();
    }
}
