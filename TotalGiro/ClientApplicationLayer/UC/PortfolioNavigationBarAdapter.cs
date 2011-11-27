using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.UC
{
    public static class PortfolioNavigationBarAdapter
    {
        public static string GetContactFullName(int contactId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IContact contact = SecurityLayerAdapter.GetOwnedContact(session, contactId);
                return contact.FullName;
            }
        }
    }
}
