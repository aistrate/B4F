using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public static class BuyingPowerDisplayMapper
    {
        public static BuyingPowerDisplayCollection GetBuyingPowerDisplay(
            IDalSession session,IAccountTypeInternal account,
            bool showForeignCurrency, bool showUnSettledCash)
        {

            BuyingPowerDisplayCollection newColl = new BuyingPowerDisplayCollection(account);
            IList<ICashPosition> positions = account.Portfolio.PortfolioCashGL.ToList();

            foreach (ICashPosition pos in positions.Where(x => (x.PositionCurrency.IsBase && !showForeignCurrency) || showForeignCurrency))
            {
                bool repeat = true;
                ICashSubPosition subPos = pos.SettledPosition;
                if (subPos != null)
                {
                    BuyingPowerDisplay sett = new BuyingPowerDisplay(subPos, repeat, account);
                    repeat = false;
                    newColl.Add(sett);
                }
                if (showUnSettledCash)
                {
                    subPos = pos.UnSettledPositions.DefaultSubPosition;
                    if (subPos != null)
                    {
                        BuyingPowerDisplay unsett = new BuyingPowerDisplay(subPos, repeat, account);
                        newColl.Add(unsett);
                    }
                }
            }

            //if (newColl.Count > 0)
            //    newColl.AddBlankLine();
            
            newColl.AddCashSummaryLine();
            //newColl.AddBlankLine();

            newColl.AddCashFundLine(account.Portfolio.PortfolioInstrument.CashFundValueInBaseCurrency);
            //newColl.AddBlankLine();

            // include unsettled with IncludeBuyingPower flag
            bool showAI = false;
            foreach (ICashSubPositionUnSettled subPos in positions.SelectMany(x => x.UnSettledPositions).Where(x => x.UnSettledType != null && x.UnSettledType.IncludeBuyingPower).OrderBy(x => x.UnSettledType.Key))
            {
                BuyingPowerDisplay sett = new BuyingPowerDisplay(subPos, true, account);
                newColl.Add(sett);
                showAI = true;
            }
            if (showAI && !showUnSettledCash)
            {
                Money ai = newColl.Where(x => x.IsUnSettledIncludeBuyingPower).Select(x => x.BaseValue).Sum();
                if (ai.IsZero)
                    newColl.RemoveAt(newColl.Count - 1);
            }

            newColl.AddBuyingPowerLine();

            return newColl;

        }
    }
}
