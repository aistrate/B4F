using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Accounts
{
    /// <summary>
    /// This nostro account is the management company's own (customer) account where they keep their own positions and trade with.
    /// It resembles the Customer account except for probably they don't pay any commission.
    /// It is a internal customertype account of the TotalGiro system.
    /// </summary>
    public class NostroAccount : OwnAccount, INostroAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Accounts.NostroAccount">NostroAccount</see> class.
        /// </summary>
        public  NostroAccount() { }

        /// <summary>
        /// The AccountType defines the type of account.
        /// </summary>
        /// 
        public NostroAccount(string number, string shortName, IManagementCompany accountOwner)
            : base(number, shortName, accountOwner)
        {
        }
        public override AccountTypes AccountType
        {
            get { return AccountTypes.Nostro; }
        }



        /// <summary>
        /// The upper limit of how much (in base currency) can be stornoed against this Nostro account.
        /// </summary>
        public Money StornoLimit
        {
            get { return stornoLimit; }
            set { stornoLimit = value; }
        }

        /// <summary>
        /// Checks whether amount throws total portfolio value of nostro account over Storno Limit.
        /// </summary>
        /// <param name="amount">The amount to add to account's total portfolio before checking against Storno Limit.</param>
        /// <param name="raiseException">If True, an exception will be thrown in case the amount doesn't verify the storno limit; if False, only the return value will be affected.</param>
        /// <returns>True if new portfolio value (of nostro account) is below or equal to Storno Limit, False if not.</returns>
        public bool VerifyStornoLimit(Money amount, bool raiseException)
        {
            if (stornoLimit != null)
            {
                Money oldTotalPortfolio = TotalAll;    // expressed in base currency
                Money baseCurrencyAmount = amount.CurrentBaseAmount;
                Money absStornoLimit = stornoLimit.CurrentBaseAmount.Abs();
                Money newTotalPortfolio = oldTotalPortfolio + baseCurrencyAmount;

                bool isOk = (newTotalPortfolio.Abs() <= absStornoLimit) ||
                            (newTotalPortfolio.Abs() - absStornoLimit < oldTotalPortfolio.Abs() - absStornoLimit);

                if (raiseException && !isOk)
                    throw new ApplicationException(
                        string.Format("New portfolio value ({0}) of Nostro account {1} ({2}) is above storno limit ({3}).",
                                      newTotalPortfolio.Abs().DisplayString, Number, ShortName, stornoLimit.Abs().DisplayString));
                else
                    return isOk;
            }
            else
                return true;
        }



        #region Private Variables

        private Money stornoLimit;
        private IManagementCompany company = null;

        #endregion
    }
}
