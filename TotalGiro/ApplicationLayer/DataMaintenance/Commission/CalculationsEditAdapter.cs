using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Fees.CommCalculations;
using System.Collections;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Fees;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission
{
    public static class CalculationsEditAdapter
    {
        public static CommCalcView LoadRecord(int commCalcId)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            CommCalc calc = CommCalcMapper.GetCommissionCalculation(session, commCalcId);
            CommCalcView commCalcView = new CommCalcView();

            commCalcView.Name = calc.Name;
            commCalcView.MinValue = (calc.MinValue != null ? calc.MinValue.Quantity : 0m);
            if (calc.MaxValue != null)
                commCalcView.MaxValue = calc.MaxValue.Quantity;
            commCalcView.FixedSetup = (calc.FixedSetup != null ? calc.FixedSetup.Quantity : 0m);
            commCalcView.CalcType = (int) calc.CalcType;

            foreach (CommCalcLine calcLine in calc.CommLines)
            {
                CommCalcLineView lineView = new CommCalcLineView();

                lineView.SerialNo = calcLine.SerialNo;
                lineView.LowerRange = calcLine.LowerRangeQuantity;
                lineView.StaticCharge = calcLine.StaticCharge;
                lineView.IsAmountBased = (calcLine.LineBasedType == CommCalcLineBasedTypes.AmountBased);
                if (lineView.IsAmountBased)
                    lineView.FeePercentage = ((CommCalcLineAmountBased)calcLine).FeePercentage;
                else
                    lineView.Tariff = ((CommCalcLineSizeBased)calcLine).Tariff.Quantity;

                commCalcView.LineViews.Add(lineView);
            }

            session.Close();

            return commCalcView;
        }

        public static void SaveRecord(int commCalcId, CommCalcView commCalcView)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                CommCalc newCalculation = null;
                CommCalc updCalculation = null;
                CommCalcLine newLine;
                ICommCalcLineCollection updCommCalcLines = null;
                ICurrency theCurrency = null;
                Money minimumSize = null;
                Money maximumSize = null;
                Money setupSize = null;

                // Take system currency for now
                theCurrency = (ICurrency)InstrumentMapper.GetBaseCurrency(session);

                minimumSize = new Money(commCalcView.MinValue, theCurrency);
                if (commCalcView.MaxValue.HasValue)
                    maximumSize = new Money(commCalcView.MaxValue.Value, theCurrency);
                else
                    maximumSize = null;
                setupSize = new Money(commCalcView.FixedSetup, theCurrency);

                if (commCalcId != 0)
                {
                    updCalculation = CommCalcMapper.GetCommissionCalculation(session, commCalcId);

                    updCalculation.Name = commCalcView.Name;
                    //updCalculation.CalcType = (FeeCalcTypes)int.Parse(commCalcView.CalcType.ToString());
                    updCalculation.CommCurrency = theCurrency;
                    updCalculation.MinValue = minimumSize;
                    updCalculation.MaxValue = maximumSize;
                    updCalculation.FixedSetup = setupSize;
                    updCommCalcLines = updCalculation.CommLines;

                }
                else
                {
                    switch ((FeeCalcTypes)int.Parse(commCalcView.CalcType.ToString()))
                    {
                        case FeeCalcTypes.Slab:
                            newCalculation = new CommCalcSlab(commCalcView.Name, theCurrency, minimumSize, maximumSize, setupSize);
                            break;
                        case FeeCalcTypes.Flat:
                            newCalculation = new CommCalcFlat(commCalcView.Name, theCurrency, minimumSize, maximumSize, setupSize);
                            break;
                        case FeeCalcTypes.Simple:
                            newCalculation = new CommCalcSimple(commCalcView.Name, theCurrency, setupSize);
                            break;
                        case FeeCalcTypes.FlatSizeBased:
                            newCalculation = new CommCalcFlatSizeBased(commCalcView.Name, theCurrency, minimumSize, maximumSize, setupSize);
                            break;
                    }
                }

                int lineViewIndex = 0;
                foreach (CommCalcLineView lineView in commCalcView.LineViews)
                {
                    if (checkToAddLine(
                            lineView.IsAmountBased,
                            lineView.StaticCharge,
                            lineView.FeePercentage,
                            lineView.LowerRange,
                            lineView.Tariff))
                    {
                        if (lineView.IsAmountBased)
                            newLine = new CommCalcLineAmountBased(new Money(lineView.LowerRange, theCurrency), lineView.StaticCharge, lineView.FeePercentage);
                        else
                            newLine = new CommCalcLineSizeBased(lineView.LowerRange, lineView.StaticCharge, new Money(lineView.Tariff, theCurrency));

                        if (commCalcId != 0)
                        {
                            if (updCommCalcLines.Count > lineViewIndex)
                            {
                                if (lineView.IsAmountBased)
                                {
                                    CommCalcLineAmountBased updCommCalcLine = (CommCalcLineAmountBased)updCommCalcLines[lineViewIndex];
                                    updCommCalcLine.LowerRange = new Money(lineView.LowerRange, theCurrency);
                                    updCommCalcLine.StaticCharge = lineView.StaticCharge;
                                    updCommCalcLine.FeePercentage = lineView.FeePercentage;
                                }
                                else
                                {
                                    CommCalcLineSizeBased updCommCalcLine = (CommCalcLineSizeBased)updCommCalcLines[lineViewIndex];
                                    updCommCalcLine.LowerRange = lineView.LowerRange;
                                    updCommCalcLine.StaticCharge = lineView.StaticCharge;
                                    updCommCalcLine.Tariff = new Money(lineView.Tariff, theCurrency);
                                }
                            }
                            else
                            {
                                updCommCalcLines.AddCalculation(newLine);
                            }
                        }
                        else
                        {
                            newCalculation.CommLines.AddCalculation(newLine);
                        }
                    }
                    // Clear next line of update
                    else
                    {
                        if (commCalcId != 0)
                        {
                            if (updCommCalcLines.Count > lineViewIndex)
                            {
                                updCommCalcLines.RemoveCalculationAt(1);
                            }
                        }
                    }

                    lineViewIndex++;
                }

                if (updCommCalcLines != null)
                {
                    for (int delindex = commCalcView.LineViews.Count; delindex < updCommCalcLines.Count; delindex++)
                        updCommCalcLines.RemoveCalculation(updCommCalcLines[delindex]);
                }

                InternalEmployeeLogin emp = LoginMapper.GetCurrentLogin(session) as InternalEmployeeLogin;

                if (commCalcId != 0)
                {
                    if (emp != null)
                        updCalculation.AssetManager = (IAssetManager)emp.Employer;
                    CommCalcMapper.Insert(session, updCalculation);
                }
                else
                {
                    if (emp != null)
                        newCalculation.AssetManager = (IAssetManager)emp.Employer;
                    CommCalcMapper.Insert(session, newCalculation);
                }
            }
        }

        private static bool checkToAddLine(bool isAmountBased, decimal flatLine, decimal percent, decimal minValue, decimal tariff)
        {
            ///a New Line has been entered if the flatline or percent boxes are not empty
            ///or if they are zero, but a Maximum Value is entered.
            ///Must take care of the initial entry ... as well you see.

            if ((minValue == 0) && ((flatLine != 0) || (isAmountBased && percent != 0) || (!isAmountBased && tariff != 0)))
            {
                return true;
            }
            else
            {
                if ((minValue != 0))

                    return true;
                else
                    return false;
            }
        }

        public static string GetCurrencySymbol()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                string symbol = "";
                ICurrency currency = InstrumentMapper.GetBaseCurrency(session);
                if (currency != null)
                    symbol = currency.AltSymbol;
                return symbol;
            }
        }
    }
}
