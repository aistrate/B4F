using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Collection.Generic;

namespace B4F.TotalGiro.Instruments
{
    public class DerivativeMaster : IDerivativeMaster
    {
        protected DerivativeMaster() 
        {
            this.UnderlyingSecCategory = SecCategories.Undefined;
        }

        public DerivativeMaster(string name, SecCategories secCategory, IExchange exchange)
            :this()
        {
            this.Name = name;
            this.SecCategory = secCategory;
            this.Exchange = exchange;
        }
        
        public virtual int Key { get; set; }
        public virtual IExchange Exchange { get; set; }
        public virtual string Name { get; set; }
        public virtual SecCategories SecCategory { get; set; }
        public virtual ITradeableInstrument Underlying { get; set; }
        public virtual SecCategories UnderlyingSecCategory { get; set; }
        public virtual short DecimalPlaces { get; set; }
        public virtual ICurrency CurrencyNominal { get; set; }
        public virtual string DerivativeSymbol { get; set; }

        /// <summary>
        /// contract size of the underlying instrument
        /// </summary>
        public virtual int ContractSize { get; set; }

        /// <summary>
        /// The series that belong to this master
        /// </summary>
        public virtual List<IDerivative> Series
        {
            get
            {
                if (!series.WasInitialized)
                    series.ForceInitialization();
                return series.ToList<IDerivative>();
            }
        }

        /// <summary>
        /// Get/set creation date
        /// </summary>
        public virtual DateTime CreationDate
        {
            get { return creationDate; }
        }

        /// <summary>
        /// Get/set date last updated
        /// </summary>
        public virtual DateTime LastUpdated
        {
            get { return lastUpdated; }
        }

        #region Methods

        protected bool validate()
        {
            if (this.ContractSize <= 0)
                throw new ApplicationException("The Contract Size has to be higher than 0.");
            return true;
        }

        #endregion

        #region Private Variables

        private PersistentGenericBag<IDerivative> series;
        private DateTime creationDate;
        private DateTime lastUpdated;

        #endregion

    }
}
