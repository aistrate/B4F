using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Instruments;
using System.Collections;
 
namespace B4F.TotalGiro.Accounts.Instructions
{
    public class InstructionOrderCollection : TransientDomainCollection<IOrder>, IInstructionOrderCollection
    {
        public InstructionOrderCollection() : base() { }
            /// <exclude/>
            /// 
        internal InstructionOrderCollection(IInstruction parentInstruction)
                : base()
            {
                this.ParentInstruction = parentInstruction;
            }

        #region IAccountOrderCollection

        /// <summary>
        /// The instruction the order collection belongs to
        /// </summary>
        public IInstruction ParentInstruction
        {
            get { return this.parentInstruction; }
            set
            {
                this.parentInstruction = value;
                IsInitialized = true;
            }
        }

        public IInstructionOrderCollection NewCollection(Func<IOrder, bool> criteria)
        {
            InstructionOrderCollection returnValue = new InstructionOrderCollection(this.ParentInstruction);
            returnValue.AddRange(this.Where(criteria));
            return returnValue;
        }

        /// <summary>
        /// This method returns a order collection without the instruments to exclude
        /// </summary>
        /// <param name="excludedInstruments">The instruments to exclude from the result</param>
        /// <returns>A filtered collection of orders</returns>
        public IInstructionOrderCollection Exclude(IList<IInstrument> excludedInstruments)
        {     
            if (excludedInstruments == null || excludedInstruments.Count == 0)
                return this;

            Func<IOrder, bool> predicate = x => !excludedInstruments.Contains(x.RequestedInstrument);                
            return NewCollection(predicate);
        }


        #endregion

        #region Private Variables

        private IInstruction parentInstruction;

        #endregion
    }
}
