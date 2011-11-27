using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Reports;
using B4F.TotalGiro.ApplicationLayer.Reports;
using B4F.TotalGiro.Valuations.ReportedData;


namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public static class ReportLetterEditAdapter
    {
        public static int GetCurrentManagementCompanyId()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                IManagementCompany company = LoginMapper.GetCurrentManagmentCompany(session);
                if (company != null)
                    return company.Key;
                else
                    throw new ApplicationException("Could not find current management company.");
            }
            finally
            {
                session.Close();
            }
        }

        public static bool GetLatestReportLetterDetails(int managementCompanyId, int reportLetterYear, string reportLetterTypeName,
                                                        out string concerning, out string description)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            
            try
            {
                ReportLetterTypes reportLetterType = ReportLetterMapper.GetReportLetterType(reportLetterTypeName);

                IReportLetter reportLetter = ReportLetterMapper.GetLatestReportLetter(session, managementCompanyId, reportLetterType, reportLetterYear);
                if (reportLetter != null)
                {
                    concerning = reportLetter.Concern;
                    description = reportLetter.Letter;
                    return true;
                }

                concerning = "";
                description = "";
                return false;
            }
            finally
            {
                session.Close();
            }
        }

        public static DateTime[] GetBeginAndEndDatesForReporting(string kindsOfReport, int year)
        {
            DateTime[] returnValue = new DateTime[2];

            switch (kindsOfReport)
            {
                case "EOY":
                    returnValue = EndTermValueMapper.GetEndDates(EndTermType.FullYear, year);
                    break;
                case "Q1":
                    returnValue = EndTermValueMapper.GetEndDates(EndTermType.FullYear, year);
                    break;
                case "Q2":
                    returnValue = EndTermValueMapper.GetEndDates(EndTermType.FullYear, year);
                    break;
                case "Q3":
                    returnValue = EndTermValueMapper.GetEndDates(EndTermType.FullYear, year);
                    break;
                case "Q4":
                    returnValue = EndTermValueMapper.GetEndDates(EndTermType.FullYear, year);
                    break;  
                default:
                    break;
            }
            return returnValue;
        }


        public static bool AddReportLetter(string reportLetterTypeName, int year, int managementCompanyId, string concerning, string description)
        {
            bool blnSaveSuccess;
            DateTime now = DateTime.Now;
            IDalSession session = NHSessionFactory.CreateSession();

            IReportLetter reportLetter = new ReportLetter();
            reportLetter.ReportLetterTypeId = (int)ReportLetterMapper.GetReportLetterType(reportLetterTypeName);
            reportLetter.ReportLetterYear = year;
            reportLetter.Concern = concerning;
            reportLetter.Letter = description;

            // ManagementCompany
            IManagementCompany managementCompany = null;
            managementCompany = ManagementCompanyMapper.GetManagementCompany(session, managementCompanyId) as IManagementCompany;
            reportLetter.ManagementCompany = managementCompany;

            // Employee
            InternalEmployeeLogin employeeId = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            reportLetter.EmployeeID = employeeId.Key;

            blnSaveSuccess = ReportLetterMapper.Insert(session, reportLetter);

            session.Close();
            return blnSaveSuccess;
        }

    }
}

