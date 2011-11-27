using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Communicator.ImportedBankFiles.Files.Records
{
    public class TradeReconRecord : ImportedRecord
    {

        public TradeReconRecord() { }
        public TradeReconRecord(string newLine)
        {
            ImportFormatter FormattedData = new ImportFormatter(newLine);
            this.BB_nummer = FormattedData[0].PadRight(6, ' ').Substring(0, 6);
            this.Rek_Soort = FormattedData[1].PadRight(4, ' ').Substring(0, 4);
            this.Valuta_rek = FormattedData[2].PadRight(3, ' ').Substring(0, 3);
            this.Transactie_Datum = FormattedData.AssignDateValue(3);
            this.Trans_Tijd = FormattedData[4].PadRight(7, ' ').Substring(0, 7);
            this.Trans_Status = FormattedData[5].PadRight(1, ' ').Substring(0, 1);
            this.Trans_Soort = FormattedData[6].PadRight(10, ' ').Substring(0, 10);
            this.FX_Koers = FormattedData.AssignDecimalValue(7);
            this.Valuta_Fonds = FormattedData[8].PadRight(3, ' ').Substring(0, 3);
            this.Aantal_Nominal = FormattedData.AssignDecimalValue(9);
            this.Koers = FormattedData.AssignDecimalValue(10);
            this.Nota_Bedrag = FormattedData.AssignDecimalValue(11);
            this.Provisie = FormattedData.AssignDecimalValue(12);
            this.Belasting = FormattedData.AssignDecimalValue(13);
            this.Opgelopen_Rente = FormattedData.AssignDecimalValue(14);
            this.Settlement_Datum = FormattedData.AssignDateValue(15);
            this.Trans_Nummer = FormattedData[16].PadRight(10, ' ').PadRight(10);
            this.ISIN_Code = FormattedData[17].PadRight(12, ' ').Substring(0, 12);
            this.Symbol = FormattedData[18].PadRight(22, ' ').Substring(0, 22);
            this.Dividend = FormattedData[19].PadRight(10, ' ').Substring(0, 10);
            this.Exp_Maand = FormattedData.AssignDateValue(20);
            this.Strike = FormattedData.AssignDecimalValue(21);
            this.Instrument_Type = FormattedData[22].PadRight(10, ' ').Substring(0, 10);
            this.Info_1 = FormattedData[23].PadRight(100, ' ').Substring(0, 100);
            this.Info_2 = FormattedData[24].PadRight(100, ' ').Substring(0, 100);
            this.Info_3 = FormattedData[25].PadRight(100, ' ').Substring(0, 100);
            this.Infor_4 = FormattedData[26].PadRight(100, ' ').Substring(0, 100);
            this.Boek_Datum = FormattedData.AssignDateValue(27);
            this.Fonds_Cat = FormattedData[28].PadRight(30, ' ').Substring(0, 30);
            this.Fonds_Code = FormattedData[29].PadRight(10, ' ').Substring(0, 10);
            this.FondsNaam = FormattedData[30].PadRight(75, ' ').Substring(0, 75);
            this.Eff_Waarde = FormattedData.AssignDecimalValue(31);
            this.Provisie_Cat = FormattedData[32].PadRight(6, ' ').Substring(0, 6);
        }

        public string BB_nummer { get; set; }
        public string Rek_Soort { get; set; }
        public string Valuta_rek { get; set; }
        public DateTime Transactie_Datum { get; set; }
        public string Trans_Tijd { get; set; }
        public string Trans_Status { get; set; }
        public string Trans_Soort { get; set; }
        public Decimal FX_Koers { get; set; }
        public string Valuta_Fonds { get; set; }
        public Decimal Aantal_Nominal { get; set; }
        public Decimal Koers { get; set; }
        public Decimal Nota_Bedrag { get; set; }
        public Decimal Provisie { get; set; }
        public Decimal Belasting { get; set; }
        public Decimal Opgelopen_Rente { get; set; }
        public DateTime Settlement_Datum { get; set; }
        public string Trans_Nummer { get; set; }
        public string ISIN_Code { get; set; }
        public string Symbol { get; set; }
        public string Dividend { get; set; }
        public DateTime Exp_Maand { get; set; }
        public Decimal Strike { get; set; }
        public string Instrument_Type { get; set; }
        public string Info_1 { get; set; }
        public string Info_2 { get; set; }
        public string Info_3 { get; set; }
        public string Infor_4 { get; set; }
        public DateTime Boek_Datum { get; set; }
        public string Fonds_Cat { get; set; }
        public string Fonds_Code { get; set; }
        public string FondsNaam { get; set; }
        public Decimal Eff_Waarde { get; set; }
        public string Provisie_Cat { get; set; }


    }
}
