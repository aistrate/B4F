using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Nav;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.VirtualFunds
{
    public static class NavCalculationDetailsAdapter
    {
        public class FundDetails
        {
            public FundDetails(IVirtualFund Fund)
            {
                this.Key = Fund.Key;
                this.FundName = Fund.Name;
                this.HoldingsAccountName = (Fund.HoldingsAccount != null) ? Fund.HoldingsAccount.ShortName : "";
                this.TradingAccountName = (Fund.TradingAccount != null) ? Fund.TradingAccount.ShortName : "";
                if (Fund.LastNavDate != new DateTime(1, 1, 1)) this.LastNavDate = Fund.LastNavDate;
                this.LastNavPerUnit = Fund.LastNavPerUnit;
                this.LastNavPerUnitDisplayString = Fund.LastNavPerUnit.Get(x => x.DisplayString);
            }

            public int Key { get; set; }
            public string FundName { get; set; }
            public string HoldingsAccountName { get; set; }
            public string TradingAccountName { get; set; }
            public DateTime? LastNavDate { get; set; }
            public Money LastNavPerUnit { get; set; }
            public string LastNavPerUnitDisplayString { get; set; }

        }

        public class CalculationDetails
        {
            public CalculationDetails() { }
            public CalculationDetails(INavCalculation Calc)
            {
                this.Key = Calc.Key;
                this.Fund = new FundDetails(Calc.Fund);
                this.ValuationDate = Calc.ValuationDate;
                this.DisplayStatus = Calc.DisplayStatus;
                this.ParticipationsBefore = Calc.TotalParticipationsBeforeFill;
                this.ParticipationsAfter = Calc.TotalParticipationsAfterFill;
                if (this.ParticipationsAfter > 0) this.ParticipationsDuring = decimal.Subtract(Calc.TotalParticipationsAfterFill, Calc.TotalParticipationsBeforeFill);
                this.GrossAssetValue = Calc.GrossAssetValue;
                this.GrossAssetValueDisplay = Calc.GrossAssetValue.DisplayString;
                this.NettAssetValue = Calc.NettAssetValue;
                this.NettAssetValueDisplay = Calc.NettAssetValue.DisplayString;
                this.NavPerUnit = Calc.NavPerUnit;
                this.NavPerUnitDisplay = Calc.NavPerUnitDisplayString;
                this.PublicOfferPrice = Calc.PublicOfferPrice;
                this.PublicOfferPriceDisplay = Calc.PublicOfferPriceDisplayString;
                this.JournalEntryKey = Calc.Bookings.Key;
            }
            public int Key { get; set; }
            public FundDetails Fund { get; set; }
            public DateTime ValuationDate { get; set; }
            public string DisplayStatus { get; set; }
            public decimal ParticipationsBefore { get; set; }
            public decimal ParticipationsDuring { get; set; }
            public decimal ParticipationsAfter { get; set; }
            public Money GrossAssetValue { get; set; }
            public string GrossAssetValueDisplay { get; set; }
            public Money NettAssetValue { get; set; }
            public string NettAssetValueDisplay { get; set; }
            public Money NavPerUnit { get; set; }
            public string NavPerUnitDisplay { get; set; }
            public Money PublicOfferPrice { get; set; }
            public string PublicOfferPriceDisplay { get; set; }
            public int JournalEntryKey { get; set; }

        }

        public class CalculationOrderDetails
        {
            public int Key { get; set; }
            public int OrderID { get; set; }
            public DateTime ApprovalDate { get; set; }
            public string OrderStatus { get; set; }

        }

        public static FundDetails GetVirtualFundDetails(int fundId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IVirtualFund fund = (IVirtualFund)InstrumentMapper.GetInstrument(session, fundId);
            FundDetails funddetails = new FundDetails(fund);

            session.Close();

            return funddetails;
        }

        public static FundDetails GetVirtualFundDetailsfromCalculation(int calculationId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            INavCalculation calc = NavCalculationMapper.GetNavCalculation(session, calculationId);
            IVirtualFund fund = calc.Fund;
            FundDetails funddetails = new FundDetails(fund);

            session.Close();

            return funddetails;
        }

        public static CalculationDetails GetCalculationDetails(int calculationId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            INavCalculation calc = NavCalculationMapper.GetNavCalculation(session, calculationId);
            CalculationDetails calcdetails = new CalculationDetails(calc);

            session.Close();

            return calcdetails;
        }

        public static void AddOrdersToNavCalculation(int calculationId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            INavCalculation calc = NavCalculationMapper.GetNavCalculation(session, calculationId);
            if (calc.Status != NavCalculationStati.Booked)
                AddOrdersToNavCalculation(session, calc);
            session.Close();
        }

        public static void AddOrdersToNavCalculation(IDalSession session, INavCalculation calc)
        {
            if (calc.Status != NavCalculationStati.Booked)
            {
                IList<IOrder> orders = NavCalculationOrderMapper.NewOrdersForFund(session, calc.Fund.Key, calc.ValuationDate);
                foreach (IOrder order in orders)
                {
                    NavCalculationOrder newOrder = new NavCalculationOrder(order, calc);
                    calc.NewOrders.Add(newOrder);
                }
            }
        }

        public static DataSet GetOrdersFromCalculation(int calcID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = null;
            IList<IOrder> orders = NavCalculationOrderMapper.OrdersForFund(session, calcID);

            if (orders != null)
                ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            orders.ToList(),
                            @"Key, OrderID, PlacedValue.DisplayString, PlacedValue, ApprovalDate, DisplayStatus");

            session.Close();

            return ds;
        }


        public static void BookNavCalculation(int calculationId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            INavCalculation calc = NavCalculationMapper.GetNavCalculation(session, calculationId);
            if (calc.Status != NavCalculationStati.Booked)
            {
                calc.Status = NavCalculationStati.Booked;
                session.Update(calc);
            }
            session.Close();
        }




    }
}
