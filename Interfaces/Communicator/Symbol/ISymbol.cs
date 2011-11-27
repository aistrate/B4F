using System;
using B4F.TotalGiro.Communicator.ExternalInterfaces;
namespace B4F.TotalGiro.Communicator
{

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.ExternalInterfaces.Symbol">Symbol</see> class
    /// </summary>
    public interface ISymbol
    {
        int Key { get; set; }
        string ExternalSymbol { get; set; }
        IExternalInterface ExternalInterface { get; set; }
    }
}
//namespace B4F.TotalGiro.ExternalInterfaces
//{
//    /// <summary>
//    /// This enumeration lists the type of external interfaces
//    /// </summary>
//    public enum ExternalInterfaces
//    {
//        /// <summary>
//        /// Cleopatra back office interface
//        /// </summary>
//        Cleopatra = 1,
//        /// <summary>
//        /// Euronext interface
//        /// </summary>
//        Euronext,
//        /// <summary>
//        /// Paerel leven Export
//        /// </summary>
//        PaerelLeven
//    }