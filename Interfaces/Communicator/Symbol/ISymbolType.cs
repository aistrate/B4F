using System;
namespace B4F.TotalGiro.ExternalInterfaces
{
	/// <summary>
	/// This enumeration lists the type of external interfaces
	/// </summary>
    public enum ExternalInterfaces
	{
		/// <summary>
        /// Cleopatra back office interface
		/// </summary>
        Cleopatra = 1,
        /// <summary>
        /// Euronext interface
        /// </summary>
		Euronext
	}

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.ExternalInterfaces.SymbolType">SymbolType</see> class
    /// </summary>
    public interface ISymbolType
	{
		string Description { get; set; }
		bool Equals(object obj);
		string Name { get; set; }
		ExternalInterfaces SymbolTypeID { get; set; }
	}
}
