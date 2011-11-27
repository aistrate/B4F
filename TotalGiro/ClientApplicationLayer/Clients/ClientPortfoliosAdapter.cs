using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.ClientApplicationLayer.Common;
using B4F.TotalGiro.ClientApplicationLayer.Planning;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Utils.Tuple;

namespace B4F.TotalGiro.ClientApplicationLayer.Clients
{
    public static class ClientPortfoliosAdapter
    {
        public static DataSet GetClientContacts(int assetManagerId, int remisierId, int remisierEmployeeId, int modelPortfolioId, 
                                                string accountNumber, string contactName)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedContacts(session, assetManagerId, remisierId, remisierEmployeeId, accountNumber, contactName,
                                                             true, true, null, null, null, true, false)
                                           .OrderBy(c => c.LoginPerson.ShortName)
                                           .Select(c => new
                                           {
                                               c.Key,
                                               c.FullName,
                                               c.LoginPerson.ShortName,
                                               c.FullAddress,
                                               DisplayContactType = c.ContactType.ToString()
                                           })
                                           .ToDataSet();
            }
        }

        public static Tuple<string, Color>[] GetActiveAccountNumbers(int contactId, bool withColors, Dictionary<int, Color> accountColorCache)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedContactAccounts(session, contactId, true)
                                           .Select(account => Tuple.Create(account.Number,
                                                                           withColors ? getAccountColor(account, accountColorCache) :
                                                                                        Color.Black))
                                           .ToArray();
            }
        }

        public static Color GetContactColor(int contactId, Dictionary<int, Color> accountColorCache)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IEnumerable<Color> colors = SecurityLayerAdapter.GetOwnedContactAccounts(session, contactId, true)
                                                                .Select(account => getAccountColor(account, accountColorCache));

                Color maxColor = Color.Black;
                foreach (Color color in colors)
                    if (color == Color.Red)
                        return Color.Red;
                    else if (colorComparer.Compare(maxColor, color) < 0)
                        maxColor = color;

                return maxColor;
            }
        }

        private static IComparer<Color> colorComparer = new SmallSetComparer<Color>(new Color[] { Color.Green, Color.Yellow, Color.Red });
        
        private static Color getAccountColor(ICustomerAccount account, Dictionary<int, Color> accountColorCache)
        {
            if (!accountColorCache.ContainsKey(account.Key))
                accountColorCache[account.Key] = getAccountColor(account);

            return accountColorCache[account.Key];
        }

        private static Color getAccountColor(ICustomerAccount account)
        {
            InvestmentScenario scenario = GetInvestmentScenario(account);

            if (scenario != null)
                return CommonAdapter.GetTrafficLightColor(Math.Round(scenario.ChanceOfMeetingTarget, 2));
            else
                return Color.Black;
        }

        public static InvestmentScenario GetInvestmentScenario(ICustomerAccount account)
        {
            try
            {
                FinancialDataView financialDataView = new FinancialDataView(account, true, false);

                financialDataView.RetrieveModelData();

                if (financialDataView.ExpectedReturn != 0m && financialDataView.StandardDeviation != 0m)
                {
                    financialDataView.RetrieveFinancialTargetData();

                    if (financialDataView.TargetValue > 0m && financialDataView.DepositPerYear >= 0m && financialDataView.YearsLeft > 0)
                    {
                        financialDataView.RetrievePositionData();

                        return new InvestmentScenario(financialDataView.PresentValue, financialDataView.DepositPerYear,
                                                      financialDataView.YearsLeft, financialDataView.ExpectedReturn,
                                                      financialDataView.StandardDeviation, financialDataView.TargetValue);
                    }
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}