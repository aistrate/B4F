using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils.Linq;

namespace B4F.TotalGiro.ClientApplicationLayer.Portfolio
{
    public static class PortfolioPositionsAdapter
    {
        public static ApplicationLayer.Portfolio.AccountDetailsView GetAccountDetails(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICustomerAccount account = SecurityLayerAdapter.GetOwnedActiveAccount(session, accountId);
                ICurrency baseCurrency = SecurityLayerAdapter.GetBaseCurrency(session);

                return ApplicationLayer.Portfolio.ClientPortfolioAdapter.GetAccountDetails(account, baseCurrency);
            }
        }
        
        public static DataSet GetOpenFundPositions(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICustomerAccount account = SecurityLayerAdapter.GetOwnedActiveAccount(session, accountId);

                return ApplicationLayer.Portfolio.ClientPortfolioAdapter.GetPositions(session, account);
            }
        }

        public static DataSet GetClosedFundPositions(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICustomerAccount account = SecurityLayerAdapter.GetOwnedActiveAccount(session, accountId);

                return ApplicationLayer.Portfolio.ClosedPositionsAdapter.GetClosedSecurityPositions(session, account);
            }
        }

        public static DataSet GetPortfolioComponents(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ICustomerAccount account = SecurityLayerAdapter.GetOwnedActiveAccount(session, accountId);
                
                List<IFundPosition> fundPositions = SecurityLayerAdapter.GetOwnedFundPositions(session, accountId, PositionsView.NotZero)
                                                                        .OrderBy(p => p.Size.Underlying.DisplayName).ToList();

                List<Money> cashValues = EnumerableExtensions.Singleton(account.TotalCashAmount)
                                                             .Where(m => m.Quantity != 0).ToList();

                var cashComponents = cashValues.Select(m => new PortfolioComponentView(m)).ToList();

                var modelComponents = expandModelVersion(session, account.ModelPortfolio.LatestVersion, 1m, null).ToList();

                var orphanInstrumentIds = fundPositions.Select(p => p.Size.Underlying.Key)
                                                       .Except(modelComponents.Where(mc => mc.PortfolioComponentType == 
                                                                                              PortfolioComponentType.Instrument)
                                                                              .Select(mc => mc.ComponentKey)).ToList();
                var orphanComponents = fundPositions.Where(p => orphanInstrumentIds.Contains(p.Size.Underlying.Key))
                                                    .Select(p => new PortfolioComponentView(p.Size)).ToList();

                var allComponents = cashComponents.Concat(orphanComponents).Concat(modelComponents).ToList();

                calculatePercentages(allComponents, cashValues, fundPositions);

                int i = 1;
                foreach (var component in allComponents)
                    component.LineNumber = i++;
                
                return allComponents.ToDataSet();
            }
        }

        private static IEnumerable<PortfolioComponentView> expandModelVersion(IDalSession session,
            IModelVersion modelVersion, decimal allocation, PortfolioComponentView parent)
        {
            PortfolioComponentView model = new PortfolioComponentView(modelVersion, allocation, parent);

            return modelVersion.ModelComponents
                               .OrderBy(mc => mc.ModelComponentType == ModelComponentType.Instrument ? 0 : 1)
                               .ThenBy(mc => mc.ComponentName)
                               //.Where(mc => mc.ModelComponentKey != 13258)
                               .Select(mc => expandModelComponent(session, mc, allocation * mc.Allocation, model))
                               .Aggregate(EnumerableExtensions.Singleton(model),
                                          Enumerable.Concat);
        }

        private static IEnumerable<PortfolioComponentView> expandModelComponent(IDalSession session,
            IModelComponent modelComponent, decimal allocation, PortfolioComponentView parent)
        {
            switch (modelComponent.ModelComponentType)
            {
                case ModelComponentType.Model:
                    return expandModelVersion(session, ModelMapper.GetModelVersion(session, modelComponent.ModelComponentKey), 
                                                       allocation, parent);
                
                default:
                    //if (modelComponent.ModelComponentKey == 13257)
                    //    return EnumerableExtensions.Singleton(new PortfolioComponentView(
                    //               InstrumentMapper.GetTradeableInstrument(session, modelComponent.ModelComponentKey), 0.12m, parent));
                    //else
                    ITradeableInstrument instrument = InstrumentMapper.GetTradeableInstrument(session, modelComponent.ModelComponentKey);
                    if (instrument != null)
                        return EnumerableExtensions.Singleton(new PortfolioComponentView(instrument, allocation, parent));
                    else
                        return Enumerable.Empty<PortfolioComponentView>();
            }
        }

        private static void calculatePercentages(List<PortfolioComponentView> allComponents, 
                                                 List<Money> cashValues, List<IFundPosition> fundPositions)
        {
            var positionValues = cashValues.Select(m => new { InstrumentId = m.Underlying.Key,
                                                              Value = m,
                                                              PositionId = 0 })
                                           .Concat(fundPositions.Select(p => new { InstrumentId = p.Size.Underlying.Key,
                                                                                   Value = p.CurrentBaseValue,
                                                                                   PositionId = p.Key })).ToList();

            foreach (var positionValue in positionValues)
            {
                var positionComponents = allComponents.Where(c => (c.PortfolioComponentType == PortfolioComponentType.Instrument ||
                                                                   c.PortfolioComponentType == PortfolioComponentType.Cash) &&
                                                                   c.ComponentKey == positionValue.InstrumentId).ToList();

                if (positionComponents.Count == 1)
                    positionComponents[0].Value = positionValue.Value;
                else
                {
                    decimal totalPositionAllocation = positionComponents.Select(c => c.ModelAllocation).Sum();
                    foreach (var positionComponent in positionComponents)
                        positionComponent.Value = Money.Multiply(positionValue.Value, positionComponent.ModelAllocation / totalPositionAllocation, true);
                }

                foreach (var positionComponent in positionComponents)
                    positionComponent.PositionId = positionValue.PositionId;
            }

            foreach (var model in allComponents.Where(c => c.PortfolioComponentType == PortfolioComponentType.Model))
                model.Value = allDescendantInstruments(allComponents, model).Select(c => c.Value).Sum();

            decimal totalPositionValue = positionValues.Count > 0 ? positionValues.Select(v => v.Value).Sum().Quantity : 0m;

            if (totalPositionValue != 0m)
                foreach (var component in allComponents)
                    component.Percentage = Math.Round(component.Value.Quantity / totalPositionValue, 4);
        }

        private static IEnumerable<PortfolioComponentView> allDescendantInstruments(List<PortfolioComponentView> allComponents,
                                                                                    PortfolioComponentView component)
        {
            if (component.PortfolioComponentType == PortfolioComponentType.Instrument ||
                component.PortfolioComponentType == PortfolioComponentType.Cash)
                return EnumerableExtensions.Singleton(component);
            else
                return allComponents.Where(c => c.Parent == component)
                                    .Select(c => allDescendantInstruments(allComponents, c))
                                    .Aggregate(Enumerable.Empty<PortfolioComponentView>(), Enumerable.Concat);
        }
    }
}
