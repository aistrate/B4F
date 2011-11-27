using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Accounts
{
    public class AccountTradeability
    {
        public Tradeability Key
        {
            get { return (Tradeability)key; }
        }

        public string Name { get; set; }
        private int key;
    }
}
