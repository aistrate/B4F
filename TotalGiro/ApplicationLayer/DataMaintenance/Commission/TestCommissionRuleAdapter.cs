using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.CommRules;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission
{
    public static class TestCommissionRuleAdapter
    {
        /// <summary>
        /// This method tries to find a commission rule based on the parameters entered
        /// </summary>
        /// <param name="saccount">Account number</param>
        /// <param name="sinstrument">Instrument</param>
        /// <param name="sordervalue">Order value</param>
        /// <param name="samount">Amount</param>
        /// <param name="sprice">Price</param>
        public static string DoTest(int accountId, int instrumentId, 
            DateTime commDate, OrderActionTypes orderactiontype,
            decimal orderValueQuantity, decimal amountQuantity, decimal priceQuantity, 
            bool isValueInclComm, BaseOrderTypes originalOrderType, Side side)
        {
            string result = "";

            try
            {

                IDalSession session = NHSessionFactory.CreateSession();

                IAccountTypeInternal account = (accountId != 0 ? (IAccountTypeInternal)AccountMapper.GetAccount(session, accountId) : null);
                IInstrument instrument = (instrumentId != 0 ? InstrumentMapper.GetInstrument(session, instrumentId) : null);

                ICurrency defcurrency = account.BaseCurrency;
                InstrumentSize size = new InstrumentSize(orderValueQuantity, instrument);
                Money amount = new Money(amountQuantity, defcurrency);
                Price price = null;
                if (priceQuantity != 0M)
                    price = new Price(priceQuantity, defcurrency, instrument);
                else if (instrument.CurrentPrice != null)
                    price = instrument.CurrentPrice.Price;
                if (originalOrderType == BaseOrderTypes.SizeBased)
                    amount = size.CalculateAmount(price);

                CommClient feeclient = new CommClient(account, instrument, side, orderactiontype,
                                                    commDate, originalOrderType == BaseOrderTypes.SizeBased, 
                                                    size, amount, price, defcurrency, isValueInclComm);
                feeclient.OriginalOrderType = originalOrderType;

                IFeeFactory feefactory = FeeFactory.GetInstance(session);
                Fees.Commission commDetails = feefactory.CalculateCommission(feeclient);

                if (commDetails != null)
                {
                    result = string.Format("Resulting commission calculated: {0}<br/>Breakup:<br/>", commDetails.Amount.ToString());
                    foreach (CommissionBreakupLine cvbl in commDetails.BreakupLines)
                    {
                        result += cvbl.CommissionType.ToString() + ": " + cvbl.Amount + "<br/>";
                    }
                    result += "<br/><br/>" + commDetails.CommissionInfo;
                }
                else
                    result = "No commission calculated";

                session.Close();
            }
            catch (Exception ex)
            {
                result = "Error during commission rule test: " + ex.Message;
            }
            return result;
        }

        public static DataSet GetBuySellOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(CommRuleBuySell), SortingDirection.Descending);
            Utility.RemoveRow(ds.Tables[0], (int)CommRuleBuySell.Both);
            return ds;
        }

        public static DataSet GetBaseOrderTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(BaseOrderTypes));
            Utility.RemoveRow(ds.Tables[0], (int)BaseOrderTypes.Both);
            return ds;
        }

        public static DataSet GetOrderActionTypeOptions()
        {
            DataSet ds = Util.GetDataSetFromEnum(
                typeof(OrderActionTypes), SortingDirection.Ascending);
            Utility.AddEmptyFirstRow(ds.Tables[0], "Key", 0);
            return ds;
        }
    }
}
