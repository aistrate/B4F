using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Stichting.Login
{
    public class CustomerLogin : ExternalLogin, ICustomerLogin
    {
        public override LoginTypes LoginType { get { return LoginTypes.Customer; } }

        public virtual IContact Contact { get; set; }
    }
}
