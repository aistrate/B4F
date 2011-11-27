using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts.Instructions;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.PortfolioComparer
{
    /// <summary>
    /// This enumeration lists the different type of actions that can be done in the portfolioComparer
    /// </summary>
    public enum PortfolioCompareAction
    {
        /// <summary>
        /// Place close orders only
        /// </summary>
        CloseOrders,
        /// <summary>
        /// Do a rebalance
        /// </summary>
        Rebalance,
        /// <summary>
        /// Buy the model
        /// </summary>
        BuyModel,
        /// <summary>
        /// Place Cash Fund Orders Only
        /// </summary>
        CashFundOrders
    }

    /// <summary>
    /// This class is used to pass rebalance parameters to the <see cref="M:B4F.TotalGiro.PortfolioComparer.PortfolioComparer(B4F.TotalGiro.PortfolioComparer.PortfolioCompareSetting)">rebalance method</see>
    /// </summary>
    public class PortfolioCompareSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.TotalGiro.PortfolioComparer.PortfolioCompareSetting">PortfolioCompareSetting</see> class.
        /// </summary>
        /// <param name="compareAction">
        /// When this argument equals 'CloseOrders' it is checked whether positions exist that do not exist in the modelportfolio. 
        /// When this is the case this will result in only sell close size based orders.
        /// When the argument equals 'Rebalance' a normal rebalance is done.
        /// However the method will generate an error when positions do exist that do not exist in the modelportfolio.
        /// When the argument equals 'CashFundOrders' the remaining cash will be transferred to cash fund.
        /// </param>
        /// <param name="instruction">The instruction that generated the orders</param>
        /// <param name="engineParams">The parameters used in porfolio compare action</param>
        public PortfolioCompareSetting(PortfolioCompareAction compareAction, IInstruction instruction, InstructionEngineParameters engineParams)
        {
            if (instruction == null)
                throw new ApplicationException("The instruction is mandatory");

            this.CompareAction = compareAction;
            this.Instruction = instruction;
            this.EngineParams = engineParams;
        }

        internal PortfolioCompareSetting() { }

        /// <summary>
        /// When this property is true the result is that only the cash is used to buy accordingly to the modelportfolio.
        /// When the property is false the account's total portfolio is compared with the modelportfolio
        /// </summary>
        public bool UseCashOnly
        {
            get 
            { 
                return (Instruction.InstructionType == InstructionTypes.BuyModel); 
            }
        }

        /// <summary>
        /// When this property is true it is checked whether positions exist that do not exist in the modelportfolio. 
        /// When this is the case this will result in only sell close size based orders.
        /// When the property is false a normal rebalance is done.
        /// However the method will generate an error when positions do exist that do not exist in the modelportfolio.
        /// </summary>
        public PortfolioCompareAction CompareAction
        {
            get { return this.compareAction; }
            set { this.compareAction = value; }
        }

        /// <summary>
        /// This is the <see cref="T:B4F.TotalGiro.Orders.OrderActionTypes">action</see> that caused the creation of the order
        /// </summary>
        public OrderActionTypes ActionType
        {
            get 
            {
                if (Instruction.IsTypeRebalance)
                    return ((IInstructionTypeRebalance)Instruction).OrderActionType;
                else
                    return OrderActionTypes.NoAction;
            }
        }

        public IInstruction Instruction
        {
            get { return instruction; }
            set { instruction = value; }
        }

        public InstructionEngineParameters EngineParams
        {
            get { return this.engineParams; }
            set { this.engineParams = value; }
        }

        public IList<ITradeableInstrument> TradeableInstrumentsToExclude
        {
            get
            {
                IList<ITradeableInstrument> instruments = null;
                if (Instruction != null && Instruction.InstructionType == InstructionTypes.Rebalance)
                    instruments = ((IRebalanceInstruction)Instruction).ExcludedComponents.TradeableInstruments;
                return instruments;
            }
        }

        public IList<IInstrument> InstrumentsToExclude
        {
            get
            {
                IList<IInstrument> instruments = null;
                if (TradeableInstrumentsToExclude != null && TradeableInstrumentsToExclude.Count > 0)
                    instruments = TradeableInstrumentsToExclude.Cast<IInstrument>().ToList();
                return instruments;
            }
        }

        /// <summary>
        /// This method checks whether the instrument is excluded from the Rebalance.
        /// </summary>
        /// <typeparam name="instrument">The instrument being searched</typeparam>
        /// <returns>True when the instrument is not excluded</returns>
        public bool IsInstrumentIncluded(IInstrument instrument)
        {
            bool retVal = true;

            IList<IInstrument> instruments = InstrumentsToExclude;
            if (instruments != null && instruments.Count > 0)
            {
                foreach (IInstrument inst in instruments)
                {
                    if (inst.Equals(instrument))
                    {
                        retVal = false;
                        break;
                    }
                }
            }
            return retVal;
        }

        #region Privates

        private PortfolioCompareAction compareAction;
        private IInstruction instruction;
        private InstructionEngineParameters engineParams;

        #endregion
    }
}
