using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios.CashPositions;
using B4F.TotalGiro.GeneralLedger.Journal;
using System.Collections;
using System.Data;

namespace B4F.TotalGiro.ApplicationLayer.Portfolio
{
    public static class CashDetailLineMapper
    {
        public static DataSet GetCashDetailLines()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            //IAccountTypeInternal acc = (IAccountTypeInternal)AccountMapper.GetAccount(session, 622);

            ICashSubPosition supPos = CashPositionMapper.GetSubPosition(session, 2);
            CashDetailLineCollection coll = GetCashDetailLines(session, supPos);

            try
            {
                IList<CashDetailLine> lines = coll.ToList();

                    DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList( 
                                    lines.ToList(),
                                    @"Key, GroupKey, GroupSubKey, TradeID, Type, Side, 
                                      Size, Fund, PriceDisplay, Transactiondate, CreationDate, CashComponent, 
                                      LedgerCode, AmountDisplay");

                    return ds;
}
            finally
            {
                session.Close();
            }
        }

        private static CashDetailLineCollection GetCashDetailLines(IDalSession session, ICashSubPosition position)
        {
            IAccountTypeInternal account = position.ParentPosition.Account;
            CashDetailLineCollection newColl = new CashDetailLineCollection(account);
            IList<IJournalEntryLine> lines = position.JournalLines.ToList();

            var grouping = from c in lines
                           group c by
                           new { groupKey = c.GroupingKey, TxDate = c.Parent.TransactionDate, IsTx = c.GroupingbyTransaction };


            foreach (var key in grouping)
            {
                bool IsTx = key.Key.IsTx;
                if (!IsTx)
                    newColl.AddcashLine(key.ElementAt(0));
                else
                    newColl.AddTradeLine(key.ToList());
            }


            return newColl;

        }

    }
}
