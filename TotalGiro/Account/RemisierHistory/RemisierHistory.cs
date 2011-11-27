using System;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Accounts.RemisierHistory
{
    public class RemisierHistory : IRemisierHistory
    {
        #region Constructor

        protected RemisierHistory() { }

        public RemisierHistory(IRemisierEmployee remisierEmployee, 
            decimal kickBack, decimal introductionFee, decimal introductionFeeReduction,
            decimal subsequentDepositFee, decimal subsequentDepositFeeReduction,
            IInternalEmployeeLogin employee, DateTime changeDate)
        {
            RemisierEmployee = remisierEmployee;
            KickBack = kickBack;
            IntroductionFee = introductionFee;
            SubsequentDepositFee = subsequentDepositFee;
            IntroductionFeeReduction = introductionFeeReduction;
            SubsequentDepositFeeReduction = subsequentDepositFeeReduction;
            Employee = employee;
            ChangeDate = changeDate.Date;
        }

        #endregion

        #region Props

        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual IAccountTypeCustomer Account
        {
            get { return this.account; }
            set { this.account = value; }
        }

        public virtual IRemisierEmployee RemisierEmployee
        {
            get { return this.remisierEmployee; }
            set { this.remisierEmployee = value; }
        }

        public decimal KickBack { get; set; }
        public decimal IntroductionFee { get; set; }
        public decimal SubsequentDepositFee { get; set; }
        public decimal IntroductionFeeReduction { get; set; }
        public decimal SubsequentDepositFeeReduction { get; set; }

        public virtual IInternalEmployeeLogin Employee
        {
            get { return this.employee; }
            private set { this.employee = value; }
        }

        public virtual DateTime ChangeDate
        {
            get { return this.changeDate; }
            set { this.changeDate = value; }
        }

        public virtual DateTime EndDate
        {
            get { return this.endDate; }
        }

        public virtual string DepositFeeInfo
        {
            get 
            {
                string info = "";
                if (IntroductionFee != 0M)
                    info = string.Format("Introduction deposit fee {0}%", IntroductionFee.ToString("0.00###"));
                if (SubsequentDepositFee != 0M)
                    info = string.Format("{0}Subsequent deposit fee {1}%", (info.Length > 0 ? info + ", " : ""), SubsequentDepositFee.ToString("0.00###"));
                return info; 
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return string.Format("Rem Emp: {0} - KickBack: {1} - Introd. Fee: {2} Reduction: {3}",
                (RemisierEmployee != null ? RemisierEmployee.Employee.FullName : ""),
                KickBack.ToString("0.00"), IntroductionFee.ToString("0.00"), IntroductionFeeReduction.ToString("0.00"));
        }

        public override bool Equals(object obj)
        {
            bool retVal = false;
            IRemisierHistory item = obj as IRemisierHistory;

            if (item != null)
            {
                if ((this.RemisierEmployee != null && item.RemisierEmployee != null && this.RemisierEmployee.Key.Equals(item.RemisierEmployee.Key)) ||
                    (this.RemisierEmployee == null && item.RemisierEmployee == null))
                {
                    if (this.KickBack.Equals(item.KickBack) &&
                        this.IntroductionFee.Equals(item.IntroductionFee) &&
                        this.IntroductionFeeReduction.Equals(item.IntroductionFeeReduction) &&
                        this.SubsequentDepositFee.Equals(item.SubsequentDepositFee) &&
                        this.SubsequentDepositFeeReduction.Equals(item.SubsequentDepositFeeReduction))
                        retVal = true;
                }
            }

            return retVal;
        }

        #endregion

        #region Privates

        private int key;
        private IAccountTypeCustomer account;
        private IRemisierEmployee remisierEmployee;
        private IInternalEmployeeLogin employee;
        private DateTime changeDate;
        private DateTime endDate;

        #endregion
    }
}