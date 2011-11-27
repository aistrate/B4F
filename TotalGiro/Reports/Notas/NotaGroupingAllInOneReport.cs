using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Reports.Notas
{
    /// <summary>
    /// Groups all notas (for an account) in one report
    /// </summary>
    public class NotaGroupingAllInOneReport : NotaGrouping
    {
        public override INota[][] GetGroups(IList notas)
        {
            INota[][] groups = new INota[1][];
            groups[0] = new INota[notas.Count];
            for (int i = 0; i < notas.Count; i++)
                groups[0][i] = (INota)notas[i];

            return groups;
        }
    }
}
