using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Orders.Transfers;

namespace B4F.TotalGiro.ApplicationLayer.UC
{
    public class TransferPositionDetailsEditView
    {
        public TransferPositionDetailsEditView()
        {

        }

        public int Key { get; set; }
        public int ParentTransfer { get; set; }
        public decimal PositionSize { get; set; }
        public TransferDirection TxDirection { get; set; }
        public int Instrumentid { get; set; }
        public decimal ActualPriceQuantity { get; set; }
        public decimal TransferPriceQuantity { get; set; }

    }
}
