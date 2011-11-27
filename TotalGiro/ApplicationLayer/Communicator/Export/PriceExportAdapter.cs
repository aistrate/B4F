using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Communicator.ExternalInterfaces;

namespace B4F.TotalGiro.ApplicationLayer.Communicator.Export
{
    public static class PriceExportAdapter
    {
        public static DataSet GetExternalInterfaces()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return ExternalInterfaceMapper.GetIntrumentExternalInterfaces(session)
                    .Select(f => new
                    {
                        f.Key,
                        f.Name,
                        f.Description
                    })
                    .OrderBy(I => I.Name)
                    .ToDataSet();
            }

        }


    }
}
