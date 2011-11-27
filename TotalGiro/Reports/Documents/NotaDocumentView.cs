using System;

namespace B4F.TotalGiro.Reports.Documents
{
    public class NotaDocumentView
    {
        public NotaDocumentView(int key, DateTime creationDate, string firstNotaNumber, string firstNotaTitle, int notaCount)
        {
            Key = key;
            CreationDate = creationDate;
            FirstNotaNumber = firstNotaNumber;
            FirstNotaTitle = firstNotaTitle;
            NotaCount = notaCount;
        }

        public int Key { get; private set; }
        public DateTime CreationDate { get; private set; }
        public string FirstNotaNumber { get; private set; }
        public string FirstNotaTitle { get; private set; }
        public int NotaCount { get; private set; }
    }
}
