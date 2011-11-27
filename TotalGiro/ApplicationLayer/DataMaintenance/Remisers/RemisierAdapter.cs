using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ApplicationLayer.UC;

namespace B4F.TotalGiro.ApplicationLayer.Remisers
{
    public static class RemisierAdapter
    {
        public static DataSet GetRemisiers(int assetManagerId, string remisierName, B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus activeStatus, string propertyList)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            IAssetManager assetManager = null;
            if (!company.IsStichting)
                assetManager = (IAssetManager)company;
            else
            {
                if (assetManagerId != 0)
                    assetManager = ManagementCompanyMapper.GetAssetManager(session, assetManagerId);
            }

            ActivityReturnFilter activityFilter = ActivityReturnFilter.All;
            if (activeStatus != B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.All)
                activityFilter = (activeStatus == B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.Active ? ActivityReturnFilter.Active : ActivityReturnFilter.InActive);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            RemisierMapper.GetRemisiers(session, assetManager, remisierName, activityFilter), propertyList);

            session.Close();
            return ds;
        }

        public static DataSet GetEmployees(int remisierId, B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus activeStatus)
        {
            DataSet ds = null;
            if (remisierId != 0 && remisierId != int.MinValue)
            {
                ActivityReturnFilter activityFilter = ActivityReturnFilter.All;
                if (activeStatus != B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.All)
                    activityFilter = (activeStatus == B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.Active ? ActivityReturnFilter.Active : ActivityReturnFilter.InActive);

                IDalSession session = NHSessionFactory.CreateSession();
                IList employees = RemisierEmployeeMapper.GetRemisierEmployees(session, remisierId, activityFilter);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                            employees,
                                            "Key, Employee.FullName, Employee.LastName, Employee.Gender, Employee.Telephone.Number, Employee.Mobile.Number, Employee.Email, Role, IsActive");
                session.Close();
            }
            return ds;
        }

        public static DataSet GetAssetManagers()
        {
            return AccountFinderAdapter.GetAssetManagers();
        }

        public static DataSet GetRemisierTypes()
        {
            return Util.GetDataSetFromEnum(typeof(RemisierTypes));
        }

        public static DataSet GetEmployeeRoles()
        {
            return Util.GetDataSetFromEnum(typeof(EmployeeRoles));
        }

        public static RemisierDetailsView GetRemisierDetails(int remisierID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                RemisierDetailsView remisierDetailsView = null;
                IRemisier remisier = (IRemisier)RemisierMapper.GetRemisier(session, remisierID);

                if (remisier != null)
                {
                    remisierDetailsView = new RemisierDetailsView(remisier);
                }
                //else
                //    remisierDetailsView = new AccountDetailsView(LoginMapper.GetCurrentManagmentCompany(session).BaseCurrency);

                return remisierDetailsView;
            }
            finally
            {
                session.Close();
            }
        }

        public static RemisierEmployeeDetailsView GetRemisierEmployeeDetails(int employeeID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                RemisierEmployeeDetailsView details = null;
                IRemisierEmployee employee = RemisierEmployeeMapper.GetRemisierEmployee(session, employeeID);

                if (employee != null)
                    details = new RemisierEmployeeDetailsView(employee);
                return details;
            }
            finally
            {
                session.Close();
            }
        }

        public static string GetRemisierName(int remisierID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                string name = "";
                IRemisier remisier = (IRemisier)RemisierMapper.GetRemisier(session, remisierID);
                if (remisier != null)
                    name = remisier.Name;
                return name;
            }
            finally
            {
                session.Close();
            }
        }

        public static void GetCurrentManagmentCompany(ref string name, ref int id)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            name = "";
            id = 0;

            if (company.CompanyName != null)
                name = company.CompanyName;

            id = company.Key;

            session.Close();
        }

        public static int GetCurrentManagmentCompanyId()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                return company.Key;
            }
            finally
            {
                session.Close();
            }
        }

        public static string GetCurrentManagmentCompany(ref string id)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            id = Convert.ToString(company.Key);
            string name = company.CompanyName;
            session.Close();

            return name;
        }

        public static bool IsLoggedInAsAssetManager()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            bool result = LoginMapper.IsLoggedInAsAssetManager(session);
            session.Close();

            return result;
        }

        public static DataSet GetAssetManagerRemisiers(int assetManagerId, RemisierFilterTypes remisierFilterType)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IAssetManager assetManager = null;

            if (assetManagerId > 0)
                assetManager = (IAssetManager)ManagementCompanyMapper.GetAssetManager(session, assetManagerId);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            RemisierMapper.GetRemisiers(session, assetManager, remisierFilterType), "Key, DisplayNameAndRefNumber");

            session.Close();

            Utility.AddEmptyFirstRow(ds.Tables[0]);

            return ds;
        }

        /// <summary>
        /// Save the Remisier
        /// </summary>
        /// <param name="details">The details of the remisier to be saved</param>
        public static bool SaveRemisier(RemisierDetailsView details, out int remisierID)
        {
            bool success = false;
            remisierID = 0;
            IDalSession session = NHSessionFactory.CreateSession();

            IRemisier remisier = null;
            bool isNewRemisier = false;
            if (details.RemisierKey != int.MinValue)
                remisier = (IRemisier)RemisierMapper.GetRemisier(session, details.RemisierKey);
            else
            {
                IAssetManager company = null;
                if (details.CompanyID != int.MinValue)
                    company = ManagementCompanyMapper.GetAssetManager(session, details.CompanyID);
                else
                    company = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);
                remisier = new Remisier(company, details.RemisierType, details.Name);
                isNewRemisier = true;
            }

            remisier.Name =  details.Name;
            remisier.InternalRef = details.InternalRef;
            if (details.OfficeAddress != null && details.OfficeAddress.CountryNeedsInitialization)
                details.OfficeAddress.Country = CountryMapper.GetCountry(session, details.OfficeAddress.CountryId);
            remisier.OfficeAddress = details.OfficeAddress;
            if (details.PostalAddress != null && details.PostalAddress.CountryNeedsInitialization)
                details.PostalAddress.Country = CountryMapper.GetCountry(session, details.PostalAddress.CountryId);
            remisier.PostalAddress = details.PostalAddress;
            remisier.ContactPerson = details.ContactPerson;
            remisier.Email = details.Email;
            remisier.Telephone.Number = details.Telephone;
            remisier.Fax.Number = details.Fax;

            if (!details.IsBankEmpty)
            {
                if (remisier.BankDetails == null)
                    remisier.BankDetails = new BankDetails();

                remisier.BankDetails.BankName = details.BankName;
                remisier.BankDetails.AccountNumber = details.BankAccountNumber;
                remisier.BankDetails.BankAccountName = details.BankAccountName;
                remisier.BankDetails.BankAddress.City = details.BankCity;
                if (details.BankID != int.MinValue)
                    remisier.BankDetails.Bank = BankMapper.GetBank(session, details.BankID);
            }
            else
                remisier.BankDetails = null;

            if (details.ParentRemisierKey.HasValue)
            {
                remisier.ParentRemisier = RemisierMapper.GetRemisier(session, details.ParentRemisierKey.Value);
                remisier.ParentRemisierKickBackPercentage = details.ParentRemisierKickBackPercentage;
            }
            else
            {
                remisier.ParentRemisier = null;
                remisier.ParentRemisierKickBackPercentage = 0M;
            }

            //remisier.ProvisieAfspraak = details.ProvisieAfspraak;
            remisier.DatumOvereenkomst = details.DatumOvereenkomst;
            remisier.NummerOvereenkomst = details.NummerOvereenkomst;
            remisier.NummerAFM = details.NummerAFM;
            remisier.NummerKasbank = details.NummerKasbank;

            if (remisier.IsActive && !details.IsActive)
            {
                if (checkDeleteRemisier(session, remisier))
                    remisier.IsActive = false;
            }
            else if (!remisier.IsActive && details.IsActive)
                remisier.IsActive = true;

            if (isNewRemisier)
                success = RemisierMapper.Insert(session, remisier);
            else
                success = RemisierMapper.Update(session, remisier);

            remisierID = remisier.Key;
            session.Close();
            return success;
        }

        /// <summary>
        /// Set Deleted flag of a remisier
        /// </summary>
        /// <param name="remisierID">The id of the remisier</param>
        public static bool DeleteRemisier(int remisierID)
        {
            bool success = false;
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IRemisier remisier = RemisierMapper.GetRemisier(session, remisierID);
                if (checkDeleteRemisier(session, remisier))
                {
                    remisier.IsActive = false;
                    success = session.Update(remisier);
                }
            }            
            return success;
        }

        /// <summary>
        /// Set Deleted flag of a remisier
        /// </summary>
        /// <param name="remisier">The remisier to inactivate</param>
        private static bool checkDeleteRemisier(IDalSession session, IRemisier remisier)
        {
            bool success = false;
            if (remisier != null)
            {
                // check if remisier is not attached to active accounts
                long attachedAccounts = session.Session.GetNamedQuery(
                    "B4F.TotalGiro.Stichting.Remisier.AccountsAttachedToRemisier")
                    .SetParameter("remisierId", remisier.Key)
                    .UniqueResult<long>();
                if (attachedAccounts > 0)
                    throw new ApplicationException(string.Format("It is not possible to delete remisier {0} since it attached to {1} active accounts.", remisier.Name, attachedAccounts));
                else
                    success = true;
            }
            return success;
        }


        /// <summary>
        /// Save the Remisier Employee
        /// </summary>
        /// <param name="details">The details of the employee to be saved</param>
        public static bool SaveRemisierEmployee(RemisierEmployeeDetailsView details, out int employeeID)
        {
            bool success = false;
            employeeID = 0;
            IDalSession session = NHSessionFactory.CreateSession();

            IRemisierEmployee employee = null;
            bool isNewEmployee = false;
            if (details.EmployeeID != int.MinValue)
                employee = RemisierEmployeeMapper.GetRemisierEmployee(session, details.EmployeeID);
            else
            {
                IRemisier remisier = RemisierMapper.GetRemisier(session, details.RemisierID);
                Person person = new Person();
                person.LastName = details.LastName;
                employee = new RemisierEmployee(remisier, person);
                isNewEmployee = true;
            }

            employee.Employee.Title = details.Title;
            employee.Employee.Gender = details.Gender;
            employee.Employee.LastName = details.LastName;
            employee.Employee.Initials = details.Initials;
            employee.Employee.MiddleName = details.MiddleName;
            employee.Employee.Telephone.Number = details.Telephone;
            employee.Employee.TelephoneAH.Number = details.TelephoneAH;
            employee.Employee.Mobile.Number = details.Mobile;
            employee.Employee.Email = details.Email;
            employee.Role = details.Role;

            if (isNewEmployee)
                success = session.Insert(employee);
            else
                success = session.Update(employee);

            employeeID = employee.Key;
            session.Close();
            return success;
        }

        /// <summary>
        /// Set Deleted flag of an Employee
        /// </summary>
        /// <param name="employeeID">The id of the employee</param>
        public static bool DeleteEmployee(int employeeID)
        {
            bool success = false;
            IDalSession session = NHSessionFactory.CreateSession();

            IRemisierEmployee employee = RemisierEmployeeMapper.GetRemisierEmployee(session, employeeID);
            if (employee != null)
            {
                // check if employee is not attached to active accounts
                long attachedAccounts = session.Session.GetNamedQuery(
                    "B4F.TotalGiro.Stichting.Remisier.AccountsAttachedToRemisierEmployee")
                    .SetParameter("remisierEmployeeId", employeeID)
                    .UniqueResult<long>();
                if (attachedAccounts > 0)
                    throw new ApplicationException(string.Format("It is not possible to delete employee {0} since it attached to {1} active accounts.", employee.Employee.FullName, attachedAccounts));

                employee.IsActive = false;
                success = session.Update(employee);
            }
            
            session.Close();
            return success;
        }
    }
}
