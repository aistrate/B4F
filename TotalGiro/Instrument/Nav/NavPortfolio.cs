using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments.Nav
{
    public class NavPortfolio : TransientDomainCollection<INavPosition>, INavPortfolio
    {
        public NavPortfolio()
            : base() { }

        public NavPortfolio(INavCalculation parent)
            : base()
        {
            Parent = parent;
        }

        public INavCalculation Parent { get; set; }

        public void AddNavPosition(INavPosition entry)
        {
            entry.Parent = Parent;
            base.Add(entry);
        }

        public Money PortfolioValue()
        {
            IList<INavPosition> positions = this.AsList();
            Money returnValue = null;
            if (positions.Count > 0) returnValue = positions[0].CurrentBaseValue.ZeroedAmount();

            foreach (INavPosition pos in positions)
            {
                returnValue += pos.CurrentBaseValue;
            }

            return returnValue;
        }


    }
}
