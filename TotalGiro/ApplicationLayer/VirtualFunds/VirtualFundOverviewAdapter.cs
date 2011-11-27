using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.VirtualFunds
{
    public static class VirtualFundOverviewAdapter
    {
        public static DataSet GetVirtualFunds()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                ISecCategory secCategory = SecCategoryMapper.GetSecCategory(session, SecCategories.VirtualFund);

                IList<IVirtualFund> funds = InstrumentMapper.GetVirtualFunds(session);

                return funds.Select(f => new
                {
                    f.Key,
                    InstrumentName = f.Name,
                    f.Isin,
                    f.LastNavDate,
                    f.CreationDate
                }).ToDataSet();

            }
        }



    }
}
