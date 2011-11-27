using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Reports.Notas
{
    /// <summary>
    /// Groups one nota per report
    /// </summary>
    public class NotaGroupingOnePerReport : NotaGrouping
    {
        public override INota[][] GetGroups(IList notas)
        {
            INota[][] groups = new INota[notas.Count][];
            for (int i = 0; i < notas.Count; i++)
            {
                groups[i] = new INota[1];
                groups[i][0] = (INota)notas[i];
            }
            return groups;
        }
    }
}
