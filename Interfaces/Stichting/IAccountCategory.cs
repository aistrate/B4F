using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Stichting
{
    //public enum EnumCustomerType
    //{
    //    Paerel = 1,
    //    EffectenGiro = 2,
    //    CBAM = 3
    //}

    public interface IAccountCategory
    {
        int Key { get; set; }
        IAssetManager AssetManager { get; set; }
        string CustomerType { get; set; }
        string AccountNrPrefix { get; set; }
        int AccountNrFountain { get; set; }
        short AccountNrLength { get; set; }
        string GenerateAccountNumber();
     }
}
