using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using System.Collections;
using B4F.TotalGiro.Stichting.Remisier;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class ContactDetailsAdapter
    {
        public static DataSet GetRemisiers()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;

            IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
            if (company != null)
            {
                if (company.CompanyType == ManagementCompanyType.EffectenGiro)
                {
                    IList allRemisierList = RemisierMapper.GetRemisiers(session);
                    ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(allRemisierList, "Key, Name");
                    Utility.AddEmptyFirstRow(ds.Tables[0]);
                }
                else if (company.CompanyType == ManagementCompanyType.AssetManager)
                {
                    IAssetManager am = (IAssetManager)company;
                    if (am.Remisiers != null)
                    {
                        ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(am.Remisiers.ToList<IRemisier>(), "Key, Name");
                        Utility.AddEmptyFirstRow(ds.Tables[0]);
                    }
                }
            }
            session.Close();
            return ds;
        }

        public static DataSet GetRemisierEmployees(int remisierID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            IRemisier rem = RemisierMapper.GetRemisier(session, remisierID);
            if (rem != null && rem.Employees != null)
            {
                IRemisierEmployeesCollection employeeColl = rem.Employees;
                IRemisierEmployee[] employeeList = new IRemisierEmployee[employeeColl.Count];
                employeeColl.CopyTo(employeeList, 0);
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(employeeList, "Key, FullName");
                Utility.AddEmptyFirstRow(ds.Tables[0]);
            }
            return ds;
        }
    }
}
