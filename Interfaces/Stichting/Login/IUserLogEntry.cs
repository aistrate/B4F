using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public interface IUserLogEntry
    {
        int Key { get; set; }
        string UserName { get; }
        DateTime LoginDate { get; }
    }
}
