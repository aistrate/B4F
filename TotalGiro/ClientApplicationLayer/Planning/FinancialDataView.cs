using System;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ClientApplicationLayer.Planning
{
    [Serializable]
    public class FinancialDataView
    {
        public FinancialDataView(ICustomerAccount account, bool allowMissingExpectedReturn)
            : this(account, allowMissingExpectedReturn, true)
        {
        }

        public FinancialDataView(ICustomerAccount account, bool allowMissingExpectedReturn, bool initializeAccountData)
        {
            if (account == null)
                throw new ArgumentNullException("account.");

            this.account = account;
            this.model = Account.ModelPortfolio;

            AllowMissingExpectedReturn = allowMissingExpectedReturn;

            if (initializeAccountData)
            {
                RetrieveFinancialTargetData();
                RetrievePositionData();

                if (account.ModelPortfolio == null)
                    throw new ArgumentException("Account does not have a Model Portfolio.");

                RetrieveModelData();
            }
        }

        public FinancialDataView(IPortfolioModel model, bool allowMissingExpectedReturn)
        {
            if (model == null)
                throw new ArgumentNullException("model.");

            this.model = model;

            AllowMissingExpectedReturn = allowMissingExpectedReturn;

            RetrieveModelData();
        }

        public FinancialDataView(decimal presentValue, decimal expectedReturn, decimal standardDeviation,
                                 decimal targetValue, DateTime targetEndDate, decimal depositPerYear)
        {
            PresentValue = presentValue;
            ExpectedReturn = expectedReturn;
            StandardDeviation = standardDeviation;
            TargetValue = targetValue;
            TargetEndDate = targetEndDate;
            DepositPerYear = depositPerYear;
        }

        internal void RetrieveFinancialTargetData()
        {
            if (Account != null && Account.CurrentFinancialTarget != null)
            {
                TargetValue = Account.CurrentFinancialTarget.TargetAmount.Quantity;
                TargetValueDisplayString = Account.CurrentFinancialTarget.TargetAmount.DisplayString;

                DepositPerYear = Account.CurrentFinancialTarget.DepositPerYear.Quantity;
                DepositPerYearDisplayString = Account.CurrentFinancialTarget.DepositPerYear.DisplayString;

                TargetEndDate = Account.CurrentFinancialTarget.TargetEndDate;
            }
            else
                TargetEndDate = DateTime.Today;
        }

        internal void RetrievePositionData()
        {
            if (Account != null)
            {
                Money totalPosition = Account.TotalPositionAmount(PositionAmountReturnValue.All);
                PresentValue = totalPosition.Quantity;
                PresentValueDisplayString = totalPosition.DisplayString;
            }
        }

        internal void RetrieveModelData()
        {
            if (Model != null)
            {
                ModelId = Model.Key;
                ModelName = Model.ModelName;

                if (!AllowMissingExpectedReturn && Model.ExpectedReturn == 0)
                    throw new ArgumentException("Expected Return of Model is unknown.");

                if (Model.ExpectedReturn != 0m && Model.StandardDeviation == 0m)
                    throw new ArgumentException("Standard Deviation of Model is unknown.");

                ExpectedReturn = Model.ExpectedReturn;
                StandardDeviation = Model.StandardDeviation;
            }
        }

        internal ICustomerAccount Account { get { return account; } }
        [NonSerialized]
        private ICustomerAccount account;

        internal IPortfolioModel Model { get { return model; } }
        [NonSerialized]
        private IPortfolioModel model;

        public bool AllowMissingExpectedReturn { get; private set; }

        public int ModelId { get; private set; }
        public string ModelName { get; private set; }
        public decimal ExpectedReturn { get; private set; }
        public decimal StandardDeviation { get; private set; }

        public decimal PresentValue { get; private set; }
        public string PresentValueDisplayString { get; private set; }

        public decimal TargetValue { get; private set; }
        public string TargetValueDisplayString { get; private set; }
        public decimal DepositPerYear { get; private set; }
        public string DepositPerYearDisplayString { get; private set; }
        public DateTime TargetEndDate { get; private set; }

        public int YearsLeft { get { return Math.Max(0, TargetEndDate.Year - DateTime.Today.Year); } }
    }
}
