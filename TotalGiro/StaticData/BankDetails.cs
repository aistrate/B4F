using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.StaticData
{
    public class BankDetails : IBankDetails
    {
        public string BankName
        {
            get 
            {
                if (Bank != null)
                    return Bank.Name;
                else
                    return bankName; 
            }
            set { bankName = value; }
        }        

        public IBank Bank
        {
            get { return bank; }
            set { bank = value; }
        }        

        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }        

        public string BankAccountName
        {
            get { return bankAccountName; }
            set { bankAccountName = value; }
        }
	

        public Address BankAddress
        {
            get { return bankAddress; }
            set { bankAddress = value; }
        }       

        public Address BankPostalAddress
        {
            get { return bankPostalAddress; }
            set { bankPostalAddress = value; }
        }


        #region Private Variables

        private string bankName;
        private IBank bank;
        private string accountNumber;
        private string bankAccountName;
        private Address bankAddress = new Address();
        private Address bankPostalAddress = new Address();
        
        #endregion



	
    }
}
