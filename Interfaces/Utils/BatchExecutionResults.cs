using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace B4F.TotalGiro.Utils
{
    public class BatchExecutionResults
    {
        // Instance members

        public int MarkSuccess()
        {
            return ++successCount;
        }

        public int MarkSuccess(int units)
        {
            successCount += units;
            return successCount;
        }

        public int SuccessCount
        {
            get { return successCount; }
        }

        public int MarkError(Exception ex)
        {
            return exceptions.Add(ex);
        }

        public int ErrorCount
        {
            get { return exceptions.Count; }
        }

        public Exception[] Errors
        {
            get { return (Exception[])exceptions.ToArray(typeof(Exception)); }
        }

        public void MarkWarning(string warning)
        {
            warnings.Add(warning);
        }

        public int WarningCount
        {
            get { return warnings.Count; }
        }

        public string[] Warnings
        {
            get { return warnings.ToArray(); }
        }

        private int successCount = 0;
        ArrayList exceptions = new ArrayList();
        List<string> warnings = new List<string>();
    }
}
