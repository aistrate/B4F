using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts.Positions;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationKey
    {
        internal ValuationKey(IInstrument instrument)
        {
            this.Instrument = instrument;
        }

        public IInstrument Instrument
        {
            get { return this.instrument; }
            set { this.instrument = value; }
        }

        internal void Add(IPositionTx posTx)
        {
            DateTime tradeDate = posTx.TransactionDate;
            ValuationMutation mutation;
            if (mutations.ContainsKey(tradeDate))
                mutation = mutations[tradeDate];
            else
            {
                mutation = new ValuationMutation(this, tradeDate, LastMutation);
                if (mutations.Count == 0)
                    FirstMutation = mutation;
                mutations.Add(tradeDate, mutation);
                LastMutation = mutation;
            }
            mutation.AddTx(posTx);
        }

        internal void AddCash(IPositionTx posTx)
        {
            DateTime tradeDate = posTx.TransactionDate;
            ValuationCostIncomeMutation mutation;
            if (mutationsCI.ContainsKey(tradeDate))
                mutation = mutationsCI[tradeDate];
            else
            {
                mutation = new ValuationCostIncomeMutation(this, tradeDate, LastMutationCI);
                if (mutationsCI.Count == 0)
                    FirstMutationCI = mutation;
                mutationsCI.Add(tradeDate, mutation);
                LastMutationCI = mutation;
            }
            mutation.AddTx(posTx);
        }

        internal bool IsDateInRange(DateTime date)
        {
            bool retVal = false;

            if (FirstMutation != null && date.CompareTo(FirstMutation.MutationDate) >= 0)
            {
                if (FirstMutation.Equals(LastMutation))
                    retVal = true;
                if (!(date.CompareTo(LastMutation.MutationDate) > 0 && LastMutation.Size.IsZero))
                    retVal = true;
            }
            return retVal;
        }

        internal ValuationMutation GetMutationByDate(DateTime date)
        {
            ValuationMutation mutation = null;
            mutations.TryGetValue(date, out mutation);
            return mutation;
        }

        internal ValuationMutation FindPreviousMutation(DateTime date)
        {
            ValuationMutation prevMutation = null;

            if (FirstMutation != null && date.CompareTo(FirstMutation.MutationDate) >= 0)
            {
                foreach (ValuationMutation mutation in mutations.Values)
                {
                    if (prevMutation == null)
                        prevMutation = mutation;
                    else
                    {
                        if (mutation.MutationDate.CompareTo(date) >= 0)
                            return prevMutation;
                    }
                }
            }
            return prevMutation;
        }

        public ValuationCostIncomeMutation GetCIMutationByDate(DateTime date)
        {
            ValuationCostIncomeMutation mutation = null;
            mutationsCI.TryGetValue(date, out mutation);
            return mutation;
        }

        public ValuationCostIncomeMutation FindPreviousCIMutation(DateTime date)
        {
            ValuationCostIncomeMutation prevMutation = null;

            if (FirstMutationCI != null && date.CompareTo(FirstMutationCI.MutationDate) >= 0)
            {
                foreach (ValuationCostIncomeMutation mutation in mutationsCI.Values)
                {
                    if (prevMutation == null)
                        prevMutation = mutation;
                    else
                    {
                        if (mutation.MutationDate.CompareTo(date) >= 0)
                            return prevMutation;
                    }
                }
            }
            return prevMutation;
        }

        public ValuationMutation FirstMutation
        {
            get { return this.firstMutation; }
            private set { this.firstMutation = value; }
        }

        public ValuationMutation LastMutation
        {
            get { return this.lastMutation; }
            private set { this.lastMutation = value; }
        }

        public ValuationCostIncomeMutation FirstMutationCI
        {
            get { return this.firstMutationCI; }
            private set { this.firstMutationCI = value; }
        }

        public ValuationCostIncomeMutation LastMutationCI
        {
            get { return this.lastMutationCI; }
            private set { this.lastMutationCI = value; }
        }
	
        #region Privates

        private IInstrument instrument;
        private Dictionary<DateTime, ValuationMutation> mutations = new Dictionary<DateTime, ValuationMutation>();
        private ValuationMutation firstMutation;
        private ValuationMutation lastMutation;
        private Dictionary<DateTime, ValuationCostIncomeMutation> mutationsCI = new Dictionary<DateTime, ValuationCostIncomeMutation>();
        private ValuationCostIncomeMutation firstMutationCI;
        private ValuationCostIncomeMutation lastMutationCI;

        #endregion
    }
}
