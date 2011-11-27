using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ClientApplicationLayer.Planning
{
    public class Annuity
    {
        private Annuity()
        {
            DepositAtBeginningOfPeriod = false;
            PeriodsPerYear = 1;
        }

        public Annuity(decimal presentValue, decimal depositPerYear)
            : this(presentValue, depositPerYear, 0)
        {
        }

        public Annuity(decimal presentValue, decimal depositPerYear, int yearsLeft)
            : this()
        {
            PresentValue = presentValue;
            DepositPerYear = depositPerYear;
            YearsLeft = yearsLeft;
        }

        public bool DepositAtBeginningOfPeriod { get; set; }
        public int PeriodsPerYear { get; set; }

        public decimal ExpectedReturn { get; set; }

        public decimal PresentValue { get; private set; }   // aka Principal
        public decimal DepositPerYear { get; private set; }
        public int YearsLeft { get; private set; }

        public decimal ExpectedReturnPerPeriod { get { return ExpectedReturn / (decimal)PeriodsPerYear; } }

        public decimal DepositPerPeriod { get { return DepositPerYear / (decimal)PeriodsPerYear; } }
        
        public int NumberOfPeriods { get { return YearsLeft * PeriodsPerYear; } }

        public decimal AnnuityFactor
        {
            get
            {
                if (ExpectedReturnPerPeriod != 0m)
                {
                    decimal totalInterest = (FinancialMath.Pow(1m + ExpectedReturnPerPeriod, NumberOfPeriods) - 1) / ExpectedReturnPerPeriod;

                    if (DepositAtBeginningOfPeriod)
                        totalInterest = totalInterest * (1 + ExpectedReturnPerPeriod);

                    return totalInterest;
                }
                else
                    return NumberOfPeriods;
            }
        }

        public decimal FutureValueOfPrincipal
        {
            get { return PresentValue * FinancialMath.Pow(1m + ExpectedReturnPerPeriod, NumberOfPeriods); }
        }

        public decimal FutureValueOfDeposits
        {
            get { return DepositPerPeriod * AnnuityFactor; }
        }

        public decimal FutureValue
        {
            get { return FutureValueOfPrincipal + FutureValueOfDeposits; }
        }

        public decimal CalculateFutureValue(int yearsLeft)
        {
            YearsLeft = yearsLeft;
            return FutureValue;
        }
    }
}
