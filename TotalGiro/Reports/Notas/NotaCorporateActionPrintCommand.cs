using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Notas
{
    public class NotaCorporateActionPrintCommand : NotaPrintCommand
    {
        public NotaCorporateActionPrintCommand()
            : base()
        {
            NotaType = NotaReturnClass.NotaInstrumentConversion;
            ReportGrouping = new NotaGroupingAllInOneReport();
            NotasPerPage = 5;
        }

        public override string NotaDataTableName { get { return "NotaCorporateAction"; } }
        public override string ReportName { get { return "NotaCorporateAction"; } }

        public override object GetNotaFields(INota nota)
        {
            INotaInstrumentConversion n = (INotaInstrumentConversion)nota;

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
                n.Description,
                ConvertedInstrumentSize_Underlying_DisplayName =
                    n.ConvertedInstrumentSize.Underlying.DisplayName,
                ConvertedInstrumentSize_Quantity =
                    n.ConvertedInstrumentSize.Quantity,
                ValueSize_Underlying_DisplayName =
                    n.ValueSize.Underlying.DisplayName,
                ValueSize_Quantity =
                    n.ValueSize.Quantity,
                InstrumentTransformation_Description =
                    n.InstrumentTransformation != null ? n.InstrumentTransformation.Description : ""
            };
        }

        protected override string GetFileSuffix(INota[] notaGroup)
        {
            return "CorporateAction";
        }
    }
}
