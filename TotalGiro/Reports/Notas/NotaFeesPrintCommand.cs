using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Notas;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Valuations;
using System.Collections;
using B4F.TotalGiro.Valuations.AverageHoldings;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.GeneralLedger.Journal.Bookings;

namespace B4F.TotalGiro.Reports.Notas
{
    public class NotaFeesPrintCommand : NotaPrintCommand
    {
        public NotaFeesPrintCommand()
            : base()
        {
            NotaType = NotaReturnClass.NotaFees;
        }

        public override object GetNotaFields(INota nota)
        {
            INotaFees n = (INotaFees)nota;

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

                n.Description,
                StornoedBookNota_NotaNumber =
                    n.StornoedBookNota != null ? n.StornoedBookNota.NotaNumber : "",
                ManagementFeeAmount_Quantity =
                    n.ManagementFeeAmount.Quantity,
                ManagementFeeAmount_UnderlyingShortName =
                    n.ManagementFeeAmount.UnderlyingShortName,
                n.TaxPercentage,
                Tax_Quantity =
                    n.Tax.Quantity,
                Tax_UnderlyingShortName =
                    n.Tax.UnderlyingShortName,
                ValueIncludingTax_Quantity =
                    n.ValueIncludingTax.Quantity,
                ValueIncludingTax_UnderlyingShortName =
                    n.ValueIncludingTax.UnderlyingShortName,
                PeriodStartDate_Month =
                    n.PeriodStartDate.Month,
                PeriodStartDate_Year =
                    n.PeriodStartDate.Year
            };
        }

        protected override string GetFileSuffix(INota[] notaGroup)
        {
            return "Fees";
        }

        protected override void AfterDataSetBuild(IDalSession session, INota[] notaGroup, DataSet ds)
        {
            // there's only one NotaFees per report
            INotaFees notaFees = (INotaFees)notaGroup[0];
            
            // Pivot the table
            DataTable dt = new DataTable("AverageValuations");
            dt.Columns.Add("InstrumentID", typeof(int));
            dt.Columns.Add("InstrumentName", typeof(string));
            dt.Columns.Add("IsTradeable", typeof(bool));
            dt.Columns.Add("HasManagementFees", typeof(bool));
            dt.Columns.Add("AvgMarketValueMonth1", typeof(decimal));
            dt.Columns.Add("AvgMarketValueMonth2", typeof(decimal));
            dt.Columns.Add("AvgMarketValueMonth3", typeof(decimal));

            Hashtable monthColumnsByMonth = new Hashtable();
            for (int i = 0; i < 3; i++)
                monthColumnsByMonth[notaFees.PeriodStartDate.Month + i] = string.Format("AvgMarketValueMonth{0}", i + 1);

            Hashtable rowsById = new Hashtable();
            foreach (IAverageHolding averageHolding in notaFees.AverageHoldings)
                if (averageHolding.AverageValue.Quantity != 0m && monthColumnsByMonth.Contains(averageHolding.Month))
                {
                    int instrumentId = averageHolding.Instrument.Key;
                    
                    if (!rowsById.Contains(instrumentId))
                    {
                        DataRow dr = dt.NewRow();
                        dr["InstrumentID"] = instrumentId;
                        dr["InstrumentName"] = averageHolding.Instrument.Name;
                        dr["IsTradeable"] = averageHolding.Instrument.IsTradeable;
                        dr["HasManagementFees"] = false;
                        dr["AvgMarketValueMonth1"] = 0m;
                        dr["AvgMarketValueMonth2"] = 0m;
                        dr["AvgMarketValueMonth3"] = 0m;

                        dt.Rows.Add(dr);
                        rowsById[instrumentId] = dr;
                    }

                    if (monthColumnsByMonth.Contains(averageHolding.Month))
                    {
                        ((DataRow)rowsById[instrumentId])[(string)monthColumnsByMonth[averageHolding.Month]] =
                                                                                    averageHolding.AverageValue.Quantity;
                        if (averageHolding.FeeItems.GetItemByType(FeeTypes.ManagementFee) != null)
                            ((DataRow)rowsById[instrumentId])["HasManagementFees"] = true;
                    }
                    else
                        throw new ApplicationException(
                            string.Format("Period {0:dd-MM-yyyy} to {1:dd-MM-yyyy} has more months than the Management Fee nota can display (3).",
                                          notaFees.PeriodStartDate, notaFees.PeriodEndDate));
                }
            
            ds.Tables.Add(dt);

            var p = from a in notaFees.FeeDetails
                    where ((IManagementFeeComponent)a).MgtFeeType != null
                    group a by ((IManagementFeeComponent)a).MgtFeeType into g
                    select new { MgtFeeType = g.Key, FeeAmount = g.Select(x => x.ComponentValue).Sum() };
            if (p == null || p.Count() == 0)
                throw new ApplicationException("No breakup lines were found.");

            dt = p.Select(c => new
                    {
                        Key = c.MgtFeeType != null ? (int)c.MgtFeeType.Key : (int)FeeTypes.None,
                        FeeTypeKey =
                            c.MgtFeeType != null ? (int)c.MgtFeeType.Key : (int)FeeTypes.None,
                        MgtFeeType_Description =
                            c.MgtFeeType != null ? c.MgtFeeType.Description : "Unknown", 
                        Amount_Quantity =
                            c.FeeAmount.Quantity, 
                        Amount_UnderlyingShortName =
                            c.FeeAmount.UnderlyingShortName
                    })
                    .ToDataTable("BreakupLines");
            ds.Tables.Add(dt);
        }
    }
}
