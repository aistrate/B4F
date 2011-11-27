using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.CRM
{
    public interface IContactSendingOptionCollection : IList<IContactSendingOption>
    {
        IContact Parent { get; }
        void Add(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption, bool value);
        bool Remove(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption);
        bool GetValue(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption);
        bool GetValueOrDefault(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption);
        void SetValue(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption, bool value);
        bool Exists(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption);
        IContactSendingOption GetItem(SendableDocumentCategories sendableDocumentCategory, SendingOptions sendingOption);
    }
}
