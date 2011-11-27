using System;

namespace B4F.TotalGiro.Instruments
{
    public class ModelHierarchy: IModelHierarchy
    {
        public int Key { get; set; }
        public IModelBase TopParentModel { get; protected set; }
        public IModelVersion TopParentModelVersion { get; protected set; }
        public IModelBase ChildModel { get; protected set; }
        public IModelVersion ChildModelVersion { get; protected set; }
        public IModelBase ParentModel { get; protected set; }
        public IModelVersion ParentModelVersion { get; protected set; }
        public int HierarchyLevel { get; protected set; }
    }
}
