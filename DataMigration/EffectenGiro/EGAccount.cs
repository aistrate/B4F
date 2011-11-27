using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace B4F.DataMigration.EffectenGiro
{
    public class EGAccount : IEGAccount

    {

        public string Nummer { get; set; }
        public string Bank { get; set; }
        public int LoginId { get; set; }
        public IEGAanvraag AccountRequest { get; set; }

        public string NummerPreFix
        {
            get
            {
                string prefix = "";
                Match match = Regex.Match(this.Nummer, @"^[a-zA-Z]+", RegexOptions.IgnoreCase);
                if (match.Success)
                    prefix = match.Groups[0].Value;
                return prefix;
            }
        }

        public B4F.TotalGiro.Accounts.IAccount TGAccount { get; set; }
        public B4F.TotalGiro.Stichting.IManagementCompany AssetManager { get; set; }

        public override string ToString()
        {
            return this.Nummer.ToString();
        }
    }
}
