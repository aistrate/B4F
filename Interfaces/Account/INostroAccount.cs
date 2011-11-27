using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Accounts.NostroAccount">NostroAccount</see> class
    /// </summary>
    public interface INostroAccount : IOwnAccount
    {
        Money StornoLimit { get; set; }
        bool VerifyStornoLimit(Money amount, bool raiseException);
    }
}
