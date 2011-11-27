using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Notas;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Notas
{
    public class NotaDividendPrintCommand : NotaPrintCommand
    {
        public NotaDividendPrintCommand()
            : base()
        {
            NotaType = NotaReturnClass.NotaDividend;
            ReportGrouping = new NotaGroupingAllInOneReport();
            NotasPerPage = 4;
        }

        public override object GetNotaFields(INota nota)
        {
            INotaDividend n = (INotaDividend)nota;

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

                Instrument_DisplayName =
                    n.Instrument.DisplayName,
                Instrument_Isin =
                    n.Instrument.Isin,
                n.ExDividendDate,
                n.SettlementDate,
                Units_Quantity =
                    n.Units.Quantity,
                UnitPrice_Quantity =
                    n.UnitPrice.Quantity,
                UnitPrice_ShortDisplayString =
                    n.UnitPrice.ShortDisplayString,
                DividendAmount_Quantity =
                    n.DividendAmount.Quantity,
                DividendAmount_UnderlyingShortName =
                    n.DividendAmount.UnderlyingShortName,
                n.TaxPercentage,
                Tax_Quantity = 
                    n.Tax.Quantity,
                Tax_UnderlyingShortName =
                    n.Tax.UnderlyingShortName,
                ValueIncludingTax_Quantity =
                    n.ValueIncludingTax.Quantity,
                ValueIncludingTax_UnderlyingShortName =
                    n.ValueIncludingTax.UnderlyingShortName
            };
        }

        protected override string GetFileSuffix(INota[] notaGroup)
        {
            return "Dividend";
        }
    }
}
