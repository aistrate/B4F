using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Accounts;
using System.IO;
using System.Collections;
using B4F.TotalGiro.Communicator.KasBank;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice
{
    public static class MoneyTransferOrderOverviewAdapter
    {
        public static bool ExistingGLDSTDExists()
        {
            string exportFilePath = (string)(System.Configuration.ConfigurationManager.AppSettings.Get("GLDSTDFilePath"));
            string fileName = exportFilePath + @"GLDSTD.";

            return File.Exists(fileName);
        }

        public static DataSet GetMoneyTransferOrders(
            bool approved, int status, DateTime fromDate, DateTime toDate,
            decimal minAmountQty, decimal maxAmountQty, string accountNumber, string beneficiary,
            string description)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                Hashtable parameters = new Hashtable();

                parameters.Add("approved", approved);
                if (status != int.MinValue)
                    parameters.Add("statusId", status);
                if (Util.IsNotNullDate(fromDate))
                    parameters.Add("fromDate", fromDate);
                if (Util.IsNotNullDate(toDate))
                    parameters.Add("toDate", toDate);
                if (minAmountQty != 0)
                    parameters.Add("minAmountQty", minAmountQty);
                if (maxAmountQty != 0)
                    parameters.Add("maxAmountQty", maxAmountQty);
                if (!string.IsNullOrEmpty(accountNumber))
                    parameters.Add("accountNumber", Util.PrepareNamedParameterWithWildcard(accountNumber));
                if (!string.IsNullOrEmpty(beneficiary))
                    parameters.Add("beneficiary", Util.PrepareNamedParameterWithWildcard(beneficiary));
                if (!string.IsNullOrEmpty(description))
                    parameters.Add("description", Util.PrepareNamedParameterWithWildcard(description));

                List<IMoneyTransferOrder> list = session.GetTypedListByNamedQuery<IMoneyTransferOrder>(
                    "B4F.TotalGiro.BackOffice.Orders.GetMoneyTransferOrders",
                    parameters);

                //"Key, Reference, BenefBankAcctNr, NarBenef1, TransferDescription1, Amount.DisplayString, Amount, ProcessDate, TransfereeAccount.Number, Status, DisplayStatus, IsEditable"
                return list.Select(c => new
                {
                    c.Key,
                    c.Reference,
                    c.BenefBankAcctNr,
                    c.NarBenef1,
                    c.DisplayDescription,
                    Amount_DisplayString = c.Amount.DisplayString,
                    c.Amount,
                    c.ProcessDate,
                    TransfereeAccount_Key = c.TransfereeAccount != null ? c.TransfereeAccount.Key : 0,
                    TransfereeAccount_Number = c.TransfereeAccount != null ? c.TransfereeAccount.Number : "",
                    c.Status,
                    c.DisplayStatus,
                    c.IsEditable,
                    c.IsSendable,
                    c.IsApproveable
                })
                .ToDataSet();

            }        
        }

        public static DataSet GetMoneyTransferOrderStati()
        {
            DataSet ds = Util.GetDataSetFromEnum(typeof(MoneyTransferOrderStati));
            Utility.AddEmptyFirstRow(ds);
            return ds;
        }

        public static bool CreateGLDSTDFile(int[] orderIds, out string errorMessage)
        {
            try
            {
                errorMessage = "";

                if (orderIds == null || orderIds.Count() == 0)
                    throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IList<MoneyTransferOrder> ordersReadyForFile = MoneyTransferOrderMapper.GetMoneyTransferOrders(session, orderIds, true);
                    string exportPath = (string)(System.Configuration.ConfigurationManager.AppSettings.Get("GLDSTDFilePath"));
                    string kasMailID = (string)(System.Configuration.ConfigurationManager.AppSettings.Get("KasMailID"));
                    GLDSTDFile theFile = new GLDSTDFile(exportPath, kasMailID);

                    if (ordersReadyForFile != null && ordersReadyForFile.Count > 0)
                    {
                        foreach (IMoneyTransferOrder mto in ordersReadyForFile)
                        {
                            GLDSTD newRecord = new GLDSTD(mto);
                            theFile.Records.Add(newRecord);
                        }

                        if (theFile.WriteOutFile())
                            GLDSTDFileMapper.Update(session, theFile);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return true;
        }

        public static bool ApproveMoneyTransferOrders(int[] orderIds)
        {
            if (orderIds == null || orderIds.Count() == 0)
                throw new B4F.TotalGiro.ApplicationLayer.Common.GridviewNoSelectionException();

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IList<MoneyTransferOrder> orders = MoneyTransferOrderMapper.GetMoneyTransferOrders(session, orderIds, false);
                foreach (IMoneyTransferOrder mto in orders)
                {
                    if (mto.Approve())
                        MoneyTransferOrderMapper.Update(session, mto);
                }
            }
            return true;
        }

        public static bool UnApproveMoneyTransferOrder(int orderId)
        {
            bool succcess = false;
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IMoneyTransferOrder order = MoneyTransferOrderMapper.GetMoneyTransferOrder(session, orderId);
                if (order.UnApprove())
                    succcess = MoneyTransferOrderMapper.Update(session, order);
            }
            return succcess;
        }

        public static bool CancelMoneyTransferOrder(int orderId)
        {
            bool success = false;

            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IMoneyTransferOrder order = MoneyTransferOrderMapper.GetMoneyTransferOrder(session, orderId);
                if (order.Cancel())
                    success = MoneyTransferOrderMapper.Update(session, order);
            }
            return success;
        }

    }
}
