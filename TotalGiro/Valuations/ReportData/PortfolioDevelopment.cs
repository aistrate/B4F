using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Valuations
{
    /// <summary>
    /// Used to report the portfolio development using the Valuation class
    /// </summary>
    public class PortfolioDevelopment
    {
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="valuations"></param>
        /// <param name="cashValuations"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        internal PortfolioDevelopment(IAccountTypeInternal account, 
            IList<IValuation> valuations, IList<ISecurityValuationMutation> securityMutations, 
            IList<IValuationCashMutation> cashMutations, IList<IDepositWithdrawal> depositsWithdrawals, 
            IList<ValuationCashType> valuationCashTypes, DateTime beginDate, DateTime endDate, int modelID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            // check dates
            if (Util.IsNullDate(beginDate) || Util.IsNullDate(endDate))
                throw new ApplicationException(errSuffix + "The Start date and End date are mandatory.");
            else if (beginDate >= endDate)
                throw new ApplicationException(errSuffix + "The Start date must be before the End date.");

            this.account = account;
            if (account.FirstTransactionDate > beginDate)
                this.beginDate = account.FirstTransactionDate;
            else
                this.beginDate = beginDate;

            if (Util.IsNotNullDate(account.ValuationsEndDate) && account.ValuationsEndDate.Value < endDate && account.ValuationsEndDate.Value >= beginDate)
                this.endDate = account.ValuationsEndDate.Value;
            else
                this.endDate = endDate;

            fillValuationData(valuations);
            fillRealisedAmountData(securityMutations);
            fillDepositWithdrawalData(depositsWithdrawals);
            fillCashMutationData(cashMutations, valuationCashTypes);

            // old stuff. Replace to fillModelBenchMarkPerformance. These fileds are move to table ModelPerformance.
            //if (((IAccountTypeCustomer)account).ModelPortfolio != null) this.benchMarkPerformance = ((IAccountTypeCustomer)account).ModelPortfolio.TempBenchMark;
            //if (((IAccountTypeCustomer)account).ModelPortfolio != null) this.benchmarkValue = ((IAccountTypeCustomer)account).ModelPortfolio.BenchMarkValue;
            //if (((IAccountTypeCustomer)account).ModelPortfolio != null) this.iboxxTarget = ((IAccountTypeCustomer)account).ModelPortfolio.IBoxxTarget;
            //if (((IAccountTypeCustomer)account).ModelPortfolio != null) this.msciworldTarget = ((IAccountTypeCustomer)account).ModelPortfolio.MSCIWorldTarget;
            //if (((IAccountTypeCustomer)account).ModelPortfolio != null) this.compositeTarget = ((IAccountTypeCustomer)account).ModelPortfolio.CompositeTarget;

            fillModelBenchMarkPerformance(GetModelBenchMarkPerformance(session, modelID, endDate));

            session.Close();
            if (valuations != null && valuations.Count > 0)
            {
                ModDietzCalculator calculator = new ModDietzCalculator(TotalValueBegin, TotalValueEnd, this.beginDate, this.endDate, depositsWithdrawals);
                this.investmentReturn = calculator.GetInvestmentReturn();
            }
        }

        public static IList<IModelPerformance> GetModelBenchMarkPerformance(IDalSession session, int modelID, DateTime endDate)
        {
            Hashtable parameters = new Hashtable(1);
            parameters.Add("modelID", modelID);
            parameters.Add("quarter", Util.GetQuarter(endDate));
            parameters.Add("yyyy", endDate.Year);
            return session.GetTypedListByNamedQuery<IModelPerformance>(
                "B4F.TotalGiro.Instruments.ModelBechmarkPerformances", parameters);

        }

        private void fillModelBenchMarkPerformance(IList<IModelPerformance> testmodelbenchmarkperformance)
        {

            foreach (var modelbenchmark in testmodelbenchmarkperformance)
            {
                this.benchmarkValue = modelbenchmark.BenchMarkValue;
                this.benchMarkPerformance = modelbenchmark.BenchMarkValue;
                this.iboxxTarget = modelbenchmark.IBoxxTarget;
                this.msciworldTarget = modelbenchmark.MSCIWorldTarget;
                this.compositeTarget = modelbenchmark.CompositeTarget;
            }
        }

        private void fillValuationData(IList<IValuation> valuations)
        {
            // put the valuations in the correct list
            foreach (IValuation valuation in valuations)
            {
                if (this.account == null)
                    this.account = valuation.Account;
                else if (!this.account.Equals(valuation.Account))
                    throw new ApplicationException(errSuffix + "All valuations should be for the same account.");

                if (valuation.Date.Equals(BeginDate.AddDays(-1)))
                    valuationsTBegin.Add(valuation);
                else if (valuation.Date.Equals(EndDate))
                    valuationsTEnd.Add(valuation);
            }

            // get begin data
            foreach (IValuation valuation in valuationsTBegin)
            {
                totalValueBegin += valuation.BaseMarketValue;
                unRealisedAmountPreviousPeriod += (valuation.BaseMarketValue - valuation.BookValue);
                ////Check to see if the Instrument is still in the Portfolio at closing time.
                ////If so, then set up the UnRealized Gain: If not then the Gain was Realized!
                //if (!valuation.Instrument.IsCash)
                //{
                //    bool closed = true;
                //    foreach (IValuation check in valuationsTEnd)
                //    {
                //        if (valuation.Instrument.TopParentInstrument == check.Instrument.TopParentInstrument)
                //        {
                //            closed = false;
                //            break;
                //        }
                //    }
                //    if (!closed)
                //    {
                //        unRealisedAmount -= valuation.UnRealisedAmountToDate;
                //    }
                //    else
                //    {
                //        realisedAmount -= valuation.UnRealisedAmountToDate;
                //    }
                //}
            }

            // get end data
            foreach (IValuation valuation in valuationsTEnd)
            {
                totalValueEnd += valuation.BaseMarketValue;
                if (!valuation.Instrument.IsCash)
                    unRealisedAmount += valuation.UnRealisedAmountToDate;
            }
        }

        private void fillRealisedAmountData(IList<ISecurityValuationMutation> securityMutations)
        {
            foreach (ISecurityValuationMutation mut in securityMutations)
                realisedAmount += mut.BaseRealisedAmount;
        }

        private void fillDepositWithdrawalData(IList<IDepositWithdrawal> depositsWithdrawals)
        {
            if (depositsWithdrawals != null)
            {
                foreach (IDepositWithdrawal dpWd in depositsWithdrawals)
                {
                    deposits += dpWd.Deposit;
                    withdrawals += dpWd.WithDrawal;
                }
            }
        }

        private void fillCashMutationData(IList<IValuationCashMutation> cashMutations, IList<ValuationCashType> valuationCashTypes)
        {
            if (cashMutations != null && cashMutations.Count > 0)
            {
                foreach (IValuationCashMutation cashMutation in cashMutations)
                {
                    if (this.account == null)
                        this.account = cashMutation.Account;
                    else if (!this.account.Equals(cashMutation.Account))
                        throw new ApplicationException(errSuffix + "All valuations should be for the same account.");

                    ValuationCashTypes key = cashMutation.ValuationCashType;
                    if (!cashMovements.Keys.Contains(key))
                        cashMovements.Add(new KeyValuePair<ValuationCashTypes, PortfolioDevelopmentCash>(key, new PortfolioDevelopmentCash(this, key, getCashTypeDescription(valuationCashTypes, key), cashMutation.BaseAmount)));
                    else
                        cashMovements[key].Amount += cashMutation.BaseAmount;
                }
            }
        }

        private string getCashTypeDescription(IList<ValuationCashType> valuationCashTypes, ValuationCashTypes key)
        {
            string description = "Unknown";
            foreach (ValuationCashType cashType in valuationCashTypes)
            {
                if (cashType.Key.Equals(key))
                    description = cashType.Description;
            }
            return description;
        }



        private ICurrency getBaseCurrency()
        {
            return Account.AccountOwner.StichtingDetails.BaseCurrency;
        }


        /// <summary>
        /// The relevant account
        /// </summary>
        public virtual IAccountTypeInternal Account
        {
            get { return account; }
        }

        public virtual DateTime BeginDate
        {
            get { return beginDate; }
        }

        public virtual DateTime EndDate
        {
            get { return endDate; }
        }

        public virtual Money TotalValueBegin
        {
            get
            {
                if (totalValueBegin != null)
                    return totalValueBegin;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public virtual Money TotalValueEnd
        {
            get
            {
                if (totalValueEnd != null)
                    return totalValueEnd;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public virtual Money TotalValueDifference
        {
            get { return (TotalValueEnd - TotalValueBegin); }
        }

        public virtual decimal InvestmentReturn
        {
            get { return investmentReturn; }
        }

        public virtual decimal InvestmentReturnPercentage
        {
            get { return Math.Round(InvestmentReturn * 100M, 2); }
        }

        public virtual string DisplayInvestmentReturnPercentage
        {
            get { return string.Format("{0}%", InvestmentReturnPercentage.ToString()); }
        }

        public virtual Money RealisedAmount
        {
            get
            {
                if (realisedAmount != null)
                    return realisedAmount;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public virtual Money UnRealisedAmount
        {
            get
            {
                if (unRealisedAmount != null)
                    return unRealisedAmount;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public virtual Money UnRealisedAmountPreviousPeriod
        {
            get
            {
                if (unRealisedAmountPreviousPeriod != null)
                    return unRealisedAmountPreviousPeriod;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public virtual Money Withdrawals
        {
            get
            {
                if (withdrawals != null)
                    return withdrawals;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public virtual Money Deposits
        {
            get
            {
                if (deposits != null)
                    return deposits;
                else
                    return new Money(0M, getBaseCurrency());
            }
        }

        public ICollection<PortfolioDevelopmentCash> CashMovements
        {
            get { return cashMovements.Values; }
        }

        public Decimal BenchMarkPerformance
        {
            get { return benchMarkPerformance; }
        }

        public Decimal BenchMarkValue
        {
            get { return benchmarkValue; }
        }

        public Decimal IBoxxTarget
        {
            get { return iboxxTarget; }
        }

        public Decimal MSCIWorldTarget
        {
            get { return msciworldTarget; }
        }

        public Decimal CompositeTarget
        {
            get { return compositeTarget; }
        }

        #region Privates

        private const string errSuffix = "Error in PortfolioDevelopment class: ";
        private IAccountTypeInternal account;
        private DateTime beginDate;
        private DateTime endDate;
        private Money totalValueBegin;
        private Money totalValueEnd;
        private Money realisedAmount;
        private Money unRealisedAmount;
        private Money unRealisedAmountPreviousPeriod;
        private Money withdrawals;
        private Money deposits;
        private decimal investmentReturn;
        private Decimal benchMarkPerformance;

        private decimal benchmarkValue;
        private decimal iboxxTarget;
        private decimal msciworldTarget;
        private decimal compositeTarget;

        private ValuationCollection valuationsTBegin = new ValuationCollection();
        private ValuationCollection valuationsTEnd = new ValuationCollection();
        private GenericDictionary<ValuationCashTypes, PortfolioDevelopmentCash> cashMovements = new GenericDictionary<ValuationCashTypes, PortfolioDevelopmentCash>();

        #endregion
    }
}
