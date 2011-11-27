using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Valuations
{
    public class ValuationCollection: IList<Valuation>
    {
        internal ValuationCollection(IAccountTypeInternal account)
        {
            this.account = account;
        }

        #region Methods

        public IList<Valuation> GetItemsByInstrument(IInstrument instrument)
        {
            IList<Valuation> list = null;
            this.valuationsByInstrument.TryGetValue(instrument, out list);
            return list;
        }

        public IList<Valuation> GetItemsByDate(DateTime date)
        {
            IList<Valuation> list = null;
            this.valuationsByDate.TryGetValue(date, out list);
            return list;
        }

        #endregion


        #region IList<Valuation> Members

        public int IndexOf(Valuation item)
        {
            int index = 0;
            foreach (Valuation valuation in this.valuations)
            {
                if (valuation.Equals(item))
                    return index;
                index++;
            }
            return -1;
        }

        public void Insert(int index, Valuation item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Valuation this[int index]
        {
            get
            {
                return this.valuations[index];
            }
            set
            {
                this.valuations[index] = value;
            }
        }

        #endregion

        #region ICollection<Valuation> Members

        public void Add(Valuation item)
        {
            IList<Valuation> list;
            item.Account = this.account;
            this.valuations.Add(item);

            // By Instrument
            if (this.valuationsByInstrument.TryGetValue(item.Instrument, out list))
                list.Add(item);
            else
            {
                list = new List<Valuation>();
                list.Add(item);
                valuationsByInstrument.Add(item.Instrument, list);
            }

            // By Date
            if (this.valuationsByDate.TryGetValue(item.Date, out list))
                list.Add(item);
            else
            {
                list = new List<Valuation>();
                list.Add(item);
                valuationsByDate.Add(item.Date, list);
            }
        }

        public void Clear()
        {
            this.valuations.Clear();
            this.valuationsByInstrument.Clear();
            this.valuationsByDate.Clear();
        }

        public bool Contains(Valuation item)
        {
            foreach (Valuation valuation in this.valuations)
                if (valuation.Equals(item))
                    return true;
            return false;
        }

        public void CopyTo(Valuation[] array, int arrayIndex)
        {
            this.valuations.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.valuations.Count; }
        }

        /// <exclude/>
        public bool IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(Valuation item)
        {
            this.valuations.Remove(item);
            return true;
        }

        #endregion

        #region IEnumerable<Valuation> Members

        public IEnumerator<Valuation> GetEnumerator()
        {
            for (int i = 0; i < valuations.Count; i++)
                yield return valuations[i];
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.valuations.GetEnumerator();
        }

        #endregion

        #region Privates

        private IAccountTypeInternal account;
        private IList<Valuation> valuations = new List<Valuation>();
        private IDictionary<IInstrument, IList<Valuation>> valuationsByInstrument = new Dictionary<IInstrument, IList<Valuation>>();
        private IDictionary<DateTime, IList<Valuation>> valuationsByDate = new Dictionary<DateTime, IList<Valuation>>();

        #endregion
    }
}
