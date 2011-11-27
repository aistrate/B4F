using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts
{
    public class AccountFamily : IAccountFamily
    {
        public AccountFamily()
        {

        }

        public int Key { get; set; }
        public IAssetManager AssetManager { get; set; }
        public string AccountPrefix { get; set; }
        public int AccountSeed { get; set; }
        public DateTime CreationDate { get; set; }
        public ManagementFeeInstalments ManagementFeeInstalment { get; set; }
        
        public bool IsManagementTypeCharged(ManagementTypes managementType) 
        {
            return Util.IsEnumInValue<ManagementTypes>(this.managementTypesCharged, managementType);
        }

        private int managementTypesCharged;

        public override string ToString()
        {
            return AccountPrefix;
        }
    }
}
