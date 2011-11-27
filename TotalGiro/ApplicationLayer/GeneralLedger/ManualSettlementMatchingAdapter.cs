using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Orders.Transactions;

namespace B4F.TotalGiro.ApplicationLayer.GeneralLedger
{
    public static class ManualSettlementMatchingAdapter
    {

        public static DataSet GetUnmatchedBankSettlements(DateTime startDate, DateTime endDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                //"Key, Parent.TransactionDate, Parent.Journal.BankAccountDescription, Description, OriginalDescription, Debit, DebitDisplayString, DebitQuantity, Credit, CreditDisplayString, CreditQuantity"
                IList<IJournalEntryLine> lines = JournalEntryMapper.GetUnmatchedBankSettlements(session, startDate, endDate);
                if (lines != null && lines.Count > 0)
                {
                    return lines.Select(c => new
                    {
                        c.Key,
                        Parent_TransactionDate =
                            c.Parent.TransactionDate,
                        Parent_Journal_BankAccountDescription =
                            c.Parent.Journal.BankAccountDescription,
                        c.Description,
                        c.OriginalDescription, 
                        c.Debit,
                        DebitDisplayString =
                            c.Debit.DisplayString,
                        DebitQuantity =
                            c.Debit.Quantity,
                        c.Credit,
                        CreditDisplayString =
                            c.Credit.DisplayString,
                        CreditQuantity =
                            c.Credit.Quantity
                    })
                    .ToDataSet();
                }
                else
                    return null;
            }
        }

        public static DataSet GetUnsettledExternalTrades(DateTime startDate, DateTime endDate)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                //"Key, Parent.TransactionDate, GLAccount.FullDescription, Debit, DebitDisplayString, DebitQuantity, Credit, CreditDisplayString, CreditQuantity, TotalGiroOrderID, TotalGiroTXType, TotalGiroInstrumentName, TotalGiroTradeSizeQuantity"
                IList<IOrderExecution> executions = JournalEntryMapper.GetUnsettledExternalTrades(session, startDate, endDate);
                if (executions != null && executions.Count > 0)
                {
                    return executions.Select(c => new
                    {
                        c.Key,
                        c.TransactionDate,
                        c.Description,
                        CounterValue =
                            c.CounterValueSize,
                        CounterValueDisplayString =
                            c.CounterValueSize.DisplayString,
                        CounterValueQuantity =
                            c.CounterValueSize.Quantity,
                        c.TxSide,
                        TotalGiroOrderID =
                            c.Order != null ? c.Order.Key : 0,
                        TotalGiroInstrumentName =
                            (c.TradedInstrument != null ? c.TradedInstrument.Name : ""),
                        TotalGiroTradeSizeQuantity =
                            c.ValueSize.Quantity,
                        CounterParty =
                            c.AccountB.ShortName
                    })
                    .ToDataSet();
                }
                else
                    return null;
            }
        }

        public static DataSet GetUnMappedBankStatements(int[] ids)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                //"Key, Parent.TransactionDate, Parent.Journal.BankAccountDescription, Description, OriginalDescription, Debit, DebitDisplayString, DebitQuantity, Credit, CreditDisplayString, CreditQuantity"
                IList<IJournalEntryLine> lines = JournalEntryMapper.GetUnmatchedBankSettlements(session, ids);
                if (lines != null && lines.Count > 0)
                {
                    return lines.Select(c => new
                    {
                        c.Key,
                        Parent_TransactionDate =
                            c.Parent.TransactionDate,
                        Parent_Journal_BankAccountDescription =
                            c.Parent.Journal.BankAccountDescription,
                        c.Description,
                        c.OriginalDescription,
                        c.Debit,
                        DebitDisplayString =
                            c.Debit.DisplayString,
                        DebitQuantity =
                            c.Debit.Quantity,
                        c.Credit,
                        CreditDisplayString =
                            c.Credit.DisplayString,
                        CreditQuantity =
                            c.Credit.Quantity
                    })
                    .ToDataSet();
                }
                else
                    return null;
            }
        }

        public static DataSet GetUnsettledTrades(int[] ids)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                //"Key, Parent.TransactionDate, GLAccount.FullDescription, Debit, DebitDisplayString, DebitQuantity, Credit, CreditDisplayString, CreditQuantity, TotalGiroOrderID, TotalGiroTXType, TotalGiroInstrumentName, TotalGiroTradeSizeQuantity"
                IList<IOrderExecution> executions = JournalEntryMapper.GetUnsettledExternalTrades(session, ids);
                if (executions != null && executions.Count > 0)
                {
                    return executions.Select(c => new
                    {
                        c.Key,
                        c.TransactionDate,
                        c.Description,
                        CounterValue =
                            c.CounterValueSize,
                        CounterValueDisplayString =
                            c.CounterValueSize.DisplayString,
                        CounterValueQuantity =
                            c.CounterValueSize.Quantity,
                        c.TxSide,
                        TotalGiroOrderID =
                            c.Order != null ? c.Order.Key : 0,
                        TotalGiroInstrumentName =
                            (c.TradedInstrument != null ? c.TradedInstrument.Name : ""),
                        TotalGiroTradeSizeQuantity =
                            c.ValueSize.Quantity
                    })
                    .ToDataSet();
                }
                else
                    return null;
            }
        }

        public static bool ProcessExternalSettlementAuto()
        {
            //IDalSession session;

            //session = NHSessionFactory.CreateSession();
            //IList listofSettlements = JournalEntryMapper.GetUnmatchedBankSettlementKeys(session);
            //session.Close();

            //foreach (int lineid in listofSettlements)            
            //{

            //    session = NHSessionFactory.CreateSession();

            //    //Get The Line Entry
            //    int[] tradeStatementIDs = null;
            //    int[] lines = new int[] { lineid };
            //    IList<IJournalEntryLine> bankStatements = JournalEntryMapper.GetUnmatchedBankSettlements(session, lines);
            //    IJournalEntryLine line = bankStatements[0];

            //    //Search for the Trades
            //    IList<IJournalEntryLine> trade = JournalEntryMapper.GetTradeToSettleExternally(session, line.Balance, line.Parent.TransactionDate);
            //    if ((trade != null) && (trade.Count == 1))
            //    {
            //        tradeStatementIDs = new int[] { trade[0].Key };
            //        IList<IGlTradeTransaction> tradeStatements = JournalEntryMapper.GetUnsettledExternalTradeEntriesByID(session, tradeStatementIDs);
            //    }

            //    session.Close();
            //    if (tradeStatementIDs != null)  CreateExternalSideSettlement(tradeStatementIDs, lines);
            //}

            return true;
        }

        public static bool CreateExternalSideSettlement(int[] tradeStatementIDs, int[] bankStatementIDs)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<ITradingJournalEntry> tradeStatements = JournalEntryMapper.GetUnsettledExternalTradeEntriesByID(session, tradeStatementIDs);
                IList<IJournalEntryLine> bankStatements = JournalEntryMapper.GetUnmatchedBankSettlements(session, bankStatementIDs);

                CreateExternalSideSettlement(session, tradeStatements, bankStatements);

                return true;
            }
        }

        public static bool CreateExternalSideSettlement(IDalSession session, IList<ITradingJournalEntry> tradeStatements, IList<IJournalEntryLine> bankStatements)
        {
            if (((tradeStatements != null) && (tradeStatements.Count > 0)) && ((bankStatements != null) && (bankStatements.Count > 0)))
            {
                DateTime settlementDate = (bankStatements[0].BookDate > tradeStatements[0].TransactionDate) ? bankStatements[0].BookDate : tradeStatements[0].TransactionDate;
                ICurrency currency = (ICurrency)bankStatements[0].Balance.Underlying;
                ITradeableInstrument instrument = (ITradeableInstrument)tradeStatements[0].TradeSize.Underlying;
                IGLAccount settleDiffGLAccount = GLAccountMapper.GetSettlementDifferenceGLAccount(session, currency, instrument);
                IJournal journal = JournalMapper.GetSettlementDifferenceJournal(session);
                string nextJournalEntryNumber = JournalEntryMapper.GetNextJournalEntryNumber(session, journal);

                IExternalSettlement settlement = new ExternalSettlement(bankStatements, tradeStatements, settlementDate);
                if (settlement.Settle(journal, nextJournalEntryNumber, settleDiffGLAccount))
                    session.InsertOrUpdate(settlement);
            }
            return true;
        }

        //private static bool foreigncashcheck(IDalSession session, IList<ITradingJournalEntry> tradeLines, IList<IJournalEntryLine> bankStatements, out bool IsForeignCash)
        //{
        //    bool result = false;
        //    IsForeignCash = false;

        //    var BankCurrencies = from c in bankStatements
        //                         group c by c.Balance.Underlying into g
        //                         select new
        //                         {
        //                             Key = g.Key,
        //                             Records = g
        //                         };

        //    var TradeCurrencies = from c in tradeLines
        //                          group c by c.Balance.Underlying into g
        //                          select new
        //                          {
        //                              Key = g.Key,
        //                              Records = g
        //                          };


        //    if ((BankCurrencies.Count() > 1) || (TradeCurrencies.Count() > 1))
        //    {
        //        result = false;
        //    }
        //    else
        //    {
        //        IsForeignCash = (((ICurrency)BankCurrencies.ElementAt(0).Key) != ((ICurrency)TradeCurrencies.ElementAt(0).Key));
        //        result = true;
        //    }

        //    return result;
        //}

        //private static bool createExternalSideSettlement(IList<IJournalEntryLine> lines, IExternalSettlement settlement, IJournal externalSettled, IDalSession session)
        //{

        //    foreach (IJournalEntryLine jel in lines)
        //    {
        //        //addBookingLine(settlement, jel.GLAccount, jel.Balance, jel.TotalGiroTradeDetails, jel.GiroAccount, true);
        //        //addBookingLine(settlement, jel.GLAccount.GLSettledAccount, jel.Balance.Negate(), jel.TotalGiroTradeDetails, jel.GiroAccount, true);

        //        jel.IsSettledStatus = true;
        //    }

        //    return true;

        //}

        //private static bool addBookingLine(IJournalEntry journalEntry, IGLAccount glAccount, Money balance, ITradeDetails tradeDetails, IAccountTypeInternal giroAccount, bool settledStatus)
        //{
        //    IJournalEntryLine newLine1 = new JournalEntryLine();
        //    newLine1.GLAccount = glAccount;
        //    newLine1.Balance = balance.Negate();
        //    newLine1.TotalGiroTradeDetails = tradeDetails;
        //    if (giroAccount != null) newLine1.GiroAccount = giroAccount;
        //    newLine1.Status = JournalEntryLineStati.Booked;
        //    newLine1.IsSettledStatus = settledStatus;
        //    journalEntry.Lines.Add(newLine1);

        //    return true;
        //}

        //private static bool createForeignCashBooking(Money euroAmount, Money foreignAmount, IExternalSettlement settlement, IDalSession session, ITradeDetails totalGiroTradeDetails)
        //{
        //    //Assumption made through previous check , that only one entry in each list
        //    int intDefaultCostOfFXPositionInEur = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultCostOfFXPositionInEur")));
        //    IGLAccount DefaultCostOfFXPositionInEur = GLAccountMapper.GetGLAccount(session, intDefaultCostOfFXPositionInEur);

        //    int intDefaultCostOfFXPositionInOrig = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultCostOfFXPositionInOrig")));
        //    IGLAccount DefaultCostOfFXPositionInOrig = GLAccountMapper.GetGLAccount(session, intDefaultCostOfFXPositionInOrig);

        //    int intDefaultSettlementAccount = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultSettlementAccount")));
        //    IGLAccount DefaultSettlementAccount = GLAccountMapper.GetGLAccount(session, intDefaultSettlementAccount);

        //    addBookingLine(settlement, DefaultSettlementAccount, euroAmount, totalGiroTradeDetails, null, true);
        //    addBookingLine(settlement, DefaultCostOfFXPositionInEur, euroAmount.Negate(), totalGiroTradeDetails, null, true);
        //    addBookingLine(settlement, DefaultSettlementAccount, foreignAmount, totalGiroTradeDetails, null, true);
        //    addBookingLine(settlement, DefaultCostOfFXPositionInOrig, foreignAmount.Negate(), totalGiroTradeDetails, null, true);

        //    return true;
        //}

        //private static bool createBookingBalance(Money bankAmount, Money tradeAmount, IExternalSettlement settlement, IDalSession session)
        //{
        //    Money difference = bankAmount + tradeAmount;

        //    int intDefaultSettlementDifferenceAccount = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultSettlementDifferenceAccount")));
        //    IGLAccount DefaultSettlementDifferenceAccount = GLAccountMapper.GetGLAccount(session, intDefaultSettlementDifferenceAccount);

        //    int intDefaultSettlementAccount = int.Parse((string)(System.Configuration.ConfigurationManager.AppSettings.Get("DefaultSettlementAccount")));
        //    IGLAccount DefaultSettlementAccount = GLAccountMapper.GetGLAccount(session, intDefaultSettlementAccount);

        //    addBookingLine(settlement, DefaultSettlementAccount, difference, null, null, true);
        //    addBookingLine(settlement, DefaultSettlementDifferenceAccount, difference.Negate(), null, null, true);
        //    return true;
        //}
    }
}
