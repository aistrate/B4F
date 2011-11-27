using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.ApplicationLayer.Remisers
{
    public static class RemisierAdapter
    {
        public static bool IsLoggedInAsStichting()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            bool result = LoginMapper.IsLoggedInAsStichting(session);
            session.Close();

            return result;
        }

        public static DataSet GetEmployees(int remisierID)
        {
            DataSet ds = null;
            if (remisierID != 0 && remisierID != int.MinValue)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                IRemisier remisier = (IRemisier)RemisierMapper.GetRemisier(session, remisierID);
                IList employees = RemisierMapper.GetRemisierEmployees(session, remisier);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                            employees,
                                            "Key, Employee.FullName, Employee.LastName, Employee.Telephone.Number, Employee.Mobile.Number, Employee.Email, IsActive");
                session.Close();
            }
            return ds;
        }

        public static DataSet GetAssetManagers()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (company.CompanyType == ManagementCompanyType.EffectenGiro)
            {
                IEffectenGiro stichting = (IEffectenGiro)company;
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                        ((IEffectenGiro)company).AssetManagers.SortedByDefault().GetList(),
                        "Key, CompanyName");
                Utility.AddEmptyFirstRow(ds.Tables[0]);
            }
            session.Close();
            return ds;
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
        public static bool SaveRemisier(RemisierDetailsView details)
        {
            bool success = false;
            IDalSession session = NHSessionFactory.CreateSession();

            IRemisier remisier = null;
            bool isNewRemisier = false;
            if (details.RemisierKey != int.MinValue)
                remisier = (IRemisier)RemisierMapper.GetRemisier(session, details.RemisierKey);
            else
            {
                remisier = new Remisier();
                isNewRemisier = true;
            }

            remisier.Name =  details.Name;
            remisier.InternalRef = details.InternalRef;
            if (details.OfficeAddress != null && details.OfficeAddress.CountryNeedsInitialization)
                details.OfficeAddress.Country = CountryMapper.GetCountry(session, details.OfficeAddress.CountryID);
            remisier.OfficeAddress = details.OfficeAddress;
            if (details.PostalAddress != null && details.PostalAddress.CountryNeedsInitialization)
                details.PostalAddress.Country = CountryMapper.GetCountry(session, details.PostalAddress.CountryID);
            remisier.PostalAddress = details.PostalAddress;
            remisier.ContactPerson = details.ContactPerson;
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

            if (isNewRemisier)
                success = RemisierMapper.Insert(session, remisier);
            else
                success = RemisierMapper.Update(session, remisier); 
            
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

            IRemisierEmployee employee = RemisierMapper.GetRemisierEmployee(session, employeeID);
            if (employee != null)
            {
                employee.IsActive = false;
                success = session.Update(employee);
            }
            
            session.Close();
            return success;
        }

    }
}
