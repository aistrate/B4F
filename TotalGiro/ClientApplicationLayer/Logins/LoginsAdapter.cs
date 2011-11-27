using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Interfaces.Util;
using B4F.TotalGiro.Reports.Letters;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils.Linq;

namespace B4F.TotalGiro.ClientApplicationLayer.Logins
{
    internal enum PasswordEmailType
    {
        FirstTime = 0,
        Reset = 1,
        ChangedByUser = 2
    }

    /// <summary>
    /// Common methods between ClientLoginsAdapter and RemisierLoginsAdapter.
    /// </summary>
    public abstract class LoginsAdapter
    {
        protected abstract LoginPerson GetOwnedLoginPerson(IDalSession session, int personKey);
        protected abstract LoginPerson[] GetLoginPersons(IDalSession session, int[] personKeys);
        protected abstract IExternalLogin CreateBlankLogin();
        protected abstract string PersonType { get; }

        protected void AssertUserHasPermissions(IDalSession session)
        {
            if ((SecurityLayerAdapter.GetCurrentLoginType(session) & LoginTypes.InternalEmployee) != LoginTypes.InternalEmployee)
                throw new SecurityLayerException();
        }

        public void DeleteLogin(int personKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                AssertUserHasPermissions(session);

                LoginPerson loginPerson = GetOwnedLoginPerson(session, personKey);

                deleteLogin(session, loginPerson);
            }
        }

        private void deleteLogin(IDalSession session, LoginPerson loginPerson)
        {
            if (loginPerson.Login.UserName != string.Empty)
                SecurityManager.DeleteUser(loginPerson.Login.UserName);

            session.Delete(loginPerson.Login);
        }

        public void UnlockLogin(int personKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                AssertUserHasPermissions(session);

                LoginPerson loginPerson = GetOwnedLoginPerson(session, personKey);

                SecurityManager.UnlockUser(loginPerson.Login.UserName);
            }
        }

        public void SetActive(int personKey, bool isActive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                AssertUserHasPermissions(session);

                LoginPerson loginPerson = GetOwnedLoginPerson(session, personKey);

                if (loginPerson.PasswordSent)
                {
                    SecurityManager.SetActive(loginPerson.Login.UserName, isActive);

                    loginPerson.Login.IsActive = isActive;
                    LoginMapper.Update(session, loginPerson.Login);
                }
                else
                    throw new ApplicationException("Cannot change Active flag before password gets sent.");
            }
        }

        public void ResetPassword(int personKey)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                AssertUserHasPermissions(session);

                LoginPerson loginPerson = GetOwnedLoginPerson(session, personKey);

                if (loginPerson.IsActive)
                    SetPassword(session, loginPerson, PasswordEmailType.Reset);
                else
                    throw new ApplicationException("Login is not (yet) active.");
            }
        }

        internal void SetPassword(IDalSession session, LoginPerson loginPerson, PasswordEmailType passwordEmailType)
        {
            loginPerson.AssertHasEmail();

            string password = SecurityManager.GeneratePassword(passwordLength);
            SecurityManager.SetPassword(loginPerson.Login.UserName, password);

            setPasswordSent(session, loginPerson.Login, true);

            try
            {
                SendPasswordEmail(loginPerson, password, passwordEmailType);
            }
            catch (Exception ex)
            {
                setPasswordSent(session, loginPerson.Login, false);
                throw new ApplicationException("Error sending e-mail after password was (re)set.", ex);
            }
        }

        internal void SendPasswordEmail(LoginPerson loginPerson, string password, PasswordEmailType passwordEmailType)
        {
            string clientWebsiteUrl = (loginPerson.AssetManager.StichtingDetails.ClientWebsiteUrl ?? "").TrimEnd('/');
            if (string.IsNullOrEmpty(clientWebsiteUrl))
                throw new ApplicationException("Client Website URL not known.");

            string body = passwordEmailTemplate.Replace("<%DearSirForm%>", loginPerson.DearSirForm)
                                               .Replace("<%LoginNameEnding%>",
                                                        loginPerson.Login.UserName.Substring(loginPerson.Login.UserName.Length - 3))
                                               .Replace("<%Password%>", password)
                                               .Replace("<%ClientWebsiteUrl%>", clientWebsiteUrl);

            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-firstTime",
                                                                   passwordEmailType == PasswordEmailType.FirstTime);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-reset",
                                                                   passwordEmailType == PasswordEmailType.Reset);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-changedByUser",
                                                                   passwordEmailType == PasswordEmailType.ChangedByUser);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-not-changedByUser",
                                                                   passwordEmailType != PasswordEmailType.ChangedByUser);

            body = ApplicationLayer.Utility.ShowOptionalTag(body, "loginType-customer",
                                                                   loginPerson.Login.LoginType == LoginTypes.Customer);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "loginType-remisierEmployee",
                                                                   loginPerson.Login.LoginType == LoginTypes.RemisierEmployee);
            
            MailMessage message = new MailMessage();

            loginPerson.AssertHasEmail();
            string testRecipients = ConfigurationManager.AppSettings.Get("TestEmailRecipients");
            message.To.Add(testRecipients == null ? loginPerson.Email : testRecipients);

            message.Subject = "Paerel portefeuille";
            message.Body = body;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        private void setPasswordSent(IDalSession session, IExternalLogin login, bool passwordSent)
        {
            bool isActive = passwordSent;
            SecurityManager.SetActive(login.UserName, isActive);

            login.IsActive = isActive;
            login.PasswordSent = passwordSent;
            LoginMapper.Update(session, login);
        }

        private string generateUniqueUserName(IDalSession session, string loginPersonName, int rootLength, int ordinalLength)
        {
            // Characters that occur in a person's name: ',-./ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzèéöü
            string root = loginPersonName.ToLower().PadRight(rootLength, '0').Substring(0, rootLength);

            string userName = "";
            int maxOrdinal = (int)Math.Pow(10.0, (double)ordinalLength);
            Random rnd = new Random();
            do
            {
                int ordinal = rnd.Next(maxOrdinal / 100 + 1, maxOrdinal);
                userName = root + ordinal.ToString().PadLeft(ordinalLength, '0');
            }
            while (LoginMapper.GetLoginByUserName(session, userName) != null || SecurityManager.UserExists(userName));

            return userName;
        }

        public BatchExecutionResults2 GenerateSendLogins(int[] personKeys)
        {
            BatchExecutionResults2 batchExecutionResults = new BatchExecutionResults2();

            LetterPrintCommand letterPrintCommand = null;
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                letterPrintCommand = new LetterPrintCommand(session, "LetterLoginName", "Letters");
            }

            int[] filteredPersonKeys = getPersonKeysWhere(personKeys, p => p.NeedsLoginSending);

            foreach (int personKey in filteredPersonKeys)
                generateSendLogin(personKey, false, letterPrintCommand, batchExecutionResults);

            return batchExecutionResults;
        }

        private void generateSendLogin(int personKey, bool isActive, LetterPrintCommand letterPrintCommand, 
                                       BatchExecutionResults2 batchExecutionResults)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                string personErrorString = personKey.ToString();

                try
                {
                    AssertUserHasPermissions(session);

                    LoginPerson loginPerson = GetOwnedLoginPerson(session, personKey);
                    personErrorString = string.Format("{0} ('{1}')", loginPerson.PersonKey, loginPerson.FullName);

                    string userName = generateUniqueUserName(session, loginPerson.ShortNameAlphaCharsOnly, 3, 5);
                    string password = SecurityManager.GeneratePassword(passwordLength);

                    string[] initialClientUserRoleList = loginPerson.InitialClientUserRoleList;

                    loginPerson.AssertAddressIsComplete();
                    loginPerson.AssertHasEmail();
                    SecurityManager.CreateUser(userName, password, loginPerson.Email, isActive);

                    IExternalLogin blankLogin = CreateBlankLogin();
                    blankLogin.UserName = userName;
                    blankLogin.IsActive = isActive;
                    loginPerson.Login = blankLogin;
                    LoginMapper.Insert(session, blankLogin);

                    try
                    {
                        foreach (string roleName in initialClientUserRoleList)
                            SecurityManager.AddUserToRole(userName, roleName);

                        letterPrintCommand.PrintLetter(userName, loginPerson);
                    }
                    catch
                    {
                        deleteLogin(session, loginPerson);
                        throw;
                    }

                    batchExecutionResults.MarkSuccess();
                }
                catch (Exception ex)
                {
                    batchExecutionResults.MarkError(
                        new ApplicationException(string.Format("Error sending login name to {0} {1}.", PersonType, personErrorString), ex));
                }
            }
        }

        public BatchExecutionResults2 GenerateSendPasswords(int[] personKeys)
        {
            BatchExecutionResults2 batchExecutionResults = new BatchExecutionResults2();

            int[] filteredPersonKeys = getPersonKeysWhere(personKeys, p => p.NeedsPasswordSending);

            foreach (int personKey in filteredPersonKeys)
                generateSendPassword(personKey, batchExecutionResults);

            return batchExecutionResults;
        }

        private void generateSendPassword(int personKey, BatchExecutionResults2 batchExecutionResults)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                string personErrorString = personKey.ToString();

                try
                {
                    AssertUserHasPermissions(session);

                    LoginPerson loginPerson = GetOwnedLoginPerson(session, personKey);
                    personErrorString = string.Format("{0} ('{1}')", loginPerson.PersonKey, loginPerson.FullName);

                    SetPassword(session, loginPerson, PasswordEmailType.FirstTime);

                    batchExecutionResults.MarkSuccess();
                }
                catch (Exception ex)
                {
                    batchExecutionResults.MarkError(
                        new ApplicationException(string.Format("Error sending password to {0} {1}.", PersonType, personErrorString), ex));
                }
            }
        }

        private int[] getPersonKeysWhere(int[] personKeys, Func<LoginPerson, bool> wherePredicate)
        {
            const int maxSubsequenceLength = 500;

            return personKeys.Split(maxSubsequenceLength)
                             .Select(keySubseq => getPersonsWhere(keySubseq.ToArray(), wherePredicate))
                             .ConcatMany()
                             .OrderBy(p => p.ShortName)
                             .Select(p => p.PersonKey)
                             .ToArray();
        }

        private LoginPerson[] getPersonsWhere(int[] personKeys, Func<LoginPerson, bool> wherePredicate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return GetLoginPersons(session, personKeys).Where(wherePredicate)
                                                           .OrderBy(p => p.ShortName)
                                                           .ToArray();
            }
        }

        private const int passwordLength = 8;
        private static string passwordEmailTemplate = ApplicationLayer.Utility.ReadResource(
                                                            Assembly.GetAssembly(typeof(LoginsAdapter)),
                                                            "B4F.TotalGiro.ClientApplicationLayer.Logins.PasswordEmail.htm");
    }
}
