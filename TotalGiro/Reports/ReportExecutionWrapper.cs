using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using B4F.TotalGiro.Reports.Properties;
using B4F.TotalGiro.Reports.ReportExecutionEngine;

namespace B4F.TotalGiro.Reports
{
    /// <summary>
    /// Usage:
    ///     ReportExecutionWrapper w = new ReportExecutionWrapper();
    ///     w.SetReportName(reportName);
    ///     w.AddParameters(...);       // optional
    ///     w.Run(...);
    /// </summary>
    public class ReportExecutionWrapper
    {
        private ReportExecutionService reportExecutionService = new ReportExecutionService();   // Web Service proxy
        private string reportName;
        private bool isInitialized = false;
        private List<string> extraParamNames = new List<string>();
        private ParameterValue[] parameters;

        public ReportExecutionWrapper()
        {
            string credentialString = Settings.Default.B4F_TotalGiro_Reports_ReportExecutionEngine_ReportExecutionService_Credentials;
            if (credentialString != string.Empty)
                reportExecutionService.Credentials = getNetworkCredential(credentialString);

            reportExecutionService.PreAuthenticate = true;
            reportExecutionService.ExecutionHeaderValue = new ExecutionHeader();
        }

        public List<string> ExtraParamNames
        {
            get { return extraParamNames.Select(name => name).ToList(); }
        }

        public void SetReportName(string reportName)
        {
            this.reportName = reportName;
            this.isInitialized = false;
            this.extraParamNames.Clear();
        }

        public void AddParameter(string name)
        {
            extraParamNames.Add(name);
        }

        public void AddParameters(string[] names)
        {
            foreach (string name in names)
                extraParamNames.Add(name);
        }

        public void Run(DataSet ds, string outputFile, string[] extraParams)
        {
            byte[] reportContent = GetReportContent(ds, extraParams);

            FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(reportContent);

            bw.Close();
            fs.Close();
        }

        public byte[] GetReportContent(DataSet ds, string[] extraParams)
        {
            if (extraParams.Length != extraParamNames.Count)
                throw new ArgumentException(string.Format(
                    "[ReportExecutionWrapper] Wrong number of extra parameters passed in to method Run() [expected {0}, was {1}].",
                    extraParamNames.Count, extraParams.Length));

            initialize();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            ds.WriteXml(sw, XmlWriteMode.WriteSchema);
            sw.Close();

            parameters[0].Value = sb.ToString();

            for (int i = 0; i < extraParams.Length; i++)
                parameters[i + 1].Value = extraParams[i];

            reportExecutionService.SetExecutionParameters(parameters, "nl-NL");

            string extension, mimeType, encoding;
            Warning[] warnings;
            string[] streamIds;
            byte[] reportContent = reportExecutionService.Render("PDF", "",
                                        out extension, out mimeType, out encoding, out warnings, out streamIds);
            return reportContent;
        }

        private void initialize()
        {
            if (!isInitialized)
            {
                if (reportName == null || reportName == string.Empty)
                    throw new ArgumentException("[ReportExecutionWrapper] No report name available in method Run().");

                ExecutionInfo execInfo = reportExecutionService.LoadReport(reportName, null);

                List<ParameterValue> parameterList = new List<ParameterValue>();
                parameterList.Add(new ParameterValue() { Name = "DataSource" });
                parameterList.AddRange(extraParamNames.Select(name => new ParameterValue() { Name = name }));
                parameters = parameterList.ToArray();

                isInitialized = true;
            }
        }

        private static NetworkCredential getNetworkCredential(string credentialString)
        {
            string userName = "", password = "", domain = "";

            string[] properties = credentialString.Split(';');
            foreach (string property in properties)
            {
                string[] pair = property.Split('=');
                if (pair.Length == 2)
                {
                    string propertyName = pair[0].Trim().ToLower();
                    string propertyValue = pair[1].Trim();
                    if (propertyName != string.Empty && propertyValue != string.Empty)
                    {
                        switch (propertyName)
                        {
                            case "username":
                                userName = propertyValue;
                                break;
                            case "password":
                                password = propertyValue;
                                break;
                            case "domain":
                                domain = propertyValue;
                                break;
                        }
                    }
                }
            }

            if (userName != string.Empty)
                return new NetworkCredential(userName, password, domain);
            else
                return null;
        }
    }
}
