using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Utils;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Stichting.Login
{
    /// <summary>
    /// This class is used to instantiate Login and Security objects. 
    /// The data is retrieved from the database using an instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class.
    /// </summary>
    public class LoginMapper
    {
        /// <summary>
        /// Gets the current login object 
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <returns>Login object</returns>
        public static ILogin GetCurrentLogin(IDalSession session)
        {
            string currentUserName = SecurityManager.CurrentUser;

            if (string.IsNullOrEmpty(currentUserName))
                return null;

            ILogin login = GetLoginByUserName(session, currentUserName);
            if (login != null && login.IsActive)
                return login;
            else
            {
                SecurityManager.SignOutUser();
                throw new NoLoginForCurrentUserException(currentUserName);
            }
        }

        /// <summary>
        /// Gets the current employee
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <returns>Employee</returns>
        public static IInternalEmployeeLogin GetCurrentEmployee(IDalSession session)
        {
            ILogin login = GetCurrentLogin(session);
            if (login != null && login.IsActive && login.IsEmployee)
                return (IInternalEmployeeLogin)login;
            else
            {
                SecurityManager.SignOutUser();
                throw new NoLoginForCurrentUserException(SecurityManager.CurrentUser);
            }
        }

        public static ILogin GetLoginByUserName(IDalSession session, string userName)
        {
            List<ILogin> logins = session.GetTypedList<Login, ILogin>(new List<ICriterion>() { Expression.Eq("UserName", userName) });

            if (logins.Count > 1)
                throw new System.Security.SecurityException(string.Format("More than one login for user name '{0}'.", userName));
            
            return logins.FirstOrDefault();
        }

        public static ILogin GetLogin(IDalSession session, int loginId)
        {
            return session.GetTypedList<Login, ILogin>(new List<ICriterion>() { Expression.Eq("Key", loginId) })
                          .FirstOrDefault();
        }

        public static List<IInternalEmployeeLogin> GetEmployees(IDalSession session, ActivityReturnFilter activityFilter)
        {
            List<ICriterion> expressions = new List<ICriterion>();

            if (activityFilter != ActivityReturnFilter.All)
                expressions.Add(Expression.Eq("IsActive", activityFilter == ActivityReturnFilter.Active));
            
            return session.GetTypedList<InternalEmployeeLogin, IInternalEmployeeLogin>(expressions);
        }

        public static int GetFirstUnusedLoginOrdinal(IDalSession session, string userNameRoot)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            expressions.Add(Expression.Like("UserName", userNameRoot, MatchMode.Start));
            IList logins = session.GetList(typeof(Login), expressions);

            int maxOrdinal = 0, rootLength = userNameRoot.Length;
            foreach (ILogin login in logins)
            {
                int ordinal;
                if (int.TryParse(login.UserName.Substring(rootLength), out ordinal) &&
                    maxOrdinal < ordinal)
                    maxOrdinal = ordinal;
            }

            return maxOrdinal + 1;
        }

        /// <summary>
        /// Gets management company instance
        /// </summary>
        /// <param name="session"></param>
        /// <returns>Management company instance</returns>
        public static IManagementCompany GetCurrentManagmentCompany(IDalSession session)
        {
            ILogin login = GetCurrentLogin(session);
            if (login != null && (login.LoginType & LoginTypes.InternalEmployee) == LoginTypes.InternalEmployee)
                return ((IInternalEmployeeLogin)login).Employer;
            else if (login != null && login.LoginType == LoginTypes.RemisierEmployee)
                return ((IRemisierEmployeeLogin)login).RemisierEmployee.Remisier.AssetManager;
            else
                throw new System.Security.SecurityException("No (internal or remisier) employee is currently logged in.");
        }

        public static bool IsLoggedInAsStichting(IDalSession session)
        {
            IInternalEmployeeLogin employee = GetCurrentLogin(session) as InternalEmployeeLogin;
            if (employee != null && employee.LoginType == LoginTypes.StichtingEmployee)
            {
                if (employee.Employer != null && employee.Employer.IsStichting)
                    return true;
                else
                    throw new ApplicationException(
                                    string.Format("Stichting company not found or incorrect for employee login '{0}'.", employee.UserName));
            }
            else
                return false;
        }

        public static bool IsLoggedInAsAssetManager(IDalSession session)
        {
            ILogin login = GetCurrentLogin(session);
            return login != null && login.LoginType == LoginTypes.AssetManagerEmployee;
        }

        public static bool IsLoggedInAsCompliance(IDalSession session)
        {
            ILogin login = GetCurrentLogin(session);
            return login != null && login.LoginType == LoginTypes.ComplianceEmployee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="expressions"></param>
        public static void GetSecurityInfo(IDalSession session, List<ICriterion> expressions)
        {
            GetSecurityInfo(session, expressions, SecurityInfoOptions.Both);
        }

        /// <summary>
        /// Extends expression list with authentication filters
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="expressions">List of filters</param>
        /// <param name="option">Hierarchical filter by sort of company</param>
        public static void GetSecurityInfo(IDalSession session, List<ICriterion> expressions, SecurityInfoOptions option)
        {
            ICriterion crit = null;
            ICriterion crit1 = null;
            ICriterion crit2 = null;
            ILogin login = GetCurrentLogin(session);
            
            if (login != null)
            {
                if (option == SecurityInfoOptions.NoFilter)
                {
                    if (login is AssetManagerEmployeeLogin)
                        option = SecurityInfoOptions.Both;
                    else if (login is CustomerLogin)
                        throw new System.Security.Authentication.AuthenticationException("The no filter option is only available for stichting employees");
                }

                if (login is StichtingEmployeeLogin)
                {
                    switch (option)
                    {
                        case SecurityInfoOptions.ManagedsAcctsOnly:
                            crit1 = Expression.Sql(string.Format("this_.AccountID IN (SELECT AccountID FROM vweInternalAccounts WHERE (ManagementCompanyID = {0}))", ((StichtingEmployeeLogin)login).Employer.Key));
                            crit2 = Expression.Sql(string.Format("this_.AccountID NOT IN (SELECT TradingAccountID FROM ManagementCompanies WHERE (ManagementCompanyID = {0}))", ((StichtingEmployeeLogin)login).Employer.Key));
                            crit = Expression.And(crit1, crit2);
                            break;
                        case SecurityInfoOptions.TradingAcctOnly:
                            crit = Expression.Sql(string.Format("this_.AccountID IN (SELECT TradingAccountID FROM ManagementCompanies WHERE (ManagementCompanyID = {0}))", ((StichtingEmployeeLogin)login).Employer.Key));
                            break;
                        case SecurityInfoOptions.NoFilter:
                            // Do Nothing
                            crit = null;
                            break;
                        default:
                            crit1 = Expression.Sql(string.Format("this_.AccountID IN (SELECT AccountID FROM vweInternalAccounts WHERE (ManagementCompanyID = {0}))", ((StichtingEmployeeLogin)login).Employer.Key));
                            crit2 = Expression.Sql(string.Format("this_.AccountID IN (SELECT TradingAccountID FROM ManagementCompanies WHERE (ManagementCompanyID = {0}))", ((StichtingEmployeeLogin)login).Employer.Key));
                            crit = Expression.Or(crit1, crit2);
                            break;
                    }
                }
                else if (login is AssetManagerEmployeeLogin)
                {
                    crit1 = Expression.Sql(string.Format("this_.AccountID IN (SELECT AccountID FROM vweInternalAccounts WHERE (ManagementCompanyID = {0}))", ((AssetManagerEmployeeLogin)login).Employer.Key));
                    crit2 = Expression.Sql(string.Format("this_.AccountID IN (SELECT TradingAccountID FROM ManagementCompanies WHERE (ManagementCompanyID = {0}))", ((AssetManagerEmployeeLogin)login).Employer.Key));
                    crit = Expression.Or(crit1, crit2);
                }
                else if (login is CustomerLogin)
                {
                    // TODO -> Get Account from Customer, take AssetManager into account
                    crit = null;
                }
            }
            else
                throw new System.Security.Authentication.AuthenticationException("You are not a registered user");

            if (crit != null)
            {
                if (expressions == null)
                    expressions = new List<ICriterion>();
                expressions.Add(crit);
            }
        }

        /// <summary>
        /// Extends expression list with authentication filters
        /// </summary>
        /// <param name="session">Data Access object</param>
        /// <param name="expressions">List of filters</param>
        /// <param name="option">Hierarchical filter by sort of company</param>
        public static string GetSecurityInfoStringForSQL(IDalSession session, SecurityInfoOptions option, string aliasName, string clause)
        {
            List<ICriterion> expressions = new List<ICriterion>();
            string query = "";

            GetSecurityInfo(session, expressions, option);
            if (expressions != null)
            {
                foreach (ICriterion criterion in expressions)
                {
                    query = (query != string.Empty ? " and " : "") + criterion.ToString();
                }
            }

            if (aliasName != string.Empty)
                query = query.Replace("this_.", aliasName + ".");
            if (clause != string.Empty)
                query = " " + clause + " " + query;
            return query;
        }

        #region CRUD

        /// <summary>
        /// Creates a new object in the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Object of type Login</param>
        public static void Insert(IDalSession session, ILogin obj)
        {
            session.Insert(obj);
        }

        /// <summary>
        /// Updates an object, saves its data to the database
        /// </summary>
        /// <param name="session">An instance of the Data Access Library <see cref="T:B4F.TotalGiro.DAL.NHSession">NHSession</see> class</param>
        /// <param name="obj">Object of type Login</param>
        public static void Update(IDalSession session, ILogin obj)
        {
            session.Update(obj);
        }

        #endregion

    }
}
