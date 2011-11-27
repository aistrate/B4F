using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class BuyingPowerDisplay
    {
        public BuyingPowerDisplay(IAccountTypeInternal account) 
        {
            Money zeroAmount = new Money(0m, account.BaseCurrency);
            this.BaseValue = this.BaseValue = this.Value = zeroAmount;
            this.ExRate = 1m;
        }

        public BuyingPowerDisplay(ICashSubPosition subPosition, bool repeat, IAccountTypeInternal account)
        {
            ICashPosition position = subPosition.ParentPosition;
            this.PositionID = position.Key;
            this.SubPositionID = subPosition.Key;
            if (subPosition.SettledFlag == CashPositionSettleStatus.UnSettled)
            {
                ICashSubPositionUnSettled unSettlPos = (ICashSubPositionUnSettled)subPosition;
                if (unSettlPos.UnSettledType != null && unSettlPos.UnSettledType.IncludeBuyingPower && !unSettlPos.UnSettledType.IsDefault)
                {
                    this.LineDescription = ((ICashSubPositionUnSettled)subPosition).UnSettledType.Description;
                    if (!subPosition.ParentPosition.PositionCurrency.IsBase)
                        this.LineDescription += " " + position.PositionCurrency.Name;
                    this.IsUnSettledIncludeBuyingPower = true;
                }
            }
            else
            {
                if (repeat) this.LineDescription = position.PositionCurrency.Name;
            }

            this.Status = subPosition.SettledFlag.ToString();
            this.Value = subPosition.Size;
            if (position.PositionCurrency.ExchangeRate != null)
                this.ExRate = position.PositionCurrency.ExchangeRate.Rate;
            else
            {
                if (position.PositionCurrency.IsBase)
                    this.ExRate = 1M;
                else
                    this.ExRate = 0M;
            }
            this.BaseValue = subPosition.SizeInBaseCurrency;
            this.IsSubTotalLine = false;
            this.IsCashFundLine = false;
            this.IsCashLine = true;
        }

        public int Key { get; set; }
        public int PositionID { get; set; }
        public int SubPositionID { get; set; }
        public string LineDescription { get; set; }
        public string Status { get; set; }
        public Money Value { get; set; }
        public decimal ExRate { get; set; }
        public Money BaseValue { get; set; }
        public bool IsCashLine { get; set; }
        public bool IsSubTotalLine { get; set; }
        public bool IsCashFundLine { get; set; }
        public bool IsUnSettledIncludeBuyingPower { get; set; }
        public bool IsSummaryLine { get; set; }

        public string ValueDisplay { get { return this.Value != null ? this.Value.DisplayString : ""; } }
        public string BaseValueDisplay { get { return this.BaseValue != null ? this.BaseValue.DisplayString : ""; } }
    }
}
