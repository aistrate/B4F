using System;
using System.Collections.Generic;
using System.Text;

namespace B4F.TotalGiro.Instruments
{
    public class BenchMarkModel : ModelBase, IBenchMarkModel
    {
        public override ModelType ModelType
        {
            get
            {
                return ModelType.BenchMark;
            }
        }
    }
}
