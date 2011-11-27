using System.Linq;
using B4F.TotalGiro.ClientApplicationLayer.Logins;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ClientApplicationLayer.Authenticate
{
    public enum ResetPasswordResult
    {
        Success,
        ElfproefFailed,
        PersonNotFound,
        NoEmail,
        NoLogin
    }

    public static class ResetPasswordAdapter
    {
        public static ResetPasswordResult ResetPassword(string sofiNumber, string accountNumber)
        {
            if (!Util.PerformElfProefCheck(ElfProefCheckType.BSN, sofiNumber))
                return ResetPasswordResult.ElfproefFailed;

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                // no need for security-filtering here, as this page is used only by anonymous users
                IContactPerson person = ContactMapper.GetContactbyBSN(session, sofiNumber);

                if (person != null &&
                    person.ActiveAccounts.Any(a => a.Number == accountNumber))
                {
                    if (!string.IsNullOrEmpty(person.Email) && person.Email.Trim() != "")
                    {
                        if (person.Login != null && person.Login.IsActive)
                        {
                            ClientLoginsAdapter.GetInstance().SetPassword(session, person.LoginPerson, PasswordEmailType.Reset);
                            return ResetPasswordResult.Success;
                        }
                        else
                            return ResetPasswordResult.NoLogin;
                    }
                    else
                        return ResetPasswordResult.NoEmail;
                }
                else
                    return ResetPasswordResult.PersonNotFound;
            }
        }
    }
}
