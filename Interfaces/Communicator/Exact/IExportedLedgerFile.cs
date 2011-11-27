using System;
using System.Collections.Generic;
using System.Text;
using B4F.TotalGiro.Collections;

namespace B4F.TotalGiro.Communicator.Exact
{
    public interface IExportedLedgerFile
    {
        int Key { get; set; }
        string Name { get; set; }
        string Ext { get; set; }
        string Path { get; set; }
        int Ordinal { get; set; }
        string FullPathName { get; }
        DateTime CreationDate { get; }
        ILedgerEntryCollection LedgerEntries { get; }
    }
}
