using System;
using System.Collections;
using System.Collections.Generic;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.StaticData
{

    public static class PandHouderMapper
    {
 
        public static IPandHouder GetPandHouder(IDalSession session, int PandhouderID)
        {
            return (IPandHouder)session.GetObjectInstance(typeof(PandHouder), PandhouderID);
        }

        public static IList GetPandHouders(IDalSession session)
        {
            return session.GetList(typeof(PandHouder));
        }

    }
}
