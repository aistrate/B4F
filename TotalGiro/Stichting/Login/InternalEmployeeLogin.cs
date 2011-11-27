using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Stichting.Login
{
    /// <summary>
    /// This class holds an employee
    /// </summary>
    public abstract class InternalEmployeeLogin : Login, IInternalEmployeeLogin
    {
        protected InternalEmployeeLogin()  {  }

        /// <summary>
        /// Get/set employer
        /// </summary>
        public virtual IManagementCompany Employer
        {
            get { return employer; }
            set { employer = value; }
        }
        
        /// <summary>
        /// The upper limit of how much (in base currency) can be stornoed by this Employee, per transaction.
        /// </summary>
        public virtual Money StornoLimit
        {
            get { return stornoLimit; }
            set { stornoLimit = value; }
        }

        public override bool IsEmployee
        {
            get { return true; }
        }

        /// <summary>
        /// Checks whether amount is below (or equal to) Storno Limit.
        /// </summary>
        /// <param name="amount">The amount to check against Storno Limit.</param>
        /// <param name="raiseException">If True, an exception will be thrown in case the amount doesn't verify the storno limit; if False, only the return value will be affected.</param>
        /// <returns>True if amount below or equal to Storno Limit, False if not.</returns>
        public bool VerifyStornoLimit(Money amount, bool raiseException)
        {
            if (stornoLimit != null && amount != null && amount.IsNotZero)
            {
                Money convertedAmount = amount.Convert((ICurrency)stornoLimit.Underlying);
                bool isOk = (convertedAmount.Abs() <= stornoLimit.Abs());
                
                if (raiseException && !isOk)
                    throw new ApplicationException(string.Format("Amount value ({0}) is above current user's storno limit ({1}).",
                                                                 convertedAmount.Abs().DisplayString, stornoLimit.Abs().DisplayString));
                else
                    return isOk;
            }
            else
                return true;
        }

        #region Private Variables

        private IManagementCompany employer;
        private Money stornoLimit;

        #endregion

    }
}
