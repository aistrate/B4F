using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer
{
    public static class CommonAdapter
    {
        public static string GetDatabaseName()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                string databaseName = session.Session.Connection.Database;
                return databaseName;
            }
        }

        public static string GetUserName()
        {
            return SecurityManager.CurrentUser;
        }

        public static bool GetManagementCompanyProperty(string propertyName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool retVal = true;
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company != null && !company.IsStichting)
                {
                    object value = company.GetType().GetProperty(propertyName).GetValue(company, null);
                    if (value != null)
                        retVal = (bool)value;
                }
                return retVal;
            }
        }


        public static bool RunningInDebugMode()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
