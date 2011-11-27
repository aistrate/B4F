using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.GeneralLedger.Journal;

namespace B4F.TotalGiro.Instruments.Nav
{
    public class NavCalculation : INavCalculation
    {
        public NavCalculation()
        {
            newOrders = new NavCalculationOrderCollection(this);
            portfolio = new NavPortfolio(this);

        }
        public NavCalculation(INavCalculation lastNavCalculation, IVirtualFund fund)
            : this()
        {
            Fund = fund;
            if (lastNavCalculation != null)
            {
                ValuationDate = getNextWorkingDay(lastNavCalculation.ValuationDate);
                if (ValuationDate > DateTime.Today)
                    throw new ApplicationException(
                        string.Format("Fund '{0}' is up-to-date: a NAV Calculation for the last working day already exists.", fund.Name));

                PrevNavCalculation = lastNavCalculation;
                TotalParticipationsBeforeFill = lastNavCalculation.TotalParticipationsAfterFill;
                TotalParticipationsAfterFill = lastNavCalculation.TotalParticipationsAfterFill;
                GrossAssetValue = lastNavCalculation.GrossAssetValue;
                NettAssetValue = lastNavCalculation.NettAssetValue;
                NavPerUnit = lastNavCalculation.NavPerUnit;
                Status = NavCalculationStati.New;
                PublicOfferPrice = lastNavCalculation.PublicOfferPrice;
            }
            
        }

        public int Key { get; set; }
        public DateTime ValuationDate { get; set; }
        public IVirtualFund Fund { get; set; }
        public decimal TotalParticipationsBeforeFill { get; set; }
        public decimal TotalParticipationsAfterFill { get; set; }

        public Money GrossAssetValue { get; set; }
        public Money NettAssetValue { get; set; }
        public Money NavPerUnit { get; set; }
        public INavCalculation PrevNavCalculation { get; set; }
        public NavCalculationStati Status { get; set; }

        public Money PublicOfferPrice { get; set; }
        public IMemorialBooking Bookings { get; set; }

        public string NavPerUnitDisplayString
        {
            get { return (NavPerUnit != null && NavPerUnit.Quantity != 0m ? NavPerUnit.ToString("{0:#,##0.00}") : ""); }
        }

        public string PublicOfferPriceDisplayString
        {
            get { return (PublicOfferPrice != null && PublicOfferPrice.Quantity != 0m ? PublicOfferPrice.ToString("{0:#,##0.00}") : ""); }
        }

        public INavCalculationOrderCollection NewOrders
        {
            get
            {
                INavCalculationOrderCollection orders = (INavCalculationOrderCollection)newOrders.AsList();
                return orders;
            }
        }

        public virtual INavPortfolio Portfolio
        {
            get
            {
                INavPortfolio port = (INavPortfolio)portfolio.AsList();
                return port;
            }

        }

        public string DisplayStatus
        {
            get { return Status.ToString(); }
        } 
        private DateTime getNextWorkingDay(DateTime date)
        {
            int daysToAdd = 1;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    daysToAdd = 3;
                    break;
                case DayOfWeek.Saturday:
                    daysToAdd = 2;
                    break;
                default:
                    daysToAdd = 1;
                    break;
            }

            return date.AddDays(daysToAdd);
        }

        private IDomainCollection<INavPosition> portfolio;
        private IDomainCollection<INavCalculationOrder> newOrders;


    }
}
