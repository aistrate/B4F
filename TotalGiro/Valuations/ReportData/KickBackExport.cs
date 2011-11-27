using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ManagementPeriodUnits.ReportData
{
    public class KickBackExport : IKickBackExport
    {
        #region Constructor

        protected KickBackExport()
        {
        }

        protected KickBackExport(ICustomerAccount account)
            : this()
        {
            this.Account = account;
            this.AccountNumber = account.Number;
            this.AccountShortName = account.ShortName;
            this.ManagementEndDate = account.FinalManagementEndDate;
            this.RemisierEmployee = account.RemisierEmployee;
            if (this.RemisierEmployee != null)
            {
                this.Adviseur = this.RemisierEmployee.Employee.FullNameLastNameFirst;
                this.Remisier = this.RemisierEmployee.Remisier;
                if (this.Remisier != null)
                {
                    this.Kantoor = this.Remisier.DisplayName;
                    this.ParentRemisier = this.Remisier.ParentRemisier;
                    if (this.ParentRemisier != null)
                        this.InkoopOrginisatie = this.ParentRemisier.DisplayName;
                }
            }
        }
        
        public KickBackExport(ICustomerAccount account, IManagementPeriodUnit unit, Money feeForPeriod)
            : this(account)
        {
            if (unit != null && unit.KickBackExport == null)
            {
                unit.KickBackExport = this;
                this.ManagementPeriodUnits.Add(unit);
                this.Period = unit.Period;
                this.Model = unit.ModelPortfolio;
                if (this.Model == null)
                    this.Model = account.ModelPortfolioChanges.GetItemByDate(Util.GetLastDateFromPeriod(unit.Period)).Get(e => e.ModelPortfolio);
                if (this.Model != null)
                    this.ModelName = this.Model.ShortName;
                if (unit.TotalValue != null)
                    this.AvgValue = unit.TotalValue.Quantity;
                if (feeForPeriod != null)
                    this.Kickback = feeForPeriod.Quantity;
            }
        }

        public KickBackExport(ICustomerAccount account, int period, decimal totalValue)
            : this(account)
        {
            this.Period = period;
            this.AvgValue = Math.Round(totalValue / (decimal)Util.GetLastDayOfPeriod(period), 2);
        }

        public KickBackExport(ICustomerAccount account, int period, IKickBackExport[] items)
            : this(account)
        {
            if (items != null && items.Count() > 0)
            {
                this.Period = period;
                foreach (IKickBackExport item in items)
                {
                    foreach (IManagementPeriodUnit unit in item.ManagementPeriodUnits)
                    {
                        if (unit.Period != this.Period)
                            throw new ApplicationException("Not good");
                        
                        unit.KickBackExport = this;
                        this.ManagementPeriodUnits.Add(unit);
                    }
                    if (this.Model == null)
                    {
                        this.Model = item.Model;
                        if (this.Model != null)
                            this.ModelName = this.Model.ShortName;
                    }
                    this.AvgValue += item.AvgValue;
                    this.Kickback += item.Kickback;
                }
            }
            else
                throw new ApplicationException("Not good");
        }

        #endregion

        #region Properties

        public virtual int Key { get; set; }
		public virtual IRemisier ParentRemisier { get; set; }
		public virtual string InkoopOrginisatie { get; set; }
		public virtual IRemisier Remisier { get; set; }
		public virtual string Kantoor { get; set; }
		public virtual IRemisierEmployee RemisierEmployee { get; set; }
		public virtual string Adviseur { get; set; }
		public virtual ICustomerAccount Account { get; set; }
		public virtual string AccountNumber { get; set; }
		public virtual string AccountShortName { get; set; }
        public virtual DateTime ManagementEndDate { get; set; }

        //public virtual IManagementPeriodUnit Unit { get; set; }
        public virtual int Period { get; set; }
        public virtual IPortfolioModel Model { get; set; }
		public virtual string ModelName { get; set; }
        public virtual decimal AvgValue { get; set; }
        public virtual decimal Kickback { get; set; }
        public virtual decimal KickbackPercentage { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public override string ToString()
        {
            if (Account != null)
                return string.Format("{0} {1}", Account.Number, Period);
            else
                return base.ToString();
        }

        /// <summary>
        /// The units that belong to this item.
        /// </summary>
        public virtual IManagementPeriodUnitCollection ManagementPeriodUnits
        {
            get
            {
                IManagementPeriodUnitCollection units = (ManagementPeriodUnitCollection)managementPeriodUnits.AsList();
                return units;
            }
        }

        #endregion

        #region Privates

        private IDomainCollection<IManagementPeriodUnit> managementPeriodUnits = new ManagementPeriodUnitCollection();
        //private DateTime startDate = DateTime.MinValue;
        //private DateTime endDate = DateTime.MinValue;

        #endregion
    }
}
