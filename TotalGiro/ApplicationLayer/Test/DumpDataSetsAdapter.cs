using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Reports.Letters;
using B4F.TotalGiro.Reports.Documents;

namespace B4F.TotalGiro.ApplicationLayer.Test
{
    public static class DumpDataSetsAdapter
    {
        public static DataSet GetLetterDataSet(int personId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                LetterPrintCommand letterPrintCommand = new LetterPrintCommand(session, "LetterLoginName", "Letters");
                return letterPrintCommand.BuildTestDataSet(session, personId);
            }
            finally
            {
                session.Close();
            }
        }

        public static int CountDocumentsSentByPost(int accountId, DateTime startDate, DateTime endDate)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            try
            {
                return DocumentMapper.CountDocumentsSentByPost(session, accountId, startDate, endDate);
            }
            finally
            {
                session.Close();
            }
        }
    }
}
