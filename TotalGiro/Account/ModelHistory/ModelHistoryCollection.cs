using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.ModelHistory
{
    /// <summary>
    /// The class that holds the modelportfolio changes per account
    /// </summary>
    public class ModelHistoryCollection : GenericCollection<IModelHistory>, IModelHistoryCollection
    {
        internal ModelHistoryCollection(IAccountTypeCustomer parent, IList bagOfModelHistories): base(bagOfModelHistories)
        {
            this.parent = parent;
        }

        /// <summary>
        /// The account the model changes belong to.
        /// </summary>
        public IAccountTypeCustomer Parent
        {
            get { return parent; }
        }

        public override void Add(IModelHistory item)
        {
            //if (Count == 1)
            //{
            //    // special case when model is null in history
            //    IModelHistory hist = this[0];
            //    if (hist.ModelPortfolio == null)
            //    {
            //        hist.ModelPortfolio = item.ModelPortfolio;
            //        hist.IsExecOnlyCustomer = item.IsExecOnlyCustomer;
            //        hist.EmployerRelationship = item.EmployerRelationship;
            //        hist.ChangeDate = Parent.CreationDate;
            //        return;
            //    }
            //}

            if (Count > 0)
            {
                int counter = 0;
                foreach (IModelHistory hist in this.OrderByDescending(a => a.ChangeDate))
                {
                    if (hist.Account.Equals(item.Account) && hist.ChangeDate.Date.Equals(item.ChangeDate.Date))
                    {
                        hist.ModelPortfolio = item.ModelPortfolio;
                        hist.IsExecOnlyCustomer = item.IsExecOnlyCustomer;
                        hist.EmployerRelationship = item.EmployerRelationship;
                        return;
                    }
                    // Change the end date of previous history item
                    if (counter == 0)
                    {
                        if (hist.ChangeDate.Date > item.ChangeDate.Date)
                            throw new Exception("The new portfoliomodel change should be newer than the previous one.");
                        hist.EndDate = item.ChangeDate.Date;
                    }

                    counter++;
                }
            }
            base.Add(item);
        }

        public IModelHistory GetItemByDate(DateTime date)
        {
            foreach (IModelHistory item in this)
                if (item.ChangeDate <= date && (date < item.EndDate || item.EndDate == DateTime.MinValue))
                    return item;
            
            return null;
        }

        #region Privates

        private IAccountTypeCustomer parent;

        #endregion
    }
}
