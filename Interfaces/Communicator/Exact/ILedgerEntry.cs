using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Communicator.Exact
{

    public interface ILedgerEntry
    {
        string FormatLine();
        int Key { get; set; }
        ILedgerType LedgerType { get; }
        string Journal { get; set; }
        string BookingNumber { get; set; }
        string Description { get; set; }
        DateTime ValueDate { get; set; }
        string Debitor { get; set; }
        string Creditor { get; set; }
        Decimal Amount { get; set; }
        //string Currency { get; set; }
        //decimal ExRate { get; set; }
        string Kredietbeperking { get; set; }
        Decimal KredietbeperkingAmount { get; set; }
        DateTime InvoiceDate { get; set; }
        DateTime KredietbeperkingDate { get; set; }
        string PaymentRef { get; set; }
        string PaymentType { get; set; }
        bool StornoBooking { get; set; }
        DateTime CreationDate { get; }
        IExportedLedgerFile ExportedLedgerFile { get; set; }
        ISubledgerEntryCollection SubledgerEntries { get; }
    }
}
