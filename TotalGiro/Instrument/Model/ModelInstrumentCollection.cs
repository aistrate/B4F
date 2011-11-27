using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts.Instructions.Exclusions;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// This class holds collection of modelcomponents
    /// </summary>
	public class ModelInstrumentCollection : IModelInstrumentCollection
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.Instruments.ModelInstrumentCollection">ModelInstrumentCollection</see> class.
        /// </summary>
        internal ModelInstrumentCollection(IModelVersion parent, IModelComponentCollection modelComponents)
		{
            this.parent = parent;
            this.modelComponents = modelComponents;
			resolveInstruments(modelComponents, 1m);
		}

        protected ModelInstrumentCollection(IModelVersion parent, IModelComponentCollection modelComponents, IRebalanceExclusionCollection excludedComponents)
            : this(parent, modelComponents)
        {
            List<ITradeableInstrument> excludedInstruments = excludedComponents.TradeableInstruments;
            if (excludedInstruments != null && excludedInstruments.Count > 0)
            {
                for (int i = this.Count; i > 0; i--)
                {
                    IModelInstrument item = this[i - 1];
                    if (excludedInstruments.Where(u => u.Key == item.Component.Key).Count() > 0)
                        this.Remove(item);
                }
                // fix the percentage
                decimal totalAllocation = this.TotalAllocation;
                if (this.TotalAllocation != 1M)
                {
                    foreach (IModelInstrument item in this)
                        item.Allocation = item.Allocation / totalAllocation; 
                }
            }
        }


        /// <summary>
        /// Group instruments in
        /// </summary>
        /// <param name="modelComponents">Instruments or models</param>
        /// <param name="Allocation">Allocation percentage</param>
		private void resolveInstruments(IModelComponentCollection modelComponents, decimal Allocation)
		{
			foreach (ModelComponent mc in modelComponents)
			{
				if (mc.ModelComponentType == ModelComponentType.Model)
				{
					resolveInstruments(((ModelModel)mc).Component.LatestVersion.ModelComponents, (Allocation * mc.Allocation));
				}
				else
				{
					ModelInstrument mi = (ModelInstrument)mc;
					if ((modelInstruments != null) && (modelInstruments.ContainsKey(mi.Component)))
						modelInstruments[mi.Component] = new ModelInstrument(this.parent, (IInstrument)mi.Component, ((Allocation * mi.Allocation) + ((ModelInstrument)modelInstruments[mi.Component]).Allocation));
					else
                        modelInstruments.Add(mi.Component, new ModelInstrument(this.parent, (IInstrument)mi.Component, (Allocation * mi.Allocation)));
				}
			}
        }


        #region IModelInstrumentCollection

        public IModelVersion Parent 
        {
            get { return this.parent; } 
        }

        public decimal TotalAllocation 
        {
            get { return this.Sum(u => u.Allocation); } 
        }

        public IModelInstrument Find(IInstrument instrument)
        {
            IModelInstrument retVal = null;
            if (modelInstruments.ContainsKey(instrument))
                retVal = modelInstruments[instrument];
            return retVal;
        }

        public ICashManagementFund GetCashFund()
        {
            ICashManagementFund retVal = null;
            if (this.Count > 0)
            {
                foreach (IModelInstrument mi in this)
                {
                    if (mi.Component.SecCategory.Key == SecCategories.CashManagementFund)
                    {
                        retVal = (ICashManagementFund)mi.Component;
                        break;
                    }
                }
            }
            return retVal;
        }


        public List<IInstrument> Instruments
        {
            get
            {
                var x = (from a in this
                        select a.Component).ToList();
                return x;
            }
        }

        public IModelInstrumentCollection StrippedCollection(IRebalanceExclusionCollection excludedComponents)
        {
            if (excludedComponents != null && excludedComponents.Count > 0)
                return new ModelInstrumentCollection(Parent, modelComponents, excludedComponents);
            else
                return this;
        }

        #endregion

        #region IList<IModelInstrument> Members

        /// <exclude/>
		public int IndexOf(IModelInstrument item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

        /// <exclude/>
        public void Insert(int index, IModelInstrument item)
		{
            if (item != null && !modelInstruments.ContainsKey(item.Component))
                modelInstruments.Add(item.Component, item);
		}

        /// <exclude/>
        public void RemoveAt(int index)
		{
            throw new Exception("The method or operation is not implemented.");
        }


        /// <summary>
        /// Operator this overload
        /// </summary>
        /// <param name="index">Index number</param>
        /// <returns>ModelInstrument</returns>
		public IModelInstrument this[int index]
		{
			get
			{
                IModelInstrument[] list = new IModelInstrument[modelInstruments.Values.Count];
                modelInstruments.Values.CopyTo(list, 0);
                return list[index];
			}
			set { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion

		#region ICollection<IModelInstrument> Members

        /// <summary>
        /// Add ModelInstrument to collection
        /// </summary>
        /// <param name="item">ModelInstrument</param>
		public void Add(IModelInstrument item)
		{
			if (!Contains(item))
                this.modelInstruments.Add(item.Component, item);
            else
                throw new ApplicationException("This modelInstrument is already present.");
		}

        /// <exclude/>
        public void Clear()
		{
            this.modelInstruments.Clear();
		}

        /// <summary>
        /// Item in collection flag
        /// </summary>
        /// <param name="item">ModelInstrument</param>
        /// <returns>Flag</returns>
		public bool Contains(IModelInstrument item)
		{
			if (this.modelInstruments.ContainsKey(item.Component))
				return true;
			else
				return false;
		}

        /// <exclude/>
        public void CopyTo(IModelInstrument[] array, int arrayIndex)
		{
            //IModelInstrument[] returnValue = new IModelInstrument[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                array[i] = this[i];
            }
		}

        /// <summary>
        /// Count number of ModelInstruments
        /// </summary>
		public int Count
		{
			get { return this.modelInstruments.Count; }
		}

        /// <exclude/>
        public bool IsReadOnly
		{
			get { return false; }
		}

        /// <exclude/>
		public bool Remove(IModelInstrument item)
		{
            bool success = false;
            if (Contains(item))
                success = this.modelInstruments.Remove(item.Component);
            return success;
		}

		#endregion

		#region IEnumerable<IModelInstrument> Members

        /// <moduleiscollection>
        /// </moduleiscollection>
		public IEnumerator<IModelInstrument> GetEnumerator()
		{
            foreach (IModelInstrument mi in modelInstruments.Values)
			{
				yield return mi;
			}
		}

		#endregion

		#region IEnumerable Members

        /// <moduleiscollection>
        /// </moduleiscollection>
        IEnumerator IEnumerable.GetEnumerator()
		{
			return modelInstruments.GetEnumerator();
		}

		#endregion

		#region Private Variables

        private IModelVersion parent;
        private IModelComponentCollection modelComponents;
        private Dictionary<IInstrument, IModelInstrument> modelInstruments = new Dictionary<IInstrument, IModelInstrument>();

		#endregion
	}
}
