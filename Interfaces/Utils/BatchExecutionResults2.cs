using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.Interfaces.Util
{
    public class BatchExecutionResults2
    {
        public BatchExecutionResults2()
        {
            SuccessCount = 0;
            Errors = new List<Exception>();
        }

        public void MarkSuccess()
        {
            SuccessCount++;
        }

        public void MarkSuccess(int units)
        {
            SuccessCount += units;
        }

        public void MarkError(Exception ex)
        {
            Errors.Add(ex);
        }

        public int SuccessCount { get; private set; }

        public int ErrorCount { get { return Errors.Count; } }

        public List<Exception> Errors { get; private set; }
    }
}
