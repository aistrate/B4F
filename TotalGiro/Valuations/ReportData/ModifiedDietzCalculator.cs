using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Valuations
{
    public class ModDietzCalculator
    {
        public ModDietzCalculator(Money totalValueBegin, Money totalValueEnd,
            DateTime beginDate, DateTime endDate, IList<IDepositWithdrawal> depositsWithdrawals)
        {
            if (totalValueBegin == null || totalValueEnd == null || endDate < beginDate)
                throw new ApplicationException("It is not possible to calculate the modified Dietz Return.");

            this.totalValueBegin = totalValueBegin;
            this.totalValueEnd = totalValueEnd;
            this.beginDate = beginDate;
            this.endDate = endDate;
            movements = new ModDietzNetMovementCollection(this);
            if (depositsWithdrawals != null && depositsWithdrawals.Count > 0)
            {
                foreach (DepositWithdrawal mov in depositsWithdrawals)
                {
                    Money movement = null;
                    if (mov.Deposit != null && mov.Deposit.IsNotZero)
                        movement += mov.Deposit;
                    if (mov.WithDrawal != null && mov.WithDrawal.IsNotZero)
                        movement += mov.WithDrawal;

                    if (movement != null)
                    {
                        if (!movements.Keys.Contains(mov.Date))
                            movements.Add(mov.Date, movement);
                        else
                            movements[mov.Date].Movement += movement;
                    }
                }
            }
        }
        
        public DateTime BeginDate
        {
            get { return beginDate; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
        }

        public Money TotalValueBegin
        {
            get { return totalValueBegin; }
        }

        public Money TotalValueEnd
        {
            get { return totalValueEnd; }
        }

        public ModDietzNetMovementCollection Movements
        {
            get { return movements; }
        }

        public decimal GetInvestmentReturn()
        {
            Money numerator = Money.Subtract(Money.Subtract(TotalValueEnd, TotalValueBegin, true), Movements.TotalMovements, true);
            Money denominator = Money.Add(TotalValueBegin, Movements.TotalWeightedMovements, true);
            if (denominator.IsZero)
                return 0M;
            else
                return numerator.Quantity / denominator.Quantity;
        }

        #region Privates

        private DateTime beginDate;
        private DateTime endDate;
        private Money totalValueBegin;
        private Money totalValueEnd;
        private ModDietzNetMovementCollection movements;

        #endregion
    }

    public class ModDietzNetMovementCollection: GenericDictionary<DateTime,ModDietzNetMovement>
    {
        internal ModDietzNetMovementCollection(ModDietzCalculator parent)
	    {
            this.parent = parent;
	    }

        public void Add(DateTime date, Money movement)
        {
            ModDietzNetMovement contrib = new ModDietzNetMovement(date, parent.BeginDate, parent.EndDate, movement);
            base.Add(new KeyValuePair<DateTime,ModDietzNetMovement>(date, contrib));
        }

        public Money TotalMovements
        {
            get { return getTotal(false); }
        }

        public Money TotalWeightedMovements
        {
            get { return getTotal(true); }
        }

        private Money getTotal(bool weighted)
        {
            Money total = null;
            foreach (ModDietzNetMovement movement in this.Values)
            {
                if (weighted)
                    total = Money.Add(total, movement.WeightedMovement, true);
                else
                    total = Money.Add(total, movement.Movement, true);
            }
            return total;
        }

        #region Privates

        private ModDietzCalculator parent;

        #endregion
    }

    public class ModDietzNetMovement
    {
        public ModDietzNetMovement(DateTime date, DateTime beginDate, DateTime endDate, Money movement)
        {
            if (endDate < beginDate || date < beginDate)
                throw new ApplicationException("It is not possible to calculate the modified Dietz Return.");

            this.date = date;
            this.movement = movement;
            TimeSpan total = endDate - beginDate;
            TimeSpan period = endDate - date;
            this.weight = (decimal)period.Days / (decimal)total.Days;
        }
        
        public DateTime Date
        {
            get { return date; }
        }

        public Money Movement
        {
            get { return movement; }
            internal set { movement = value; }
        }

	    public decimal Weight
	    {
		    get { return weight;}
	    }

        public Money WeightedMovement
        {
            get { return Money.Multiply(Movement, Weight, true); }
        }

        #region Privates

        private DateTime date;
        private Money movement;
        private decimal weight;

        #endregion
    }
}
