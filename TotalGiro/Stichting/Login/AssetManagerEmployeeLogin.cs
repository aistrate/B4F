using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Stichting.Login
{
    public class AssetManagerEmployeeLogin : InternalEmployeeLogin, IAssetManagerEmployeeLogin
    {
        public override LoginTypes LoginType { get { return LoginTypes.AssetManagerEmployee; } }
    }
}
