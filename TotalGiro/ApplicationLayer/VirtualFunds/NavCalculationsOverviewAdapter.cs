using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments.Nav;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.VirtualFunds
{
    public static class NavCalculationsOverviewAdapter
    {
        public static DataSet GetNavCalculations(
            int fundId, DateTime navDateFrom, DateTime navDateTo)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IVirtualFund fund = (IVirtualFund)InstrumentMapper.GetInstrument(session, fundId);

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            NavCalculationMapper.GetNavCalculations(session, fund, navDateFrom, navDateTo),
                            @"Key, ValuationDate, TotalParticipationsBeforeFill, TotalParticipationsAfterFill, NavPerUnitDisplayString, PublicOfferPriceDisplayString, DisplayStatus, Status");

            session.Close();

            return ds;
        }



        public static DateTime GetLastValuationDate(int fundID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IVirtualFund fund = (IVirtualFund)InstrumentMapper.GetInstrument(session, fundID);
                INavCalculation lastNavCalculation = (INavCalculation)NavCalculationMapper.GetLastNavCalculation(session, fund);

                return (lastNavCalculation != null ? lastNavCalculation.ValuationDate : DateTime.MinValue);
            }
            finally
            {
                session.Close();
            }
        }
    }


}
