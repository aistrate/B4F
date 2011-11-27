using System;
using B4F.TotalGiro.BackOffice.Orders;

namespace B4F.TotalGiro.Communicator.KasBank
{
    public interface IGLDSTD
    {
        string Amount { get; set; }
        string BankBankAcctNr { get; set; }
        string BenefBankAcctNr { get; set; }
        string CircuitCode { get; set; }
        string CodeOnForex { get; set; }
        string CountryCodeForex { get; set; }
        string CurrencyCode { get; set; }
        string DebetAcctNr { get; set; }
        string Filler1 { get; set; }
        string Filler2 { get; set; }
        string Filler3 { get; set; }
        string Filler4 { get; set; }
        string Filler5 { get; set; }
        string Filler6 { get; set; }
        string Filler7 { get; set; }
        string Filler8 { get; set; }
        string Filler9 { get; set; }
        string GroundForPayment1 { get; set; }
        string GroundForPayment2 { get; set; }
        string GroundForPayment3 { get; set; }
        string GroundForPayment4 { get; set; }
        string IndicationOfCost { get; set; }
        string IndicationOfNonRes { get; set; }
        int Key { get; set; }
        string NarBenef1 { get; set; }
        string NarBenef2 { get; set; }
        string NarBenef3 { get; set; }
        string NarBenef4 { get; set; }
        string NarBenefBank1 { get; set; }
        string NarBenefBank2 { get; set; }
        string NarBenefBank3 { get; set; }
        string NarBenefBank4 { get; set; }
        string NarCorrespondentBank1 { get; set; }
        string NarCorrespondentBank2 { get; set; }
        string NarCorrespondentBank3 { get; set; }
        string NarCorrespondentBank4 { get; set; }
        string NarDebet1 { get; set; }
        string NarDebet2 { get; set; }
        string NarDebet3 { get; set; }
        string NarDebet4 { get; set; }
        string NatureOfCP { get; set; }
        string OptionsContract { get; set; }
        IMoneyTransferOrder OriginalMoneyOrder { get; set; }
        IGLDSTDFile ParentFile { get; set; }
        string PriorityCode { get; set; }
        string ProcessDate { get; set; }
        string Reference { get; set; }
        string SwiftBenefBank { get; set; }
        string SwiftCorrespondentBank { get; set; }
        string TestKey { get; set; }
        string TextOnForex { get; set; }
        DateTime CreationDate { get; }
        string ToString();
    }
}
