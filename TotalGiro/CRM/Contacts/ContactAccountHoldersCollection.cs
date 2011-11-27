using System.Linq;
using B4F.TotalGiro.Collections.Persistence;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.CRM
{
    public class ContactAccountHoldersCollection : TransientDomainCollection<IAccountHolder>, IContactAccountHoldersCollection
    {
        public ContactAccountHoldersCollection() { }

        public ContactAccountHoldersCollection(IContact parent)
        {
            Parent = parent;
        }

        public IContact Parent { get; set; }

        public IAccountHolder PrimaryAccountHolder
        {
            get { return this.FirstOrDefault(ah => ah.IsPrimaryAccountHolder); }
        }
    }
}
