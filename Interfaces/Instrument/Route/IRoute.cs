using System;
using System.Collections.Generic;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.Routes
{
	/// <summary>
	/// This enumeration lists the possible routes
	/// </summary>
    public enum RouteTypes
	{
		/// <summary>
		/// Manual desk
		/// </summary>
        ManualDesk = 1,
        /// <summary>
        /// All automatic routes (fund desk or exchange)
        /// </summary>
		Automatic = 2,
        /// <summary>
        /// Money desk
        /// </summary>
		MoneyDesk = 3,
        /// <summary>
        /// Foreign Exchange desk
        /// </summary>
		ForExDesk = 4
	}

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Routes.Route">Route</see> class
    /// </summary>
    public interface IRoute
	{
		int Key { get; set; }
        RouteTypes Type { get; }
		string Name { get; }
		string Description { get; }
		bool IsDefault { get; }
		bool ApproveTransactions { get; }
        IExchange Exchange { get; }
        bool ResendSecurityOrdersAllowed { get; }
	}
}
