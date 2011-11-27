using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web.Security;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Security;

namespace B4F.TotalGiro.ApplicationLayer.Admin
{
    public enum PasswordEmailType
    {
        FirstTime = 0,
        Reset = 1,
        ChangedByUser = 2
    }

    public static class UserOverviewAdapter
    {
        public static DataSet GetUsers()
        {
            return Membership.GetAllUsers()
                             .Cast<MembershipUser>()
                             .Select(u => new
                             {
                                 u.IsApproved,
                                 u.UserName,
                                 u.Email,
                                 u.CreationDate,
                                 u.Comment,
                             })
                             .ToDataSet();
        }

        public static void ResetPassword(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);

            assertExists(user);
            assertHasEmail(user);

            if (!user.IsApproved)
                throw new ApplicationException("Cannot reset password because user is inactive.");

            SecurityManager.UnlockUser(user.UserName);

            string password = SecurityManager.GeneratePassword(passwordLength, passwordNonAlphaNumChars);

            SecurityManager.SetPassword(user.UserName, password);

            SendPasswordEmail(user, password, PasswordEmailType.Reset);
        }

        public static void SendPasswordEmail(string userName, string password, PasswordEmailType passwordEmailType)
        {
            SendPasswordEmail(Membership.GetUser(userName), password, passwordEmailType);
        }

        public static void SendPasswordEmail(MembershipUser user, string password, PasswordEmailType passwordEmailType)
        {
            assertExists(user);
            assertHasEmail(user);

            string body = passwordEmailTemplate.Replace("<%LoginName%>", user.UserName)
                                               .Replace("<%Password%>", password);

            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-firstTime",
                                                                   passwordEmailType == PasswordEmailType.FirstTime);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-reset",
                                                                   passwordEmailType == PasswordEmailType.Reset);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-changedByUser",
                                                                   passwordEmailType == PasswordEmailType.ChangedByUser);
            body = ApplicationLayer.Utility.ShowOptionalTag(body, "passwordEmailType-not-changedByUser",
                                                                   passwordEmailType != PasswordEmailType.ChangedByUser);

            MailMessage message = new MailMessage();

            string testRecipients = ConfigurationManager.AppSettings.Get("TestEmailRecipients");
            message.To.Add(testRecipients == null ? user.Email : testRecipients);

            message.Subject = "Your new Total Giro password";
            message.Body = body;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        private static void assertExists(MembershipUser user)
        {
            if (user == null)
                throw new ApplicationException("User could not be found.");
        }

        private static void assertHasEmail(MembershipUser user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new ApplicationException("User does not have an e-mail address.");
        }

        private const int passwordLength = 8;
        private const int passwordNonAlphaNumChars = 1;
        private static string passwordEmailTemplate = ApplicationLayer.Utility.ReadResource(
                                                            Assembly.GetAssembly(typeof(UserOverviewAdapter)),
                                                            "B4F.TotalGiro.ApplicationLayer.Admin.PasswordEmail.htm");
    }
}
