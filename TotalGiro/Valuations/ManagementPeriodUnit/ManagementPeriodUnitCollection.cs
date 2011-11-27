using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.ManagementPeriodUnits
{
    public class ManagementPeriodUnitCollection : TransientDomainCollection<IManagementPeriodUnit>, IManagementPeriodUnitCollection
    {
        public ManagementPeriodUnitCollection()
            : base() { }

        public ManagementPeriodUnitCollection(IManagementPeriod parent)
            : base()
        {
            Parent = parent;
        }

        public virtual IManagementPeriod Parent 
        { 
            get
            {
                if (this.parent == null && this.Count > 0)
                    this.parent = this[0].ManagementPeriod;
                return this.parent;
            }
            set { this.parent = value; }
        }

        public IManagementPeriodUnit GetItemByDate(DateTime date)
        {
            foreach (IManagementPeriodUnit item in this)
                if (item.IsBetween(date))
                    return item;
            return null;
        }

        public IManagementPeriodUnit GetItemByType(ManagementTypes managementType)
        {
            foreach (IManagementPeriodUnit item in this)
                if (item.ManagementPeriod.ManagementType == managementType)
                    return item;
            return null;
        }

        #region Privates

        private IManagementPeriod parent;

        #endregion

    }
}