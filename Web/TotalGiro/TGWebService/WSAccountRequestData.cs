using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using B4F.TotalGiro.CRM;

namespace B4F.Web.TGWebService
{
    [Serializable]
    [XmlRoot("WSAccountRequestData")]
    public class WSAccountRequestData
    {
        private int includeOPALData = 0;
        private string accountnumber = "";
        private WSPerson applicant = null;
        private WSPerson secondapplicant = null;
        private string moneyaccount = "";
        private string moneyaccountholder = "";
        private string moneyaccountbank = "";
        private string moneyaccountbankcity = "";
        private float firstdeposit = 0;
        private bool periodicwithdrawal = false;
        private string periodwithdrawal = "";
        private float periodicwithdrawalamount = 0;
        private bool periodicdeposit = false;
        private string perioddeposit = "";
        private float periodicdepositamount = 0;
        private string ledger = "";
        private string remisier = "";
        private string contactperson = "";
        private string modelportfolio = "";
        private WSQuestionnaire questionnaire = null;
        private WSOPALData opaldata = null;

        [XmlElement("IncludeOPALData")]
        public int IncludeOPALData
        {
            get { return includeOPALData; }
            set { includeOPALData = value; }
        }

        [XmlElement("AccountNumber")]
        public string AccountNumber
        {
            get { return accountnumber; }
            set { accountnumber = value; }
        }

        [XmlElement("Applicant")]
        public WSPerson Applicant
        {
            get { return applicant; }
            set { applicant = value; }
        }

        [XmlElement("SecondApplicant")]
        public WSPerson SecondApplicant
        {
            get { return secondapplicant; }
            set { secondapplicant = value; }
        }

        [XmlElement("MoneyAccountHolder")]
        public string MoneyAccountHolder
        {
            get { return moneyaccountholder; }
            set { moneyaccountholder = value; }
        }

        [XmlElement("MoneyAccount")]
        public string MoneyAccount
        {
            get { return moneyaccount; }
            set { moneyaccount = value; }
        }

        [XmlElement("MoneyAccountBank")]
        public string MoneyAccountBank
        {
            get { return moneyaccountbank; }
            set { moneyaccountbank = value; }
        }

        [XmlElement("MoneyAccountBankCity")]
        public string MoneyAccountBankCity
        {
            get { return moneyaccountbankcity; }
            set { moneyaccountbankcity = value; }
        }

        [XmlElement("FirstDeposit")]
        public float FirstDeposit
        {
            get { return firstdeposit; }
            set { firstdeposit = value; }
        }

        [XmlElement("PeriodicWithdrawal")]
        public bool PeriodicWithdrawal
        {
            get { return periodicwithdrawal; }
            set { periodicwithdrawal = value; }
        }

        [XmlElement("PeriodWithdrawal")]
        public string PeriodWithdrawal
        {
            get { return periodwithdrawal; }
            set { periodwithdrawal = value; }
        }

        [XmlElement("PeriodicWithdrawalAmount")]
        public float PeriodicWithdrawalAmount
        {
            get { return periodicwithdrawalamount; }
            set { periodicwithdrawalamount = value; }
        }

        [XmlElement("PeriodicDeposit")]
        public bool PeriodicDeposit
        {
            get { return periodicdeposit; }
            set { periodicdeposit = value; }
        }

        [XmlElement("PeriodDeposit")]
        public string PeriodDeposit
        {
            get { return perioddeposit; }
            set { perioddeposit = value; }
        }

        [XmlElement("PeriodicDepositAmount")]
        public float PeriodicDepositAmount
        {
            get { return periodicdepositamount; }
            set { periodicdepositamount = value; }
        }

        [XmlElement("Ledger")]
        public string Ledger
        {
            get { return ledger; }
            set { ledger = value; }
        }

        [XmlElement("Remisier")]
        public string Remisier
        {
            get { return remisier; }
            set { remisier = value; }
        }

        [XmlElement("ContactPerson")]
        public string ContactPerson
        {
            get { return contactperson; }
            set { contactperson = value; }
        }

        [XmlElement("ModelPortfolio")]
        public string ModelPortfolio
        {
            get { return modelportfolio; }
            set { modelportfolio = value; }
        }

        [XmlElement("Questionnaire")]
        public WSQuestionnaire Questionnaire
        {
            get { return questionnaire; }
            set { questionnaire = value; }
        }

        [XmlElement("OPALData")]
        public WSOPALData OPALData
        {
            get { return opaldata; }
            set { opaldata = value; }
        }

        /// <summary>
        /// Returns a string with all attributes seperated by a |. This can be used to import into an Excel file
        /// </summary>
        /// <returns>A string containing structured request information</returns>
        public string WriteToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}",
                includeOPALData, accountnumber, applicant.WriteToString(), secondapplicant != null ? secondapplicant.WriteToString() : "",
                    moneyaccount, moneyaccountholder, firstdeposit.ToString(), periodicwithdrawal,
                    periodicwithdrawalamount.ToString(), remisier, contactperson, modelportfolio,
                    questionnaire != null ? questionnaire.WriteToString() : "", 
                    opaldata != null ? "<OPALDATA>" : "");

        }
    }
}
