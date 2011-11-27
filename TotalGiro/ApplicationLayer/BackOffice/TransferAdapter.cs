using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Stichting;
using System.Globalization;
using B4F.TotalGiro.Orders.Transfers;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Instruments.ExRates;
using B4F.TotalGiro.GeneralLedger.Journal;
using B4F.TotalGiro.ApplicationLayer.TGTransactions;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.ApplicationLayer.Reports;
using B4F.TotalGiro.ApplicationLayer.Portfolio;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice
{
    public enum TransferPortfolioType
    {
        PortfolioABefore = 0,
        PortfolioBBefore,
        PortfolioAAfter,
        PortfolioBAfter
    }

    public class TransferAdapter
    {


        public static DataSet GetHistoricalPositions(int accountID, DateTime positionDate)
        {
            return HistoricalPositionAdapter.GetHistoricalPositions(accountID, positionDate);
        }

        public static IList<HistoricalPositionRowView> GetHistoricalPositions(IDalSession session, IPortfolioHistorical portfolio)
        {
            return HistoricalPositionAdapter.GetHistoricalPositions(session, portfolio);
        }

        public static void SetTransferBackToNew(int positionTransferID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {

            }


        }

        public static bool ExecuteTransfer(PositionTransferDetails details)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                bool success = false;
                SavePositionTransferDetails(details);
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, details.Key);

                IPositionTransferDetailCollection transferDetails = transfer.TransferDetails;
                executeFundTransfer(session, transferDetails, transfer);
                session.InsertOrUpdate(transfer);

                if (transfer.AIsInternal)
                    transfer.APortfolioAfter = assemblePortfolio(session, transfer.AccountA.Key, transfer.TransferDate, transfer);

                if (transfer.BIsInternal)
                    transfer.BPortfolioAfter = assemblePortfolio(session, transfer.AccountB.Key, transfer.TransferDate, transfer);

                transfer.TransferStatus = TransferStatus.Executed;
                success = session.InsertOrUpdate(transfer);
                return success;
            }
        }


        private static void executeFundTransfer(IDalSession session, IPositionTransferDetailCollection transferDetails, IPositionTransfer transfer)
        {
            var fundPositions = transferDetails.Where(p => p.IsFundPosition);
            bool transferOut;

            foreach (IPositionTransferDetail detail in fundPositions)
            {
                if (transfer.AIsInternal)
                {
                    transferOut = (detail.TxDirection == TransferDirection.FromAtoB);
                    ITransactionNTM tradeOut = transferPosition(session, detail, transfer.AccountA, transferOut);
                    detail.Transactions.AddTransactionNTM(tradeOut);
                }

                if (transfer.BIsInternal)
                {
                    transferOut = (detail.TxDirection == TransferDirection.FromBtoA);
                    ITransactionNTM tradeIn = transferPosition(session, detail, transfer.AccountB, transferOut);
                    detail.Transactions.AddTransactionNTM(tradeIn);
                }
            }

        }

        private static ITransactionNTM transferPosition(IDalSession session, IPositionTransferDetail detail, IAccountTypeInternal account, bool transferOut)
        {
            IAccountTypeInternal AcctA = account;
            IAccount AcctB = account.DefaultAccountforTransfer;

            Price Price = detail.TransferPrice;
            ICurrency currency = detail.TransferPrice.Underlying;
            Money CounterValueSize = Price.Amount.ZeroedAmount();
            decimal ExRate = detail.ExchangeRate;
            DateTime TransactionDate = detail.TransferDate;
            string ExternalSource = transferOut ? detail.ParentTransfer.DescriptionAccountA : detail.ParentTransfer.DescriptionAccountA;
            Side txSide = transferOut ? Side.XO : Side.XI;
            InstrumentSize ValueSize = transferOut ? detail.PositionSize.Abs().Negate() : detail.PositionSize.Abs();

            ITradingJournalEntry tradingJournalEntry = TransactionAdapter.GetNewTradingJournalEntry(session, currency.Symbol, TransactionDate);
            IGLLookupRecords lookups = GlLookupRecordMapper.GetGLLookupRecords(session, BookingComponentParentTypes.Transaction);
            ListOfTransactionComponents[] txcomps = new ListOfTransactionComponents[0];

            TransactionNTM ntm = new TransactionNTM(AcctA, AcctB, ValueSize, Price, ExRate,
                                    TransactionDate, TransactionDate.AddHours(10), 0M, txSide,
                                    tradingJournalEntry, lookups, txcomps, ExternalSource);
            IInternalEmployeeLogin employee = detail.ParentTransfer.CreatedBy;
            ntm.Approve(employee);
            return ntm;
        }



        public static int SetupTransfer(PositionTransferDetails details)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                SavePositionTransferDetails(details);
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, details.Key);

                if (!transfer.IsInitialised)
                {
                    if (transfer.AIsInternal)
                    {
                        transfer.APortfolioBefore = assemblePortfolio(session, transfer.AccountA.Key, transfer.TransferDate, transfer);
                        if ((transfer.TypeOfTransfer == TransferType.Full) || (transfer.TypeOfTransfer == TransferType.Amount))
                            createTransferDetails(transfer.APortfolioBefore, transfer.TypeOfTransfer, transfer.TransferAmount.Quantity, transfer);
                    }

                    if (transfer.BIsInternal)
                    {
                        transfer.BPortfolioBefore = assemblePortfolio(session, transfer.AccountB.Key, transfer.TransferDate, transfer);
                    }

                    transfer.CreatedBy = LoginMapper.GetCurrentEmployee(session); 
                    transfer.IsInitialised = true;

                    session.InsertOrUpdate(transfer);
                }
                return transfer.Key;
            }


        }

        public static int CreateNewPositionTransfer()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IPositionTransfer transfer = new PositionTransfer();
                transfer.TransferDate = DateTime.Now;
                transfer.AIsInternal = false;
                transfer.BIsInternal = false;
                session.InsertOrUpdate(transfer);
                return transfer.Key;
            }
        }

        private static void createTransferDetails(IPositionTransferPortfolio currentPortfolio, TransferType typeOfTransfer, decimal transferAmount, IPositionTransfer parent)
        {
            foreach (IPositionTransferPosition pos in currentPortfolio.Positions)
            {
                IPositionTransferDetail newDetail = new PositionTransferDetail(pos, typeOfTransfer, transferAmount);
                parent.TransferDetails.AddPosition(newDetail);
            }

        }

        public static DataSet ShowPortfolio(int positionTransferID, bool showSideA)
        {
            return OperationalReportAdapter.GetClientTransferReport(positionTransferID, showSideA);
        }

        private static IPositionTransferPortfolio assemblePortfolio(IDalSession session, int accountId, DateTime positionDate, IPositionTransfer transfer)
        {
            IPortfolioHistorical portfolio = HistoricalPositionAdapter.GetHistoricalPortfolio(session, accountId, positionDate);
            IList<HistoricalPositionRowView> portfolioRows = GetHistoricalPositions(session, portfolio);
            IPositionTransferPortfolio transferPortfolio = new PositionTransferPortfolio(portfolio.ParentAccount, positionDate, transfer);
            foreach (HistoricalPositionRowView row in portfolioRows)
            {
                IPositionTransferPosition newRow = new PositionTransferPosition();
                newRow.PositionSize = row.PositionSize;
                newRow.ActualPrice = row.Price;
                newRow.ExchangeRate = row.ExchangeRate;
                newRow.ValueinEuro = row.Value;
                newRow.PercentageOfPortfolio = row.Percentage;
                transferPortfolio.Positions.AddPosition(newRow);
            }
            return transferPortfolio;
        }



        public static DataSet GetCustomerAccounts(int assetManagerId, int modelPortfolioId, string accountNumber, string accountName,
                                          bool retrieveNostroAccounts)
        {
            DataSet ds = GetCustomerAccounts(assetManagerId, 0, 0, modelPortfolioId, accountNumber, accountName, retrieveNostroAccounts,
                                             true, true, 0, true, true,
                                             "Key, Number, DisplayNumberWithName");

            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetCustomerAccounts(int assetManagerId, int remisierId, int remisierEmployeeId, int modelPortfolioId,
                                          string accountNumber, string accountName, bool retrieveNostroAccounts,
                                          bool showActive, bool showInactive, int year, bool showTradeable, bool showNonTradeable,
                                          string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return AccountMapper.GetCustomerAccounts(session, assetManagerId, remisierId, remisierEmployeeId,
                    0, modelPortfolioId, accountNumber, accountName, false, retrieveNostroAccounts, showActive,
                    showInactive, year, showTradeable, showNonTradeable)
                    .ToDataSet(propertyList);
            }
        }

        public static DataSet GetNtmTransfers()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return PositionTransferMapper.GetPositionTransfers(session).Select(pt => new
                {
                    pt.Key,
                    pt.TransferDate,
                    OriginInternal = pt.AIsInternal,
                    AccountA_Key = pt.AccountA != null ? pt.AccountA.Key : 0,
                    pt.AccountNumberA,
                    DestinationInternal = pt.BIsInternal,
                    AccountB_Key = pt.AccountB != null ? pt.AccountB.Key : 0,
                    pt.AccountNumberB,
                    pt.TransferStatus
                }).ToDataSet();

            }
        }

        public static void SavePositionTransferDetails(PositionTransferDetails details)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, details.Key);
                transfer.AIsInternal = details.AIsInternal;
                if ((details.AIsInternal) && (details.AccountAID != 0))
                {
                    IAccountTypeInternal acc = (IAccountTypeInternal)AccountMapper.GetAccount(session, details.AccountAID);
                    transfer.AccountA = acc;
                }

                transfer.BIsInternal = details.BIsInternal;
                if ((details.BIsInternal) && (details.AccountBID != 0))
                {
                    IAccountTypeInternal acc = (IAccountTypeInternal)AccountMapper.GetAccount(session, details.AccountBID);
                    transfer.AccountB = acc;
                }

                ICurrency baseCurrency = transfer.AccountA != null ? transfer.AccountA.BaseCurrency : transfer.AccountB.BaseCurrency;

                transfer.TypeOfTransfer = details.TypeOfTransfer;
                transfer.TransferAmount = new Money(details.TransferAmount, baseCurrency);
                transfer.TransferDate = details.TransferDate;

                session.InsertOrUpdate(transfer);

            }
        }

        public static PositionTransferDetails GetPositionTransfer(int positionTransferID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, positionTransferID);
                return new PositionTransferDetails(transfer);
            }
        }

        public class PositionTransferDetails
        {
            public PositionTransferDetails() { }
            public PositionTransferDetails(IPositionTransfer transfer)
            {
                this.AIsInternal = transfer.AIsInternal;
                this.BIsInternal = transfer.BIsInternal;
                if (transfer.AccountA != null) this.AccountAID = transfer.AccountA.Key;
                if (transfer.AccountB != null) this.AccountBID = transfer.AccountB.Key;
                this.TransferDate = transfer.TransferDate;
                this.TypeOfTransfer = transfer.TypeOfTransfer;
                this.TransferAmount = transfer.TransferAmount == null ? 0 : transfer.TransferAmount.Quantity;
                this.Status = transfer.TransferStatus;
                this.IsInitialised = transfer.IsInitialised;
                this.IsEditable = transfer.IsEditable;
            }
            public int Key;
            public bool AIsInternal;
            public bool BIsInternal;
            public int AccountAID;
            public int AccountBID;
            public DateTime TransferDate;
            public TransferType TypeOfTransfer;
            public Decimal TransferAmount;
            public TransferStatus Status;
            public bool IsInitialised;
            public bool IsEditable;
        }

    }
}
