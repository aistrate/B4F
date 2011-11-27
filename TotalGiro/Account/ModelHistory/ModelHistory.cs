using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.Accounts.ModelHistory
{
    public class ModelHistory: IModelHistory
    {
        protected ModelHistory() { }

        public ModelHistory(IAccountTypeCustomer account, ILifecycle lifecycle, IPortfolioModel model, 
            bool isExecOnlyCustomer, AccountEmployerRelationship employerRelationship,
            IInternalEmployeeLogin employee, DateTime changeDate)
        {
            Account = account;
            Lifecycle = lifecycle;
            ModelPortfolio = model;
            IsExecOnlyCustomer = isExecOnlyCustomer;
            EmployerRelationship = employerRelationship;
            Employee = employee;
            ChangeDate = changeDate.Date;
        }

        public virtual int Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public virtual IAccountTypeCustomer Account
        {
            get { return this.account; }
            private set { this.account = value; }
        }

        public virtual ILifecycle Lifecycle { get; set; }

        public virtual IPortfolioModel ModelPortfolio
        {
            get { return this.model; }
            set { this.model = value; }
        }

        public virtual int ModelPortfolioKey
        {
            get 
            {
                if (ModelPortfolio != null)
                    return ModelPortfolio.Key;
                else
                    return int.MinValue;
            }
        }

        public virtual bool IsExecOnlyCustomer
        {
            get { return isExecOnlyCustomer; }
            set { isExecOnlyCustomer = value; }
        }

        public virtual AccountEmployerRelationship EmployerRelationship
        {
            get { return employerRelationship; }
            set { employerRelationship = value; }
        }

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
            set { this.endDate = value; }
        }

        public bool Edit(IPortfolioModel model, bool isExecOnlyCustomer, AccountEmployerRelationship employerRelationship,
            IInternalEmployeeLogin employee, DateTime changeDate)
        {
            // It is not possible to edit the model, IsExecOnlyCustomer || EmployerRelationship for the latest item
            if (EndDate == DateTime.MinValue)
            {
                if (this.ModelPortfolio != model ||
                    this.IsExecOnlyCustomer != isExecOnlyCustomer ||
                    this.EmployerRelationship != employerRelationship)
                    throw new ApplicationException("It is not possible to update data on the latest history item. Otherwise the data would no longer be in sync");

                if (changeDate != this.ChangeDate && Account != null && Account.ModelPortfolioChanges != null && Account.ModelPortfolioChanges.Count > 0)
                {
                    int check = (from a in Account.ModelPortfolioChanges
                                where a.Key != this.Key && a.ChangeDate > changeDate
                                select a).Count();
                    if (check > 0)
                        throw new ApplicationException("It is not possible to change the date of the latest history item to a date prior a previous history items.");
                }
            }

            this.ModelPortfolio = model;
            this.IsExecOnlyCustomer = isExecOnlyCustomer;
            this.EmployerRelationship = employerRelationship;
            this.ChangeDate = changeDate;
            this.Employee = employee;
            return true;
        }

        #region Privates

        private int key;
        private IAccountTypeCustomer account;
        private IPortfolioModel model;
        private bool isExecOnlyCustomer;
        private AccountEmployerRelationship employerRelationship = AccountEmployerRelationship.None;
        private IInternalEmployeeLogin employee;
        private DateTime changeDate;
        private DateTime endDate;

        #endregion
    }
}
