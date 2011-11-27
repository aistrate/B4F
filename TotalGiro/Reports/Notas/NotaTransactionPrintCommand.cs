using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Reports.Notas
{
    public class NotaTransactionPrintCommand : NotaPrintCommand
    {
        public NotaTransactionPrintCommand()
            : base()
        {
            NotaType = NotaReturnClass.NotaTransaction;
            ReportGrouping = new NotaGroupingAllInOneReport();
            NotasPerPage = 3;
        }

        public override object GetNotaFields(INota nota)
        {
            INotaTransaction n = (INotaTransaction)nota;
            
            return new
            {
                n.Key,
                n.NotaNumber,
                n.TransactionDate,
                Account_Number =
                    n.Account.Number,
                Account_AccountOwner_CompanyName =
                    n.Account.AccountOwner.CompanyName,
                n.ModelPortfolioName,
                n.IsStorno,

                n.TxSide,
                StornoedTransactionNota_NotaNumber =
                    n.StornoedTransactionNota != null ? n.StornoedTransactionNota.NotaNumber : "",
                Order_OrderID = 
                    n.Order.OrderID,
                Order_OrderInfo =
                    n.Order.OrderInfo,
                n.ExchangeName,
                TradedInstrument_DisplayName =
                    n.TradedInstrument.DisplayName,
                TradedInstrument_Isin =
                    n.TradedInstrument.Isin,
                ValueSize_Quantity =
                    n.ValueSize.Quantity,
                Price_Quantity =
                    n.Price.Quantity,
                Price_ShortDisplayString =
                    n.Price.ShortDisplayString,
                n.ExchangeRate,
                CounterValue_Quantity =
                    n.CounterValue.Quantity,
                CounterValue_UnderlyingShortName =
                    n.CounterValue.UnderlyingShortName,
                TotalValue_Quantity =
                    n.TotalValue.Quantity,
                TotalValue_UnderlyingShortName =
                    n.TotalValue.UnderlyingShortName,
                n.ServiceChargePercentage,
                ServiceCharge_Quantity =
                    n.ServiceCharge.Quantity,
                ServiceCharge_UnderlyingShortName =
                    n.ServiceCharge.UnderlyingShortName,
                Commission_Quantity =
                    n.Commission.Quantity,
                Commission_UnderlyingShortName =
                    n.Commission.UnderlyingShortName,
                GrandTotalValue_Quantity =
                    n.GrandTotalValue.Quantity,
                GrandTotalValue_UnderlyingShortName =
                    n.GrandTotalValue.UnderlyingShortName,
                n.TransactionDateTime
            };
        }

        protected override string GetFileSuffix(INota[] notaGroup)
        {
            return "Transaction";
        }

        // Model of how to add new parameters to the report:

        //protected override void GenerateParamNames(StringList paramNames)
        //{
        //    base.GenerateParamNames(paramNames);
        //    paramNames.Add("...");
        //}

        //protected override void GenerateParamValues(StringList paramValues, bool showLogo, IAccountTypeInternal account)
        //{
        //    base.GenerateParamValues(paramValues, showLogo, account);
        //    paramValues.Add(...);
        //}
    }
}
