using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using System.Collections;
using B4F.TotalGiro.Valuations;
using B4F.TotalGiro.ApplicationLayer.UC;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.ApplicationLayer.Test
{
    public static class TestManagementFeeAdapter
    {
        public static DataSet GetValuations(int accountId, DateTime dateFrom, DateTime dateTo)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            AverageHoldingMapper.GetAverageHoldings(
                                session, account, dateFrom, dateTo, ValuationTypesFilterOptions.Security),
                            "Instrument.Name, Period, AverageValue.DisplayString");

            session.Close();

            return ds;
        }

        //public static DataSet GetFeeNotas(int accountId, DateTime dateFrom, DateTime dateTo)
        //{
        //    IDalSession session = NHSessionFactory.CreateSession();

        //    IAccountTypeInternal account = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId);

        //    DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
        //                    NotaMapper.GetNotas(session, NotaReturnClass.NotaFees, account, dateFrom, dateTo),
        //                    "Key, TxStartDate, TxEndDate, TransactionDate, PeriodStartDate, PeriodEndDate, ValueSize.DisplayString");
        //    //INotaFees nf;
        //    //nf.ValueSize.DisplayString;

        //    session.Close();

        //    return ds;
        //}

        public static DataSet GetCustomerAccounts()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetCustomerAccounts(session, 0, "", "")
                    .Select(c => new
                    {
                        c.Key,
                        c.Number, 
                        c.DisplayNumberWithName
                    })
                    .ToDataSet();
            }
        }
    }
}
