using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Instruments.Prices;
using System.Collections;
using B4F.TotalGiro.Communicator.ExtCustodians;
using B4F.TotalGiro.Communicator.ExtCustodians.Positions;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    public class ExternalCustodianPositionEntryAdapter
    {

        public static DataSet GetCustodians()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IList custodians = ExtCustodianMapper.GetExtCustodians(session);
            DataSet GetQuantityDs = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                custodians,
                "Key, Name");

            session.Close();

            return GetQuantityDs;
        }

        public static DataSet GetExtCustodianPositions(int custodianID, DateTime date)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ExtCustodian custodian = ExtCustodianMapper.GetExtCustodian(session, custodianID);

            IList positions = ExtCustodianMapper.GetExtCustodianPositions(session, custodian, date);
            //DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(positions,
            //    "Key, Size.DisplayString, Size.Underlying.DisplayName, Size.Underlying.DisplayIsin");

            IList<ITradeableInstrument> instruments = InstrumentMapper.GetTradeableInstruments(session);

            int i = 0;
            IList positionRowViews = new ExtCustodianPositionRowView[instruments.Count];
            foreach (ITradeableInstrument instrument in instruments)
            {
                ExtPosition pos = findPos(positions, instrument);
                decimal size = (pos != null ? pos.Size.Quantity : 0m);
                positionRowViews[i++] = new ExtCustodianPositionRowView(instrument, size);
            }

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                positionRowViews, "Key, Isin, InstrumentName, Size");

            
            session.Close();

            return ds;
        }

        private static ExtPosition findPos(IList positions, IInstrument instrument)
        {
            foreach (ExtPosition pos in positions)
            {
                if (pos.Size.Underlying.Equals(instrument))
                    return pos;
            }
            return null;
        }

        public static void UpdateExtPosition(DateTime date, decimal Size, int custodianID, int original_Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            ITradeableInstrument instrument = (ITradeableInstrument)InstrumentMapper.GetInstrument(session, original_Key);
            ExtCustodian custodian = ExtCustodianMapper.GetExtCustodian(session, custodianID);
            ExtPosition position = ExtCustodianMapper.GetExtCustodianPosition(session, custodian, instrument, date);

            InstrumentSize size = new InstrumentSize(Size, instrument);

            if (position == null)
                position = new ExtPosition(custodian, size, date);
            else
                position.Size = size;

            ExtCustodianMapper.InsertOrUpdate(session, position);
            
            session.Close();
        }
    }
}
