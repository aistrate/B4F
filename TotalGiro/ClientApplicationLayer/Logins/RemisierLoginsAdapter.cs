using System.Data;
using System.Linq;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ClientApplicationLayer.Logins
{
    public class RemisierLoginsAdapter : LoginsAdapter
    {
        #region Instance methods

        protected override LoginPerson GetOwnedLoginPerson(IDalSession session, int personKey)
        {
            return SecurityLayerAdapter.GetOwnedRemisierEmployee(session, personKey).LoginPerson;
        }

        protected override LoginPerson[] GetLoginPersons(IDalSession session, int[] personKeys)
        {
            // no need for security-filtering here, as caller methods always use GetOwnedRemisierEmployee() 
            // on each of the returned remisierEmployeeId's
            return RemisierEmployeeMapper.GetRemisierEmployees(session, personKeys)
                                         .Select(c => c.LoginPerson)
                                         .ToArray();
        }

        protected override IExternalLogin CreateBlankLogin() { return new RemisierEmployeeLogin(); }

        protected override string PersonType { get { return "Remisier Employee"; } }

        #endregion

        #region Static methods

        public static LoginsAdapter GetInstance() { return new RemisierLoginsAdapter(); }

        public static DataSet GetRemisierEmployees(int assetManagerId, int remisierId, string remisierEmployeeName,
                                                   bool emailStatusYes, bool emailStatusNo, 
                                                   bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedRemisierEmployees(session,
                                                assetManagerId, remisierId, remisierEmployeeName,
                                                emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive)
                                           .OrderBy(re => re.LoginPerson.ShortName)
                                           .Select(re => new
                                           {
                                               re.LoginPerson.PersonKey,
                                               re.LoginPerson.IsActive,
                                               re.LoginPerson.FullName,
                                               re.LoginPerson.ShortName,
                                               re.LoginPerson.FullAddress,
                                               re.LoginPerson.Email,
                                               
                                               RemisierName = re.Remisier.Name,

                                               //re.IsLocalAdministrator,
                                               re.LoginPerson.IsAddressComplete,
                                               re.LoginPerson.HasLogin,
                                               re.LoginPerson.LoginUserName,
                                               re.LoginPerson.IsLoginActive,
                                               re.LoginPerson.PasswordSent,
                                               re.LoginPerson.NeedsHandling,
                                               LoginPersonStatus = re.LoginPerson.Status
                                           })
                                           .ToDataSet();
            }
        }

        public static void SetLocalAdministrator(int remisierEmployeeId, bool value)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                
            }
        }

        #endregion
    }
}
