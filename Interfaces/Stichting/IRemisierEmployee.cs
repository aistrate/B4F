using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Stichting.Remisier
{
    public enum EmployeeRoles
    {
        Unknown,
        Employee,
        Administrator
    }
    
    public interface IRemisierEmployee
    {
        int Key { get; set; }
        Person Employee { get; set; }
        IRemisier Remisier { get; set; }
        EmployeeRoles Role { get; set; }
        IRemisierEmployeeLogin Login { get; set; }
        RemisierEmployeeLoginPerson LoginPerson { get; }
        bool IsLocalAdministrator { get; }
        bool IsActive { get; set; }
        bool IsDefault { get; set; }
    }
}
