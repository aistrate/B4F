using System.Data;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.Reports
{
    public static class NotasAdapter
    {
        public static DataSet GetNotaDocuments(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedNotaDocumentViews(session, accountId)
                                           .ToDataSet();
            }
        }
    }
}
