using System;
using System.Collections;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Dal;
using NHibernate;
using System.Globalization;


namespace B4F.TotalGiro.Instruments
{ 
    public class ModelPerformance: IModelPerformance
    {
        public ModelPerformance() { }

        public ModelPerformance(IPortfolioModel model, decimal iboxxtarget, decimal msciworldtarget, decimal compositetarget, decimal benchmarkperformance, int quarter, int yyyy)
        {
            this.IBoxxTarget = iboxxtarget;
            this.MSCIWorldTarget = msciworldtarget;
            this.CompositeTarget = compositetarget;
            this.BenchMarkValue = benchmarkperformance;
            this.Quarter = quarter;
            this.PerformanceYear = yyyy;
            this.ModelPortfolio = model;
        }
   
        public virtual IPortfolioModel ModelPortfolio { get; set; }


        public virtual Int32 Key { get; set; }
        public virtual int Quarter { get; set; }
        public virtual int PerformanceYear { get; set; }
        public virtual decimal IBoxxTarget { get; set; }
        public virtual decimal MSCIWorldTarget { get; set; }
        public virtual decimal CompositeTarget { get; set; }
        public virtual decimal BenchMarkValue { get; set; }
        public virtual int EmployeeID { get; set; }
        

        private IPortfolioModel modelPortfolio;

        #region IModelPerformance Members

        public string DisplayBenchMarkValue
        {
            get { return this.BenchMarkValue.ToString("P5", CultureInfo.CreateSpecificCulture("nl-NL")); }
        }

        #endregion

    }
}