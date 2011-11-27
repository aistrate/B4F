using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class ModelPerformanceCollection : GenericCollection<IModelPerformance>, IModelPerformanceCollection
    {
        internal ModelPerformanceCollection(IPortfolioModel parent, IList bagOfModelPerformances)
            : base(bagOfModelPerformances)
        {
            this.parent = parent;
        }

        public IPortfolioModel Parent
        {
            get { return parent; }
        }

        #region Privates

        private IPortfolioModel parent;

        #endregion
    }
}
