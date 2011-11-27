using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.RemisierHistory;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees.CommCalculations;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.ManagementPeriodUnits;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Reports.Financial;
using B4F.TotalGiro.TaxRates;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Valuations.AverageHoldings;

namespace B4F.TotalGiro.Fees
{
	/// <summary>
    /// Class used by Order classes to calculate their attached fees.
	/// </summary>
    public class FeeFactory : IFeeFactory
    {
        #region Initialization

        protected FeeFactory() { }

        /// <summary>
        /// Static method that gets an instance of this class. Used whenever an order is created 
        /// (orders need <b>FeeFactory</b> objects passed into their constructors to be able to calculate their fees).
        /// </summary>
        /// <param name="session">A <b>DAL</b> session.</param>
        /// <returns>A FeeFactory instance.</returns>
        public static FeeFactory GetInstance(IDalSession session)
        {
            return GetInstance(session, FeeFactoryInstanceTypes.Commission, false);
        }

        /// <summary>
        /// Static method that gets an instance of this class. Used whenever an order is created 
        /// (orders need <b>FeeFactory</b> objects passed into their constructors to be able to calculate their fees).
        /// </summary>
        /// <param name="session">A <b>DAL</b> session.</param>
        /// <param name="instanceType">What kind of instance</param>
        /// <returns>A FeeFactory instance.</returns>
        public static FeeFactory GetInstance(IDalSession session, FeeFactoryInstanceTypes instanceType)
        {
            return GetInstance(session, instanceType, false);
        }

        /// <summary>
        /// Static method that gets an instance of this class. Used whenever an order is created 
        /// (orders need <b>FeeFactory</b> objects passed into their constructors to be able to calculate their fees).
        /// </summary>
        /// <param name="session">A <b>DAL</b> session.</param>
        /// <param name="instanceType">What kind of instance</param>
        /// <param name="keepSession">Holds the session object in this instance</param>
        /// <returns>A FeeFactory instance.</returns>
        public static FeeFactory GetInstance(IDalSession session, FeeFactoryInstanceTypes instanceType, bool keepSession)
		{
            bool success = false;
            bool useTaxData = false;
            switch (instanceType)
            {
                case FeeFactoryInstanceTypes.Commission:
                    success = feeFactory.getCommRuleFinder(session) != null;
                    break;
                case FeeFactoryInstanceTypes.Fee:
                    success = feeFactory.getFeeRuleFinder(session) != null;
                    useTaxData = true;
                    break;
                case FeeFactoryInstanceTypes.All:
                    if (feeFactory.getCommRuleFinder(session) != null)
                        success = feeFactory.getFeeRuleFinder(session) != null;
                    useTaxData = true;
                    break;
            }
            if (success && useTaxData && feeFactory.taxRates == null)
                feeFactory.taxRates = HistoricalTaxRateMapper.GetHistoricalTaxRates(session);

            if (success)
            {
                if (keepSession)
                    FeeFactory.session = session;
                return feeFactory;
            }
            else
                return null;
		}

        public bool IsInstanceTypeActivated(FeeFactoryInstanceTypes instanceType)
        {
            bool activated = false;
            switch (instanceType)
            {
                case FeeFactoryInstanceTypes.Commission:
                    activated = commRuleFinder != null;
                    break;
                case FeeFactoryInstanceTypes.Fee:
                    activated = (feeRuleFinder != null && taxRates != null);
                    break;
                case FeeFactoryInstanceTypes.All:
                    activated = (commRuleFinder != null && feeRuleFinder != null && taxRates != null);
                    break;
            }
            return activated;
        }

        public void InitiateInstance(IDalSession session, FeeFactoryInstanceTypes instanceType)
        {
            InitiateInstance(session, instanceType, false);
        }

        public void InitiateInstance(IDalSession session, FeeFactoryInstanceTypes instanceType, bool keepSession)
        {
            bool getCom = false;
            bool getFee = false;
            switch (instanceType)
            {
                case FeeFactoryInstanceTypes.Commission:
                    getCom = true;
                    break;
                case FeeFactoryInstanceTypes.Fee:
                    getFee = true;
                    break;
                case FeeFactoryInstanceTypes.All:
                    getCom = true;
                    getFee = true;
                    break;
            }

            if (getCom) getCommRuleFinder(session);
            if (getFee)
            {
                getFeeRuleFinder(session);
                taxRates = HistoricalTaxRateMapper.GetHistoricalTaxRates(session);
            }
            if (keepSession)
                FeeFactory.session = session;
        }

        /// <summary>
        /// Set the session variable to null
        /// </summary>
        public void CloseSession()
        {
            if (FeeFactory.session != null)
                FeeFactory.session = null;
        }

        #endregion

        #region Commission

        /// <summary>
        /// The method used by <b>Order</b> classes to calculate their attached fees.
        /// </summary>
        /// <param name="order">The order for which the fee is calculated.</param>
        /// <returns>The value of the fee.</returns>
        public Commission CalculateCommission(IOrder order)
        {
            CommClient client = new CommClient(order);
            if (!order.IsAggregateOrder)
            {
                if (order.IsAmountBased && ((IOrderAmountBased)order).IsValueInclComm &&
                    order.RequestedInstrument.IsTradeable && !((ITradeableInstrument)order.RequestedInstrument).IsCommissionLinear)
                {
                    ITradeableInstrument instrument = (ITradeableInstrument)order.RequestedInstrument;
                    IExchange exchange = instrument.DefaultExchange ?? instrument.HomeExchange;

                    TransactionFillDetails details = instrument.GetTransactionFillDetails(order,
                        instrument.CurrentPrice.Price,
                        instrument.GetSettlementDate(order.CreationDate.Date, exchange),
                        this, order.ExRate, exchange);
                    return new Commission(details.Commission, "Commission determined by goalseek -> " + details.ToString());
                }
                else
                {
                    return CalculateCommission(client);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// The method used by <b>Transaction</b> classes to calculate their attached fees.
        /// </summary>
        /// <param name="transaction">The transaction for which the fee is calculated.</param>
        /// <returns>The value of the fee.</returns>
        public Commission CalculateCommission(ITransactionOrder transaction)
        {
            if (transaction == null)
                throw new ApplicationException("It is not possible to calculate the commission when the transaction is null.");

            IOrder order = transaction.Order;
            if (transaction.TransactionType == TransactionTypes.Execution ||
                (transaction.AccountA.AccountType != AccountTypes.Customer && transaction.AccountB.AccountType != AccountTypes.Customer))
                return null;

            Money total = (transaction.CounterValueSize * -1);
            ICurrency commCur = transaction.TradedInstrument.CurrencyNominal;

            // Commission on the order is 0
            if (order.DoNotChargeCommission || order.Commission == null || (order.Commission != null && order.Commission.IsZero))
                return null;
            //else if (order.Commission != null && order.Commission.IsZero)
            //    return new CommValueDetails(new Money(0m, commCur), "");

            // AmountBased Order -> use commission from the order
            if (order.IsAmountBased)
            {
                Money orderAmount = order.Amount;
                Money diff;
                Money serviceCharge = transaction.ServiceCharge;

                // convert to transaction currency
                if (!orderAmount.Underlying.Equals(total.Underlying))
                    orderAmount = orderAmount.Convert(order.ExRate, (ICurrency)total.Underlying);
                // deduct serviceCharge
                orderAmount = MoneyMath.AdjustAmountForServiceCharge(orderAmount, serviceCharge, order.Side, MathOperator.Subtract);

                diff = orderAmount - total.GetMoney();

                // if the trade fills the Order completely -> take the Commission from the Order
                if (orderAmount.Equals(total) || diff.IsWithinTolerance(0.01M))
                {
                    //if (convert)
                    //{
                    //    Commission convCommDetails = new Commission(order.CommissionDetails, (ICurrency)total.Underlying, order.ExRate);
                    //    Money commConv = convCommDetails.Amount;
                        // No more 2 cent differences
                        //if (order.MoneyOrder != null && session != null)
                        //{
                        //    IList transactions = getMonetaryTransactions(order.MoneyOrder);
                        //    if (transactions != null && transactions.Count == 1)
                        //    {
                        //        Money tradeAmount = order.MoneyOrder.Transactions[0].ValueSize.GetMoney();
                        //        if (tradeAmount.Underlying.Equals(commConv.Underlying))
                        //        {
                        //            Money diffTx = tradeAmount + (transaction.CounterValueSize + commConv + transaction.ServiceCharge);
                        //            if (diffTx.IsNotZero && diffTx.Abs().IsWithinTolerance(0.02M))
                        //                convCommDetails.BreakupLines.GetItemByType(CommissionBreakupTypes.Commission).Amount -= diffTx;
                        //        }
                        //    }
                        //}
                    //    return convCommDetails;
                    //}
                    //else
                        return new Commission(order.CommissionDetails);
                }
            }

            if (order.Transactions != null && order.Transactions.Count > 0)
                total += (order.Transactions.TotalCounterValueSize * -1);

            CommClient client = new CommClient(transaction, total);
            Commission commdetails = CalculateCommission(client);
            Money commission = commdetails.Amount;

            //// convert the commission to instrument currency
            //if (!total.Underlying.Equals(commission.Underlying))
            //    commission = commission.Convert(transaction.Order.ExRate, commCur);

            if (order.Transactions != null && order.Transactions.Count > 0)
                commission -= order.Transactions.TotalCommission;

            return new Commission(CommissionBreakupTypes.Commission, commission, commdetails.CommissionInfo);
        }

        // Get the transactions from the MoneyOrder
        private IList getMonetaryTransactions(IMonetaryOrder order)
        {
            string hql = string.Format("from TransactionOrder T where T.Order.Key = {0}", order.Key.ToString());
            IList transactions = session.GetListByHQL(hql, null);
            return transactions;
        }

        /// <summary>
        /// The method used by <b>Order</b> and <b>Transaction</b> classes to calculate their attached fees. Also, if applicable,
        /// the service charge is calculated
        /// </summary>
        /// <param name="client">The client (order/transaction) for which the fee is calculated.</param>
        /// <returns>The commission value .</returns>
        public Commission CalculateCommission(ICommClient client)
        {
            Commission cvd = new Commission();

            Money fee = null;
            if (feeFactory.commRuleFinder == null)
            {
                throw new ApplicationException("There are no Commission Rules initialised");
            }

            if (client.Account.AccountType != AccountTypes.Customer)
                return null;

            ICommRule commRule = feeFactory.commRuleFinder.FindRule(client);
            ICommCalc commCalculation = null;

            if (commRule != null)
            {
                // Step 01 : Introduction Fee
                if (commRule.AdditionalCalculation != null)
                {
                    ICommCalc feeCalc2 = commRule.AdditionalCalculation;
                    client.CommissionInfo = "Additional Fee: " + feeCalc2.Name + ". ";
                    Money fee2 = feeCalc2.Calculate(client);

                    if (fee2 != null)
                    {
                        if (fee2.Sign)
                            fee2 = fee2.Abs() * -1;

                        if (!fee2.Underlying.Equals(client.OrderCurrency))
                            fee2 = fee2.Convert(client.OrderCurrency);
                    }
                    else
                        fee2 = new Money(0, client.OrderCurrency);

                    client.PreviousCalculatedFee = fee2;

                    cvd.BreakupLines.Add(new CommissionBreakupLine(fee2, CommissionBreakupTypes.AdditionalCommission, client.CommissionInfo));
                    client.CommissionInfo = "";
                }

                // Step 02 : Commission
                commCalculation = commRule.CommCalculation;

                fee = commCalculation.Calculate(client);
                client.CommissionInfo = "Commission rule: " + commRule.ToString() + ". " + client.CommissionInfo;
            }
            else
                client.CommissionInfo = "No commission rule found.";

            if (fee != null)
            {
                if (fee.Sign)
                    fee = fee.Abs() * -1;

                if (!fee.Underlying.Equals(client.OrderCurrency))
                    fee = fee.Convert(client.OrderCurrency);
            }
            else
                fee = new Money(0, client.OrderCurrency);

            cvd.BreakupLines.Add(new CommissionBreakupLine(fee, CommissionBreakupTypes.Commission, client.CommissionInfo));
            return cvd;
        }

        public ICommRule GetRelevantCommRule(ICommClient client)
        {
            return feeFactory.commRuleFinder.FindRule(client);
        }

        public ICommRule GetRelevantCommRule(IAccountTypeInternal account, IInstrument instrument, 
            Side side, OrderActionTypes actiontype, DateTime transactionDate, bool isAmountBased,
            out ICommClient client)
        {
            client = new CommClient(account, instrument, side, actiontype, transactionDate, !isAmountBased,
                null, null, null, null, false);
            return GetRelevantCommRule(client);
        }

		internal CommRuleFinder getCommRuleFinder(IDalSession session)
		{
            IList<ICommRule> rules = CommRuleMapper.GetCommissionRules(session);
            commRuleFinder = new CommRuleFinder(rules);

            return commRuleFinder;
		}

		#endregion

        #region Fees

        /// <summary>
        /// The method used to calculate the fees
        /// </summary>
        /// <param name="session">The dal session.</param>
        /// <param name="unit">The unit for which the fee is calculated.</param>
        /// <returns>The MgtFee value.</returns>
        public bool CalculateFeePerUnit(IDalSession session, IManagementPeriodUnit unit)
        {
            bool result = false;

            if (feeRuleFinder == null)
                throw new ApplicationException("No fee rules instantiated.");

            if (unit.IsRelevantForFees)
            {
                if (unit.TotalValue != null && unit.TotalValue.IsNotZero)
                {
                    if (unit.ManagementFee != null && (unit.ManagementFee.IsStorno || unit.ManagementFee.StornoBooking != null))
                        unit.IsStornoed = true;

                    if (!(unit.ManagementFee == null || (unit.ManagementFee != null && (unit.ManagementFee.IsStorno || unit.ManagementFee.StornoBooking != null))))
                        throw new ApplicationException(string.Format("The unit {0} has already been calculated.", unit.Key.ToString()));

                    if (unit.AverageHoldings != null && unit.AverageHoldings.Count > 0)
                    {
                        // Get the CountDocumentsByPost for the unit
                        long count = session.Session.GetNamedQuery(
                            "B4F.TotalGiro.Reports.Documents.CountDocumentsSentByPost")
                            .SetInt32("accountId", unit.Account.Key)
                            .SetInt32("reportStatusId", (int)ReportStatuses.PrintSuccess)
                            .SetDateTime("startDate", unit.StartDate)
                            .SetDateTime("endDate", unit.EndDate.AddDays(1))
                            .UniqueResult<long>();
                        unit.DocumentsSentByPost = Convert.ToInt32(count);

                        // Get relevant Calculation Versions using Rules for the unit
                        IFeeCalcVersion[] calculations = feeRuleFinder.FindCalculations(unit);

                        if (calculations != null && calculations.Length > 0)
                        {
                            unit.RulesFound = calculations.Length;
                            // Loop through Calculations
                            foreach (FeeCalcVersion calc in calculations)
                                calc.Calculate(unit);
                            unit.FeesCalculated = FeesCalculatedStates.Yes;
                            result = true;
                        }
                        else
                        {
                            unit.RulesFound = 0;
                            unit.FeesCalculated = FeesCalculatedStates.Irrelevant;
                            unit.Message = "No fee rules matched";
                            result = true;
                        }
                    }
                }
                else
                {
                    unit.FeesCalculated = FeesCalculatedStates.Irrelevant;
                    unit.Message = "Total Value is 0 so fee will be 0 as well";
                    result = true;
                }
            }
            else
            {
                unit.FeesCalculated = FeesCalculatedStates.Irrelevant;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Calculate kickback on a unit
        /// </summary>
        /// <param name="session">The dal session.</param>
        /// <param name="unit">The unit for which the fee is calculated.</param>
        /// <param name="feeTypeKickBack">FeeType of type KickBack.</param>
        /// <returns></returns>
        public static bool CalculateKickBackOnUnit(IDalSession session, IManagementPeriodUnit unit)
        {
            string message;
            return CalculateKickBackOnUnit(session, unit, null, out message);
        }

        /// <summary>
        /// Calculate kickback on a unit
        /// </summary>
        /// <param name="session">The dal session.</param>
        /// <param name="unit">The unit for which the fee is calculated.</param>
        /// <param name="feeTypeKickBack">FeeType of type KickBack.</param>
        /// <param name="message">returns stuff when fu</param>
        /// <returns></returns>
        public static bool CalculateKickBackOnUnit(IDalSession session, IManagementPeriodUnit unit, FeeType feeTypeKickBack, out string message)
        {
            bool success = false;
            message = "";

            if (unit.Account != null && unit.Account.AccountType == AccountTypes.Customer)
            {
                ICustomerAccount account = (ICustomerAccount)unit.Account;
                if (account.UseKickback)
                {
                    IRemisierHistory remisierHistory = account.CurrentRemisierDetails;
                    if (remisierHistory != null && remisierHistory.KickBack > 0)
                    {
                        if (feeTypeKickBack == null)
                            feeTypeKickBack = (FeeType)session.GetObjectInstance(typeof(FeeType), (int)FeeTypes.KickbackFee);

                        if (unit.AverageHoldings != null && unit.AverageHoldings.Count > 0)
                        {
                            Money totalKickBackFee = null;
                            foreach (IAverageHolding holding in unit.AverageHoldings)
                            {
                                Money avgAmount = holding.AverageValue;
                                if (avgAmount != null && avgAmount.IsNotZero)
                                {
                                    Money kickbackFee = avgAmount * ((remisierHistory.KickBack / 100M) * (unit.Days / 365M) * feeTypeKickBack.FeeTypeSign);

                                    if (kickbackFee != null && kickbackFee.IsNotZero)
                                    {
                                        totalKickBackFee += kickbackFee;
                                        holding.FeeItems.AddFeeItem(feeTypeKickBack, kickbackFee, unit, null, remisierHistory.KickBack);
                                        unit.FeesCalculated = FeesCalculatedStates.Yes;
                                        unit.Message = "";
                                    }
                                }
                            }
                            if (unit.FeesCalculated != FeesCalculatedStates.Yes && (totalKickBackFee == null || totalKickBackFee.IsZero))
                            {
                                unit.FeesCalculated = FeesCalculatedStates.Irrelevant;
                                unit.Message = "Calculated kickback amount is either zero or too small";
                            }
                            success = true;
                        }
                    }
                    else
                        message = "No kickback percentage found";
                }
                else
                {
                    unit.FeesCalculated = FeesCalculatedStates.Irrelevant;
                    success = true;
                }
            }
            return success;
        }


        ///// <summary>
        ///// The method used to calculate the fees
        ///// </summary>
        ///// <param name="managementPeriod">The management Period for which the fee is calculated.</param>
        ///// <param name="startDatePeriod">Starting Date of the Period.</param>
        ///// <param name="endDatePeriod">End Date of the Period.</param>
        ///// <param name="units">The units involved for this management fee.</param>
        ///// <returns>The MgtFee value.</returns>
        //public MgtFee CalculateMgtFee(IManagementPeriod managementPeriod, DateTime startDatePeriod, DateTime endDatePeriod, IList<IManagementPeriodUnit> units, out decimal taxPercentage)
        //{
        //    //int startPeriod;
        //    //int endPeriod;
        //    taxPercentage = 0M;
        //    MgtFee result = new MgtFee(startDatePeriod, endDatePeriod);

        //    if (startDatePeriod.Year != endDatePeriod.Year)
        //        throw new ApplicationException("The year of the start date and end date for the management fee should equal");

        //    if (units != null && units.Count > 0)
        //    {
        //        // check the number of units
        //        int expectedUnitCount = Util.DateDiff(DateInterval.Month, startDatePeriod, endDatePeriod) + 1;
        //        if (expectedUnitCount != units.Count)
        //            throw new ApplicationException(string.Format("The number of units {0} does not equal the number of expected units {1}.", units.Count, expectedUnitCount));

        //        // check if all have the same management period
        //        var mps = from a in units
        //                 group a by a.ManagementPeriod into g
        //                 select g.Key;
        //        if (mps.Count() != 1)
        //            throw new ApplicationException("Not all units belong to the same management period.");

        //        if (mps.First().Key != managementPeriod.Key)
        //            throw new ApplicationException("The management period is not ok.");

        //        if (Util.GetPeriodFromDate(managementPeriod.StartDate) == Util.GetPeriodFromDate(startDatePeriod) && managementPeriod.StartDate.Day != startDatePeriod.Day)
        //            throw new ApplicationException(string.Format("The start date of the management period ({0}) does not equal the presented start date ({1}).", managementPeriod.StartDate.ToString("yyyy-MM-dd"), startDatePeriod.ToString("yyyy-MM-dd")));

        //        if (managementPeriod.EndDate.HasValue)
        //        {
        //            if (endDatePeriod > managementPeriod.EndDate)
        //                throw new ApplicationException(string.Format("The end date of the management period ({0}) does not equal the presented end date ({1}).", managementPeriod.EndDate.Value.ToString("yyyy-MM-dd"), endDatePeriod.ToString("yyyy-MM-dd")));
        //            else if (Util.GetPeriodFromDate(managementPeriod.EndDate.Value) == Util.GetPeriodFromDate(endDatePeriod) && managementPeriod.EndDate.Value.Day != endDatePeriod.Day)
        //                throw new ApplicationException(string.Format("The end date of the management period ({0}) does not equal the presented end date ({1}).", managementPeriod.EndDate.Value.ToString("yyyy-MM-dd"), endDatePeriod.ToString("yyyy-MM-dd")));
        //        }

        //        foreach (IManagementPeriodUnit unit in units)
        //        {
        //            if (unit.Transaction != null)
        //                throw new ApplicationException(string.Format("The unit {0} is already used for a management fee transaction.", unit.Key.ToString()));

        //            if (!(unit.Success && unit.FeesCalculated == FeesCalculatedStates.Yes))
        //                throw new ApplicationException(string.Format("The unit {0} is not correct for the management fee transaction.", unit.Key.ToString()));

        //            result.AddFee(unit.AverageHoldingFeeItems);
        //            result.AddFee(unit.FeeItems);
        //        }

        //        // set tax
        //        taxPercentage = getHistoricalTaxRate(endDatePeriod).StandardRate;
        //        foreach (MgtFeeBreakupLine line in result.BreakupLines)
        //        {
        //            if (line.MgtFeeType.UseTax)
        //                line.SetBTW(taxPercentage);
        //        }
        //        return result;
        //    }
        //    return null;
        //}

        private bool getPeriodsInQuarter(ICustomerAccount account, DateTime startDatePeriod, DateTime endDatePeriod, out int startPeriod, out int endPeriod)
        {
            startPeriod = 0;
            endPeriod = 0;
            bool retVal = false;

            if (account.ManagementStartDate < endDatePeriod && (Util.IsNullDate(account.ManagementEndDate) || !(account.ManagementEndDate < startDatePeriod)))
            {
                if (account.ManagementStartDate <= startDatePeriod)
                    startPeriod = Util.GetPeriodFromDate(startDatePeriod);
                else
                    startPeriod = Util.GetPeriodFromDate(account.ManagementStartDate);

                if (Util.IsNullDate(account.ManagementEndDate) || account.ManagementEndDate >= endDatePeriod)
                    endPeriod = Util.GetPeriodFromDate(endDatePeriod);
                else
                    endPeriod = Util.GetPeriodFromDate(account.ManagementEndDate);

                retVal = true;
            }
            else
                throw new ApplicationException(string.Format("The selected period falls out of the management scope of account {0}", account.DisplayNumberWithName));

            return retVal;
        }

        internal FeeRuleFinder getFeeRuleFinder(IDalSession session)
        {
            List<FeeRule> rules = session.GetTypedList<FeeRule>();
            feeRuleFinder = new FeeRuleFinder(rules);
            
            return feeRuleFinder;
        }

        public IHistoricalTaxRate GetHistoricalTaxRate(DateTime date)
        {
            var result = from a in taxRates
                        where a.StartDate <= date && (Util.IsNullDate(a.EndDate) || a.EndDate >= date)
                        select a;
            if (result.Count() == 1)
                return result.First();
            else
                return null;
        }

        #endregion

        #region Privates

        private static FeeFactory feeFactory = new FeeFactory();
        internal CommRuleFinder commRuleFinder = null;
        internal FeeRuleFinder feeRuleFinder = null;
        internal IList<IHistoricalTaxRate> taxRates = null;
        private static IDalSession session;
	
		#endregion

	}
}
