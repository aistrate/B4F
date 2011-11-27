using System;

namespace B4F.TotalGiro.Instruments
{
    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.ModelComponent">ModelComponent</see> class
    /// </summary>
    public interface IModelComponent
    {
        int Key { get; set; }
        decimal Allocation { get; set; }
        string DisplayAllocation { get; }
        ModelComponentType ModelComponentType { get;}
        IModelVersion ParentVersion { get; set; }
        int ModelComponentKey { get; }
        string ComponentName { get; }
    }
}
