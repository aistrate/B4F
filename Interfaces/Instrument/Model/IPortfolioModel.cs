using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Fees.FeeRules;

namespace B4F.TotalGiro.Instruments
{
    public interface IPortfolioModel : IModelBase
    {
        IBenchMarkModel ModelBenchMark { get; set; }
        IModelDetail Details { get; set; }
        bool AllowExecOnlyCustomers { get; }
        ExecutionOnlyOptions ExecutionOptions { get; set; }
        ITradeableInstrument CashFundAlternative { get; set; }
        short MaxWithdrawalAmountPercentage { get; }
        IFeeRuleCollection FeeRules { get; }

        IModelPerformance ModelPerformances { get; }
    }
}

//IModelPerformanceCollection ModelPerformances { get; }
