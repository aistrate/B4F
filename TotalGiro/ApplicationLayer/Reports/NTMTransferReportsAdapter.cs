using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transfers;
using B4F.TotalGiro.Orders.Transfers.Reporting;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public class NTMTransferReportsAdapter
    {
        public static DataSet PortfolioTransferReport(IAccountTypeInternal account, DateTime positionDate,
            IPositionTransfer parentTransfer, IPositionTransferPortfolio beforePortfolio,
            IPositionTransferPortfolio afterPortfolio)
        {

            DataSet ds = new DataSet();
            IPositionTransferReportPortfolio returnValue = new PositionTransferReportPortfolio(account, 
                positionDate,
                parentTransfer, 
                beforePortfolio,
                afterPortfolio);
            
            
            
            return ds;


        }
    }
}
