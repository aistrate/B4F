using System;
using System.Collections.Generic;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer.Filters;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Reports.Documents;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Valuations;

namespace B4F.TotalGiro.ClientApplicationLayer.SecurityLayer
{
    /// <summary>
    /// Methods of this class are just wrappers around Mapper methods, which filter data based on the current login.
    /// All Mapper calls within the ClientApplicationLayer should go through this class (or at least the login-dependent ones).
    /// </summary>
    internal static class SecurityLayerAdapter
    {
        #region Contacts

        public static List<IContact> GetOwnedContacts(IDalSession session,
            int assetManagerId, int remisierId, int remisierEmployeeId, string accountNumber, string contactName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive,
            bool accountActive, bool accountInactive)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedContacts(
                        assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName,
                        emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive, accountActive, accountInactive);
        }

        public static IContact GetOwnedContact(IDalSession session, int contactId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedContact(contactId);
        }

        public static void AssertContactIsOwned(IDalSession session, int contactId)
        {
            GetOwnedContact(session, contactId);
        }

        #endregion


        #region Accounts

        public static List<ICustomerAccount> GetOwnedContactAccounts(IDalSession session, int contactId, bool activeOnly)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedContactAccounts(contactId, activeOnly);
        }

        public static ICustomerAccount GetOwnedActiveAccount(IDalSession session, int accountId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedActiveAccount(accountId);
        }

        public static void AssertActiveAccountIsOwned(IDalSession session, int accountId)
        {
            GetOwnedActiveAccount(session, accountId);
        }

        #endregion


        #region Fund Positions

        public static List<IFundPosition> GetOwnedFundPositions(IDalSession session, int accountId, PositionsView view)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return FundPositionMapper.GetPositions(session, accountId, view);
        }

        public static List<IFundPosition> GetOwnedFundPositionsByParentInstrument(IDalSession session, 
                                                int accountId, int[] parentInstrumentIds, PositionsView view)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return FundPositionMapper.GetPositionsByParentInstrument(session, accountId, parentInstrumentIds, view);
        }

        public static IFundPosition GetOwnedFundPosition(IDalSession session, int positionId)
        {
            IFundPosition position = FundPositionMapper.GetPosition(session, positionId);
            if (position != null)
            {
                AssertActiveAccountIsOwned(session, position.Account.Key);
                return position;
            }
            else
                throw new SecurityLayerException("Fund Position not found.");
        }

        public static void AssertFundPositionIsOwned(IDalSession session, int positionId)
        {
            GetOwnedFundPosition(session, positionId);
        }

        #endregion


        #region Cash Positions

        public static ICashPosition GetOwnedCashPosition(IDalSession session, int accountId)
        {
            return GetOwnedCashSubposition(session, accountId).ParentPosition;
        }

        public static ICashSubPosition GetOwnedCashSubposition(IDalSession session, int accountId)
        {
            ICustomerAccount account = GetOwnedActiveAccount(session, accountId);

            ICashSubPosition subposition = account.Portfolio.PortfolioCashGL.GetSettledBaseSubPosition();
            if (subposition != null)
                return subposition;
            else
                throw new SecurityLayerException("Cash Subposition not found.");
        }

        #endregion


        #region Valuations

        public static List<IValuation> GetOwnedValuations(IDalSession session, int accountId, int instrumentId, DateTime[] dates,
                                                          bool includeClosedPositions, bool includeChildInstruments)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return ValuationMapper.GetValuations(session, accountId, instrumentId, dates, includeClosedPositions, includeChildInstruments);
        }

        public static List<IValuationTotalPortfolio> GetOwnedValuationsTotalPortfolio(IDalSession session, int accountId, DateTime[] dates)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return ValuationMapper.GetValuationsTotalPortfolio(session, accountId, dates);
        }

        #endregion


        #region Documents

        public static List<INotaDocument> GetOwnedNotaDocuments(IDalSession session, int accountId)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return DocumentMapper.GetNotaDocuments(session, accountId);
        }

        public static List<NotaDocumentView> GetOwnedNotaDocumentViews(IDalSession session, int accountId)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return DocumentMapper.GetNotaDocumentViews(session, accountId);
        }

        public static List<IFinancialReportDocument> GetOwnedFinancialReportDocuments(IDalSession session, int accountId)
        {
            AssertActiveAccountIsOwned(session, accountId);
            return DocumentMapper.GetFinancialReportDocuments(session, accountId);
        }

        public static IDocument GetOwnedDocument(IDalSession session, int documentId)
        {
            IDocument document = DocumentMapper.GetDocument(session, documentId);
            if (document != null)
            {
                AssertActiveAccountIsOwned(session, document.Account.Key);
                return document;
            }
            else
                throw new ApplicationException(string.Format("Document not authorized or not found.", documentId));
        }

        #endregion


        #region Asset Managers

        public static List<IAssetManager> GetOwnedAssetManagers(IDalSession session)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedAssetManagers();
        }

        public static IAssetManager GetCurrentAssetManager(IDalSession session)
        {
            return SecurityFilter.GetCurrent(session).CurrentAssetManager;
        }

        public static ICurrency GetBaseCurrency(IDalSession session)
        {
            return SecurityFilter.GetCurrent(session).BaseCurrency;
        }

        #endregion


        #region Remisiers

        public static List<IRemisier> GetOwnedRemisiers(IDalSession session, int assetManagerId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedRemisiers(assetManagerId);
        }

        public static IRemisier GetCurrentRemisier(IDalSession session)
        {
            return SecurityFilter.GetCurrent(session).CurrentRemisier;
        }

        #endregion


        #region Remisier Employees

        public static List<IRemisierEmployee> GetOwnedRemisierEmployees(IDalSession session,
            int assetManagerId, int remisierId, string remisierEmployeeName,
            bool emailStatusYes, bool emailStatusNo, bool? hasLogin, bool? passwordSent, bool? isLoginActive)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedRemisierEmployees(
                        assetManagerId, remisierId, remisierEmployeeName,
                        emailStatusYes, emailStatusNo, hasLogin, passwordSent, isLoginActive);
        }

        public static List<IRemisierEmployee> GetOwnedRemisierEmployees(IDalSession session, int remisierId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedRemisierEmployees(remisierId);
        }

        public static IRemisierEmployee GetOwnedRemisierEmployee(IDalSession session, int remisierEmployeeId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedRemisierEmployee(remisierEmployeeId);
        }

        public static IRemisierEmployee GetCurrentRemisierEmployee(IDalSession session)
        {
            return SecurityFilter.GetCurrent(session).CurrentRemisierEmployee;
        }

        #endregion

        
        #region Models

        public static List<IPortfolioModel> GetOwnedModels(IDalSession session)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedModels();
        }

        public static List<IPortfolioModel> GetOwnedModels(IDalSession session, int assetManagerId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedModels(assetManagerId);
        }

        public static IPortfolioModel GetOwnedModel(IDalSession session, int modelId)
        {
            return SecurityFilter.GetCurrent(session).GetOwnedModel(modelId);
        }

        #endregion


        #region Current Login

        public static LoginTypes GetCurrentLoginType(IDalSession session)
        {
            return SecurityFilter.GetCurrentLoginType(session);
        }

        #endregion
    }
}
