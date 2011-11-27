using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Stichting.Login
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Stichting.Login.InternalEmployeeLogin">Employee</see> class.
    /// </summary>
    public interface IInternalEmployeeLogin : ILogin
    {
        IManagementCompany Employer { get; set; }
        Money StornoLimit { get; set; }
        bool VerifyStornoLimit(Money amount, bool raiseException);
    }
}
