using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Accounts.Portfolios.CashPositions
{
    public class BuyingPowerDisplayCollection : IList<BuyingPowerDisplay>
    {
        public BuyingPowerDisplayCollection(IAccountTypeInternal account)
        {
            this.Account = account;
        }

        public void AddBlankLine()
        {
            BuyingPowerDisplay newLine = new BuyingPowerDisplay(Account);
            newLine.IsSubTotalLine = false;
            Add(newLine);

        }



        public void AddCashSummaryLine()
        {
            BuyingPowerDisplay summary = new BuyingPowerDisplay(this.Account);
            summary.LineDescription = "Cash Total";
            summary.BaseValue = this.dataStore.Select(c => c.BaseValue).Sum(baseCurrency);
            summary.IsSubTotalLine = true;
            summary.IsCashFundLine = false;
            Add(summary);
        }

        public void AddCashFundLine(Money CMF)
        {
            BuyingPowerDisplay summary = new BuyingPowerDisplay(this.Account);
            summary.LineDescription = "Cash Fund(CMF)";
            summary.Value = summary.BaseValue = CMF;
            summary.IsSubTotalLine = false;
            summary.IsCashFundLine = true;
            Add(summary);
        }

        public void AddBuyingPowerLine()
        {
            BuyingPowerDisplay summary = new BuyingPowerDisplay(this.Account);
            summary.LineDescription = "Buying Power";
            summary.BaseValue = this.dataStore.Where(x => x.IsSubTotalLine).Select(c => c.BaseValue).Sum(baseCurrency);
            summary.BaseValue += this.dataStore.Where(x => x.IsCashFundLine).Select(c => c.BaseValue).FirstOrDefault();
            summary.BaseValue += this.dataStore.Where(x => x.IsUnSettledIncludeBuyingPower).Select(c => c.BaseValue).Sum(baseCurrency);
            summary.IsSubTotalLine = false;
            summary.IsCashFundLine = false;
            summary.IsSummaryLine = true;
            Add(summary);
        }

        private ICurrency baseCurrency
        {
            get
            {
                return this.Account.BaseCurrency;
            }
        }



        #region IList<BuyingPowerDisplay> Members

        public int IndexOf(BuyingPowerDisplay item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, BuyingPowerDisplay item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            dataStore.RemoveAt(index);
        }

        public BuyingPowerDisplay this[int index]
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

        #region ICollection<BuyingPowerDisplay> Members

        public IList<BuyingPowerDisplay> ToList()
        {
            return this.dataStore.ToList();
        }



        public void Add(BuyingPowerDisplay item)
        {
            item.Key = this.Count + 1;
            this.dataStore.Add(item);
            
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(BuyingPowerDisplay item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(BuyingPowerDisplay[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return this.dataStore.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(BuyingPowerDisplay item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<BuyingPowerDisplay> Members

        public IEnumerator<BuyingPowerDisplay> GetEnumerator()
        {
            foreach (BuyingPowerDisplay bpd in dataStore)
            {
                yield return bpd;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
        public IAccountTypeInternal  Account { get; set; }
        private List<BuyingPowerDisplay> dataStore = new List<BuyingPowerDisplay>();
    }
}
