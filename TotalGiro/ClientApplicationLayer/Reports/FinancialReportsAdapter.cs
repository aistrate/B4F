using System.Data;
using System.Linq;
using B4F.TotalGiro.ClientApplicationLayer.SecurityLayer;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.Reports
{
    public static class FinancialReportsAdapter
    {
        public static DataSet GetFinancialReportDocuments(int accountId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return SecurityLayerAdapter.GetOwnedFinancialReportDocuments(session, accountId)
                                            .Select(d => new
                                            {
                                                d.Key,
                                                d.Report.ReportLetter.YearAndType,
                                                d.Report.ReportLetter.Concern,
                                                d.Report.CreationDate
                                            })
                                            .ToDataSet();
            }
        }
    }
}
