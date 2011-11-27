using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public class CashDetailLineCollection : IList<CashDetailLine>
    {

        public CashDetailLineCollection() { }
        public CashDetailLineCollection(IAccountTypeInternal account) { this.Account = account; }

        private int groupKey = 0;
        private int groupSubKey = 1;

        public void AddcashLine(IJournalEntryLine line)
        {
            groupSubKey = 1;
            CashDetailLine newLine = new CashDetailLine(line);
            this.dataStore.Add(newLine);
            newLine.Key = this.dataStore.Count;
            newLine.GroupKey = ++groupKey;
            newLine.GroupSubKey = groupSubKey++;
        }

        public void AddTradeLine(IList<IJournalEntryLine> lines)
        {
            bool firstkey = true;
            groupSubKey = 1;
            ++groupKey; 
            foreach (IJournalEntryLine line in lines)
            {
                CashDetailLine newLine = new CashDetailLine(line,firstkey);
                this.dataStore.Add(newLine);
                newLine.Key = this.dataStore.Count;
                newLine.GroupKey = groupKey;
                newLine.GroupSubKey = groupSubKey++;
                firstkey = false;
            }


        }


        #region IList<CashDetailLine> Members

        public int IndexOf(CashDetailLine item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, CashDetailLine item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public CashDetailLine this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<CashDetailLine> Members

        public void Add(CashDetailLine item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(CashDetailLine item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(CashDetailLine[] array, int arrayIndex)
        {
            this.dataStore.CopyTo(array,arrayIndex);
        }

        public int Count
        {
            get { return this.dataStore.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(CashDetailLine item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<CashDetailLine> Members

        public IEnumerator<CashDetailLine> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Privates

        public IAccountTypeInternal Account { get; set; }
        private List<CashDetailLine> dataStore = new List<CashDetailLine>();

        
        #endregion


    }
}
