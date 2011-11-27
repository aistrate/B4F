using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Accounts
{
    public class AccountStatus
    {
        public AccountStati Key
        {
            get { return (AccountStati)key; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }
        
        private int key;
        private string name;
        private bool isOpen;
    }
}
