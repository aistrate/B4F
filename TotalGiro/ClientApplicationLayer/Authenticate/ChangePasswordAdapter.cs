using System;
using B4F.TotalGiro.ClientApplicationLayer.Logins;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ClientApplicationLayer.Authenticate
{
    public static class ChangePasswordAdapter
    {
        public static void AssertIsLoggedIn()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ILogin login = LoginMapper.GetCurrentLogin(session);
                if (login == null)
                    throw new ApplicationException("No valid user is currently logged in.");
            }
        }

        public static bool SendPasswordEmail(string password)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                LoginPerson loginPerson = SecurityFilter.GetCurrent(session).CurrentLoginPerson;

                if (loginPerson != null)
                {
                    LoginsAdapter loginsAdapter;

                    switch (loginPerson.Login.LoginType)
                    {
                        case LoginTypes.Customer:
                            loginsAdapter = ClientLoginsAdapter.GetInstance();
                            break;
                        case LoginTypes.RemisierEmployee:
                            loginsAdapter = RemisierLoginsAdapter.GetInstance();
                            break;
                        default:
                            throw new SecurityLayerException();
                    }

                    loginsAdapter.SendPasswordEmail(loginPerson, password, PasswordEmailType.ChangedByUser);

                    return true;
                }

                return false;
            }
        }
    }
}
