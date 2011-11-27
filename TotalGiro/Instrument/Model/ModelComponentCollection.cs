using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments
{
    public class ModelComponentCollection : TransientDomainCollection<IModelComponent>, IModelComponentCollection
    {
        public ModelComponentCollection()
            : base() { }

        public ModelComponentCollection(IModelVersion parent)
            : base()
        {
            Parent = parent;
        }

        public IModelVersion Parent { get; set; }

        #region Methods

        public void AddComponent(IModelComponent item)
        {
            base.Add(item);
            item.ParentVersion = Parent;
        }

        public bool RemoveComponent(IModelComponent item)
        {
            bool success = false;
            if (this.Count > 0)
            {
                IModelComponent component = getItem(item);
                if (component != null)
                {
                    component.ParentVersion = null;
                    success = base.Remove(component);
                }
            }
            return success;
        }

        public void Clear()
        {
            if (this.Count > 0)
            {
                foreach (IModelComponent component in this)
                    component.ParentVersion = null;
                base.Clear();
            }
        }

        public bool Contains(IModelComponent item)
        {
            bool hit = false;
            if (this.Count > 0)
            {
                IModelComponent component = getItem(item);
                if (component != null)
                    hit = true;
            }
            return hit;
        }

        #endregion

        #region Helpers

        private IModelComponent getItem(IModelComponent item)
        {
            IModelComponent component = null;
            if (this.Count > 0)
                component = this.Where(x => x.ModelComponentType == item.ModelComponentType && x.ModelComponentKey == item.ModelComponentKey).FirstOrDefault();
            return component;
        }

        #endregion

    }
}
