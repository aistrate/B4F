namespace B4F.TotalGiro.Stichting.Login
{
    public interface IExternalLogin : ILogin
    {
        bool PasswordSent { get; set; }
    }
}
