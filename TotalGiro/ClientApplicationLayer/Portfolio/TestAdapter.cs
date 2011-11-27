using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.FundPositions;
using B4F.TotalGiro.Dal;

namespace B4F.TotalGiro.ClientApplicationLayer.Portfolio
{
    public static class TestAdapter
    {
        public static DataSet GetResults()
        {
            return new[] { new { Key = 0 } }.ToDataSet();
        }
    }
}
