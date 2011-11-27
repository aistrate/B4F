using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.Settings
{
    public struct OptionView
    {
        public SendableDocumentCategories Category { get; set; }
        public SendingOptions Option { get; set; }
        public bool Value { get; set; }
    }

    public static class SettingsAdapter
    {
        public static List<OptionView> GetContactSendingOptions(int contactId, IEnumerable<OptionView> displayOptionViews)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IContact contact = SecurityLayerAdapter.GetOwnedContact(session, contactId);
                IContactSendingOptionCollection retrievedSendingOptions = contact.ContactSendingOptions;

                return (from ov in displayOptionViews
                        select new OptionView
                        {
                            Category = ov.Category,
                            Option = ov.Option,
                            Value = retrievedSendingOptions.GetValueOrDefault(ov.Category, ov.Option)
                        }).ToList();
            }
        }

        public static void SaveContactSendingOptions(int contactId, List<OptionView> optionViews)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IContact contact = SecurityLayerAdapter.GetOwnedContact(session, contactId);
                ContactSendingOptionCollection sendingOptions = (ContactSendingOptionCollection)contact.ContactSendingOptions;

                sendingOptions.RemoveAll(option => !optionViews.Exists(
                    optionView => optionView.Category == option.SendableDocumentCategory && optionView.Option == option.SendingOption));

                optionViews.FindAll(optionView => sendingOptions.Exists(optionView.Category, optionView.Option))
                           .ForEach(optionView => sendingOptions.SetValue(optionView.Category, optionView.Option, optionView.Value));

                optionViews.FindAll(optionView => !sendingOptions.Exists(optionView.Category, optionView.Option))
                           .ForEach(optionView => sendingOptions.Add(optionView.Category, optionView.Option, optionView.Value));

                session.Update(contact);
            }
        }

        //public static List<IContact> Test_GetContacts()
        //{
        //    using (IDalSession session = NHSessionFactory.CreateSession())
        //    {
        //        return ContactMapper.GetContacts(session, SendableDocumentCategories.NotasAndQuarterlyReports, SendingOptions.ByPost, true);
        //    }
        //}
    }
}
