using System;
using System.Collections.Generic;
using System.Linq;

namespace B4F.TotalGiro.Instruments
{
    public interface IModelComponentCollection : IList<IModelComponent>
    {
        IModelVersion Parent { get; set; }
        void AddComponent(IModelComponent item);
        bool RemoveComponent(IModelComponent item);
    }
}
