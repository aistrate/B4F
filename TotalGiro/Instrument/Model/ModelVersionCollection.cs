using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.Instruments
{
    public class ModelVersionCollection : TransientDomainCollection<IModelVersion>, IModelVersionCollection
    {
        public ModelVersionCollection()
            : base() { }

        public ModelVersionCollection(IModelBase parent)
            : base()
        {
            Parent = parent;
        }

        public IModelBase Parent { get; set; }


        /// <summary>
        /// Gets the relevant version for a specific date
        /// </summary>
        public IModelVersion GetVersionByDate(DateTime date)
        {
            return this.Where(x => x.LatestVersionDate <= date).OrderByDescending(x => x.LatestVersionDate).FirstOrDefault();
        }
    }
}
