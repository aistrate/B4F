using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Notas;

namespace B4F.TotalGiro.Reports.Documents
{
    public interface INotaDocument : IDocument
    {
        List<INota> Notas { get; }
        INota FirstNota { get; }
    }
}
