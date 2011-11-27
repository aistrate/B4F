using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Valuations.ReportedData;
namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public interface IDividWepRecord
    {
        string AccountNumber { get; set; }
        string Bedragbronbelasting { get; }
        int BedragbronbelastingValue { get; set; }
        string Correctie { get; set; }
        string Divrentebedrag { get; }
        int DivrentebedragValue { get; set; }
        string Geboortedag { get; set; }
        string Geboorteeeuw { get; set; }
        string Geboortejaar { get; set; }
        string Geboortemaand { get; set; }
        string GezamenlijkBelang { get; set; }
        string Huisnummer { get; set; }
        int Key { get; set; }
        string KvKnummer { get; set; }
        string Naam { get; set; }
        IDividWepFile ParentFile { get; set; }
        string Plaatsnaam { get; set; }
        string Postcode { get; set; }
        string Rechtsvormcode { get; set; }
        string RecordType { get; set; }
        string Reserve { get; }
        string SingleRecord { get; }
        string Sofinummer { get; set; }
        string Soortdivrente { get; set; }
        string SoortFonds { get; set; }
        string Soortvalutawep { get; set; }
        string Straatnaam { get; set; }
        string Toevoeging { get; set; }
        string Typebronbelasting { get; set; }
        string Typeobligatie { get; set; }
        string ValutaBedragbronbelasting { get; set; }
        string ValutaDivrentebedrag { get; set; }
        string Valutajaar { get; set; }
        string Voorletters { get; set; }
        string Voorvoegsels { get; set; }
        string WEP { get; }
        int WepValue { get; set; }
        ICustomerAccount Account { get; set; }
        IEndTermValue EndTermRecord { get; set; }
    }
}
