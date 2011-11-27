using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Accounts.ManagementPeriods
{
    /// <summary>
    /// The class that holds the account's remisier related data changes per account
    /// </summary>
    public class ManagementPeriodCollection : TransientDomainCollection<IManagementPeriod>, IManagementPeriodCollection
    {
        /// <summary>
        /// The account the remisier related data changes belong to.
        /// </summary>
        public ICustomerAccount Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public void AddManagementPeriod(IManagementPeriod item)
        {
            if (Count > 0)
            {
                foreach (IManagementPeriod hist in this)
                {
                    if (hist.ManagementType.Equals(item.ManagementType) && Util.IsNullDate(hist.EndDate))
                        throw new ApplicationException("It is not possible to have two management periods with an open end date.");

                    if (hist.IsBetween(item.ManagementType, item.StartDate) ||
                        (Util.IsNotNullDate(item.EndDate) && hist.IsBetween(item.ManagementType, item.EndDate.Value)))
                        throw new ApplicationException("An overlap was found within two management periods.");
                }
            }
            item.Account = Parent;
            base.Add(item);
            if (item.ManagementType == ManagementTypes.ManagementFee)
                Parent.CurrentManagementFeePeriod = item;
        }

        public IManagementPeriod GetItemByDate(DateTime date)
        {
            foreach (IManagementPeriod item in this)
                if (item.IsBetween(date))
                    return item;
            return null;
        }

        public IList<IManagementPeriod> Filter(ManagementTypes managementType)
        {
            return this.Where(a => a.ManagementType == managementType).ToList<IManagementPeriod>();
        }

        public bool IsManagementPeriodEditable(ManagementTypes managementType)
        {
            bool isEditable = false;
            IList<IManagementPeriod> periods = Filter(managementType);
            if (periods != null && periods.Count == 1)
            {
                if (periods[0].ManagementPeriodUnits == null || periods[0].ManagementPeriodUnits.Count == 0)
                    isEditable = true;
            }
            return isEditable;
        }

        #region Privates

        private ICustomerAccount parent;

        #endregion
    }
}