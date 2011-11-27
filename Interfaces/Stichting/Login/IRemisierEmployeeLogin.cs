using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.Stichting.Login
{
    public interface IRemisierEmployeeLogin : IExternalLogin
    {
        IRemisierEmployee RemisierEmployee { get; set;}
        bool IsLocalAdministrator { get; set; }
        string FullName { get; }
    }
}
