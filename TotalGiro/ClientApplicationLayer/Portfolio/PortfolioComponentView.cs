using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ClientApplicationLayer.Portfolio
{
    public enum PortfolioComponentType
    {
        Model,
        Instrument,
        Cash
    }
    
    public class PortfolioComponentView
    {
        public PortfolioComponentView(IModelVersion modelVersion, decimal modelAllocation, PortfolioComponentView parent)
            : this(parent)
        {
            PortfolioComponentType = PortfolioComponentType.Model;
            ComponentKey = modelVersion.Key;
            ComponentName = modelVersion.ParentModel.ModelName;
            ModelAllocation = modelAllocation;
        }

        public PortfolioComponentView(IInstrument instrument, decimal modelAllocation, PortfolioComponentView parent)
            : this(parent)
        {
            PortfolioComponentType = PortfolioComponentType.Instrument;
            ComponentKey = instrument.Key;
            ComponentName = instrument.DisplayName;
            ModelAllocation = modelAllocation;
        }

        // Orphan instruments
        public PortfolioComponentView(InstrumentSize size)
            : this(size.Underlying, 0m, null)
        {
        }

        public PortfolioComponentView(Money size)
            : this(size.Underlying, 0m, null)
        {
            PortfolioComponentType = PortfolioComponentType.Cash;
        }

        private PortfolioComponentView(PortfolioComponentView parent)
        {
            Parent = parent;
        }

        public PortfolioComponentType PortfolioComponentType { get; private set; }

        public PortfolioComponentView Parent { get; private set; }

        public int ComponentKey { get; private set; }
        public string ComponentName { get; private set; }
        public decimal ModelAllocation { get; private set; }

        public Money Value { get; set; }
        public decimal Percentage { get; set; }

        public int PositionId { get; set; }

        public int LineNumber { get; set; }

        public int ParentLineNumber { get { return Parent != null ? Parent.LineNumber : 0; } }

        public bool IsModel { get { return PortfolioComponentType == PortfolioComponentType.Model; } }
        public bool IsInstrument { get { return PortfolioComponentType == PortfolioComponentType.Instrument; } }
        public bool IsCash { get { return PortfolioComponentType == PortfolioComponentType.Cash; } }
    }
}
