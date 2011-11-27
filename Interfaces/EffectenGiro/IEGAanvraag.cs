using System;
using B4F.TotalGiro.StaticData;
namespace B4F.DataMigration.EffectenGiro
{
    public interface IEGAanvraag
    {
        bool IsPersonalAccount { get; }
        string AccountName { get; }
        bool AanvraagCompleet { get; set; }
        string BBevoegdheid1 { get; set; }
        string BBevoegdheid2 { get; set; }
        string BedragOntvangen { get; set; }
        string BNaam { get; set; }
        B4F.TotalGiro.StaticData.Address BAddress { get; set; }
        B4F.TotalGiro.StaticData.IContactDetails BContactDetails { get; set; }
        string BRechtsvorm { get; set; }
        string BRekHouder1 { get; set; }
        string BRekHouder2 { get; set; }
        string BRekHouderGeslacht1 { get; set; }
        string BRekHouderGeslacht2 { get; set; }
        string BRekHouderTitel1 { get; set; }
        string BRekHouderTitel2 { get; set; }
        string BRekHouderTussenv1 { get; set; }
        string BRekHouderTussenv2 { get; set; }
        string BRekHouderVoorl1 { get; set; }
        string BRekHouderVoorl2 { get; set; }
        DateTime DatumBedragOntvangen { get; set; }
        DateTime DatumFormOntvangen { get; set; }
        DateTime DatumOprichting { get; set; }
        DateTime DatumTijd { get; set; }
        string EersteInleg { get; set; }

        B4F.TotalGiro.StaticData.INationality Nationality1 { get; set; }
        Gender PrimaryGender { get; }
        Gender SecondaryGender { get; }
        bool IsDualAccount { get; set; }

        bool FormOntvangen { get; set; }
        bool FormVolledig { get; set; }
        bool Geactiveerd { get; set; }
        DateTime Geboortedatum { get; set; }
        string Geslacht { get; set; }

        bool IngevoerdKasweb { get; set; }
        bool KasbankBevestigd { get; set; }
        bool KasbankStorting { get; set; }
        int Key { get; set; }
        string KVK { get; set; }
        string Land { get; set; }

        int LoginId { get; set; }

        B4F.TotalGiro.Instruments.IPortfolioModel ModelPortfolio { get; set; }
        string Naam { get; set; }
        string Nationaliteit { get; set; }
        string Onttrekking { get; set; }
        string OnttrekkingBedrag { get; set; }
        string Pandhouder { get; set; }

        string PeriodeInleg { get; set; }
        string PeriodiekeInleg { get; set; }

        DateTime PGeboortedatum { get; set; }
        string PGeslacht { get; set; }
        B4F.TotalGiro.StaticData.INationality Nationality2 { get; set; }
        B4F.TotalGiro.StaticData.Address ResidentialAddress2 { get; set; }
        B4F.TotalGiro.StaticData.IContactDetails ContactDetails2 { get; set; }
        B4F.TotalGiro.CRM.IIdentification Identification2 { get; set; }


        B4F.TotalGiro.StaticData.Address ResidentialAddress1 { get; set; }
        B4F.TotalGiro.StaticData.Address PostalAddress { get; set; }
        B4F.TotalGiro.CRM.IIdentification Identification1 { get; set; }
        string PNaam { get; set; }

        string PSOFI { get; set; }

        string PTitel { get; set; }
        string PTussenvoegsels { get; set; }
        string PVoorletters { get; set; }

        string SOFI { get; set; }

        string TegenRekening { get; set; }
        string TegenRekeningBank { get; set; }
        string TegenRekeningPlaats { get; set; }
        string TegenRekeningTNV { get; set; }
        IBank Bank { get; set; }

        B4F.TotalGiro.StaticData.IContactDetails ContactDetails1 { get; set; }
        string Titel { get; set; }
        string Tussenvoegsels { get; set; }

        bool Verified { get; set; }
        string VerpandSoort { get; set; }
        bool VierlanderImport { get; set; }
        string Voorletters { get; set; }
        bool IsExecutionOnly { get; set; }
        //string Woonplaats { get; set; }
        //string Postcode { get; set; }
        //string PostHuisnummer { get; set; }
        //string PostHuisnummerToevoeging { get; set; }
        //string PostLand { get; set; }
        //string PostPostcode { get; set; }
        //string PostStraat { get; set; }
        //string PostWoonplaats { get; set; }
        //string UserProfile { get; set; }
        //string Telefoon { get; set; }
        //string TelefoonAvond { get; set; }
        //string Mobiel { get; set; }
        //string Straat { get; set; }
        //string Email { get; set; }
        //string Fax { get; set; }
        //string Huisnummer { get; set; }
        //string HuisnummerToevoeging { get; set; }
        //DateTime LegitimatieGeldigTot { get; set; }
        //string LegitimatieNummer { get; set; }
        //string LegitimatieSoort { get; set; }
        //string PHuisnummer { get; set; }
        //string PHuisnummerToevoeging { get; set; }
        //string PWoonplaats { get; set; }
        //string PStraat { get; set; }
        //string PTelefoon { get; set; }
        //string PTelefoonAvond { get; set; }
        //string PFax { get; set; }
        //string PEmail { get; set; }
        //string PMobiel { get; set; }
        //string PNationaliteit { get; set; }
        //string PPostcode { get; set; }
        //DateTime PLegitimatieGeldigTot { get; set; }
        //string PLegitimatieNummer { get; set; }
        //string PLegitimatieSoort { get; set; }

    }
}
