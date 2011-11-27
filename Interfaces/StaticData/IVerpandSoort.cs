using System;
using B4F.TotalGiro.Stichting;
namespace B4F.TotalGiro.StaticData
{
    public interface IVerpandSoort
    {
        string Description { get; set; }
        int Key { get; set; }
        IManagementCompany VerpandOwner { get; set; }
    }
}
