using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Utils;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using B4F.TotalGiro.Collections.Persistence;

namespace B4F.TotalGiro.CRM
{
    public class ContactSendingOptionCollection : TransientDomainCollection<IContactSendingOption>, IContactSendingOptionCollection
    {
        public ContactSendingOptionCollection()
            : base() { }

        public ContactSendingOptionCollection(IContact parent)
            : base()
        {
            Parent = parent;
        }

        private ContactSendingOptionCollection(IEnumerable<IContactSendingOption> collection)
            : base(collection)
        {
        }


        /// <summary>
        /// A copy of the actual default values.
        /// </summary>
        public static ContactSendingOptionCollection DefaultValues
        {
            get
            {
                return new ContactSendingOptionCollection(from opt in defaultValues 
                                                          select (IContactSendingOption)((ContactSendingOption)opt).Clone());
            }
        }

        private static readonly ContactSendingOptionCollection defaultValues = new ContactSendingOptionCollection()
        {
            // Current
            new ContactSendingOption(null, SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByPost, false),
            new ContactSendingOption(null, SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByEmail, true),
            
            new ContactSendingOption(null, SendableDocumentCategories.YearlyReports, SendingOptions.ByPost, true),
            new ContactSendingOption(null, SendableDocumentCategories.YearlyReports, SendingOptions.ByEmail, false)
        };
        
        
        public IContact Parent { get; internal set; }

        /// <summary>
        /// Use this method to add sending options (instead of the generic one).
        /// </summary>
        public void Add(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption, bool value)
        {
            if (Parent != null)
            {
                if (!Exists(sendableDocumentCategory, sendingOption))
                    Add(new ContactSendingOption(Parent, sendableDocumentCategory, sendingOption, value));
                else
                    throw new ApplicationException(string.Format("ContactSendingOption already exists for '{0}' and '{1}' (Contact {2}).",
                                                                 Util.SplitCamelCase(sendableDocumentCategory.ToString()),
                                                                 Util.SplitCamelCase(sendingOption.ToString()),
                                                                 Parent.Key));
            }
            else
                throw new ApplicationException("ContactSendingOption is missing Contact.");
        }

        public bool Remove(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption)
        {
            return RemoveAll(item => item.SendableDocumentCategory == sendableDocumentCategory && item.SendingOption == sendingOption) > 0;
        }

        public bool GetValue(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption)
        {
            return GetItem(sendableDocumentCategory, sendingOption).Value;
        }

        public bool GetValueOrDefault(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption)
        {
            if (Exists(sendableDocumentCategory, sendingOption))
                return GetValue(sendableDocumentCategory, sendingOption);
            else
                return defaultValues.GetValue(sendableDocumentCategory, sendingOption);
        }

        public void SetValue(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption, bool value)
        {
            GetItem(sendableDocumentCategory, sendingOption).Value = value;
        }

        public bool Exists(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption)
        {
            return Exists(item => item.SendableDocumentCategory == sendableDocumentCategory && item.SendingOption == sendingOption);
        }

        public IContactSendingOption GetItem(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption)
        {
            IContactSendingOption foundItem = Find(
                item => item.SendableDocumentCategory == sendableDocumentCategory && item.SendingOption == sendingOption);
            
            if (foundItem != null)
                return foundItem;
            else
                throw new ApplicationException(string.Format("ContactSendingOption could not be found for '{0}' and '{1}' (Contact {2}).",
                                                             Util.SplitCamelCase(sendableDocumentCategory.ToString()),
                                                             Util.SplitCamelCase(sendingOption.ToString()),
                                                             (Parent != null ? Parent.Key : 0)));
        }
    }
}
