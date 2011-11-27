using System;

namespace B4F.TotalGiro.Instruments
{
    public interface IModelHierarchy
    {
        int Key { get; set; }
        IModelBase TopParentModel { get; }
        IModelVersion TopParentModelVersion { get; }
        IModelBase ChildModel { get; }
        IModelVersion ChildModelVersion { get; }
        IModelBase ParentModel { get; }
        IModelVersion ParentModelVersion { get; }
        int HierarchyLevel { get; }
    }
}
