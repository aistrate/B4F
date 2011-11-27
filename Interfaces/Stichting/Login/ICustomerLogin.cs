using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.Stichting.Login
{
    public interface ICustomerLogin : IExternalLogin
    {
        IContact Contact { get; set; }
    }
}
