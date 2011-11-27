using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.CRM
{
    /// <summary>
    /// Specifies document categories for which the end client can change their own 'By Post' / 'By Email' settings.
    /// Used by the client site (page Settings), and the Management Fee calculation (indirectly).
    /// </summary>
    public enum SendableDocumentCategories
    {
        Notas = 1,              // currently NOT used
        QuarterlyReports = 2,   // currently NOT used
        NotasAndQuarterlyReports = 3,
        YearlyReports = 4
    }

    public enum SendingOptions
    {
        ByPost = 1,
        ByEmail = 2
    }

    public interface IContactSendingOption
    {
        int Key { get; set; }
        IContact Contact { get; }
        SendableDocumentCategories SendableDocumentCategory { get; }
        SendingOptions SendingOption { get; }
        bool Value { get; set; }
    }
}
