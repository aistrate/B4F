using System;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.StaticData
{
    public interface IContactDetails
    {
        string Email { get; set; }
        TelephoneNumber Fax { get; set; }
        TelephoneNumber Mobile { get; set; }
        TelephoneNumber Telephone { get; set; }
        TelephoneNumber TelephoneAH { get; set; }
        bool SendNewsItem { get; set; }
        string ToString();
    }
}
