using System;
using System.Text;
using B4F.TotalGiro.Collections;
using System.Collections;

namespace B4F.TotalGiro.CRM.Contacts
{
 
        public class CompanyContactPersonCollection : GenericCollection<ICompanyContactPerson>, ICompanyContactPersonCollection
        {
            public CompanyContactPersonCollection() { }
            public CompanyContactPersonCollection(IContact parent, IList bagOfContactPerson)
                : base(bagOfContactPerson)
            {
                this.Parent = parent;
            }

            public IContact Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            public override void Add(ICompanyContactPerson item)
            {
                if (!item.Company.Equals(Parent))
                    throw new ApplicationException("The account should match the parent account.");

                base.Add(item);
            }


            public override bool Contains(ICompanyContactPerson item)
            {
                bool ret = false;

                foreach (ICompanyContactPerson contactPers in this)
                {
                    if (contactPers.ContactPerson.Equals(item.ContactPerson))
                    {
                        return ret = true;
                    }
                }
                return ret;
            }

            #region Private Variables

            private IContact parent;

            #endregion
        }
}
