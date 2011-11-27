using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.ClientApplicationLayer.Logins
{
    public class ClientLoginsAdapter : LoginsAdapter
    {
        #region Instance methods

        protected override LoginPerson GetOwnedLoginPerson(IDalSession session, int personKey)
        {
            return SecurityLayerAdapter.GetOwnedContact(session, personKey).LoginPerson;
        }

        protected override LoginPerson[] GetLoginPersons(IDalSession session, int[] personKeys)
        {
            // no need for security-filtering here, as caller methods always use GetOwnedContact() on each of the returned contactId's
            return ContactMapper.GetContacts(session, personKeys)
                                .Select(c => c.LoginPerson)
                                .ToArray();
        }

        protected override IExternalLogin CreateBlankLogin() { return new CustomerLogin(); }

        protected override string PersonType { get { return "Contact"; } }

        #endregion

        #region Static methods

        public static LoginsAdapter GetInstance() { return new ClientLoginsAdapter(); }

        public static DataSet GetClientContacts(
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName, 
            bool accountStatusActive, bool accountStatusInactive, bool emailStatusYes, bool emailStatusNo,
            bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedContacts(session, assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName,
                                                             emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive,
                                                             accountStatusActive, accountStatusInactive)
                                           .OrderBy(c => c.LoginPerson.ShortName)
                                           .Select(c => new
                                           {
                                               c.LoginPerson.PersonKey,
                                               c.LoginPerson.IsActive,
                                               c.LoginPerson.FullName,
                                               c.LoginPerson.ShortName,
                                               c.LoginPerson.FullAddress,
                                               c.LoginPerson.Email,

                                               DisplayContactType = c.ContactType.ToString(),

                                               c.SendByPost,
                                               c.SendByEmail,

                                               c.LoginPerson.HasLogin,
                                               c.LoginPerson.LoginUserName,
                                               c.LoginPerson.IsLoginActive,
                                               c.LoginPerson.PasswordSent,
                                               c.LoginPerson.NeedsHandling,
                                               LoginPersonStatus = c.LoginPerson.Status
                                           })
                                           .ToDataSet();
            }
        }

        public static Tuple<string, bool>[] GetAccountNumbers(int contactId, bool activeOnly)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedContactAccounts(session, contactId, activeOnly)
                                           .OrderByDescending(a => a.Status == AccountStati.Active)
                                           .ThenBy(a => a.Number)
                                           .Select(a => Tuple.Create(a.Number, a.Status == AccountStati.Active))
                                           .ToArray();
            }
        }

        public static void SetSendByPost(int contactId, bool value)
        {
            setSendingOption(contactId, SendingOptions.ByPost, value);
        }

        public static void SetSendByEmail(int contactId, bool value)
        {
            setSendingOption(contactId, SendingOptions.ByEmail, value);
        }

        private static void setSendingOption(int contactId, SendingOptions sendingOption, bool value)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IContact contact = SecurityLayerAdapter.GetOwnedContact(session, contactId);

                if (contact.ContactSendingOptions.Exists(SendableDocumentCategories.NotasAndQuarterlyReports, sendingOption))
                    contact.ContactSendingOptions.SetValue(SendableDocumentCategories.NotasAndQuarterlyReports, sendingOption, value);
                else
                    contact.ContactSendingOptions.Add(SendableDocumentCategories.NotasAndQuarterlyReports, sendingOption, value);

                session.Update(contact);
            }
        }

        #endregion
    }
}
