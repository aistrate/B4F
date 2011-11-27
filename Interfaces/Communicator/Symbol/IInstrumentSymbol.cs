using System;

namespace B4F.TotalGiro.Communicator
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.ExternalInterfaces.InstrumentSymbol">InstrumentSymbol</see> class
    /// </summary>
    public interface IInstrumentSymbol : ISymbol
	{
		B4F.TotalGiro.Instruments.IInstrument Instrument { get; set; }
	}
}
