using System;
using System.IO;
using System.Collections.Generic;

namespace B4F.TotalGiro.Reports
{
    public class ReportCompanyInfo
    {
        public ReportCompanyInfo(IReportTemplate reportTemplate, ReportCompanyInfoCollection reportInfo)
        {
            ManagementCompanyId = reportTemplate.ManagementCompany.Key;
            CompanyName = reportTemplate.ManagementCompany.CompanyName.Trim();
            PdfFolder = reportTemplate.ManagementCompany.PdfReportsFolder.Trim();
            ShowLogoByDefault = reportTemplate.ManagementCompany.ShowLogoByDefault;
            ReportTemplateName = reportTemplate.ReportTemplateName.Trim();

            ReportInfo = reportInfo;
        }

        public int ManagementCompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string PdfFolder { get; private set; }
        public bool ShowLogoByDefault { get; private set; }
        public string ReportTemplateName { get; private set; }

        public ReportCompanyInfoCollection ReportInfo { get; private set; }

        public string FullPdfFolder
        {
            get
            {
                if (fullPdfFolder == null)
                {
                    if (PdfFolder == string.Empty)
                        throw new ApplicationException(string.Format("PDF report-generation folder not set for management company '{0}' ({1}).",
                                                                     CompanyName, ManagementCompanyId));

                    fullPdfFolder = string.Format(@"{0}\{1}\{2}{3:yyyy.MM.dd}",
                                                         PdfFolder,
                                                         ReportInfo.ReportFolderName,
                                                         ReportInfo.NeedsLogoSubfolders ?
                                                                (ShowLogoByDefault ? @"WithLogo\" : @"NoLogo\") :
                                                                "",
                                                         DateTime.Today);

                    if (!Directory.Exists(fullPdfFolder))
                        Directory.CreateDirectory(fullPdfFolder);
                }

                return fullPdfFolder;
            }
        }
        private string fullPdfFolder = null;

        public FolderCollection FullPdfSubfolders
        {
            get
            {
                if (fullPdfSubfolders == null)
                    fullPdfSubfolders = new FolderCollection(FullPdfFolder);
                return fullPdfSubfolders;
            }
        }
        private FolderCollection fullPdfSubfolders = null;
    }

    public class FolderCollection
    {
        public FolderCollection(string rootFolder)
        {
            RootFolder = rootFolder;
        }

        public string RootFolder { get; private set; }

        public string this[string folder]
        {
            get
            {
                if (!fullPaths.ContainsKey(folder))
                {
                    fullPaths[folder] = string.Format(@"{0}\{1}", RootFolder, folder);

                    if (!Directory.Exists(fullPaths[folder]))
                        Directory.CreateDirectory(fullPaths[folder]);
                }

                return fullPaths[folder];
            }
        }

        Dictionary<string, string> fullPaths = new Dictionary<string, string>();
    }
}
