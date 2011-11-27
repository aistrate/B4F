using System;

namespace B4F.TotalGiro.CRM
{
    public class ContactDelegate : Contact, IContactDelegate
    {
        public ContactDelegate() { }

        public override ContactTypeEnum ContactType
        {
            get { return ContactTypeEnum.Delegate; }
        }

        public override string GetBSN
        {
            get { return this.KvKNumber; }
        }

        public override DateTime GetBirthFounding
        {
            get { return this.DateOfFounding; }
        }

        public virtual string KvKNumber { get; set; }
        public virtual DateTime DateOfFounding { get; set; }
    }
}
