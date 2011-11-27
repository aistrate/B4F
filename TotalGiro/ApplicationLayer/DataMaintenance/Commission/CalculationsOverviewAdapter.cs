using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.CommCalculations;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission
{
    public static class CalculationsOverviewAdapter
    {
        public static DataSet GetCommissionCalculationsOverview(string calcname)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return CommCalcMapper.GetCommissionCalculations(session, calcname)
                    .Select(c => new
                    {
                        c.Key,
                        c.Name,
                        MinValue_DisplayString = c.MinValue != null ? c.MinValue.DisplayString : "",
                        MaxValue_DisplayString = c.MaxValue != null ? c.MaxValue.DisplayString : "",
                        FixedSetup_DisplayString = c.FixedSetup != null ? c.FixedSetup.DisplayString : "",
                        c.CalcType
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetCommCalcLines(int calcID, string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                CommCalc calc = CommCalcMapper.GetCommissionCalculation(session, calcID);
                if (calc.CalcType == FeeCalcTypes.Simple)
                    return null;

                List<CommCalcLine> lines = (from a in calc.CommLines
                                            select a).ToList();

                if (lines != null)
                {
                    // Key, SerialNo, DisplayRange, FeePercentage, StaticCharge
                    ds = lines.Select(c => new
                    {
                        c.Key,
                        c.SerialNo,
                        c.DisplayRange,
                        Fee = c.LineBasedType == CommCalcLineBasedTypes.AmountBased ? 
                            ((CommCalcLineAmountBased)c).FeePercentage.ToString("0.0000") + "%" :
                            ((CommCalcLineSizeBased)c).Tariff.DisplayString,
                        FeeQuantity = c.LineBasedType == CommCalcLineBasedTypes.AmountBased ? 
                            ((CommCalcLineAmountBased)c).FeePercentage :
                            ((CommCalcLineSizeBased)c).Tariff != null ? ((CommCalcLineSizeBased)c).Tariff.Quantity : 0M,
                        StaticCharge = c.StaticChargeAmount.DisplayString
                    })
                    .ToDataSet();
                }
                return ds;
            }
        }

        public static void deleteCommissionCalculation(int commCalcId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                CommCalc calc = CommCalcMapper.GetCommissionCalculation(session, commCalcId);
                session.Delete(calc);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error during delete of commission calculation. Check if it in use by a commission rule.");
            }

            session.Close();
        }
    }
}
