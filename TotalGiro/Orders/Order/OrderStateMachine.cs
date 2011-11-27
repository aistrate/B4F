using System;
using System.Collections;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.Routes;

namespace B4F.TotalGiro.Orders
{
	internal enum OrderStateEvents
	{
		New,
		Send,
		Place,
		Fill,
		CheckFill,
		Terminate
	}

	internal enum OrderCancelStateEvents
	{
		Cancel /*,
		QueueCancel,
		RequestCancel,
		CancelSuccess,
		CancelFail,
		CancelByElimination */
	}

	internal class OrderStateMachine
	{
		protected OrderStateMachine() { }

		static OrderStateMachine()
		{
			machine.initialise();
		}

		private void initialise()
		{
			if (stati == null)
				stati = getSession().GetList(typeof(OrderStatus));

			if (cancelStati == null)
				cancelStati = getSession().GetList(typeof(OrderCancelStatus));
		}

		private IDalSession getSession()
		{
			if (session == null)
				session = NHSessionFactory.CreateSession();
			return session;
		}

		public static bool SetNewStatus(IOrder order, OrderStateEvents stateEvent)
		{
			if (order == null)
			{
				throw new ApplicationException("The order cannot be null when changing the status");
			}

			OrderStati oldStatus = order.Status;

			switch (stateEvent)
			{
				//case OrderStateEvents.Approve:
				//    machine.checkApprove(order);
				//    break;
				case OrderStateEvents.New:
					machine.checkNew(order);
					break;
				case OrderStateEvents.Send:
					machine.checkSend(order);
					break;
				case OrderStateEvents.Place:
					machine.checkPlaced(order);
					break;
				case OrderStateEvents.Fill:
					machine.checkFill(order);
					break;
				case OrderStateEvents.CheckFill:
					machine.checkCheck(order);
					break;
				case OrderStateEvents.Terminate:
					machine.checkTerminate(order);
					break;
				default:
					break;
			}

            // Set Close Date
            machine.checkSetDateClosed(order);

            return (oldStatus != order.Status);
		}

		public static bool SetNewCancelStatus(IOrder order, OrderCancelStateEvents stateEvent)
		{
			if (order == null)
			{
				throw new ApplicationException("The order cannot be null when changing the cancel status");
			}

			OrderCancelStati oldStatus = order.CancelStatus;

			// For now -> always cancel
			//switch (stateEvent)
			//{
			//	default:
					machine.checkCancel(order);
			//}

            // Set Close Date
            machine.checkSetDateClosed(order);
            return (oldStatus != order.CancelStatus);
		}

        // Special overload for fuckup with NHibernate casting
        public static bool SetNewCancelStatus(IMonetaryOrder order, OrderCancelStateEvents stateEvent)
        {
            return order.Cancel();
        }

        internal static bool ResetStatus(IOrder order)
        {
            OrderStatus status = GetOrderStatus(order.Status);
            if (!status.IsOpen)
                throw new ApplicationException("The status of the order cannot be rolled back.");

            order.Status = OrderStati.Placed;
            machine.checkFill(order);
            return true;
        }

		internal static OrderStatus GetOrderStatus(OrderStati key)
		{
			foreach (OrderStatus status in stati)
			{
				if (status.Key == key)
				{
					return status;
					//break;
				}
			}
			throw new ApplicationException("The Order Status could not be found.");
		}

		internal static OrderCancelStatus GetOrderCancelStatus(OrderCancelStati key)
		{
			foreach (OrderCancelStatus status in cancelStati)
			{
				if (status.Key == key)
				{
					return status;
				}
			}
			throw new ApplicationException("The Order Cancel Status could not be found.");
		}

		//private void checkApprove(Order order)
		//{
		//    if (order is IAggregatedOrder)
		//    {
		//        if (order.Status == OrderStati.New || order.Approved)
		//        {
		//            order.Approved = true;
		//            if (order.ExRate != 0 && ((IAggregatedOrder)order).Route != null)
		//            {
		//                if (((IAggregatedOrder)order).Route.Key != FixedRoutes.Automatic)
		//                    order.Status = OrderStati.Routed;
		//            }
		//        }
		//    }
		//    else
		//    {
		//        if (order.Status == OrderStati.New)
		//        {
		//            order.Status = OrderStati.Approved;
		//        }
		//    }
		//    order.ApprovalDate = DateTime.Now;
		//}

		private void checkNew(IOrder order)
		{
			if (order.IsStgOrder)
			{
				if (order.Status == OrderStati.New || order.Status == OrderStati.Routed)
				{
					if (((IStgOrder)order).Route.Type != RouteTypes.Automatic)
						order.Status = OrderStati.New;
				}
			}
			else
			{
				if (order.Status == OrderStati.Routed)
				{
					order.Status = OrderStati.New;
				}
			}
		}

		private void checkSend(IOrder order)
		{
			if (order.IsStgOrder)
			{
				if (((IStgOrder)order).IsSendable)
				{
					order.Status = OrderStati.Routed;
				}
			}
		}

		private void checkPlaced(IOrder order)
		{
			if (order.IsAggregateOrder)
			{
				if (order.Status == OrderStati.Routed)
				{
					order.Status = OrderStati.Placed;
				}
			}
		}
		
		private void checkFill(IOrder order)
		{
			if (order.FilledValue != null && order.FilledValue.IsNotZero)
			{
				order.Status = OrderStati.PartFilled;
				if (order.OpenValue.IsZero)
					checkCheck(order);
			}
		}

		private void checkCheck(IOrder order)
		{
			bool approved = true;
			bool check = false;

			if (order.FilledValue != null)
			{
				if (order.OpenValue.IsZero)
					check =true;
			}

			if (check)
			{
				order.Status = OrderStati.Filled;
				if (order.Transactions != null && order.Transactions.Count > 0)
				{
					foreach (Transaction transaction in order.Transactions)
					{
						if (!(transaction.Approved))
						{
							approved = false;
							break;
						}
					}
				}
				else
				{
					approved = false;
				}

				if (approved)
					order.Status = OrderStati.Checked;
			}
		}

		private void checkTerminate(IOrder order)
		{
			OrderStatus status = GetOrderStatus(order.Status);
			bool transactionNotAllocated = false;

			if (status.IsOpen)
			{
				if (order.Transactions != null && order.Transactions.Count > 0)
				{
					foreach (Transaction transaction in order.Transactions)
					{
						if (!(transaction.Approved))
						{
							throw new ApplicationException("It is not possible to terminate an order with unapproved transactions. Approve/cancel the transactions");
						}
                        OrderExecution exec = null;
                        if (transaction.TransactionType == TransactionTypes.Execution)
                            exec = transaction as OrderExecution;
                        if (exec != null)
                            transactionNotAllocated = !exec.IsAllocated;
                        else
                            transactionNotAllocated = false;
					}

                    if (transactionNotAllocated)
						order.Status = OrderStati.Checked;
					else
						order.Status = OrderStati.Terminated;
				}
				else
					order.Status = OrderStati.Terminated;
			}
		}

		private void checkCancel(IOrder order)
		{
			OrderStatus status = GetOrderStatus(order.Status);

			if (status.IsOpen)
			{
				// Only allow the child to get cancelled if the parent is also cancelled/terminated
				if (order.ParentOrder != null)
				{
					status = GetOrderStatus(order.ParentOrder.Status);
					if (status.IsOpen)
					{
						throw new ApplicationException("This order can not be cancelled.");
					}
				}

				order.CancelStatus = OrderCancelStati.Cancelled;
				// also update the status
				checkTerminate(order);
			}
		}

        private void checkSetDateClosed(IOrder order)
        {
            // Set Close Date
            if ((order.Status == OrderStati.Checked || order.Status == OrderStati.Terminated) && !order.IsClosed)
            {
                OrderStatus status = GetOrderStatus(order.Status);
                if (!status.IsOpen)
                    order.DateClosed = DateTime.Now;
            }
        }

		private static OrderStateMachine machine = new OrderStateMachine();
		private static IList stati;
		private static IList cancelStati;
		private IDalSession session;
	}
}
