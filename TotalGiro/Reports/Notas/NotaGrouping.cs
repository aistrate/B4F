using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Reports.Notas
{
    public abstract class NotaGrouping
    {
        public abstract INota[][] GetGroups(IList notas);
    }
}
