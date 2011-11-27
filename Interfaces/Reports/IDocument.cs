using System;
using B4F.TotalGiro.Accounts;

namespace B4F.TotalGiro.Reports.Documents
{
    public interface IDocument
    {
        int Key { get; set; }
        string FileName { get; set; }
        string FilePath { get; set; }
        bool EmailNotificationHandled { get; set; }
        bool SentByPost { get; set; }
        DateTime CreationDate { get; }
        string FullPath { get; }
        ICustomerAccount Account { get; }
    }
}
