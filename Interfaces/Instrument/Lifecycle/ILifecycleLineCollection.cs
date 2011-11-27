using System;
using System.Collections.Generic;

namespace B4F.TotalGiro.Instruments
{
    public interface ILifecycleLineCollection : IList<ILifecycleLine>
    {
        ILifecycle Parent { get; set; }
        void AddLine(int ageFrom, IPortfolioModel model);
        void AddEmptyLine();
        bool RemoveLine(ILifecycleLine item);
        void InsertLine(int index, ILifecycleLine item);
        void RemoveLineAt(int index);
        void ArrangeLines();
    }
}
