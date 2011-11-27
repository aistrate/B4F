using System;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Orders;

namespace B4F.TotalGiro.Instruments
{
    public static class MoneyMath
    {
        public static Money AdjustAmountForServiceCharge(Money amount, Money serviceCharge, Side side, MathOperator action)
        {
            if (serviceCharge != null && serviceCharge.IsNotZero)
            {
                if (action == MathOperator.Add)
                {
                    if (side == Side.Buy)
                        amount -= serviceCharge;
                    else
                        amount += serviceCharge;
                }
                else // Subtract
                {
                    if (side == Side.Buy)
                        amount += serviceCharge;
                    else
                        amount -= serviceCharge;
                }
            }
            return amount;
        }

    }
}
