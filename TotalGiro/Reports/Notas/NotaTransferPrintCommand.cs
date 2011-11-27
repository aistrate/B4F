using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Notas
{
    public class NotaTransferPrintCommand : NotaPrintCommand
    {
        public NotaTransferPrintCommand()
            : base()
        {
            NotaType = NotaReturnClass.NotaTransfer;
            ReportGrouping = new NotaGroupingAllInOneReport();
            NotasPerPage = 7;
        }

        public override object GetNotaFields(INota nota)
        {
            INotaTransfer n = (INotaTransfer)nota;

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
                Instrument_DisplayName = 
                    n.Instrument.DisplayName,
                Instrument_Isin =
                    n.Instrument.Isin,
                ValueSize_Quantity =
                    n.ValueSize.Quantity,
                Price_Quantity =
                    n.Price.Quantity,
                Price_Underlying_Symbol =
                    n.Price.Underlying.Symbol,
                Price_ShortDisplayString =
                    n.Price.ShortDisplayString
            };
        }

        protected override string GetFileSuffix(INota[] notaGroup)
        {
            return "Transfer";
        }
    }
}
