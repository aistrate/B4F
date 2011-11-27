using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public class UserLogEntry : IUserLogEntry
    {
        private UserLogEntry() { }

        public UserLogEntry(string userName)
            : this(userName, DateTime.Now) { }

        public UserLogEntry(string userName, DateTime loginDate)
        {
            UserName = userName;
            LoginDate = loginDate;
        }

        public int Key { get; set; }

        public string UserName { get; private set; }

        public DateTime LoginDate { get; private set; }
    }
}
