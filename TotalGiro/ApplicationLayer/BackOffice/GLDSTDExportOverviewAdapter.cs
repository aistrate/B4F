using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B4F.TotalGiro.Dal;
//using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.Accounts;
using System.IO;
using System.Collections;
using B4F.TotalGiro.Communicator.KasBank;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice
{
    public static class GLDSTDExportOverviewAdapter
    {
        public static DataSet GetGLDSTDFileOverviewByCriteria(DateTime p_datStartDate, DateTime p_datEndDate, string p_strReference)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                // Key, FullFileName, Records.Count, CreationDate
                return GLDSTDFileMapper.GetExportFilesByCriteria(session, p_datStartDate, p_datEndDate, p_strReference)
                    .Cast<IGLDSTDFile>()
                    .Select(c => new
                    {
                        c.Key,
                        c.FullFileName, 
                        Records_Count = c.Records.Count, 
                        c.CreationDate
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetGLDSTDRecordsByFile(int theFileID)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                // Key, Reference, DebetAcctNr, NarBenef1, Amount, GroundForPayment1
                return GLDSTDMapper.GetRecordsByFile(session, theFileID)
                    .Cast<IGLDSTD>()
                    .Select(c => new
                    {
                        c.Key,
                        c.Reference,
                        c.DebetAcctNr,
                        c.NarBenef1,
                        Amount = (Convert.ToDecimal(c.Amount) / 100M).ToString("#.00"),
                        c.GroundForPayment1
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetGLDSTDRecord(int recordID)
        {
            DataSet ds = null;
            if (recordID > 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IGLDSTD record = GLDSTDMapper.GetRecord(session, recordID);
                    IList<IGLDSTD> list = new List<IGLDSTD> { record };
                    // Key, PriorityCode, Reference, CurrencyCode, NarDebet1, NarDebet2, NarDebet3, NarDebet4, DebetAcctNr, NarCorrespondentBank1, NarCorrespondentBank2, NarCorrespondentBank3, NarCorrespondentBank4, SwiftCorrespondentBank, NarBenefBank1, NarBenefBank2, NarBenefBank3, NarBenefBank4, SwiftBenefBank, BankBankAcctNr, NarBenef1, NarBenef2, NarBenef3, NarBenef4, BenefBankAcctNr, GroundForPayment1, GroundForPayment2, GroundForPayment3, GroundForPayment4, IndicationOfCosts, Amount, IndicationOfNonRes, NatureOfCP, ProcessDate, CircuitCode, TestKey, OptionsContract, TextOnForex, CodeOnForex, CountryCodeForex, CreationDate
                    ds = list.Select(c => new
                    {
                        c.Key,
                        c.PriorityCode,
                        c.Reference,
                        c.CurrencyCode,
                        c.NarDebet1,
                        c.NarDebet2,
                        c.NarDebet3,
                        c.NarDebet4,
                        c.DebetAcctNr,
                        c.NarCorrespondentBank1,
                        c.NarCorrespondentBank2,
                        c.NarCorrespondentBank3,
                        c.NarCorrespondentBank4,
                        c.SwiftCorrespondentBank,
                        c.NarBenefBank1,
                        c.NarBenefBank2,
                        c.NarBenefBank3,
                        c.NarBenefBank4,
                        c.SwiftBenefBank,
                        c.BankBankAcctNr,
                        c.NarBenef1,
                        c.NarBenef2,
                        c.NarBenef3,
                        c.NarBenef4,
                        c.BenefBankAcctNr,
                        c.GroundForPayment1,
                        c.GroundForPayment2,
                        c.GroundForPayment3,
                        c.GroundForPayment4,
                        IndicationOfCosts = c.IndicationOfCost,
                        c.Amount,
                        c.IndicationOfNonRes,
                        c.NatureOfCP,
                        c.ProcessDate,
                        c.CircuitCode,
                        c.TestKey,
                        c.OptionsContract,
                        c.TextOnForex,
                        c.CodeOnForex,
                        c.CountryCodeForex,
                        c.CreationDate
                    })
                    .ToDataSet();
                }
            }
            return ds;
        }
    }
}
