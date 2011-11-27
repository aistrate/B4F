using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Accounts.RemisierHistory
{
    /// <summary>
    /// The class that holds the account's remisier related data changes per account
    /// </summary>
    public class RemisierHistoryCollection: GenericCollection<IRemisierHistory>, IRemisierHistoryCollection
    {
        internal RemisierHistoryCollection(ICustomerAccount parent, IList bagOfRemisierHistories)
            : base(bagOfRemisierHistories)
        {
            this.parent = parent;
        }

        /// <summary>
        /// The account the remisier related data changes belong to.
        /// </summary>
        public ICustomerAccount Parent
        {
            get { return parent; }
        }

        public override void Add(IRemisierHistory item)
        {
            if (Count > 0)
            {
                foreach (IRemisierHistory hist in this)
                {
                    if (hist.ChangeDate.Date.Equals(item.ChangeDate.Date))
                    {
                        hist.RemisierEmployee = item.RemisierEmployee;
                        Parent.RemisierEmployee = item.RemisierEmployee;
                        hist.KickBack = item.KickBack;
                        hist.IntroductionFee = item.IntroductionFee;
                        hist.IntroductionFeeReduction = item.IntroductionFeeReduction;
                        hist.SubsequentDepositFee = item.SubsequentDepositFee;
                        hist.SubsequentDepositFeeReduction = item.SubsequentDepositFeeReduction;
                        return;
                    }
                }
            }
            item.Account = Parent;
            Parent.RemisierEmployee = item.RemisierEmployee;
            Parent.CurrentRemisierDetails = item;
            base.Add(item);
        }

        public IRemisierHistory GetItemByDate(DateTime date)
        {
            foreach (IRemisierHistory item in this)
                if (item.ChangeDate <= date && (date < item.EndDate || item.EndDate == DateTime.MinValue))
                    return item;
            return null;
        }

        #region Privates

        private ICustomerAccount parent;

        #endregion
    }
}
