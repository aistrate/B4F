using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Stichting
{
    public class AccountCategory : IAccountCategory
    {
        public AccountCategory() { }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }	

        public IAssetManager AssetManager
        {
            get { return assetManager; }
            set { assetManager = value; }
        }

        public string AccountNrPrefix
        {
            get { return accountNrPrefix; }
            set { accountNrPrefix = value; }
        }

        public int AccountNrFountain
        {
            get { return accountNrFountain; }
            set { accountNrFountain = value; }
        }

        public short AccountNrLength
        {
            get { return accountNrLength; }
            set { accountNrLength = value; }
        }

        public string GenerateAccountNumber()
        {
            AccountNrFountain++;
            string format = new string('0', AccountNrLength);
            return accountNrPrefix + AccountNrFountain.ToString(format);
        }

        public string CustomerType
        {
            get { return customerType; }
            set { customerType = value; }
        }

        #region Private Variables

        private int key;
        private IAssetManager assetManager;
        private string customerType;
        private string accountNrPrefix;
        private int accountNrFountain;
        private short accountNrLength;

        #endregion

    }
}
