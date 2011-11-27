using System;
using System.Data;
using System.Linq;
using System.Web.Security;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.Admin
{
    public static class UserInsertAdapter
    {
        public static void ActivateUser(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            user.IsApproved = true;
            Membership.UpdateUser(user);
        }

        public static void SetLoginType(string userName, LoginTypes loginType, int managementCompanyId)
        {
            if (Membership.GetUser(userName) == null)
                throw new ApplicationException(string.Format("User '{0}' does not exist", userName));

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Login login = null;

                switch (loginType)
                {
                    case LoginTypes.AssetManagerEmployee:
                        login = new AssetManagerEmployeeLogin();
                        break;
                    case LoginTypes.StichtingEmployee:
                        login = new StichtingEmployeeLogin();
                        break;
                    case LoginTypes.ComplianceEmployee:
                        login = new ComplianceEmployeeLogin();
                        break;
                    default:
                        throw new ApplicationException("Unsupported login type.");
                }

                if (login != null)
                {
                    login.UserName = userName;

                    InternalEmployeeLogin employee = (InternalEmployeeLogin)login;
                    employee.Employer = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId);
                    if (loginType != LoginTypes.ComplianceEmployee)
                        employee.StornoLimit = new Money(0, employee.Employer.BaseCurrency);

                    LoginMapper.Insert(session, login);
                }
            }
        }

        public static DataSet GetAssetManagers()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = ManagementCompanyMapper.GetAssetManagers(session)
                                                    .Select(am => new
                                                    {
                                                        am.Key,
                                                        am.CompanyName,
                                                    })
                                                    .ToDataSet();

                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        public static int GetEffectenGiroId()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return ManagementCompanyMapper.GetEffectenGiroCompany(session).Key;
            }
        }

        public static bool LoginExists(string userName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return LoginMapper.GetLoginByUserName(session, userName) != null;
            }
        }
    }
}
