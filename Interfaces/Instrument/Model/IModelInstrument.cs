using System;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ModelInstrument">ModelInstrument</see> class
    /// </summary>
    public interface IModelInstrument : IModelComponent
	{
		IInstrument Component { get; }
	}
}
