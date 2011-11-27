using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Instruments;


namespace B4F.DataMigration.EffectenGiro
{
    public class EGAanvraag : IEGAanvraag
    {


        public bool IsPersonalAccount
        {
            get
            { return (BNaam == null); }
        }

        public string AccountName
        {
            get
            { return (IsPersonalAccount ? this.Naam : this.BNaam); }
        }


        public int Key { get; set; }
        public DateTime DatumTijd { get; set; }
        public int LoginId { get; set; }
        public bool AanvraagCompleet { get; set; }
        public bool FormOntvangen { get; set; }

        public DateTime DatumFormOntvangen { get; set; }
        public string BedragOntvangen { get; set; }
        public DateTime DatumBedragOntvangen { get; set; }
        public bool FormVolledig { get; set; }
        public bool IngevoerdKasweb { get; set; }

        public bool Verified { get; set; }
        public bool KasbankBevestigd { get; set; }
        public bool KasbankStorting { get; set; }
        public bool Geactiveerd { get; set; }

        public string TegenRekening { get; set; }
        public string TegenRekeningTNV { get; set; }
        public string TegenRekeningPlaats { get; set; }
        public string TegenRekeningBank { get; set; }
        public IBank Bank { get; set; }

        public string Onttrekking { get; set; }
        public string OnttrekkingBedrag { get; set; }
        public string VerpandSoort { get; set; }
        public string Pandhouder { get; set; }
        public string EersteInleg { get; set; }

        public bool IsDualAccount { get; set; }


        // Contact person 1 Details
        public string Titel { get; set; }
        public string Naam { get; set; }
        public string Tussenvoegsels { get; set; }
        public string Voorletters { get; set; }
        public string Geslacht { get; set; }
        public Gender PrimaryGender
        {
            get
            {
                if (this.Geslacht == null)
                    return Gender.Unknown;
                else return (this.Geslacht.ToUpper() == "M") ? Gender.Male : Gender.Female;
            }
        }
        public Gender SecondaryGender
        {
            get
            {
                if (this.PGeslacht == null)
                    return Gender.Unknown;
                else return (this.PGeslacht.ToUpper() == "M") ? Gender.Male : Gender.Female;
            }
        }
        public string Nationaliteit { get; set; }
        public B4F.TotalGiro.StaticData.INationality Nationality1 { get; set; }
        public B4F.TotalGiro.StaticData.Address PostalAddress { get; set; }
        public B4F.TotalGiro.StaticData.Address ResidentialAddress1 { get; set; }
        public B4F.TotalGiro.StaticData.IContactDetails ContactDetails1 { get; set; }
        public B4F.TotalGiro.CRM.IIdentification Identification1 { get; set; }
        public string Land { get; set; }
        public string PostLand { get; set; }
        public DateTime Geboortedatum { get; set; }
        public string SOFI { get; set; }
        

        public string PNaam { get; set; }
        public string PTitel { get; set; }
        public string PTussenvoegsels { get; set; }
        public string PVoorletters { get; set; }
        public string PGeslacht { get; set; }

        public B4F.TotalGiro.StaticData.INationality Nationality2 { get; set; }
        public B4F.TotalGiro.StaticData.Address ResidentialAddress2 { get; set; }
        public B4F.TotalGiro.StaticData.IContactDetails ContactDetails2 { get; set; }
        public B4F.TotalGiro.CRM.IIdentification Identification2 { get; set; }

        public DateTime PGeboortedatum { get; set; }
        public string PSOFI { get; set; }

        public string PLegitimatieSoort { get; set; }
        public string PLegitimatieNummer { get; set; }

        public string PNationaliteit { get; set; }
        public DateTime PLegitimatieGeldigTot { get; set; }

        public string BNaam { get; set; }
        public DateTime DatumOprichting { get; set; }
        public string KVK { get; set; }
        public B4F.TotalGiro.StaticData.Address BAddress { get; set; }
        public B4F.TotalGiro.StaticData.IContactDetails BContactDetails { get; set; }

        public string BRekHouderTitel1 { get; set; }
        public string BRekHouder1 { get; set; }
        public string BRekHouderTussenv1 { get; set; }
        public string BRekHouderVoorl1 { get; set; }

        public string BRekHouderGeslacht1 { get; set; }
        public string BBevoegdheid1 { get; set; }
        public string BRekHouderTitel2 { get; set; }
        public string BRekHouder2 { get; set; }
        public string BRekHouderTussenv2 { get; set; }

        public string BRekHouderVoorl2 { get; set; }
        public string BRekHouderGeslacht2 { get; set; }
        public string BBevoegdheid2 { get; set; }
        public B4F.TotalGiro.Instruments.IPortfolioModel ModelPortfolio { get; set; }


        public string BRechtsvorm { get; set; }
        public string PeriodiekeInleg { get; set; }
        public string PeriodeInleg { get; set; }
        public bool VierlanderImport { get; set; }
        public bool IsExecutionOnly { get; set; }

        public virtual DateTime LastUpdated
        {
            get { return this.lastUpdated; }
        }

        private DateTime lastUpdated;


        //public string UserProfile { get; set; }
        //public string Straat { get; set; }
        //public string Huisnummer { get; set; }
        //public string HuisnummerToevoeging { get; set; }
        //public string Postcode { get; set; }
        //public string Woonplaats { get; set; }
        //public string Telefoon { get; set; }
        //public string TelefoonAvond { get; set; }
        //public string Mobiel { get; set; }
        //public string Fax { get; set; }
        //public string PostStraat { get; set; }
        //public string PostHuisnummer { get; set; }
        //public string PostHuisnummerToevoeging { get; set; }
        //public string PostPostcode { get; set; }
        //public string PostWoonplaats { get; set; }
        //public string LegitimatieSoort { get; set; }
        //public string LegitimatieNummer { get; set; }
        //public DateTime LegitimatieGeldigTot { get; set; }
        //public string PStraat { get; set; }
        //public string PHuisnummer { get; set; }
        //public string PHuisnummerToevoeging { get; set; }
        //public string PPostcode { get; set; }
        //public string PWoonplaats { get; set; }
        //public string PTelefoon { get; set; }
        //public string PTelefoonAvond { get; set; }
        //public string PMobiel { get; set; }
        //public string PFax { get; set; }
        //public string PEmail { get; set; }
        //public string BStraat { get; set; }
        //public string BHuisnummer { get; set; }
        //public string BHuisnummerToevoeging { get; set; }
        //public string BPostcode { get; set; }
        //public string BPlaats { get; set; }
        //public string BLand { get; set; }
        //public string BTelefoon { get; set; }
        //public string BTelefoonAvond { get; set; }
        //public string BMobiel { get; set; }
        //public string BFax { get; set; }
        //public string BEmail { get; set; }

    }
}
