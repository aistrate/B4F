using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Stichting.Remisier;
using B4F.TotalGiro.Stichting.Login;

namespace B4F.TotalGiro.CRM
{
    public class ContactsIntroducer : IContactsIntroducer
    {
        public ContactsIntroducer() { }

        public ContactsIntroducer(IContact con, IRemisier rem, IRemisierEmployee emp)
        {
            this.Contact = con;
            this.Remisier = rem;
            this.RemisierEmployee = emp;
        }

        public virtual int Key
        {
            get { return key; }
            set { key = value; }
        }

        public IContact Contact
        {
            get { return contact; }
            set { contact = value; }
        }

        public IRemisier Remisier
        {
            get { return remisier; }
            set { remisier = value; }
        }

        public IRemisierEmployee RemisierEmployee
        {
            get { return remisierEmployee; }
            set { remisierEmployee = value; }
        }

        public virtual DateTime CreationDate
        {
            get { return this.creationDate; }
        }

        public override bool Equals(object obj)
        {
            IContactsIntroducer otherIntroducer = obj as IContactsIntroducer;
            if (otherIntroducer != null)
            {
                return (this.Remisier == otherIntroducer.Remisier &&
                    this.RemisierEmployee == otherIntroducer.RemisierEmployee);
            }
            else
                return false;
        }

        #region Private Variables

		private int key;
        private IContact contact;
        private IRemisier remisier;
        private IRemisierEmployee remisierEmployee;
        private DateTime creationDate;

		#endregion
    }
}
