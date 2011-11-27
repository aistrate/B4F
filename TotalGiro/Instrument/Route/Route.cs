using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Routes
{
    /// <summary>
    /// Class holds information about an order route
    /// </summary>
	public class Route : IRoute
	{
		protected Route() { }

        /// <summary>
        /// Get system order routes
        /// </summary>
		public virtual int Key
		{
			get { return this.key; }
			set { this.key = value; }
		}

        /// <summary>
        /// The specific route type of the route
        /// </summary>
        public RouteTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Get/set nameof route
        /// </summary>
		public virtual string Name
		{
			get { return this.name; }
			internal set { this.name = value; }
		}

        /// <summary>
        /// Get/set description of route
        /// </summary>
		public virtual string Description
		{
			get { return this.description; }
			internal set { this.description = value; }
		}

        /// <summary>
        /// Get/set one system default route
        /// </summary>
		public virtual bool IsDefault
		{
			get { return this.isDefault; }
			set { this.isDefault = value; }
		}

        /// <summary>
        /// Get/set flag for transactions automatically approved
        /// </summary>
		public virtual bool ApproveTransactions
		{
			get { return approveTransactions; }
			set { approveTransactions = value; }
		}

        public IExchange Exchange
        {
            get { return exchange; }
            set { exchange = value; }
        }

        public bool ResendSecurityOrdersAllowed
        {
            get { return resendSecurityOrdersAllowed; }
            set { resendSecurityOrdersAllowed = value; }
        }
	
		#region OverRides

        /// <summary>
        /// Overridden composition of name
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return Description;
		}

        /// <summary>
        /// Overridden hashcode composition
        /// </summary>
        /// <returns>Hashcode</returns>
		public override int GetHashCode()
		{
			return this.Key.GetHashCode();
		}

		#endregion

		#region Private Variables

		private int key;
        private RouteTypes type;
        private string name;
		private string description;
		private bool isDefault;
		private bool approveTransactions;
        private IExchange exchange;
        private bool resendSecurityOrdersAllowed;

		#endregion

    }
}
