using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.Utils;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.Fees.FeeCalculations;
using B4F.TotalGiro.Stichting;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.ApplicationLayer.DataMaintenance.Commission;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Fees.FeeRules;
using B4F.TotalGiro.Accounts.ManagementPeriods;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance.Fee
{
    #region Helper Classes

    public class FeeCalcHelper
    {
        public FeeCalcHelper()
        {
            CalcID = int.MinValue;
            CurrencyID = int.MinValue;
            IsActive = true;
            Lines = new List<FeeCalcLineHelper>();
        }

        public FeeCalcHelper(IFeeCalc calc)
            : this()
        {
            if (calc != null)
            {
                CalcID = calc.Key;
                if (calc.FeeCurrency != null)
                    CurrencyID = calc.FeeCurrency.Key;
                CalcName = calc.Name;
                FeeType = calc.FeeType.Key;
                IsActive = calc.IsActive;
                if (calc.LatestVersion != null)
                {
                    FeeCalcType = calc.LatestVersion.FeeCalcType;
                    StartPeriod = calc.LatestVersion.StartPeriod;
                    EndPeriod = calc.LatestVersion.EndPeriod;
                    if (calc.LatestVersion.FixedSetup != null)
                        SetUp = calc.LatestVersion.FixedSetup.Quantity;

                    switch (calc.LatestVersion.FeeCalcType)
                    {
                        case FeeCalcTypes.Flat:
                            IFeeCalcVersionFlat flat = (IFeeCalcVersionFlat)calc.LatestVersion;
                            if (flat.MinValue != null)
                                MinValue = flat.MinValue.Quantity;
                            if (flat.MaxValue != null)
                                MaxValue = flat.MaxValue.Quantity;
                            foreach (IFeeCalcLine line in flat.FeeLines)
                                Lines.Add(new FeeCalcLineHelper(line));
                            break;
                        case FeeCalcTypes.Simple:
                            IFeeCalcVersionSimple simple = (IFeeCalcVersionSimple)calc.LatestVersion;
                            NoFees = simple.NoFees;
                            break;
                    }
                }
            }
        }

        public int CalcID { get; set; }
        public int CurrencyID { get; set; }
        public string CalcName { get; set; }
        public FeeTypes FeeType { get; set; }
        public bool IsActive { get; set; }
        public int StartPeriod { get; set; }
        public int EndPeriod { get; set; }
        public FeeCalcTypes FeeCalcType { get; set; }
        public bool NoFees { get; set; }
        public decimal SetUp { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public List<FeeCalcLineHelper> Lines { get; set; }

        public override bool Equals(object obj)
        {
            IFeeCalc calc = obj as IFeeCalc;
            if (calc == null)
                return base.Equals(obj);
            else
            {
                bool isEqual = false;
                if (calc.LatestVersion.FeeCalcType == this.FeeCalcType)
                {
                    if (this.SetUp == (calc.LatestVersion.FixedSetup != null ? calc.LatestVersion.FixedSetup.Quantity : 0M))
                    {
                        switch (calc.LatestVersion.FeeCalcType)
                        {
                            case FeeCalcTypes.Flat:
                                IFeeCalcVersionFlat flat = (IFeeCalcVersionFlat)calc.LatestVersion;
                                if (this.MinValue == (flat.MinValue != null ? flat.MinValue.Quantity : 0M) &&
                                    this.MaxValue == (flat.MaxValue != null ? flat.MaxValue.Quantity : 0M))
                                {
                                    IFeeCalcLineCollection lines = flat.FeeLines;
                                    if (lines.Count == this.Lines.Count)
                                    {
                                        isEqual = true;
                                        foreach (FeeCalcLineHelper line in this.Lines)
                                        {
                                            if (lines.Where(u =>
                                                line.LowerRange == (u.LowerRange != null ? u.LowerRange.Quantity : 0M) &&
                                                line.FeePercentage == u.FeePercentage &&
                                                line.StaticCharge == (u.StaticCharge != null ? u.StaticCharge.Quantity : 0M)).Count() != 1)
                                            {
                                                isEqual = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case FeeCalcTypes.Simple:
                                IFeeCalcVersionSimple simple = (IFeeCalcVersionSimple)calc.LatestVersion;
                                if (simple.NoFees = this.NoFees)
                                    isEqual = true;
                                break;
                        }
                    }
                }
                return isEqual;
            }
        }
    }

    public class FeeCalcLineHelper
    {
        public FeeCalcLineHelper() { }
        
        public FeeCalcLineHelper(IFeeCalcLine line)
        {
            LineID = line.Key;
            SerialNo = line.SerialNo;
            FeePercentage = line.FeePercentage;
            if (line.LowerRange != null)
                LowerRange = line.LowerRange.Quantity;
            if (line.StaticCharge != null)
                StaticCharge = line.StaticCharge.Quantity;
        }

        public int LineID { get; set; }
        public short SerialNo { get; set; }
        public decimal LowerRange { get; set; }
        public decimal StaticCharge { get; set; }
        public decimal FeePercentage { get; set; }
    }

    #endregion

    public static class FeeCalculationsOverviewAdapter
    {
        public static DataSet GetFeeCalculations(
            string calcName, FeeTypes feeType, 
            B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus activeStatus, 
            string propertyList)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();

            if (!string.IsNullOrEmpty(calcName))
                parameters.Add("calcName", Util.PrepareNamedParameterWithWildcard(calcName, MatchModes.Anywhere));
            if (feeType > 0)
                parameters.Add("feeType", feeType);
            if (activeStatus != B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.All)
                parameters.Add("isActive", (activeStatus == B4F.TotalGiro.ApplicationLayer.UC.AccountFinderAdapter.AccountGuiStatus.Active));
            IManagementCompany comp = LoginMapper.GetCurrentManagmentCompany(session);
            if (comp == null)
                throw new ApplicationException("Not good");
            else if (!comp.IsStichting)
                parameters.Add("managementCompanyID", comp.Key);

            List<IFeeCalc> calculations = session.GetTypedListByNamedQuery<IFeeCalc>(
                "B4F.TotalGiro.Fees.FeeCalculations.FeeCalculations",
                parameters);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                calculations,
                propertyList);

            session.Close();
            return ds;
        }

        public static FeeCalcHelper GetFeeCalculation(int calcID)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            FeeCalcHelper helper = null;

            IFeeCalc calc = FeeCalcMapper.GetFeeCalculation(session, calcID);
            if (calc != null)
                helper = new FeeCalcHelper(calc);

            session.Close();
            return helper;
        }

        public static DataSet GetFeeTypes()
        {
            return GetFeeTypes(0);
        }

        public static DataSet GetFeeTypes(int managementType)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();

            if (managementType > 0)
                parameters.Add("managementType", managementType);
            List<FeeType> versions = session.GetTypedListByNamedQuery<FeeType>(
                "B4F.TotalGiro.Fees.FeeTypes",
                parameters);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                versions,
                "Key, Name");

            Utility.AddEmptyFirstRow(ds.Tables[0]);

            session.Close();
            return ds;
        }

        public static DataSet GetFeeCalcVersions(int calcID, string propertyList)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            Hashtable parameters = new Hashtable();

            parameters.Add("calcID", calcID);
            List<IFeeCalcVersion> versions = session.GetTypedListByNamedQuery<IFeeCalcVersion>(
                "B4F.TotalGiro.Fees.FeeCalculations.FeeCalcVersions",
                parameters);

            ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                versions,
                propertyList);

            session.Close();
            return ds;
        }

        public static DataSet GetFeeCalcLines(int versionID, string propertyList)
        {
            DataSet ds = null;
            IDalSession session = NHSessionFactory.CreateSession();
            IFeeCalcVersion version = FeeCalcMapper.GetFeeCalcVersion(session, versionID);
            List<IFeeCalcLine> lines = null;
            if (version != null)
            {
                switch (version.FeeCalcType)
                {
                    case FeeCalcTypes.Flat:
                        IFeeCalcVersionFlat flat = (IFeeCalcVersionFlat)version;
                        lines = (from a in flat.FeeLines
                                select a).ToList();
                        ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            lines,
                            propertyList);
                        break;
                }
            }
            session.Close();
            return ds;
        }

        public static void CancelFeeCalculation(int endPeriod, int Key)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            IFeeCalc calc = FeeCalcMapper.GetFeeCalculation(session, Key);
            if (calc != null)
            {
                calc.IsActive = !(endPeriod > 0);
                calc.LatestVersion.EndPeriod = endPeriod;
                session.Update(calc);
            }
            session.Close();
        }

        public static bool UpdateCalcVersion(FeeCalcHelper helper)
        {
            if ((helper != null))
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ILogin login = LoginMapper.GetCurrentLogin(session);
                IFeeCalc calc;
                IFeeCalcVersion version = null;
                ICurrency currency = null;
                bool createNewVersion = true;

                if (helper.CalcID == 0)   //First Create the Model
                {
                    FeeType feeType = FeeRuleMapper.GetFeeType(session, helper.FeeType);
                    currency = InstrumentMapper.GetCurrency(session, helper.CurrencyID);
                    IAssetManager manager = (IAssetManager)LoginMapper.GetCurrentManagmentCompany(session);
                    calc = new FeeCalc(feeType, helper.CalcName, currency, manager);
                }
                else
                {
                    calc = FeeCalcMapper.GetFeeCalculation(session, helper.CalcID);
                    currency = calc.FeeCurrency;
                    if (helper.Equals(calc))
                        createNewVersion = false;
                    else if (calc.LatestVersion.StartPeriod == helper.StartPeriod)
                        createNewVersion = false;

                    // check if previous version exists with the same startPeriod
                    if (createNewVersion)
                    {
                        if (calc.Versions.Where(u => u.StartPeriod == helper.StartPeriod).Count() == 1)
                            createNewVersion = false;
                    }
                }

                calc.Name = helper.CalcName;
                calc.IsActive = helper.IsActive;

                if (createNewVersion)
                {
                    Money fixedSetup = new Money(helper.SetUp, currency);
                    switch (helper.FeeCalcType)
                    {
                        case FeeCalcTypes.Flat:
                            version = new FeeCalcVersionFlat(calc, fixedSetup, null, null, helper.StartPeriod, login.UserName);
                            break;
                        case FeeCalcTypes.Simple:
                            version = new FeeCalcVersionSimple(calc, fixedSetup, helper.StartPeriod, login.UserName);
                            break;
                    }
                    calc.Versions.Add(version);
                }
                else
                {
                    version = calc.Versions.Where(u => u.StartPeriod == helper.StartPeriod).FirstOrDefault();
                    if (version == null)
                        version = calc.LatestVersion;
                }

                if (version.Equals(calc.LatestVersion))
                    version.EndPeriod = helper.EndPeriod;
                switch (version.FeeCalcType)
                {
                    case FeeCalcTypes.Flat:
                        IFeeCalcVersionFlat flat = (IFeeCalcVersionFlat)version;
                        flat.MinValue = (helper.MinValue != 0 ? new Money(helper.MinValue, currency) : null);
                        flat.MaxValue = (helper.MaxValue != 0 ? new Money(helper.MaxValue, currency) : null);

                        if (helper.Lines == null || helper.Lines.Count == 0)
                            throw new ApplicationException("Staffel is missing.");

                        int counter = 0;
                        foreach (FeeCalcLineHelper line in helper.Lines.OrderBy(u => u.LowerRange))
                        {
                            IFeeCalcLine feeLine = flat.FeeLines.GetItemBySerialNo(counter);
                            if (feeLine == null)
                            {
                                feeLine = new FeeCalcLine(
                                    new Money(line.LowerRange, currency),
                                    (line.StaticCharge != 0 ? new Money(line.StaticCharge, currency) : null),
                                    line.FeePercentage);
                                flat.FeeLines.Add(feeLine);
                            }
                            else
                            {
                                feeLine.LowerRange = new Money(line.LowerRange, currency);
                                feeLine.FeePercentage = line.FeePercentage;
                                feeLine.StaticCharge = (line.StaticCharge != 0 ? new Money(line.StaticCharge, currency) : null);
                            }
                            counter++;
                        }

                        // get rid of removed lines
                        if (flat.FeeLines.Count > helper.Lines.Count)
                        {
                            for (int i = flat.FeeLines.Count; i > helper.Lines.Count; i--)
                            {
                                flat.FeeLines.RemoveAt(i - 1);
                            }
                        }
                        break;
                    case FeeCalcTypes.Simple:
                        IFeeCalcVersionSimple simple = (IFeeCalcVersionSimple)version;
                        simple.NoFees = helper.NoFees;
                        break;

                }

                session.InsertOrUpdate(calc);
                session.Close();
            }
            return true;
        }
    }
}
