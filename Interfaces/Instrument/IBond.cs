using System;
using System.Collections.Generic;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    #region Enums

    /// <summary>
    /// The different types of day count conventions. Method to calculate the fraction of a year between two dates.
    /// </summary>
    public enum AccruedInterestCalcTypes
    {
        /// <summary>
        /// There is no accrued interest
        /// </summary>
        Zero = 0,
        Spaarbrief = 1,
        /// <summary>
        /// ISMA-99 Normal
        /// </summary>
        ACT_ACT = 2,
        /// <summary>
        /// ISMA-99 Ultimo
        /// </summary>
        ACT_ACT_Ultimo = 3,
        French_ACT_360 = 4,
        English_ACT_365 = 5,
        ACT_365_L = 6,
        /// <summary>
        /// German
        /// </summary>
        German_30_360 = 7,            
        /// <summary>
        /// German Special
        /// </summary>
        German_30E_360 = 8,           
        /// <summary>
        /// US
        /// </summary>
        US_30U_360 = 9,           
        ac30_ACT = 10,
        Undefined = 16
    }

    #endregion

    #region Helper Struct

    public struct AccruedInterestDetails
    {
        public AccruedInterestDetails(bool isRelevant)
        {
            AccruedInterestInOriginalCurrency = null;
            AccruedInterestPercentage = 0M;
            CouponRate = 0M;
            InterestDays = 0;
            PreviousCouponDate = DateTime.MinValue;
            NextCouponDate = DateTime.MinValue;
            SettlementDate = DateTime.MinValue;
            IsRelevant = isRelevant;
        }

        public AccruedInterestDetails(Money accruedInterest, decimal accruedInterestPercentage, 
            decimal couponRate, int interestDays, DateTime previousCouponDate,
            DateTime nextCouponDate, DateTime settlementDate)
        {
            AccruedInterestInOriginalCurrency = accruedInterest;
            AccruedInterestPercentage = accruedInterestPercentage;
            CouponRate = couponRate;
            InterestDays = interestDays;
            PreviousCouponDate = previousCouponDate;
            NextCouponDate = nextCouponDate;
            SettlementDate = settlementDate;
            IsRelevant = true;
        }

        public bool IsRelevant;
        public Money AccruedInterestInOriginalCurrency;
        public decimal AccruedInterestPercentage;
        public decimal CouponRate;
        public int InterestDays;
        public DateTime PreviousCouponDate;
        public DateTime NextCouponDate;
        public DateTime SettlementDate;

        public Money AccruedInterest        
        {
            get { return AccruedInterestInOriginalCurrency.AmountInActiveCurrency(SettlementDate); }
        }

        public string DisplayString
        {
            get 
            { 
                string description = "Accrued interest is not relevant";
                if (IsRelevant)
                {
                    if (AccruedInterest != null)
                        description = string.Format("Accrued Interest: {0}", AccruedInterest.DisplayString);
                    if (!AccruedInterest.EqualCurrency(AccruedInterestInOriginalCurrency))
                        description += string.Format("<br>Accrued Interest IC: {0}", AccruedInterestInOriginalCurrency.DisplayString);
                    description = string.Format("{0}<br>Interest Days: {1}<br>Previous CouponDate: {2}<br>Next CouponDate: {3}<br>CouponRate: {4}<br>AI factor: {5}%", 
                        description, InterestDays, 
                        Util.IsNotNullDate(PreviousCouponDate) ? PreviousCouponDate.ToString("dd-MM-yyyy") : "", 
                        Util.IsNotNullDate(NextCouponDate) ? NextCouponDate.ToString("dd-MM-yyyy") : "",
                        CouponRate.ToString("0.0000"),
                        AccruedInterestPercentage.ToString("0.0000"));
                }
                return description;
            }
        }
    }

    #endregion

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Bond">Bond</see> class
    /// </summary>
    public interface IBond : ISecurityInstrument
    {
        Money NominalValue { get; set; }
        decimal CouponRate { get; set; }
        Regularities CouponFreq { get; set; }
        DateTime MaturityDate { get; set; }
        AccruedInterestCalcTypes AccruedInterestCalcType { get; set; }
        DateTime FirstCouponPaymntDate { get; set; }
        bool UltimoDating { get; set; }
        bool DoesPayInterest { get; }
        bool IsPerpetual { get; set; }
        bool IsFixedCouponRate { get; set; }
        Money RedemptionAmount { get; set; }
        ICouponHistoryCollection Coupons { get; }
        IBondCouponRateHistoryCollection CouponRates { get; }

        InstrumentSize CalculateSizeBackwards(Money amount);
        InstrumentSize CalculateSizeBackwards(Money amount, Price price, DateTime settlementDate);
        DateTime NextCouponDate(DateTime settlementDate);
        DateTime NextCouponDate(DateTime settlementDate, IExchange exchange);
        DateTime LastCouponDate(DateTime settlementDate);
        DateTime LastCouponPaymentDate(DateTime settlementDate);
        DateTime CalculateLastCouponDate(DateTime firstCouponDate, Regularities freq, bool ultimoDating, DateTime settlementDate);
        decimal AI_Factor(DateTime settlementDate);
        AccruedInterestDetails AccruedInterest(InstrumentSize size, DateTime settlementDate, IExchange exchange);
        AccruedInterestDetails AccruedInterest(InstrumentSize size, DateTime tradeDate, Int16 settlementPeriod, IExchange exchange);
    }
}
