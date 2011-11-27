using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public class StichtingEmployeeLogin : InternalEmployeeLogin, IStichtingEmployeeLogin
    {
        public override LoginTypes LoginType { get { return LoginTypes.StichtingEmployee; } }
    }
}
