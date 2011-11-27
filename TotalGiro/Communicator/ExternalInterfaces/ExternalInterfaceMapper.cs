using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.TotalGiro.Communicator.ExternalInterfaces
{
    public static class ExternalInterfaceMapper
    {
        public static IList<IExternalInterface> GetIntrumentExternalInterfaces(IDalSession session)
        {
            IList<IExternalInterface> list = session.GetTypedListByNamedQuery<IExternalInterface>(
                "B4F.TotalGiro.Communicator.ExternalInterfaces.ExternalInterface.GetIntrumentExternalInterfaces");
            return list;
        }

    }
}
