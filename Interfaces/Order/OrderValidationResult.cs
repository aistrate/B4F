using System;

namespace B4F.TotalGiro.Orders
{
    /// <summary>
    /// This enumeration lists the (maintype of) success of the order validation
    /// </summary>
    public enum OrderValidationType
    {
        /// <summary>
        /// The order validation was succesfull
        /// </summary>
        Success,
        /// <summary>
        /// The order validation raised a warning but can still proceed
        /// </summary>
        Warning,
        /// <summary>
        /// The order validation was unsuccesfull
        /// </summary>
        Invalid
    }

    /// <summary>
    /// This enumeration lists the (subtype of) success of the order validation
    /// </summary>
    public enum OrderValidationSubType
    {
        /// <summary>
        /// The order validation was succesfull
        /// </summary>
        Success = 0,
        /// <summary>
        /// The order validation raised a warning since no current price was found for the instrument
        /// </summary>
        Warning_NoCurrentPrice = 10,
        /// <summary>
        /// The order validation raised a warning since the current price found for the instrument was of an old date
        /// </summary>
        Warning_OldPrice            = 11,
        /// <summary>
        /// The order validation raised a warning since (an) opposite side order(s) already exists
        /// </summary>
        Warning_OppositeSideOrder = 12,
        /// <summary>
        /// The order validation raised a warning since (an) buy order(s) already exists when trying to close a short position
        /// </summary>
        Warning_BuyCloseOrderAlreadyExists = 13,
        /// <summary>
        /// The order validation was unsuccesfull since the account has no cash
        /// </summary>
        Invalid_NoCash              = 21,
        /// <summary>
        /// The order validation was unsuccesfull since the account has not enough cash
        /// </summary>
        Invalid_NotEnoughCash       = 22,
        /// <summary>
        /// The order validation was unsuccesfull since a position was being sold that didn't exist
        /// </summary>
        Invalid_NoPosition          = 23,
        /// <summary>
        /// The order validation was unsuccesfull since the sell order size exceeded the position size
        /// </summary>
        Invalid_NotEnoughPosition   = 24,
        /// <summary>
        /// The order can not be placed since a rebalance is happening
        /// </summary>
        Invalid_InstructionExists        = 25,
        /// <summary>
        /// The order validation did not happen yet
        /// </summary>
        Invalid_NotValidated        = 26
    }
    
    /// <summary>
    /// This class is the result of a <see cref="M:B4F.TotalGiro.Orders.IOrder.Approve">order validation</see>.
    /// </summary>
    public class OrderValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:B4F.Orders.OrderValidationResult">OrderValidationResult</see> class.
        /// </summary>
        /// <param name="type">The subtype of the validation result</param>
        /// <param name="message">A descriptive message in case of warings and errors</param>
        public OrderValidationResult(OrderValidationSubType type, string message)
        {
            this.type = type;
            this.message = message;
        }

        /// <summary>
        /// The return subtype of succesfullness
        /// </summary>
        public OrderValidationSubType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// The return (main)type of succesfullness
        /// </summary>
        public OrderValidationType MainType
        {
            get 
            {
                if (this.type == OrderValidationSubType.Success)
                {
                    return OrderValidationType.Success;
                }
                else if (((int)this.type) >= 10 && ((int)this.type) < 20)
                {
                    return OrderValidationType.Warning;
                }
                else
                {
                    return OrderValidationType.Invalid;
                }
            }
        }

        /// <summary>
        /// A descriptive message in case of warings and errors
        /// </summary>
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        #region Privates

        private OrderValidationSubType type;
        private string message;
        
        #endregion

    }
}
