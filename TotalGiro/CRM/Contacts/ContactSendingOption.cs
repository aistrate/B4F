using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Reports;

namespace B4F.TotalGiro.CRM
{
    public class ContactSendingOption : IContactSendingOption
    {
        private ContactSendingOption() { }

        public ContactSendingOption(IContact contact, SendableDocumentCategories sendableDocumentCategory, 
                                      SendingOptions sendingOption, bool value)
        {
            Contact = contact;
            SendableDocumentCategory = sendableDocumentCategory;
            SendingOption = sendingOption;
            Value = value;
        }

        public virtual int Key { get; set; }
        public virtual IContact Contact { get; private set; }
        public virtual SendableDocumentCategories SendableDocumentCategory { get; private set; }
        public virtual SendingOptions SendingOption { get; private set; }
        public virtual bool Value { get; set; }

        internal ContactSendingOption Clone()
        {
            return (ContactSendingOption)this.MemberwiseClone();
        }
    }
}
