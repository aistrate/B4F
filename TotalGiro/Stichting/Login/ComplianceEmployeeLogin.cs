using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public class ComplianceEmployeeLogin: InternalEmployeeLogin, IComplianceEmployeeLogin
    {
        public override LoginTypes LoginType { get { return LoginTypes.ComplianceEmployee; } }
    }
}
