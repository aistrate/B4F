using System;
namespace B4F.DataMigration.SyncAccounts
{
    public interface INewAcctForCleopatra
    {
        string AccNumb { get; set; }
        string AccType { get; set; }
        string AccSub { get; set; }
        string Address1 { get; set; }
        string Address2 { get; set; }
        string Balancyn { get; set; }
        string Brunet { get; set; }
        string City { get; set; }
        string Currac { get; set; }
        string CustGrp { get; set; }
        string InpUser { get; set; }
        int Key { get; set; }
        string NaamKort { get; set; }
        string Name1 { get; set; }
        string Name2 { get; set; }
        string NumCopy1 { get; set; }
        string NumCopy2 { get; set; }
        string NumCopy3 { get; set; }
        string PostalCD { get; set; }
        string SoortRel { get; set; }
        string SwiftAdr { get; set; }
        string Taal { get; set; }
    }
}
