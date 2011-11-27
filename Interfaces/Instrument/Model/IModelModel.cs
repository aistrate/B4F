using System;

namespace B4F.TotalGiro.Instruments
{
    public interface IModelModel: IModelComponent
    {
        IModelVersion Version { get; }
        IModelBase Component { get; }
    }
}
