using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Notas;
using System.Collections;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Reports.Documents
{
    public class NotaDocument : Document, INotaDocument
    {
        public NotaDocument() : base() { }

        public NotaDocument(string fileName, string filePath, bool sentByPost)
            : base(fileName, filePath, sentByPost) { }

        public List<INota> Notas
        {
            get
            {
                if (notas == null)
                    notas = NHSession.ToList<INota>(bagOfNotas);
                return notas;
            }
        }

        public INota FirstNota
        {
            get { return (Notas.Count > 0 ? Notas[0] : null); }
        }

        public override ICustomerAccount Account
        {
            get
            {
                if (FirstNota != null)
                    return FirstNota.Account;
                else
                    throw new ApplicationException(
                        string.Format("Could not determine account for document '{0}' because no notas are associated with it.", Key));
            }
        }

        private IList bagOfNotas;
        private List<INota> notas;
    }
}
