using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Accounts.ManagementPeriods;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.ManagementPeriodUnits.ReportData;

namespace B4F.TotalGiro.ApplicationLayer.Fee
{
    public class UnitFeeOverview
    {
        #region Ctor

        internal UnitFeeOverview(IManagementPeriod managementPeriod, List<IManagementPeriodUnit> units) // List<int, Money> feeAmounts
        {
            this.ManagementPeriod = managementPeriod;
            this.Account = ManagementPeriod.Account;
            // if Account.Family.ManagementFeeInstalment = 12 -> use ManagementPeriod.Key + Period / 1000000
            this.Key = managementPeriod.Key;

            foreach (IManagementPeriodUnit unit in units)
            {
                if (this.Account != unit.Account)
                    throw new ApplicationException("This is the wrong account.");

                int index = getIndexFromPeriod(unit.Period);
                this.units[index] = new UnitFeePerPeriod(this, unit);
                this.IsKickBackExported = (unit.KickBackExport != null);
            }
        }

        internal UnitFeeOverview(int key, string tradeID)
        {
            this.Key = key;
            this.allowCreateTransaction = string.IsNullOrEmpty(tradeID);
        }

        #endregion

        #region Props

        public virtual int Key { get; private set; }
        public virtual IManagementPeriod ManagementPeriod { get; private set; }
        public virtual ICustomerAccount Account { get; private set; }
        public virtual bool IsKickBackExported { get; set; }
        public static string Quarter { get; set; }
        public static string Year { get; set; }
        public static int[] Periods { get; set; }

        public virtual IDictionary<int, UnitFeePerPeriod> Units 
        {
            get
            {
                return units;
            }
        }

        public virtual string Period1
        {
            get { return getPeriodFromIndex(0).ToString(); }
        }

        public virtual string Period2
        {
            get { return getPeriodFromIndex(1).ToString(); }
        }

        public virtual string Period3
        {
            get { return getPeriodFromIndex(2).ToString(); }
        }

        public virtual UnitFeePerPeriod MgtPeriod1 
        {
            get 
            { 
                if (mgtPeriod1 == null)
                    mgtPeriod1 = getMgtPeriod(0);
                return mgtPeriod1; 
            }
        }

        public virtual IManagementPeriodUnit Unit1
        {
            get
            {
                if (MgtPeriod1 != null)
                    return MgtPeriod1.Unit;
                return null;
            }
        }

        public virtual Money FeeAmount1
        {
            get
            {
                if (MgtPeriod1 != null)
                    return MgtPeriod1.FeeAmount;
                return null;
            }
        }

        public virtual UnitFeePerPeriod MgtPeriod2
        {
            get
            {
                if (mgtPeriod2 == null)
                    mgtPeriod2 = getMgtPeriod(1);
                return mgtPeriod2;
            }
        }

        public virtual IManagementPeriodUnit Unit2
        {
            get
            {
                if (MgtPeriod2 != null)
                    return MgtPeriod2.Unit;
                return null;
            }
        }

        public virtual Money FeeAmount2
        {
            get
            {
                if (MgtPeriod2 != null)
                    return MgtPeriod2.FeeAmount;
                return null;
            }
        }

        public virtual UnitFeePerPeriod MgtPeriod3
        {
            get
            {
                if (mgtPeriod3 == null)
                    mgtPeriod3 = getMgtPeriod(2);
                return mgtPeriod3;
            }
        }

        public virtual IManagementPeriodUnit Unit3
        {
            get
            {
                if (MgtPeriod3 != null)
                    return MgtPeriod3.Unit;
                return null;
            }
        }

        public virtual Money FeeAmount3
        {
            get
            {
                if (MgtPeriod3 != null)
                    return MgtPeriod3.FeeAmount;
                return null;
            }
        }

        public virtual string TradeID
        {
            get
            {
                var tradeIds = (from c in this.units.Values
                                where c.TradeID != 0
                                select c.TradeID.ToString()).Distinct().ToArray();

                if (tradeIds.Length > 0)
                    return string.Join(",", tradeIds);
                else
                    return null;
            }
        }

        public virtual bool AllowCreateTransaction 
        {
            get 
            {
                if (!this.allowCreateTransaction.HasValue)
                    this.allowCreateTransaction = (Account.Status == AccountStati.Active && string.IsNullOrEmpty(TradeID));
                return this.allowCreateTransaction.Value;
            }
            private set { this.allowCreateTransaction = value; } 
        }

        public DateTime ManagementEndDate 
        {
            get
            {
                if (Util.IsNotNullDate(ManagementPeriod.EndDate) && UnitFeeOverview.Periods.Contains(Util.GetPeriodFromDate(ManagementPeriod.EndDate.Value)))
                    return ManagementPeriod.EndDate.Value;
                else                    
                    return DateTime.MinValue;
            }
        }

        public virtual bool Error 
        {
            get
            {
                return (from c in this.units.Values
                        where !c.Success && !string.IsNullOrEmpty(c.Message)
                        select c).Count() > 0;
            }
        }

        #endregion

        #region Methods

        private UnitFeePerPeriod getMgtPeriod(int index)
        {
            UnitFeePerPeriod data = null;
            if (units != null && units.Keys.Contains(index))
                data = units[index];
            return data;
        }

        private int getIndexFromPeriod(int period)
        {
            int index = 0;
            foreach (int p in UnitFeeOverview.Periods)
            {
                if (p == period)
                    break;
                index++;
            }
            return index;
        }

        private int getPeriodFromIndex(int index)
        {
            return UnitFeeOverview.Periods[index];
        }

        public void AddFeeDetails(int period, Money feeAmount)
        {
            int index = getIndexFromPeriod(period);
            units[index].FeeAmount += feeAmount;
        }

        //public IKickBackExport KickBackReportData
        //{
        //    get
        //    {
        //        if (kickBackExport == null)
        //            kickBackExport = new KickBackExport(this.Account, Year, Quarter,
        //                Unit1, FeeAmount1,
        //                Unit2, FeeAmount2,
        //                Unit3, FeeAmount3);

        //        return kickBackExport;
        //    }
        //}

        #endregion

        #region Export

        public virtual bool IsValid
        {
            get
            {
                bool isValid = false;

                if (this.Account == null || string.IsNullOrEmpty(Year) || string.IsNullOrEmpty(Quarter))
                    isValid = false;
                else if (units == null || units.Count == 0)
                    isValid = false;
                else if (units.Values.Where(n => n.Unit.KickBackExport != null).Count() > 0)
                    isValid = false;
                else
                    isValid = checkUnits();
                return isValid;
            }
        }

        private bool checkUnits()
        {
            bool isOK = false;

            var p = (from a in units.Values
                     group a by a.Unit.ManagementPeriod into g
                     select g.Key).ToList();

            switch (p.Count())
            {
                case 0:
                    break;
                case 1:
                    DateTime startDate = DateTime.MinValue;
                    DateTime endDate = DateTime.MinValue;

                    Util.GetDatesFromQuarter(Convert.ToInt32(UnitFeeOverview.Year), UnitFeeOverview.Quarter, out startDate, out endDate);
                    if (p[0].StartDate > startDate) startDate = p[0].StartDate;
                    if (Util.IsNotNullDate(p[0].EndDate) && p[0].EndDate < endDate) endDate = p[0].EndDate.Value;

                    int expectedUnitCount = Util.DateDiff(DateInterval.Month, startDate, endDate) + 1;
                    if (expectedUnitCount == units.Count)
                        isOK = true;
                    break;
                default:
                    isOK = true;
                    break;
            }
            return isOK;
        }

        #endregion


        #region Privates

        private IDictionary<int, UnitFeePerPeriod> units = new Dictionary<int, UnitFeePerPeriod>(3);
        private UnitFeePerPeriod mgtPeriod1;
        private UnitFeePerPeriod mgtPeriod2;
        private UnitFeePerPeriod mgtPeriod3;
        private bool? allowCreateTransaction;
        private IKickBackExport kickBackExport;

        #endregion
    }

    public class UnitFeePerPeriod
    {
        #region Ctor

        public UnitFeePerPeriod(UnitFeeOverview parent, IManagementPeriodUnit unit)
        {
            this.Parent = parent;
            this.ModelPortfolio = unit.ModelPortfolio;
            this.Period = unit.Period;
            this.TotalValue = unit.TotalValue;
            this.IsStornoed = unit.IsStornoed;
            this.FeesCalculated = unit.FeesCalculated;
            this.Success = unit.Success;
            this.unitMessage = unit.Message;
            if (unit.ManagementFee != null)
                this.TradeID = unit.ManagementFee.Key;
            this.Unit = unit;
        }

        #endregion

        #region Props

        internal UnitFeeOverview Parent { get; set; }
        public virtual IPortfolioModel ModelPortfolio { get; set; }
        public virtual int Period { get; set; }
        public virtual Money TotalValue { get; set; }
        public virtual Money FeeAmount { get; set; }
        public virtual bool IsStornoed { get; set; }
        public virtual FeesCalculatedStates FeesCalculated { get; set; }
        public virtual int RulesFound { get; set; }
        public virtual bool Success { get; set; }
        public virtual int TradeID { get; set; }

        public virtual string Message 
        {
            get
            {
                if (!string.IsNullOrEmpty(this.unitMessage))
                    return this.unitMessage;
                else if (this.FeesCalculated == FeesCalculatedStates.Irrelevant)
                    return "Not relevant for management fee.";
                else if (this.FeesCalculated == FeesCalculatedStates.Yes && !this.Success)
                    return "Error in calculation management fee.";
                else
                    return "";
            }
        }

        public virtual bool HasMessage
        {
            get
            {
                return (!string.IsNullOrEmpty(Message));
            }
        }

        public IKickBackExport KickBackReportData
        {
            get
            {
                if (kickBackExport == null)
                    kickBackExport = new KickBackExport(Parent.Account, Unit, FeeAmount);

                return kickBackExport;
            }
        }

        public IManagementPeriodUnit Unit { get; set; }

        #endregion

        #region Privates

        private string unitMessage;
        private IKickBackExport kickBackExport;

        #endregion
    }
}
