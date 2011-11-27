using System;
using System.Data;
using System.Linq;
using B4F.TotalGiro.CRM;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils.Linq;

namespace B4F.TotalGiro.Reports.Letters
{
    public class LetterPrintCommand
    {
        public LetterPrintCommand(IDalSession session, string reportName, string reportFolderName)
        {
            this.reportCompanies = new ReportCompanyInfoCollection(session, reportName, reportFolderName, true);
        }

        public void PrintLetter(string userName, LoginPerson loginPerson)
        {
            try
            {
                int managementCompanyId = loginPerson.AssetManager.Key;

                reportExecutionWrapper.SetReportName(reportCompanies[managementCompanyId].ReportTemplateName);

                DataSet ds = buildDataSet(loginPerson);

                string pdfFileName = string.Format(@"{0}\{1}_{2}_{3}.pdf",
                                                    reportCompanies[managementCompanyId].FullPdfSubfolders[loginPerson.PdfSubfolder],
                                                    loginPerson.ShortNameAlphaCharsOnly,
                                                    loginPerson.PersonKey,
                                                    loginPerson.PdfSubfolder.Substring(0, 1));

                reportExecutionWrapper.AddParameter("ShowLogo");
                string[] paramValues = new string[] { reportCompanies[managementCompanyId].ShowLogoByDefault.ToString() };

                reportExecutionWrapper.Run(ds, pdfFileName, paramValues);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error generating PDF letter.", ex);
            }
        }

        private DataSet buildDataSet(LoginPerson loginPerson)
        {
            if (string.IsNullOrEmpty(loginPerson.Login.UserName))
                throw new ApplicationException("User name is empty.");

            DataSet ds = new DataSet();

            ds.Tables.Add(EnumerableExtensions.Singleton(loginPerson)
                                              .Select(p => new
                                              {
                                                  p.AddressFirstLine,
                                                  p.AddressSecondLine,
                                                  p.Address.StreetAddressLine,
                                                  p.Address.CityAddressLine,
                                                  p.Address.CountryAddressLine,
                                                  PrintCount = 0
                                              })
                                              .ToDataTable("ContactHeaderInfo"));

            ds.Tables.Add(EnumerableExtensions.Singleton(loginPerson)
                                              .Select(p => new
                                              {
                                                  p.PersonType,
                                                  p.DearSirForm,
                                                  p.Email,
                                                  LoginUserName = p.Login.UserName,
                                                  p.AssetManager.CompanyName,
                                                  p.AccountNumbers
                                              })
                                              .ToDataTable("LetterLoginName"));

            return ds;
        }

        public DataSet BuildTestDataSet(IDalSession session, int contactId)
        {
            IContact contact = ContactMapper.GetContact(session, contactId);
            if (contact != null)
                return buildDataSet(new CustomerLoginPerson(contact));
            else
                throw new ArgumentException(string.Format("{0} is not a valid ContactID.", contactId));
        }

        ReportCompanyInfoCollection reportCompanies;
        ReportExecutionWrapper reportExecutionWrapper = new ReportExecutionWrapper();
    }
}
