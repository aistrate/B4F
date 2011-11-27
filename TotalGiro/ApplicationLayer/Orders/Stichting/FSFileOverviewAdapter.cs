using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Communicator.FSInterface;
using B4F.TotalGiro.Orders;
using System.Collections;
using B4F.TotalGiro.Instruments;

namespace B4F.TotalGiro.ApplicationLayer.Orders.Stichting
{
    public static class FSFileOverviewAdapter
    {
        public static DataSet GetFSFileOverview()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                FSExportFileMapper.GetExportFiles(session),
                "Key, FilePath, FileName, Orders.Count, FileExt, CreationDate, FSNumber");
            session.Close();

            return ds;
        }


        public static DataSet GetFSFileOverviewByCriteria(DateTime p_datStartDate, DateTime p_datEndDate, int p_intFileID, string p_strFsNumber)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return FSExportFileMapper.GetExportFilesByCriteria(session,p_datStartDate, p_datEndDate, p_intFileID, p_strFsNumber)
                .Cast<IFSExportFile>()
                .Select(c => new
                {
                    c.Key,
                    c.FilePath,
                    c.FileName,
                    Orders_Count = c.Orders.Count,
                    c.FileExt,
                    c.CreationDate,
                    c.FSNumber
                })
                .ToDataSet();
            }
        }

        public static DataSet GetOrdersPerExportFile(int fileid)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                OrderMapper.GetOrdersPerExportFile(session, fileid),
                "TradedInstrument.DisplayName, DisplayTradedInstrumentIsin, Side, Value, Value.DisplayString, DisplayIsSizeBased, Status, DisplayStatus, Route, CreationDate, OrderID");
            session.Close();

            return ds;
        }

        public static DataSet GetSubtotalsPerCurrency(int fileid)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList orders = OrderMapper.GetOrdersPerExportFile(session, fileid);
            
            Hashtable currencySubtotals = new Hashtable();
            foreach (IOrder order in orders)
                if (!currencySubtotals.Contains(order.OrderCurrency.Key))
                    currencySubtotals[order.OrderCurrency.Key] = new CurrencySubtotalRowView(order.OrderCurrency);

            foreach (IOrder order in orders)
            {
                CurrencySubtotalRowView currencySubtotalRowView = (CurrencySubtotalRowView)currencySubtotals[order.OrderCurrency.Key];
                currencySubtotalRowView.OrderCount++;
                currencySubtotalRowView.Value += order.EstimatedAmount;
            }
            
            session.Close();

            CurrencySubtotalRowView[] currencySubtotalRowViews = new CurrencySubtotalRowView[currencySubtotals.Values.Count];
            currencySubtotals.Values.CopyTo(currencySubtotalRowViews, 0);

            return DataSetBuilder.CreateDataSetFromBusinessObjectList(currencySubtotalRowViews, "CurrencyName, OrderCount, Value");
        }

        public static void UpdateFSFileNumber(string FSNumber, int Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            FSExportFile exportfile = FSExportFileMapper.GetExportFile(session, Key);
            exportfile.FSNumber = FSNumber;
            FSExportFileMapper.Update(session, exportfile);

            session.Close();
        }

        public static void DeleteFSExportFile(int fileId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            FSExportFileMapper.Delete(session, fileId);
            session.Close();
        }

        public static void PlaceOrdersForFile(int fileId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList orders = OrderMapper.GetOrdersPerExportFile(session, fileId);

            foreach (IStgOrder order in orders)
            {
                order.Place();
            }

            OrderMapper.Update(session, orders);
            session.Close();
        }
    }
}
