using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.Stichting.Login
{
    public class RemisierEmployeeLogin : ExternalLogin, IRemisierEmployeeLogin
    {
        public override LoginTypes LoginType { get { return LoginTypes.RemisierEmployee; } }

        public virtual IRemisierEmployee RemisierEmployee { get; set; }

        public virtual bool IsLocalAdministrator { get; set; }

        public string FullName
        {
            get { return this.RemisierEmployee.Employee.FullName; }
        }
    }
}
