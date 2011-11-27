using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    /// <summary>
    /// This class is used as a key in the LastValuationCashMutationHolder class
    /// </summary>
    public class ValuationCashMutationKey
    {
        #region Constructors

        protected ValuationCashMutationKey() { }

        public ValuationCashMutationKey(IAccountTypeInternal account, IInstrument instrument, ValuationCashTypes valuationCashType)
        {
            this.Account = account;
            this.Instrument = instrument;
            this.ValuationCashType = valuationCashType;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overridden equality operator
        /// </summary>
        /// <param name="obj">ValuationCashMutationKey object to compare to</param>
        /// <returns>true if equal, false if not equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is System.DBNull)
                    return false;
                else
                    return (this == (ValuationCashMutationKey)obj);
            }
            else
                return false;
        }

        /// <summary>
        /// Check wether two instances of the <see cref="T:B4F.TotalGiro.Interfaces.ValuationCashMutationKey">ValuationCashMutationKey</see> class are equal
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Interfaces.ValuationCashMutationKey">ValuationCashMutationKey</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Interfaces.ValuationCashMutationKey">ValuationCashMutationKey</see> class on the right hand side</param>
        /// <returns>true if equal, false if not equal.</returns>
        public static bool operator ==(ValuationCashMutationKey lhs, ValuationCashMutationKey rhs)
        {
            bool retVal = false;
            if (((Object)rhs == null) && ((Object)lhs == null))
                retVal = true;
            else if (((Object)rhs == null) || ((Object)lhs == null))
                retVal = false;
            else
            {
                // Use Eq because it is not possible to do operator overloading on an interface
                if ((lhs.Account.Equals(rhs.Account)) && (lhs.Instrument == rhs.Instrument) && (lhs.ValuationCashType == rhs.ValuationCashType))
                    retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Check wether two instances of the <see cref="T:B4F.TotalGiro.Interfaces.ValuationCashMutationKey">ValuationCashMutationKey</see> class are not equal
        /// </summary>
        /// <param name="lhs">The first instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the left hand side</param>
        /// <param name="rhs">The second instance of the <see cref="T:B4F.TotalGiro.Instruments.InstrumentSize">InstrumentSize</see> class on the right hand side</param>
        /// <returns>true if not equal, false if equal.</returns>
        public static bool operator !=(ValuationCashMutationKey lhs, ValuationCashMutationKey rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Account.Key;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Account.Number, (Instrument != null ? Instrument.DisplayIsin : "No instrument"), ValuationCashType.ToString());
        }

        #endregion

        #region Properties

        public IAccountTypeInternal Account
        {
            get { return account; }
            private set { account = value; }
        }

        public IInstrument Instrument
        {
            get { return instrument; }
            private set { instrument = value; }
        }


        public ValuationCashTypes ValuationCashType
        {
            get { return valuationCashType; }
            private set { valuationCashType = value; }
        }

        #endregion

        #region Privates

        private IAccountTypeInternal account;
        private IInstrument instrument;
        private ValuationCashTypes valuationCashType;

        #endregion

    }
}
