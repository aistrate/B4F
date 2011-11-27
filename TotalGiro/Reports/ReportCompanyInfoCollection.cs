using System;
using System.Collections.Generic;
using System.Linq;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.Reports
{
    /// <summary>
    /// Cache ReportTemplate and ManagementCompany info.
    /// </summary>
    public class ReportCompanyInfoCollection
    {
        public ReportCompanyInfoCollection(IDalSession session, string reportName, string reportFolderName, bool needsLogoSubfolders)
        {
            ReportName = reportName;
            ReportFolderName = reportFolderName;
            NeedsLogoSubfolders = needsLogoSubfolders;

            dictionary = ReportTemplateMapper.GetReportTemplates(session, reportName)
                                             .Where(rt => rt.ManagementCompany != null)
                                             .ToDictionary(rt => rt.ManagementCompany.Key, rt => new ReportCompanyInfo(rt, this));
        }

        public string ReportName { get; private set; }
        public string ReportFolderName { get; private set; }
        public bool NeedsLogoSubfolders { get; private set; }

        public ReportCompanyInfo this[int managementCompanyId]
        {
            get
            {
                if (dictionary.ContainsKey(managementCompanyId))
                    return dictionary[managementCompanyId];
                else
                    throw new ApplicationException(string.Format("Management company {0} missing report template '{1}'.",
                                                                 managementCompanyId, ReportName));
            }
        }

        private Dictionary<int, ReportCompanyInfo> dictionary;
    }
}
