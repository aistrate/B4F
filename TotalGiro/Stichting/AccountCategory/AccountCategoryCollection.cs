using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;
using B4F.TotalGiro.Stichting;

namespace B4F.TotalGiro.Stichting
{
    public class AccountCategoryCollection: GenericCollection<IAccountCategory>, IAccountCategoryCollection
    {
        public AccountCategoryCollection(IList bagOfAccountCategories, IAssetManager parent)
            : base(bagOfAccountCategories)
        {
            this.Parent = parent;
        }


        public IAssetManager Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        #region Private Variables

        private IAssetManager parent;

        #endregion
    }
}
