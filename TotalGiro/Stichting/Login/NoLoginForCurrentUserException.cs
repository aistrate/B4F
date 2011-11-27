using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public class NoLoginForCurrentUserException : ApplicationException
    {
        public NoLoginForCurrentUserException(string userName)
            : base(string.Format("User '{0}' was authenticated, but no (active) TotalGiro login could be found for it.", userName))
        {
            UserName = userName;
        }

        public string UserName { get; private set; }
    }
}
