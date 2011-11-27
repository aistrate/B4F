using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Orders.Transfers;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Orders.Transfers.Reporting;

namespace B4F.TotalGiro.ApplicationLayer.Reports
{
    public static class OperationalReportAdapter
    {
        public static DataSet GetClientTransferReport(int positionTransferID, bool chooseAccountOut)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, positionTransferID);
                IAccountTypeInternal account = chooseAccountOut ? transfer.AccountA : transfer.AccountB;
                IPositionTransferPortfolio beforePortfolio = chooseAccountOut ? transfer.APortfolioBefore : transfer.BPortfolioBefore;
                IPositionTransferPortfolio afterPortfolio = chooseAccountOut ? transfer.APortfolioAfter : transfer.BPortfolioAfter;


                IPositionTransferReportPortfolio report = new PositionTransferReportPortfolio(account, transfer.TransferDate, transfer,
                    beforePortfolio, afterPortfolio);

                DataSet ds = report.Positions.Select(p =>
                    new
                    {
                        p.Key,
                        AccountDescription = p.AccountDescription,
                        AccountNumber = p.Account.Number,
                        AccountName = p.Account.ShortName,
                        TransferDate = p.ParentPortfolio.PositionDate,
                        p.PercentageOfPortfolioBeforeDisplayString,
                        p.PercentageOfPortfolioAfterDisplayString,
                        Instrument = p.InstrumentOfPosition.Name,
                        p.ActualPriceShortDisplayString,
                        p.Isin,
                        BeforeQuantity = p.BeforePositionSize.Quantity,
                        AfterQuantity = p.AfterPositionSize.Quantity,
                        p.AfterPositionSizeDisplayString,
                        p.ValueinEuroBefore,
                        p.IsChanged


                    }).ToDataSet();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Console.WriteLine(dr["AccountNumber"].ToString());
                }

                return ds;
                        

            }
        }

    }
}
