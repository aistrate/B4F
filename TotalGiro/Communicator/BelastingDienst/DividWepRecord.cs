using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Valuations.ReportedData;

namespace B4F.TotalGiro.Communicator.BelastingDienst
{
    public class DividWepRecord : IDividWepRecord
    {
        public DividWepRecord()
        {

        }
        public DividWepRecord(ICustomerAccount account, IEndTermValue endTermValue )
        {
            this.Account = account;
            this.EndTermRecord = endTermValue;
            this.EndTermRecord.DividWepRecord = this;
            IContact primary = account.PrimaryAccountHolder.Contact;
            bool isPerson = (primary.ContactType == ContactTypeEnum.Person);


            this.recordType = "5";
            this.AccountNumber = massageData(account.Number, 10);
            this.Naam = massageData(primary.CurrentNAW.Name, 49);

            this.Voorletters = massageData(isPerson ? ((IContactPerson)primary).FirstName : "", 5);
            this.Voorvoegsels = massageData(isPerson ? ((IContactPerson)primary).MiddleName : "",8);
            this.Straatnaam = massageData(primary.CurrentNAW.ResidentialAddress.Street, 25);
            this.Huisnummer = massageData(primary.CurrentNAW.ResidentialAddress.HouseNumber,5);
            this.Toevoeging = massageData(primary.CurrentNAW.ResidentialAddress.HouseNumberSuffix,5);
            this.Postcode = massageData(primary.CurrentNAW.ResidentialAddress.PostalCode,6);
            this.Plaatsnaam = massageData(primary.CurrentNAW.ResidentialAddress.City,20);

            if ((isPerson) && (primary.GetBirthFounding != null))
            {
                DateTime dob = primary.GetBirthFounding;
                this.Geboortedag = dob.Day.ToString().PadLeft(2, '0');
                this.Geboortemaand = dob.Month.ToString().PadLeft(2, '0');
                string interim = dob.Year.ToString().PadLeft(4, '0');
                this.Geboorteeeuw = interim.Substring(0, 2);
                this.Geboortejaar = interim.Substring(2, 2);
            }
            else
            {
                this.Geboortedag = this.Geboortemaand = this.Geboorteeeuw = this.Geboortejaar = "00";
            }
            this.Rechtsvormcode = isPerson ? "01" : "02";
            this.GezamenlijkBelang = "01";
            this.SoortFonds = "01";
            this.WepValue = Convert.ToInt32(Math.Truncate(endTermValue.ClosingValue.Quantity));
            this.Soortvalutawep = endTermValue.ClosingValue.Underlying.ToCurrency.ToString();
            this.Soortdivrente = "01";
            this.DivrentebedragValue = Convert.ToInt32(endTermValue.InternalDividend.Quantity);
            this.ValutaDivrentebedrag = endTermValue.InternalDividend.Underlying.ToCurrency.ToString();
            this.Typeobligatie = "00";
            this.Typebronbelasting = "01";
            this.BedragbronbelastingValue = Convert.ToInt32(endTermValue.InternalDividendTax.Quantity);
            this.ValutaBedragbronbelasting = endTermValue.InternalDividendTax.Underlying.ToCurrency.ToString();
            this.Valutajaar = endTermValue.EndTermDate.Year.ToString();
            string bsn = primary.GetBSN;
            this.Sofinummer = isPerson ? bsn : "000000000";
            this.KvKnummer = !isPerson ? bsn : "000000000";



        }
        public virtual int Key { get; set; }
        public ICustomerAccount Account { get; set; }
        public IEndTermValue EndTermRecord { get; set; }


        public virtual string RecordType
        {
            get { return recordType; }
            set { recordType = value; }
        }
        public virtual string AccountNumber
        {
            get { return accountNumber.PadRight(10, ' '); }
            set { accountNumber = value; }
        }
        public virtual string Naam
        {
            get { return naam.PadRight(49, ' '); }
            set { naam = value; }
        }
        public virtual string Voorletters
        {
            get { return voorletters.PadRight(5, ' '); }
            set { voorletters = value; }
        }
        public virtual string Voorvoegsels
        {
            get { return voorvoegsels.PadRight(8, ' '); }
            set { voorvoegsels = value; }
        }

        public virtual string Straatnaam
        {
            get { return straatnaam.PadRight(25, ' '); }
            set { straatnaam = value; }
        }

        public virtual string Huisnummer
        {
            get { return huisnummer.PadRight(5, ' '); }
            set { huisnummer = value; }
        }

        public virtual string Toevoeging
        {
            get { return toevoeging.PadRight(5, ' '); }
            set { toevoeging = value; }
        }

        public virtual string Postcode
        {
            get { return postcode.PadRight(6, ' '); }
            set { postcode = value; }
        }

        public virtual string Plaatsnaam
        {
            get { return plaatsnaam.PadRight(20, ' '); }
            set { plaatsnaam = value; }
        }
        public virtual string Geboortedag
        {
            get { return geboortedag != null ? geboortedag.PadLeft(2, '0') : "00"; }
            set { geboortedag = value; }
        }

        public virtual string Geboortemaand
        {
            get { return geboortemaand != null ? geboortemaand.PadLeft(2, '0') : "00"; }
            set { geboortemaand = value; }
        }

        public virtual string Geboorteeeuw
        {
            get { return geboorteeeuw != null ? geboorteeeuw.PadLeft(2, '0') : "00"; }
            set { geboorteeeuw = value; }
        }

        public virtual string Geboortejaar
        {
            get { return geboortejaar != null ? geboortejaar.PadLeft(2, '0') : "00"; }
            set { geboortejaar = value; }
        }
        public virtual string Rechtsvormcode
        {
            get { return rechtsvormcode.PadLeft(2, '0'); }
            set { rechtsvormcode = value; }
        }

        public virtual string GezamenlijkBelang
        {
            get { return gezamenlijkBelang.PadLeft(2, '0'); }
            set { gezamenlijkBelang = value; }
        }

        public virtual string SoortFonds
        {
            get { return soortFonds.PadLeft(2, '0'); }
            set { soortFonds = value; }
        }


        public virtual int WepValue
        {
            get { return wepValue; }
            set { wepValue = value; }
        }

        public virtual string WEP
        {
            get { return PadValueFields(wepValue, 16); }
        }

        public virtual string Soortvalutawep
        {
            get { return soortvalutawep.PadRight(3, ' '); }
            set { soortvalutawep = value; }
        }

        public virtual string Soortdivrente
        {
            get { return soortdivrente.PadRight(2, ' '); }
            set { soortdivrente = value; }
        }


        public virtual int DivrentebedragValue
        {
            get { return divrentebedragValue; }
            set { divrentebedragValue = value; }
        }


        public virtual string Divrentebedrag
        {
            get { return PadValueFields(divrentebedragValue, 13); ; }
        }

        public virtual string ValutaDivrentebedrag
        {
            get { return valutaDivrentebedrag.PadRight(3, ' '); }
            set { valutaDivrentebedrag = value; }
        }

        public virtual string Typeobligatie
        {
            get { return typeobligatie.PadLeft(2, '0'); }
            set { typeobligatie = value; }
        }

        public virtual string Typebronbelasting
        {
            get { return typebronbelasting.PadLeft(2, '0'); }
            set { typebronbelasting = value; }
        }


        public virtual int BedragbronbelastingValue
        {
            get { return bedragbronbelastingValue; }
            set { bedragbronbelastingValue = value; }
        }

        public virtual string Bedragbronbelasting
        {
            get { return PadValueFields(bedragbronbelastingValue, 13); ; }
        }

        public virtual string ValutaBedragbronbelasting
        {
            get { return valutaBedragbronbelasting.PadRight(3, ' '); }
            set { valutaBedragbronbelasting = value; }
        }

        public virtual string Valutajaar
        {
            get { return valutajaar.PadRight(4, ' '); }
            set { valutajaar = value; }
        }

        public virtual string Sofinummer
        {
            get { return sofinummer.PadLeft(9, '0'); }
            set { sofinummer = value; }
        }

        public virtual string KvKnummer
        {
            get { return kvKnummer.PadLeft(10, '0'); }
            set { kvKnummer = value; }
        }

        public virtual string Correctie
        {
            get { return correctie.PadLeft(1, '0'); }
            set { correctie = value; }
        }

        public virtual string Reserve
        {
            get { return new String(' ', 27); }

        }

        public virtual IDividWepFile ParentFile { get; set; }

        public virtual string SingleRecord
        {
            get
            {
                StringBuilder returnValue = new StringBuilder();
                returnValue.Append(this.RecordType);
                returnValue.Append(this.AccountNumber);
                returnValue.Append(this.Naam);
                returnValue.Append(this.Voorletters);
                returnValue.Append(this.Voorvoegsels);
                returnValue.Append(this.Straatnaam);
                returnValue.Append(this.Huisnummer);
                returnValue.Append(this.Toevoeging);
                returnValue.Append(this.Postcode);
                returnValue.Append(this.Plaatsnaam);
                returnValue.Append(this.Geboortedag);
                returnValue.Append(this.Geboortemaand);
                returnValue.Append(this.Geboorteeeuw);
                returnValue.Append(this.Geboortejaar);
                returnValue.Append(this.Rechtsvormcode);
                returnValue.Append(this.GezamenlijkBelang);
                returnValue.Append(this.SoortFonds);
                returnValue.Append(this.WEP);
                returnValue.Append(this.Soortvalutawep);
                returnValue.Append(this.Soortdivrente);
                returnValue.Append(this.Divrentebedrag);
                returnValue.Append(this.ValutaDivrentebedrag);
                returnValue.Append(this.Typeobligatie);
                returnValue.Append(this.Typebronbelasting);
                returnValue.Append(this.Bedragbronbelasting);
                returnValue.Append(this.ValutaBedragbronbelasting);
                returnValue.Append(this.Valutajaar);
                returnValue.Append(this.Sofinummer);
                returnValue.Append(this.KvKnummer);
                returnValue.Append(this.Correctie);
                returnValue.Append(this.Reserve);
                return returnValue.ToString();
            }



        }

        private string massageData(string input, int maxlength)
        {
            string returnValue = "";
            if (input == null)
                returnValue = "";
            else
            {
                returnValue = Utils.Util.ConvertToAscii(input);
                if (returnValue.Length > maxlength) returnValue = returnValue.Substring(0, maxlength);
            }


            return returnValue;

        }

        private string PadValueFields(int fieldValue, Int16 TotalLength)
        {
            StringBuilder retunValue;
            bool IsPositive = (fieldValue >= 0);

            int tempfieldValue = Math.Abs(fieldValue);

            int lastPlace = tempfieldValue % 10;
            retunValue = new StringBuilder().Append((tempfieldValue.ToString().Substring(0, tempfieldValue.ToString().Length - 1)));
            retunValue.Append(SpecialChar(lastPlace, IsPositive));

            return retunValue.ToString().PadLeft(TotalLength, '0');
        }

        private string SpecialChar(int lastChar, bool IsPositive)
        {
            string returnValue = "{";

            switch (IsPositive)
            {
                case true:

                    switch (lastChar)
                    {
                        case 0:
                            returnValue = "{";
                            break;
                        case 1:
                            returnValue = "A";
                            break;
                        case 2:
                            returnValue = "B";
                            break;
                        case 3:
                            returnValue = "C";
                            break;
                        case 4:
                            returnValue = "D";
                            break;
                        case 5:
                            returnValue = "E";
                            break;
                        case 6:
                            returnValue = "F";
                            break;
                        case 7:
                            returnValue = "G";
                            break;
                        case 8:
                            returnValue = "H";
                            break;
                        case 9:
                            returnValue = "I";
                            break;
                        default:
                            break;
                    }

                    break;

                case false:
                    switch (lastChar)
                    {
                        case 0:
                            returnValue = "}";
                            break;
                        case 1:
                            returnValue = "J";
                            break;
                        case 2:
                            returnValue = "K";
                            break;
                        case 3:
                            returnValue = "L";
                            break;
                        case 4:
                            returnValue = "M";
                            break;
                        case 5:
                            returnValue = "N";
                            break;
                        case 6:
                            returnValue = "O";
                            break;
                        case 7:
                            returnValue = "P";
                            break;
                        case 8:
                            returnValue = "Q";
                            break;
                        case 9:
                            returnValue = "R";
                            break;
                        default:
                            break;
                    }
                    break;

            }

            return returnValue;
        }






        private string recordType = "";
        private string accountNumber = "";
        private string naam = "";
        private string voorletters = "";
        private string voorvoegsels = "";
        private string straatnaam = "";
        private string huisnummer = "";
        private string toevoeging = "";
        private string postcode = "";
        private string plaatsnaam = "";
        private string geboortedag = "";
        private string geboortemaand = "";
        private string geboorteeeuw = "";
        private string geboortejaar = "";
        private string rechtsvormcode = "";
        private string gezamenlijkBelang = "";
        private string soortFonds = "";
        private int wepValue = 0;
        private string soortvalutawep = "";
        private string soortdivrente = "";
        private int divrentebedragValue = 0;
        private string valutaDivrentebedrag = "";
        private string typeobligatie = "";
        private int bedragbronbelastingValue = 0;
        private string typebronbelasting = "";
        private string valutaBedragbronbelasting = "";
        private string valutajaar = "";
        private string sofinummer = "";
        private string kvKnummer = "";
        private string correctie = "";

    }
}
