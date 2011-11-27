using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.BackOffice.Orders;
using B4F.TotalGiro.Utils;

namespace B4F.TotalGiro.ApplicationLayer.BackOffice
{
    public struct PredefinedBeneficiariesDetails
    {
        public int Key;
        public string SwiftAddress;
        public string BankAcctNr;
        public string NarBenef1;
        public string NarBenef2;
        public string NarBenef3;
        public string NarBenef4;
        public string Description1;
        public string Description2;
        public string Description3;
        public string Description4;
        public int CostIndicationKey;

    }
    public static class PredefinedBeneficiariesAdapter
    {
        public static DataSet GetPredefinedBeneficiaries()
        {
            IDalSession session = NHSessionFactory.CreateSession();

            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                                            PredefinedBeneficiaryMapper.GetPredefinedBeneficiaries(session),
                                            "Key, BenefBankAcctNr, NarBenef1, NarBenef2, Description1, Description2");

            session.Close();

            return ds;
        }

        //public static DataSet GetCostIndications()
        //{
        //    IndicationOfCosts[] costs = EnumExtensions.ToEnumArray<IndicationOfCosts>(

        //}


        public static PredefinedBeneficiariesDetails GetPredefinedBeneficiary(int benefKey)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            PredefinedBeneficiariesDetails pbd = new PredefinedBeneficiariesDetails();

            PredefinedBeneficiary pb = PredefinedBeneficiaryMapper.GetPredefinedBeneficiary(session, benefKey);
            if (pb != null)
            {
                pbd.Key = pb.Key;
                if (pb.SwiftAddress != null) pbd.SwiftAddress = pb.SwiftAddress;
                if (pb.BenefBankAcctNr != null) pbd.BankAcctNr = pb.BenefBankAcctNr;
                if (pb.NarBenef1 != null) pbd.NarBenef1 = pb.NarBenef1;
                if (pb.NarBenef2 != null) pbd.NarBenef2 = pb.NarBenef2;
                if (pb.NarBenef3 != null) pbd.NarBenef3 = pb.NarBenef3;
                if (pb.NarBenef4 != null) pbd.NarBenef4 = pb.NarBenef4;
                if (pb.Description1 != null) pbd.Description1 = pb.Description1;
                if (pb.Description2 != null) pbd.Description2 = pb.Description2;
                if (pb.Description3 != null) pbd.Description3 = pb.Description3;
                if (pb.Description4 != null) pbd.Description4 = pb.Description4;
                pbd.CostIndicationKey = (int)pb.CostIndication;
            }

            return pbd;
        }

        public static DataSet GetCostIndications()
        {
            return (Enum.GetValues(typeof(IndicationOfCosts))).Cast<IndicationOfCosts>()
                .Select(e => new
                {
                    Key = (int)e,
                    Description = e.ToString()
                })
                .ToDataSet();
        }

        public static void SavePredefinedBeneficiary(ref bool blnSaveSuccess, ref PredefinedBeneficiariesDetails pbd)
        {

            IDalSession session = NHSessionFactory.CreateSession();
            int predBenefKey = pbd.Key;

            try
            {
                PredefinedBeneficiary predBenef = null;

                if (predBenefKey != 0)
                {
                    predBenef = PredefinedBeneficiaryMapper.GetPredefinedBeneficiary(session, predBenefKey);
                }
                else
                {
                    predBenef = new PredefinedBeneficiary();
                }

                predBenef.SwiftAddress = pbd.SwiftAddress;
                predBenef.BenefBankAcctNr = pbd.BankAcctNr;
                predBenef.NarBenef1 = pbd.NarBenef1;
                predBenef.NarBenef2 = pbd.NarBenef2;
                predBenef.NarBenef3 = pbd.NarBenef3;
                predBenef.NarBenef4 = pbd.NarBenef4;

                predBenef.Description1 = pbd.Description1;
                predBenef.Description2 = pbd.Description2;
                predBenef.Description3 = pbd.Description3;
                predBenef.Description4 = pbd.Description4;
                predBenef.CostIndication = (IndicationOfCosts)pbd.CostIndicationKey;

                blnSaveSuccess = PredefinedBeneficiaryMapper.Update(session, predBenef);

            }
            finally
            {
                session.Close();
            }
        }
    }


}
