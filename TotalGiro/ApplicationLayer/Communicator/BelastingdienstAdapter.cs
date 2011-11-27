using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Communicator.BelastingDienst;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Valuations.ReportedData;
using System.Data;
using B4F.TotalGiro.Valuations.ReportedData.Reports;

namespace B4F.TotalGiro.ApplicationLayer.Communicator
{
    public static class BelastingdienstAdapter
    {
        public static void CreateDividWepFileforYear(int year)
        {
            ReportingPeriodDetail period = new ReportingPeriodDetail(EndTermType.FourthQtr, year);
            IDalSession session = NHSessionFactory.CreateSession();
            IList<int> listOfAccounts = AccountMapper.GetCustomerAccountKeysActiveforDividWEP(session, period);
            string pathName =  Utility.GetPathFromConfigFile("DividWepFilePath");
            IDividWepFile file = new DividWepFile(year, pathName);
            file.CreateCloseRecord();
            session.InsertOrUpdate(file);
            int fileID = file.Key;
            session.Close();

            foreach (int i in listOfAccounts)
            {
                session = NHSessionFactory.CreateSession();
                ICustomerAccount acct = (ICustomerAccount) AccountMapper.GetAccount(session, i);

                IEndTermValue etv = acct.EndTermValues.Where(e => ((e.EndTermDate.Year == year)
                                       && ((e.TermType == B4F.TotalGiro.Valuations.ReportedData.EndTermType.FullYear) || (e.TermType == B4F.TotalGiro.Valuations.ReportedData.EndTermType.FourthQtr)))).ElementAt(0);
                
                file = DividWepFileMapper.GetDividWepFile(session, fileID);
                IDividWepRecord record = new DividWepRecord(acct, etv);
                string test = record.SingleRecord;
                record.ParentFile = file;
                session.InsertOrUpdate(record);
                etv.DividWepRecord = record;
                session.InsertOrUpdate(etv);
                session.Close();
            }
            session = NHSessionFactory.CreateSession();
            file = DividWepFileMapper.GetDividWepFile(session, fileID);
            file.CreateCloseRecord();
            file.CreateOutputFile();
            session.InsertOrUpdate(file);
            session.Close();


        }

        public static DataSet GetDividWepFiles()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return DividWepFileMapper.GetDividWepFiles(session)
                    .Select(f => new
                    {
                        f.Key,
                        f.FileName,
                        f.CodeFinance,
                        f.InstelRecord,
                        f.SluitRecord,
                        f.TotalWep,
                        f.FinancialYear
                    }).ToDataSet();

            }

        }

        public static DataSet GetDividWepFile(int dividWepFileID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return (new List<IDividWepFile>() { DividWepFileMapper.GetDividWepFile(session, dividWepFileID) })
                    .Select(f => new
                    {
                        f.Key,
                        f.FileName,
                        f.CodeFinance,
                        f.InstelRecord,
                        f.SluitRecord,
                        f.TotalWep,
                        f.FinancialYear,
                        f.TotalDividend,
                        f.TotalTax
                    }).ToDataSet();

            }

        }
        public static DataSet GetEndValueDividWepComparison(int year)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ReportingPeriodDetail detail = new ReportingPeriodDetail(EndTermType.FourthQtr, year);
                IList<IEndTermValue> endValues = EndTermValueMapper.GetYearEndValues(session, detail.GetEndDate());

                IReportEndTermDividWep reportData = new ReportEndTermDividWep(detail, endValues);

                return reportData.Records
                    .Select(r => new
                    {
                        r.AccoutNumber,
                        r.AccoutShortName,
                        r.CashValue,
                        r.FundValue,
                        r.FullValue,
                        r.IncludedinDividWep,
                        r.FullValueForDividWep,
                        r.WEP,
                        r.RoundingError,
                        r.ValuesNotIncludedinWEP
                    }).ToDataSet();

            }
        }


        public static IDividWepFile GetLastDividWepFileCreated()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return DividWepFileMapper.GetLastDividWepFileCreated(session);
            }
        }

    }
}
