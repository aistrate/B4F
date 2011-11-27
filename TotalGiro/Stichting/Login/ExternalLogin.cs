using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public abstract class ExternalLogin : Login
    {
        public virtual bool PasswordSent { get; set; }
    }
}
