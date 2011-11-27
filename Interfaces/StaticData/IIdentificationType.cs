using System;
namespace B4F.TotalGiro.StaticData
{
    public interface IIdentificationType
    {
        string IdType { get; set; }
        int Key { get; set; }
        string ToString();
    }
}
