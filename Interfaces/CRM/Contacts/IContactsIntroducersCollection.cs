using System;
using System.Collections.Generic;
using B4F.TotalGiro.Collections;
using B4F.TotalGiro.CRM;

namespace B4F.TotalGiro.CRM
{
    public interface IContactsIntroducersCollection : IGenericCollection<IContactsIntroducer>
    {
        IContact Parent { get; }
    }
}
