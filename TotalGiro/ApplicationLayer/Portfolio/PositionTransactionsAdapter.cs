using System;
using System.Collections;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.ApplicationLayer.TGTransactions;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using System.Data.SqlTypes;
using System.Collections.Generic;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.ApplicationLayer.BackOffice;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public enum PositionType
    {
        Security = 0,
        CashBaseCurrency,
        CashForeignCurrency
    }

    public static class PositionTransactionsAdapter
    {
        public static DataSet GetPositionTxsSecurity(int positionId)
        {
            if (positionId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IFundPosition position = FundPositionMapper.GetPosition(session, positionId);

                    if (position != null)
                        return GetPositionTxsSecurity(session, position, true);
                    else
                        throw new ApplicationException(string.Format("Could not find portfolio position with ID '{0}'.", positionId));
                }
            }
            return null;
        }

        public static DataSet GetPositionTxsSecurity(IDalSession session, IFundPosition position, bool retrieveNonClientDisplayable)
        {
            return FundPositionMapper.GetPositionTransactions(session, position, retrieveNonClientDisplayable)
                                     .Select(ptx => new
                                     {
                                         ptx.Key,
                                         ptx.IsStornoable,
                                         TransactionId = 
                                            ptx.ParentTransaction.Key, 
                                         ptx.Size, 
                                         ptx.Price,
                                         ptx.PriceShortDisplayString,
                                         ptx.Value, 
                                         ptx.ValueType,
                                         ptx.Side, 
                                         ptx.ParentTransaction.TransactionTypeDisplay,
                                         Description =
                                            ptx.Description + (ptx.Instrument.Key != position.InstrumentOfPosition.Key ? 
                                                                        string.Format(" ({0})", ptx.Instrument.DisplayName) : 
                                                                        ""),
                                         ptx.TransactionDate,
                                         TransactionCreationDate =
                                            ptx.ParentTransaction.CreationDate
                                     })
                                     .ToDataSet();
        }

        

//        public static DataSet GetPositionTxsCashForeignCurrency(int positionId)
//        {
//            IDalSession session = NHSessionFactory.CreateSession();

//            try
//            {
//                IPosition position = FundPortfolioMapper.GetPosition(session, positionId);

//                if (position != null)
//                    return DataSetBuilder.CreateDataSetFromBusinessObjectList(
//                                AccountMapper.GetPositionTransactions(session, position, true),
//                                @"Key, IsStornoable, ParentTransaction.Key, Value, ExchangeRate, IsCV, Side, ParentTransaction.TransactionType, 
//                                  Description, TransactionDate, ParentTransaction.CreationDate");
//                else
//                    throw new ApplicationException(string.Format("Could not find portfolio position with ID '{0}'.", positionId));
//            }
//            finally
//            {
//                session.Close();
//            }
//        }

        public static DataSet GetStornoAccounts(int positionId, int positionTxId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IInternalEmployeeLogin employee = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
            IFundPosition position = FundPositionMapper.GetPosition(session, positionId);
            IList positionTxs = FundPositionMapper.GetPositionTransactions(session, position, new int[] { positionTxId });

            IAccountTypeInternal[] accounts = new IAccountTypeInternal[0];
            if (positionTxs.Count > 0)
                accounts = ((IFundPositionTx)positionTxs[0]).ParentTransaction.GetStornoAccounts(employee.Employer);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(accounts, "Key, DisplayNumberWithName");

            session.Close();

            return ds;
        }

        public static void GetPositionDetails(int positionId, 
                                              out string accountDescription, out string instrumentDescription, out string valueDisplayString)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IFundPosition position = FundPositionMapper.GetPosition(session, positionId);
                accountDescription = string.Format("{0} ({1})", position.Account.Number, position.Account.ShortName);
                instrumentDescription = string.Format("{0} ({1})",
                    (position.InstrumentOfPosition.IsWithPrice ? ((IInstrumentsWithPrices)position.InstrumentOfPosition).Isin : ((ICurrency)position.InstrumentOfPosition).AltSymbol),
                    position.InstrumentOfPosition.DisplayName);
                valueDisplayString = (position.CurrentBaseValue != null ? position.CurrentBaseValue.DisplayString : "");
            }            
        }

        public static Exception[] StornoTransactions(int positionId, int[] positionTxIds, int stornoAccountId, string reason)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ArrayList exceptions = new ArrayList();

                IFundPosition position = FundPositionMapper.GetPosition(session, positionId);
                if (!SecurityManager.IsCurrentUserInRole("Portfolio: Storno Ability"))
                    throw new ApplicationException("Stornoing not allowed.");
                
                IInternalEmployeeLogin employee = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;
                IAccountTypeInternal stornoAccount = (IAccountTypeInternal)AccountMapper.GetAccount(session, stornoAccountId);

                foreach (IFundPositionTx positionTx in FundPositionMapper.GetPositionTransactions(session, position, positionTxIds))
                {
                    try
                    {
                        ITradingJournalEntry tradingJournalEntry = TransactionAdapter.GetNewTradingJournalEntry(session, positionTx.ParentTransaction.CounterValueSize.UnderlyingShortName.ToUpper(), positionTx.ParentTransaction.TransactionDate);
                        ITransaction storno = positionTx.ParentTransaction.Storno(stornoAccount, employee, reason, tradingJournalEntry);

                        bool succes = false;
                        if (storno != null)
                        {
                            succes = storno.Approved;
                            if (!succes)
                                storno.Approve(LoginMapper.GetCurrentEmployee(session));

                            BondCouponAccrualAdapter.StornoBondTransaction(session, storno, employee);
                            succes = TransactionMapper.Update(session, storno);
                        }
                        if (!succes)
                            throw new ApplicationException("Transaction could not be stornoed.");
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new ApplicationException(
                            string.Format("Error stornoing transaction {0}:", positionTx.ParentTransaction.Key), ex));
                    }
                }
                
                return (Exception[])exceptions.ToArray(typeof(Exception));
            }
        }
    }
}
