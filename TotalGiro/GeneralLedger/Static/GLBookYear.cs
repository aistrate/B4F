using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4F.TotalGiro.GeneralLedger.Static
{
    public class GLBookYear : IGLBookYear
    {
        public int Key { get; set; }
        public int BookYear { get; set; }

        public DateTime StartDate
        {
            get
            {
                return new DateTime( BookYear , 1, 1);
            }
        }

        public DateTime EndDate
        {
            get
            {
                return new DateTime(BookYear, 12, 31);
            }
        }
        
        
        
        
        
        public override string ToString()
        {
            return BookYear.ToString();
        }
    }
}
