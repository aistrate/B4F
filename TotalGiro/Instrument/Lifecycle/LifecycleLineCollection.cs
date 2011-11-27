using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.Instruments
{
    public class LifecycleLineCollection : TransientDomainCollection<ILifecycleLine>, ILifecycleLineCollection
    {
        public LifecycleLineCollection()
            : base() { }


        public LifecycleLineCollection(ILifecycle parent)
            : base()
        {
            Parent = parent;
        }

        public ILifecycle Parent { get; set; }

        #region Methods

        /// <summary>
        /// Adds a line to the collection.
        /// </summary>
        /// <param name="item">The line to add to the collection.</param>
        public void AddLine(int ageFrom, IPortfolioModel model)
        {
            ILifecycleLine item = this.Where(x => x.AgeFrom == ageFrom).FirstOrDefault();
            if (item != null)
            {
                if (item.Model.Key != model.GetV(x => x.Key))
                {
                    item.Model = model;
                    item.LastUpdated = DateTime.Now;
                }
            }
            else
            {
                item = new LifecycleLine(Parent, ageFrom, model);
                Add(item);
            }
            if (model.AssetManager.Key != Parent.AssetManager.Key)
                throw new ApplicationException("The model does not have the same assetmanager as the lifecycle");

            ArrangeLines();
        }

        /// <summary>
        /// Adds an empty line to the collection.
        /// </summary>
        public void AddEmptyLine()
        {
            int? maxAge = this.OrderByDescending(x => x.AgeFrom).FirstOrDefault().GetV(x => x.AgeFrom);
            ILifecycleLine emptyLine = new LifecycleLine();
            emptyLine.Parent = Parent;
            emptyLine.AgeFrom = maxAge.HasValue ? maxAge.Value + 1 : 15;
            emptyLine.CreatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
            Add(emptyLine);
            ArrangeLines();
        }

        public bool RemoveLine(ILifecycleLine item)
        {
            item.Parent = null;
            bool success = base.Remove(item);
            if (success && Count > 1)
                ArrangeLines();
            return success;
        }

        public void InsertLine(int index, ILifecycleLine item)
        {
            Add(item);
        }

        public void RemoveLineAt(int index)
        {
            if (index >= 0 && index < this.Count)
                Remove(this[index]);
        }

        /// <summary>
        /// Orders the calILifecycleLines and fills the range attributes
        /// </summary>
        public void ArrangeLines()
        {
            if (this.Count > 0)
            {
                short i = 0;
                ILifecycleLine prev = null;
                foreach (var item in this.OrderBy(x => x.AgeFrom))
                {
                    item.SerialNo = i;
                    if (prev != null)
                        prev.AgeTo = item.AgeFrom;
                    prev = item;
                    i++;
                }
                prev.AgeTo = 300;
            }
        }

        #endregion

    }
}
