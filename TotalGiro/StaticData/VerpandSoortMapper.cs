using System;
using System.Collections;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{
    public static class VerpandSoortMapper
    {
        public static IVerpandSoort GetVerpandSoort(IDalSession session, int VerpandSoortID)
        {
            return (IVerpandSoort)session.GetObjectInstance(typeof(VerpandSoort), VerpandSoortID);
        }

        public static IList GetVerpandSoorten(IDalSession session)
        {
            return session.GetList(typeof(VerpandSoort));
        }
    }
}
