using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transfers;
using B4F.TotalGiro.Dal;
using System.Data;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;


namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public static class TransferPositionDetailsAdapter
    {
        public static DataSet GetPositionTransferDetails(int positionTransferID, bool isInsert)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, positionTransferID);
                IPositionTransferDetailCollection col = transfer.TransferDetails;
                if (isInsert) col.AddPosition(new PositionTransferDetail());
                var interim =  col.Select(p => new
                {
                    p.Key,
                    InstrumentDescription = (p.InstrumentOfPosition != null) ? p.InstrumentDescription : "",
                    p.Size,
                    p.TransferPriceShortDisplayString,
                    p.ActualPriceShortDisplayString,
                    PriceQuantity = p.TransferPrice != null ? p.TransferPrice.Quantity : 0m,
                    ValueinEuro = p.ValueinEuro.DisplayString,
                    ValueinEuroQty = (p.ValueinEuro != null ? p.ValueinEuro.Quantity : 0M),
                    ExchangeRate = (p.InstrumentOfPosition == null || ((IInstrumentsWithPrices)p.InstrumentOfPosition).CurrencyNominal.IsBase ? 1M : p.ExchangeRate),
                    p.IsEditable,
                    p.IsDeletable,
                    p.TxDirection,
                    InstrumentID = (p.InstrumentOfPosition != null) ? p.InstrumentOfPosition.Key : 0
                }).ToList();
                
                return interim.ToDataSet();
            }

        }

        public static void DeletePositionTransferDetail(int transferID, int transferDetailID)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, transferID);
                IPositionTransferDetail deletingLine = PositionTransferMapper.getTransferDetail(session, transferDetailID);

                if (deletingLine.IsDeletable)
                {
                    session.BeginTransaction();
                    deletingLine.ParentTransfer = null;
                    session.Delete(deletingLine);
                    session.CommitTransaction();
                }
                else
                    throw new ApplicationException(string.Format("Transfer Detail number {0} is not deletable.", deletingLine.Key));
            }
            finally
            {
                session.Close();
            }
        }

        public static decimal GetMaximumSize(TransferDirection txDirection, int instrumentKey, int transferID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IPositionTransferPortfolio port = null;

            try
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, transferID);

                if (txDirection == TransferDirection.FromAtoB)
                {
                    if (!transfer.AIsInternal)
                        return 0M;
                    port = transfer.APortfolioBefore;
                }
                else
                {
                    if (!transfer.BIsInternal)
                        return 0M;
                    port = transfer.BPortfolioBefore;
                }
                return port.Positions
                            .Where(p => p.InstrumentOfPosition.Key == instrumentKey)
                            .Select(i => i.PositionSize.Quantity).FirstOrDefault();

            }
            finally
            {
                session.Close();
            }

        }

        public static DataSet GetTxDirection()
        {
            return (Enum.GetValues(typeof(TransferDirection))).Cast<TransferDirection>()
                .Select(e => new
                {
                    Key = (int)e,
                    Description = e.ToString()
                })
                .ToDataSet();
        }


        public static bool UpdatePositionTransferDetails(TransferPositionDetailsEditView updateInput)
        {

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, updateInput.ParentTransfer);
                IPositionTransferDetail updatingLine = null;

                if (updateInput.Key == 0)
                    updatingLine = new PositionTransferDetail() { ParentTransfer = transfer };
                else
                    updatingLine = PositionTransferMapper.getTransferDetail(session, updateInput.Key);

                updatingLine = assignProperties(updateInput, updatingLine, session);

                return session.InsertOrUpdate(updatingLine);

            }

            finally
            {
                session.Close();
            }

        }

        private static IPositionTransferDetail assignProperties(TransferPositionDetailsEditView updateInput, IPositionTransferDetail updatingLine, IDalSession session)
        {

            IInstrument instrument = InstrumentMapper.GetInstrument(session, updateInput.Instrumentid);

            updatingLine.TxDirection = updateInput.TxDirection;

            updatingLine.PositionSize = new InstrumentSize(updateInput.PositionSize, instrument);
            
            updatingLine.ActualPrice = getPriceForInstrument(session, ((ITradeableInstrument)instrument), updatingLine.ParentTransfer.TransferDate);

            if((updateInput.TransferPriceQuantity != 0m))
                updatingLine.TransferPrice = new Price(updateInput.TransferPriceQuantity, ((ITradeableInstrument)instrument).CurrencyNominal, instrument);
            else
                updatingLine.TransferPrice = updatingLine.ActualPrice;


            updatingLine.ValueVV = updatingLine.PositionSize.CalculateAmount(updatingLine.TransferPrice);
            updatingLine.ValueinEuro = updatingLine.ValueVV.CurrentBaseAmount;

            return updatingLine;
        }

        private static Price getPriceForInstrument(IDalSession session, ITradeableInstrument instrument, DateTime priceDate)
        {
            return HistoricalPriceMapper.GetLastValidHistoricalPrice(session, instrument, priceDate).Price;

        }


        public static DataSet GetInstruments(int positionTransferID, TransferDirection txDirection, int instrumentID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet returnValue = new DataSet();
                IPositionTransfer transfer = PositionTransferMapper.getTransfer(session, positionTransferID);
                IPositionTransferDetailCollection details = transfer.TransferDetails;

                var detailInstruments = details.Where(d => d.InstrumentOfPosition.Key != instrumentID);

                if (transfer.AIsInternal && transfer.BIsInternal)
                {
                    IPositionTransferPortfolio port = (txDirection == TransferDirection.FromAtoB ? transfer.APortfolioBefore : transfer.BPortfolioBefore);

                    returnValue = port.Positions.Where(p => p.IsFundPosition)
                        .Select(p => new
                        {
                            Key = p.InstrumentOfPosition.Key,
                            Description = p.InstrumentDescription
                        })
                    .Except(detailInstruments.Select(d => new
                        {
                            Key = d.InstrumentOfPosition.Key,
                            Description = d.InstrumentDescription
                        }))
                        .OrderBy(o => o.Description)
                        .ToDataSet();
                }
                else
                {
                    returnValue = Instruments.InstrumentMapper.GetTradeableInstrumentsForDropDownList(session)
                    .Select(p => new
                    {
                        Key = p.Key,
                        Description = p.Value
                    })
                    .Except(detailInstruments.Select(d => new
                    {
                        Key = d.InstrumentOfPosition.Key,
                        Description = d.InstrumentDescription
                    }))
                    .OrderBy(o => o.Description)
                    .ToDataSet();

                }

                Utility.AddEmptyFirstRow(returnValue);
                return returnValue;
            }
        }



    }
}
