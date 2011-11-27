using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Dal;
using NHibernate.Criterion;

namespace B4F.DataMigration.EffectenGiro
{
    public class EGAanvraagMapper
    {




        public static IEGAanvraag GetEGAanvraag(IDalSession session, int id)
        {
            return (IEGAanvraag)session.GetObjectInstance(typeof(EGAanvraag), id);
        }




    }
}
