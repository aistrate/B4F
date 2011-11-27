using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Nav;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using System.Data;
using B4F.TotalGiro.Instruments.ExRates;

namespace B4F.TotalGiro.ApplicationLayer.VirtualFunds
{
    public static class VirtualFundNavOverviewAdapter
    {
        public static INavCalculation GetNavCalculation(int navCalculationID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            INavCalculation calc = NavCalculationMapper.GetNavCalculation(session, navCalculationID);
            INavPortfolio portfolio = calc.Portfolio;
            foreach (INavPosition pos in calc.Portfolio)
            {
                Console.WriteLine(pos.ClosingPriceUsed.ToString());
            }


            session.Close();
            return calc;

        }

        public static IAccountTypeInternal GetAccount(int accountID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IAccountTypeInternal testacc = (IAccountTypeInternal)AccountMapper.GetAccount(session, accountID);
            //if (testacc.GLLedgerBookings != null)
            //{

            //    foreach (IJournalEntryLine jel in testacc.GLLedgerBookings)
            //    {
            //        Console.WriteLine(jel.CreditDisplayString);
            //    }

            //    foreach (IGLPosition pos in testacc.GLCashPortfolio.Positions)
            //    {
            //        Console.WriteLine(pos.ValueSize.ToString());
            //    }

            //    Console.WriteLine(testacc.GLCashPortfolio.CashTotalInBaseValue.ToString());
            //    Console.WriteLine(testacc.GLCashPortfolio.CashTotalUnsettledInBaseValue.ToString());
            //}


            session.Close();
            return testacc;

        }

        public static DataSet GetVirtualFunds()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ISecCategory secCategory = SecCategoryMapper.GetSecCategory(session, SecCategories.VirtualFund);

            IList<IVirtualFund> funds = InstrumentMapper.GetVirtualFunds(session);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            funds.ToList(),
                            @"Key, Name, Isin, LastNavDate");

            session.Close();

            return ds;
        }

        public static int CreateNavCalculation(int fundId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IVirtualFund fund = (IVirtualFund)InstrumentMapper.GetInstrument(session, fundId);
                INavCalculation lastNavCalculation = (INavCalculation)NavCalculationMapper.GetLastNavCalculation(session, fund);

                if (lastNavCalculation != null && lastNavCalculation.Status == NavCalculationStati.New)
                    throw new ApplicationException(
                        string.Format("Please book the last Nav Calcualtion of  '{0}' before creating a new one.", fund.Name));

                INavCalculation newNavCalculation = new NavCalculation(lastNavCalculation, fund);
                NavCalculationDetailsAdapter.AddOrdersToNavCalculation(session, newNavCalculation);
                createJournalEntryForNav(session, newNavCalculation, newNavCalculation.ValuationDate);
                NavCalculationMapper.Insert(session, newNavCalculation);

                //NavCalculationMapper.Update(session, newNavCalculation);

                return newNavCalculation.Key;
            }
            finally
            {
                session.Close();
            }
        }

        private static void createJournalEntryForNav(IDalSession session, INavCalculation calc, DateTime bookingDate)
        {
            IVirtualFund fund = calc.Fund;
            string nextJournalNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, fund.JournalForFund);

            IMemorialBooking newEntry = new MemorialBooking(fund.JournalForFund, nextJournalNumber);
            newEntry.TransactionDate = calc.ValuationDate;
            calc.Bookings = newEntry;
        }
    }
}
