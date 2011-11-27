using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using B4F.TotalGiro.Accounts;
using B4F.TotalGiro.Accounts.Portfolios;
using B4F.TotalGiro.Dal;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments;
using B4F.TotalGiro.Routes;
using B4F.TotalGiro.Security;
using B4F.TotalGiro.StaticData;
using B4F.TotalGiro.Stichting.Login;
using B4F.TotalGiro.Utils;
using NHibernate.Linq;

namespace B4F.TotalGiro.ApplicationLayer.DataMaintenance
{
    #region helper classes

    public class SecurityDetails
    {
        public SecurityDetails()
        {
            HomeExchangeID = int.MinValue;
            DefaultExchangeID = int.MinValue;
            RouteID = int.MinValue;
            CountryID = int.MinValue;
            NominalCurrencyID = int.MinValue;
            DefaultCounterpartyID = int.MinValue;
            AccruedInterestCalcType = AccruedInterestCalcTypes.Undefined;
            IsCurrencyActive = true;
        }

        public int Key;
        public string InstrumentName, ISIN, CompanyName, ParentInstrumentName;
        public DateTime IssueDate, InActiveDate;
        public int HomeExchangeID, DefaultExchangeID, RouteID, CountryID, NominalCurrencyID, DecimalPlaces;
        public bool IsGreenFund, IsCultureFund, AllowNetting, IsActive, IsCurrencyActive;
        public SecCategories SecCategory;
        public PricingTypes PriceType;

        public short DefaultSettlementPeriod;
        public byte NumberOfDecimals;
        public bool CertificationRequired, DoesSupportAmountBasedBuy, DoesSupportAmountBasedSell, DoesSupportServiceCharge;
        public string RegisteredInNameof, DividendPolicy, CommissionRecipientName;
        public decimal ServiceChargePercentageBuy, ServiceChargePercentageSell, TickSize;
        public int DefaultCounterpartyID;

        // virtual fund
        public string TradingAccountNumber, HoldingAccountNumber;
        public string JournalNumber, JournalDescription, ExactJournalNumber;
        public decimal InitialNavPerUnit;

        // bond
        public decimal NominalValue, CouponRate, RedemptionAmount;
        public Regularities CouponFreq;
        public DateTime MaturityDate, FirstCouponPaymntDate;
        public AccruedInterestCalcTypes AccruedInterestCalcType;
        public bool IsPerpetual, IsFixedCouponRate, UltimoDating;
    }

    public class DerivativeMasterDetails
    {
        public DerivativeMasterDetails()
        {
            ExchangeID = int.MinValue;
            UnderlyingID = int.MinValue;
            NominalCurrencyID = int.MinValue;
        }

        public int Key;
        public string Name;
        public int ExchangeID, UnderlyingID, NominalCurrencyID, ContractSize, DecimalPlaces;
        public SecCategories SecCategory, UnderlyingSecCategory;
        public string Symbol;
    }

    public class TurboDetails
    {
        public TurboDetails()
        {
            DerivativeMasterID = int.MinValue;
        }

        public int Key;
        public string InstrumentName, ISIN;
        public int DerivativeMasterID;
        public decimal Leverage, FinanceLevel, StopLoss;
        public short Ratio;
        public IsLong Sign;
    }

    public class OptionDetails
    {
        public OptionDetails()
        {
            DerivativeMasterID = int.MinValue;
        }

        public int Key;
        public string InstrumentName, ISIN;
        public int DerivativeMasterID;
        public decimal StrikePrice;
        public DateTime ExpiryDate;
        public OptionTypes OptionType;

        public void SetExpiryDate(int period)
        {
            ExpiryDate = Util.GetSpecificDayInPeriod(period, DayOfWeek.Friday, 3);
        }
    }

    #endregion

    public static class InstrumentEditAdapter
    {
        public static SecurityDetails GetSecurityDetails(int instrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ISecurityInstrument sec = (ISecurityInstrument)InstrumentMapper.GetTradeableInstrument(session, instrumentId);
            SecurityDetails returnValue = new SecurityDetails();

            if (sec != null)
            {
                returnValue.Key = sec.Key;
                returnValue.SecCategory = sec.SecCategory.Key;
                returnValue.InstrumentName = sec.Name;
                returnValue.ISIN = sec.Isin;
                returnValue.CompanyName = sec.CompanyName;
                returnValue.IssueDate = sec.IssueDate;
                if (sec.HomeExchange != null)
                    returnValue.HomeExchangeID = sec.HomeExchange.Key;
                if (sec.DefaultExchange != null)
                    returnValue.DefaultExchangeID = sec.DefaultExchange.Key;
                if (sec.DefaultRoute != null)
                    returnValue.RouteID = sec.DefaultRoute.Key;
                if (sec.Country != null)
                    returnValue.CountryID = sec.Country.Key;
                if (sec.CurrencyNominal != null)
                {
                    returnValue.NominalCurrencyID = sec.CurrencyNominal.Key;
                    returnValue.IsCurrencyActive = sec.CurrencyNominal.IsActive;
                }
                returnValue.DecimalPlaces = sec.DecimalPlaces;
                returnValue.IsGreenFund = sec.IsGreenFund;
                returnValue.IsCultureFund = sec.IsCultureFund;
                returnValue.AllowNetting = sec.AllowNetting;
                returnValue.PriceType = sec.PriceType;
                if (sec.ParentInstrument != null)
                    returnValue.ParentInstrumentName = sec.ParentInstrument.DisplayNameWithIsin;
                returnValue.InActiveDate = sec.InActiveDate;
                returnValue.IsActive = sec.IsActive;

                IInstrumentExchange ie = sec.InstrumentExchanges.GetDefault() ?? sec.InstrumentExchanges.FirstOrDefault();
                if (ie != null)
                {
                    returnValue.NumberOfDecimals = ie.NumberOfDecimals;
                    returnValue.DefaultSettlementPeriod = ie.DefaultSettlementPeriod;
                    returnValue.CertificationRequired = ie.CertificationRequired;
                    returnValue.DoesSupportAmountBasedBuy = ie.DoesSupportAmountBasedBuy;
                    returnValue.DoesSupportAmountBasedSell = ie.DoesSupportAmountBasedSell;
                    returnValue.DoesSupportServiceCharge = ie.DoesSupportServiceCharge;
                    returnValue.RegisteredInNameof = ie.RegisteredInNameOf;
                    returnValue.DividendPolicy = ie.DividendPolicy;
                    returnValue.CommissionRecipientName = ie.CommissionRecipientName;
                    returnValue.ServiceChargePercentageBuy = ie.ServiceChargePercentageBuy;
                    returnValue.ServiceChargePercentageSell = ie.ServiceChargePercentageSell;
                    returnValue.TickSize = ie.TickSize;
                    if (ie.DefaultCounterParty != null)
                        returnValue.DefaultCounterpartyID = ie.DefaultCounterParty.Key;
                }

                switch (sec.SecCategory.Key)
                {
                    case SecCategories.Bond:
                        IBond bond = sec as IBond;
                        if (bond != null)
                        {
                            if (bond.NominalValue != null)
                                returnValue.NominalValue = bond.NominalValue.Quantity;
                            returnValue.CouponRate = bond.CouponRate;
                            returnValue.CouponFreq = bond.CouponFreq;
                            returnValue.MaturityDate = bond.MaturityDate;
                            returnValue.FirstCouponPaymntDate = bond.FirstCouponPaymntDate;
                            returnValue.AccruedInterestCalcType = bond.AccruedInterestCalcType;
                            if (bond.RedemptionAmount != null)
                                returnValue.RedemptionAmount = bond.RedemptionAmount.Quantity;
                            returnValue.IsPerpetual = bond.IsPerpetual;
                            returnValue.UltimoDating = bond.UltimoDating;
                            returnValue.IsFixedCouponRate = bond.IsFixedCouponRate;
                        }
                        break;
                    case SecCategories.VirtualFund:
                        IVirtualFund vf = sec as IVirtualFund;
                        if (vf != null)
                        {
                            if (vf.TradingAccount != null)
                                returnValue.TradingAccountNumber = vf.TradingAccount.Number;
                            if (vf.HoldingsAccount != null)
                                returnValue.HoldingAccountNumber = vf.HoldingsAccount.Number;
                            if (vf.InitialNavPerUnit != null && vf.InitialNavPerUnit.IsNotZero)
                                returnValue.InitialNavPerUnit = vf.InitialNavPerUnit.Quantity;
                            if (vf.JournalForFund != null)
                            {
                                returnValue.JournalNumber = vf.JournalForFund.JournalNumber;
                                returnValue.JournalDescription = vf.JournalForFund.BankAccountDescription;
                                //returnValue.ExactJournalNumber = vf.JournalForFund.ExactJournal;
                            }
                        }
                        break;
                }
            }
            session.Close();
            return returnValue;
        }

        public static DataSet GetStockDividend(int instrumentId, string propertyList)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = null;
                ISecurityInstrument sec = (ISecurityInstrument)InstrumentMapper.GetTradeableInstrument(session, instrumentId);
                if (sec != null && sec.CorporateActionInstruments != null && sec.CorporateActionInstruments.GetLatestStockDividend() != null)
                    ds = new IInstrumentCorporateAction[] { sec.CorporateActionInstruments.GetLatestStockDividend() }
                        .ToDataSet(propertyList);
                return ds;
            }
        }

        public static void UpdateStockDividend(string DisplayName, string DisplayIsin, int original_Key)
        {
            if (string.IsNullOrEmpty(DisplayName))
                throw new ApplicationException("Name is mandatory");
            if (string.IsNullOrEmpty(DisplayIsin))
                throw new ApplicationException("Isin is mandatory");
            
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IStockDividend sec = (IStockDividend)InstrumentMapper.GetInstrument(session, original_Key);
                if (sec != null)
                {
                    sec.Name = DisplayName;
                    sec.Isin = DisplayIsin;
                    session.Update(sec);
                }
                else
                    throw new ApplicationException("Instrument could not be found");
            }

        }

        public static void SaveSecurity(ref int instrumentID, ref bool blnSaveSuccess, SecurityDetails secDetails)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                throw new System.Security.SecurityException("You are not authorized to update instrument details.");

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                SecurityInstrument sec = null;
                if (instrumentID != 0)
                    sec = (SecurityInstrument)InstrumentMapper.GetTradeableInstrument(session, instrumentID);
                else
                {
                    switch (secDetails.SecCategory)
                    {
                        case SecCategories.Stock:
                            sec = new Stock();
                            break;
                        case SecCategories.Bond:
                            sec = new Bond();
                            break;
                        case SecCategories.MutualFund:
                            sec = new MutualFund();
                            break;
                        case SecCategories.VirtualFund:
                            IVirtualFundTradingAccount tradingAcc = new VirtualFundTradingAccount();
                            tradingAcc.ShortName = secDetails.InstrumentName + " Trading Account";
                            if (string.IsNullOrEmpty(secDetails.TradingAccountNumber))
                                tradingAcc.Number = secDetails.InstrumentName + " TA";
                            else
                                tradingAcc.Number = secDetails.TradingAccountNumber;

                            IVirtualFundHoldingsAccount holdingAcc = new VirtualFundHoldingsAccount();
                            holdingAcc.ShortName = secDetails.InstrumentName + " Holding Account";
                            if (string.IsNullOrEmpty(secDetails.HoldingAccountNumber))
                                holdingAcc.Number = secDetails.InstrumentName + " HA";
                            else
                                holdingAcc.Number = secDetails.HoldingAccountNumber;

                            IJournal journal = new Journal(JournalTypes.Memorial,
                                secDetails.JournalNumber,
                                LoginMapper.GetCurrentManagmentCompany(session),
                                InstrumentMapper.GetCurrency(session, secDetails.NominalCurrencyID));
                            sec = new VirtualFund(holdingAcc, tradingAcc, journal);
                            break;
                        case SecCategories.CashManagementFund:
                            sec = new CashManagementFund();
                            break;
                        default:
                            throw new ApplicationException("This seccategory is not supported.");
                    }
                }
                if (sec == null)
                    throw new ApplicationException("The instrument could not be found.");

                if (secDetails.DefaultExchangeID != int.MinValue)
                    sec.DefaultExchange = ExchangeMapper.GetExchange(session, secDetails.DefaultExchangeID);
                else
                    throw new ApplicationException("The default exchange is mandatory.");

                sec.Name = secDetails.InstrumentName;
                sec.Isin = secDetails.ISIN;
                sec.CompanyName = secDetails.CompanyName;
                sec.IssueDate = secDetails.IssueDate;
                sec.PriceType = secDetails.PriceType;
                sec.IsGreenFund = secDetails.IsGreenFund;
                sec.IsCultureFund = secDetails.IsCultureFund;
                sec.AllowNetting = secDetails.AllowNetting;
                sec.DecimalPlaces = secDetails.DecimalPlaces;

                if (secDetails.HomeExchangeID != int.MinValue)
                    sec.HomeExchange = ExchangeMapper.GetExchange(session, secDetails.HomeExchangeID);
                else
                    sec.HomeExchange = null;
                if (secDetails.RouteID != int.MinValue)
                    sec.DefaultRoute = RouteMapper.GetRoute(session, secDetails.RouteID);
                else
                    sec.DefaultRoute = null;
                if (secDetails.CountryID != int.MinValue)
                    sec.Country = CountryMapper.GetCountry(session, secDetails.CountryID);
                else
                    sec.Country = null;
                if (secDetails.NominalCurrencyID != int.MinValue)
                    sec.CurrencyNominal = InstrumentMapper.GetCurrency(session, secDetails.NominalCurrencyID);
                else
                    sec.CurrencyNominal = null;

                // sec category specific stuff
                switch (secDetails.SecCategory)
                {
                    case SecCategories.Bond:
                        IBond bond = (IBond)sec;
                        bond.AccruedInterestCalcType = secDetails.AccruedInterestCalcType;
                        bond.NominalValue = new Money(secDetails.NominalValue, bond.CurrencyNominal);
                        bond.CouponRate = secDetails.CouponRate;
                        if ((int)secDetails.CouponFreq >= (int)Regularities.Annual)
                            bond.CouponFreq = secDetails.CouponFreq;
                        else
                            bond.CouponFreq = 0;
                        bond.MaturityDate = secDetails.MaturityDate;
                        bond.UltimoDating = secDetails.UltimoDating;
                        bond.FirstCouponPaymntDate = secDetails.FirstCouponPaymntDate;

                        if (secDetails.RedemptionAmount != 0)
                            bond.RedemptionAmount = new Money(secDetails.RedemptionAmount, bond.CurrencyNominal);
                        else
                            bond.RedemptionAmount = null;
                        bond.IsPerpetual = secDetails.IsPerpetual;
                        bond.IsFixedCouponRate = secDetails.IsFixedCouponRate;
                        break;
                    case SecCategories.VirtualFund:
                        IVirtualFundTradingAccount tradingAcc = ((IVirtualFund)sec).TradingAccount;
                        if (tradingAcc != null)
                        {
                            if (!string.IsNullOrEmpty(secDetails.TradingAccountNumber))
                                tradingAcc.Number = secDetails.TradingAccountNumber;
                        }
                        IVirtualFundHoldingsAccount holdingAcc = ((IVirtualFund)sec).HoldingsAccount;
                        if (holdingAcc != null)
                        {
                            if (!string.IsNullOrEmpty(secDetails.HoldingAccountNumber))
                                holdingAcc.Number = secDetails.HoldingAccountNumber;
                        }
                        IJournal journal = ((IVirtualFund)sec).JournalForFund;
                        if (journal != null)
                        {
                            journal.BankAccountDescription = secDetails.JournalDescription;
                            //journal.ExactJournal = secDetails.ExactJournalNumber;
                        }
                        ((IVirtualFund)sec).InitialNavPerUnit = new Money(secDetails.InitialNavPerUnit, sec.CurrencyNominal);
                        break;
                }

                // instrument exchange
                IInstrumentExchange ie = sec.InstrumentExchanges.GetDefault() ?? sec.InstrumentExchanges.FirstOrDefault();
                if (ie == null)
                {
                    ie = new InstrumentExchange(sec, sec.DefaultExchange, secDetails.NumberOfDecimals);
                    sec.InstrumentExchanges.Add(ie);
                }
                else
                {
                    ie.Exchange = sec.DefaultExchange;
                    ie.NumberOfDecimals = secDetails.NumberOfDecimals;
                }

                if (secDetails.DefaultCounterpartyID != int.MinValue)
                    ie.DefaultCounterParty = (ICounterPartyAccount)AccountMapper.GetAccount(session, secDetails.DefaultCounterpartyID);
                else
                    ie.DefaultCounterParty = null;
                ie.DefaultSettlementPeriod = secDetails.DefaultSettlementPeriod;
                ie.TickSize = secDetails.TickSize;
                ie.DoesSupportAmountBasedBuy = secDetails.DoesSupportAmountBasedBuy;
                ie.DoesSupportAmountBasedSell = secDetails.DoesSupportAmountBasedSell;
                ie.DoesSupportServiceCharge = secDetails.DoesSupportServiceCharge;
                ie.ServiceChargePercentageBuy = secDetails.ServiceChargePercentageBuy;
                ie.ServiceChargePercentageSell = secDetails.ServiceChargePercentageSell;
                ie.RegisteredInNameOf = secDetails.RegisteredInNameof;
                ie.DividendPolicy = secDetails.DividendPolicy;
                ie.CommissionRecipientName = secDetails.CommissionRecipientName;
                ie.CertificationRequired = secDetails.CertificationRequired;

                // set Activity
                if (sec.Key != 0)
                {
                    if ((sec.IsActive && !secDetails.IsActive) || (Util.IsNullDate(sec.InActiveDate) && Util.IsNotNullDate(secDetails.InActiveDate)))
                    {
                        int activePositions = GetNumberActivePositionsForInstrument(session, sec.Key);
                        if (activePositions > 0)
                            throw new ApplicationException(string.Format("There are still {0} open positions in {1}, close these positions before the instrument can be inactivated.", activePositions, sec.DisplayNameWithIsin));
                    }

                    sec.IsActive = secDetails.IsActive;
                    sec.InActiveDate = secDetails.InActiveDate;
                }

                if (sec.Validate())
                {
                    blnSaveSuccess = InstrumentMapper.Update(session, sec);
                    instrumentID = sec.Key;
                }
            }
            finally
            {
                session.Close();
            }
        }

        public static int GetNumberActivePositionsForInstrument(IDalSession session, int instrumentId)
        {
            int count = 0;
            Hashtable parameters = new Hashtable();
            parameters.Add("instrumentId", instrumentId);
            parameters.Add("accountTypeId", AccountTypes.Customer);
            IList<long> list = session.GetTypedListByNamedQuery<long>(
                "B4F.TotalGiro.Instruments.Instrument.GetNumberActivePositionsForInstrument",
                parameters);
            if (list != null && list.Count > 0)
                count = (int)list[0];
            return count;
        }


        public static DerivativeMasterDetails GetDerivativeMasterDetails(int derivativeMasterId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IDerivativeMaster master = session.GetTypedList<DerivativeMaster, IDerivativeMaster>(derivativeMasterId).FirstOrDefault();
            DerivativeMasterDetails returnValue = new DerivativeMasterDetails();

            if (master != null)
            {
                returnValue.Key = master.Key;
                returnValue.SecCategory = master.SecCategory;
                returnValue.Name = master.Name;
                returnValue.UnderlyingSecCategory = master.UnderlyingSecCategory;
                returnValue.ContractSize = master.ContractSize;
                returnValue.DecimalPlaces = master.DecimalPlaces;
                if (master.Exchange != null)
                    returnValue.ExchangeID = master.Exchange.Key;
                if (master.Underlying != null)
                    returnValue.UnderlyingID = master.Underlying.Key;
                if (master.CurrencyNominal != null)
                    returnValue.NominalCurrencyID = master.CurrencyNominal.Key;
                returnValue.Symbol = master.DerivativeSymbol;
            }
            session.Close();
            return returnValue;
        }

        public static void SaveDerivativeMaster(ref int derivativeMasterId, ref bool blnSaveSuccess, DerivativeMasterDetails masterDetails)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                throw new System.Security.SecurityException("You are not authorized to update instrument details.");

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                DerivativeMaster master = null;
                IExchange exchange = null;
                if (masterDetails.ExchangeID != int.MinValue)
                    exchange = ExchangeMapper.GetExchange(session, masterDetails.ExchangeID);
                else
                    throw new ApplicationException("The default exchange is mandatory.");


                if (derivativeMasterId != 0)
                {
                    master = (DerivativeMaster)session.GetTypedList<DerivativeMaster, IDerivativeMaster>(derivativeMasterId).FirstOrDefault();
                    if (master == null)
                        throw new ApplicationException("The derivative master could not be found.");
                    master.Name = masterDetails.Name;
                    master.Exchange = exchange;
                }
                else
                    master = new DerivativeMaster(masterDetails.Name, masterDetails.SecCategory, exchange);

                master.ContractSize = masterDetails.ContractSize;
                master.DecimalPlaces = Convert.ToInt16(masterDetails.DecimalPlaces);
                master.UnderlyingSecCategory = masterDetails.UnderlyingSecCategory;
                master.DerivativeSymbol = masterDetails.Symbol;

                if (masterDetails.UnderlyingID != int.MinValue)
                    master.Underlying = InstrumentMapper.GetTradeableInstrument(session, masterDetails.UnderlyingID);
                else
                    master.Underlying = null;
                if (masterDetails.NominalCurrencyID != int.MinValue)
                    master.CurrencyNominal = InstrumentMapper.GetCurrency(session, masterDetails.NominalCurrencyID);
                else
                    master.CurrencyNominal = null;

                blnSaveSuccess = session.InsertOrUpdate(master);
                derivativeMasterId = master.Key;
            }
            finally
            {
                session.Close();
            }
        }

        public static TurboDetails GetTurboDetails(int instrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            ITurbo turbo = (ITurbo)InstrumentMapper.GetTradeableInstrument(session, instrumentId);
            TurboDetails returnValue = new TurboDetails();

            if (turbo != null)
            {
                returnValue.Key = turbo.Key;
                returnValue.InstrumentName = turbo.Name;
                returnValue.ISIN = turbo.Isin;
                returnValue.DerivativeMasterID = turbo.Master.Key;
                returnValue.Sign = turbo.Sign;

                if (turbo.StopLoss != null && turbo.StopLoss.IsNotZero)
                    returnValue.StopLoss = turbo.StopLoss.Quantity;
                returnValue.Leverage = turbo.Leverage;
                returnValue.FinanceLevel = turbo.FinanceLevel;
                returnValue.Ratio = turbo.Ratio;
            }
            session.Close();
            return returnValue;
        }

        public static void SaveTurbo(ref int instrumentID, ref bool blnSaveSuccess, TurboDetails turboDetails)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                throw new System.Security.SecurityException("You are not authorized to update instrument details.");

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                Turbo turbo = null;
                if (instrumentID != 0)
                    turbo = (Turbo)InstrumentMapper.GetTradeableInstrument(session, instrumentID);
                else
                {
                    IDerivativeMaster master = session.GetTypedList<DerivativeMaster, IDerivativeMaster>(turboDetails.DerivativeMasterID).FirstOrDefault();
                    if (master == null)
                        throw new ApplicationException("The derivative master is mandatory.");
                    turbo = new Turbo(master);
                }

                if (turbo == null)
                    throw new ApplicationException("The instrument could not be found.");

                if (turbo.Master == null)
                    throw new ApplicationException("The turbo needs a master.");

                if (instrumentID == 0 && turbo.Master.Series != null && turbo.Master.Series.Count > 0)
                {
                    if ((from a in turbo.Master.Series.Cast<ITurbo>()
                         where a.StopLoss.Quantity == turboDetails.StopLoss
                         && a.Sign == turboDetails.Sign
                         select a).Count() > 0)
                        throw new ApplicationException("This turbo already exists");
                }

                turbo.Name = turboDetails.InstrumentName;
                turbo.Isin = turboDetails.ISIN;
                turbo.Sign = turboDetails.Sign;
                turbo.StopLoss = new Price(turboDetails.StopLoss, turbo.CurrencyNominal, turbo);
                turbo.Leverage = turboDetails.Leverage;
                turbo.FinanceLevel = turboDetails.FinanceLevel;
                turbo.Ratio = turboDetails.Ratio;

                if (turbo.Validate())
                {
                    blnSaveSuccess = InstrumentMapper.Update(session, turbo);
                    instrumentID = turbo.Key;
                }
            }
            finally
            {
                session.Close();
            }
        }

        public static OptionDetails GetOptionDetails(int instrumentId)
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IOption opt = (IOption)InstrumentMapper.GetTradeableInstrument(session, instrumentId);
            OptionDetails returnValue = new OptionDetails();

            if (opt != null)
            {
                returnValue.Key = opt.Key;
                returnValue.InstrumentName = opt.Name;
                returnValue.ISIN = opt.Isin;
                returnValue.DerivativeMasterID = opt.Master.Key;
                returnValue.OptionType = opt.OptionType;
                returnValue.ExpiryDate = opt.ExpiryDate;
                if (opt.StrikePrice != null && opt.StrikePrice.IsNotZero)
                    returnValue.StrikePrice = opt.StrikePrice.Quantity;
            }
            session.Close();
            return returnValue;
        }

        public static void SaveOption(ref int instrumentID, ref bool blnSaveSuccess, OptionDetails optionDetails)
        {
            if (!SecurityManager.IsCurrentUserInRole("Data Mtce: Instrument Edit"))
                throw new System.Security.SecurityException("You are not authorized to update instrument details.");

            IDalSession session = NHSessionFactory.CreateSession();

            try
            {
                Option option = null;
                if (instrumentID != 0)
                    option = (Option)InstrumentMapper.GetTradeableInstrument(session, instrumentID);
                else
                {
                    IDerivativeMaster master = session.GetTypedList<DerivativeMaster, IDerivativeMaster>(optionDetails.DerivativeMasterID).FirstOrDefault();
                    if (master == null)
                        throw new ApplicationException("The derivative master is mandatory.");
                    option = new Option(master);
                }

                if (option == null)
                    throw new ApplicationException("The instrument could not be found.");

                if (option.Master == null)
                    throw new ApplicationException("The turbo needs a master.");

                if (instrumentID == 0 && option.Master.Series != null && option.Master.Series.Count > 0)
                {
                    if ((from a in option.Master.Series.Cast<IOption>()
                         where a.StrikePrice.Quantity == optionDetails.StrikePrice
                         && a.OptionType == optionDetails.OptionType
                         && a.ExpiryDate.Year == optionDetails.ExpiryDate.Year
                         && a.ExpiryDate.Month == optionDetails.ExpiryDate.Month
                         select a).Count() > 0)
                        throw new ApplicationException("This turbo already exists");
                }

                option.OptionType = optionDetails.OptionType;
                option.StrikePrice = new Price(optionDetails.StrikePrice, option.CurrencyNominal, option);
                option.ExpiryDate = optionDetails.ExpiryDate;
                option.Isin = optionDetails.ISIN;
                if (!string.IsNullOrEmpty(optionDetails.InstrumentName))
                    option.Name = optionDetails.InstrumentName;
                else
                {
                    CultureInfo american = new CultureInfo("en-US");

                    option.Name = string.Format("{0} {1} {2} {3}",
                        option.Master.DerivativeSymbol,
                        (option.OptionType == OptionTypes.Call ? "C" : "P"),
                        option.ExpiryDate.ToString("MMM yyyy", american).ToUpper(),
                        option.StrikePrice.Quantity.ToString("F", american));
                }

                if (option.Validate())
                {
                    blnSaveSuccess = InstrumentMapper.Update(session, option);
                    instrumentID = option.Key;
                }
            }
            finally
            {
                session.Close();
            }
        }

        public static DataSet GetDerivativeMasters(
            SecCategoryFilterOptions secCategoryFilter, string isin, string instrumentName,
            SecCategories secCategoryId, int exchangeId, int currencyNominalId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {

                Hashtable parameters = new Hashtable();

                if (secCategoryFilter != SecCategoryFilterOptions.All)
                    parameters.Add("secCategoryType", (int)secCategoryFilter);
                if (!string.IsNullOrEmpty(isin))
                    parameters.Add("isin", Util.PrepareNamedParameterWithWildcard(isin, MatchModes.Anywhere));
                if (!string.IsNullOrEmpty(instrumentName))
                    parameters.Add("instrumentName", Util.PrepareNamedParameterWithWildcard(instrumentName, MatchModes.Anywhere));
                if (secCategoryId != SecCategories.Undefined)
                    parameters.Add("secCategoryId", secCategoryId);
                if (exchangeId > 0)
                    parameters.Add("exchangeId", exchangeId);
                if (currencyNominalId > 0)
                    parameters.Add("currencyNominalId", currencyNominalId);
                IList<IDerivativeMaster> list = session.GetTypedListByNamedQuery<IDerivativeMaster>(
                    "B4F.TotalGiro.Instruments.Instrument.DerivativeMasters",
                    parameters);

                return list.Select(l => new
                                {
                                    l.Key,
                                    UnderlyingName = l.Underlying.Name,
                                    UnderlyingIsin = l.Underlying.Isin,
                                    l.Name,
                                    l.SecCategory,
                                    ExchangeName = l.Exchange.ExchangeName,
                                    NominalCurrency = l.CurrencyNominal.DisplayName,
                                    Symbol = l.DerivativeSymbol
                                }).ToDataSet();

            };
        }

        public static DataSet GetDerivativeSeries(int derivativeMasterId, string propertyList)
        {
            IDalSession session = NHSessionFactory.CreateSession();

            Hashtable parameters = new Hashtable();

            parameters.Add("derivativeMasterId", derivativeMasterId);
            IList<IDerivative> list = session.GetTypedListByNamedQuery<IDerivative>(
                "B4F.TotalGiro.Instruments.Instrument.Derivatives",
                parameters);


            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(
                            list.ToList(),
                            propertyList);
            session.Close();
            return ds;
        }

        public static DataSet GetRoutes()
        {
            IDalSession session = NHSessionFactory.CreateSession();
            IList routes = RouteMapper.GetRoutes(session);
            DataSet ds = DataSetBuilder.CreateDataSetFromBusinessObjectList(routes, "Key, Name");
            Utility.AddEmptyFirstRow(ds.Tables[0]);

            session.Close();
            return ds;
        }

        public static DataSet GetSecurities(SecCategories secCategoryId)
        {
            DataSet ds = null;
            if ((int)secCategoryId != int.MinValue && secCategoryId != SecCategories.Undefined)
            {
                IDalSession session = NHSessionFactory.CreateSession();
                ds = InstrumentMapper.GetInstruments(session, secCategoryId)
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayName
                    }).OrderBy(o => o.DisplayName)
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                session.Close();
            }
            return ds;
        }

        public static DataSet GetCounterParties()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = AccountMapper.GetAccounts<ICounterPartyAccount>(session, AccountTypes.Counterparty)
                    .Select(c => new
                    {
                        c.Key,
                        c.DisplayNumberWithName
                    })
                    .ToDataSet();
                Utility.AddEmptyFirstRow(ds.Tables[0]);
                return ds;
            }
        }

        public static DataSet GetAllSecCategories()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                DataSet ds = SecCategoryMapper.GetSecCategories(session, SecCategoryFilterOptions.All, true)
                    .Select(c => new
                    {
                        c.Key,
                        c.Description
                    })
                    .ToDataSet();

                Utility.AddEmptyFirstRow(ds);
                return ds;
            }

        }

        public static DataSet GetCurrencies(bool onlyActive)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                return InstrumentMapper.GetCurrencies(session, onlyActive)
                    .Select(c => new
                    {
                        c.Key,
                        c.Symbol
                    })
                    .ToDataSet();
            }
        }

        public static DataSet GetPricingTypes()
        {
            return Util.GetDataSetFromEnum(typeof(PricingTypes));
        }

        public static DataSet GetSignOptions()
        {
            return Util.GetDataSetFromEnum(typeof(B4F.TotalGiro.Accounts.Portfolios.IsLong));
        }

        public static DataSet GetOptionTypes()
        {
            return Util.GetDataSetFromEnum(typeof(OptionTypes));
        }

        public static DataSet GetAccruedInterestCalcTypes()
        {
            DataSet ds = Util.GetDataSetFromEnum(typeof(AccruedInterestCalcTypes));
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetBondCouponFrequencies()
        {
            DataSet ds = Util.GetDataSetFromEnum(typeof(Regularities));
            Utility.AddEmptyFirstRow(ds.Tables[0]);
            return ds;
        }

        public static DataSet GetBondCouponRateLines(int instrumentId, bool isInsert)
        {
            DataSet ds = null;
            if (instrumentId != 0)
            {
                using (IDalSession session = NHSessionFactory.CreateSession())
                {
                    IBond instrument = (IBond)InstrumentMapper.GetInstrument(session, instrumentId);
                    IBondCouponRateHistoryCollection lines = instrument.CouponRates;

                    if (isInsert)
                    {
                        DateTime startDate = instrument.IssueDate;
                        IBondCouponRateHistory latest = instrument.CouponRates.LastCouponRate;
                        if (latest != null)
                        {
                            if (Util.IsNotNullDate(latest.EndDate))
                                startDate = latest.EndDate.AddDays(1);
                            else
                                startDate = instrument.NextCouponDate(latest.StartDate.AddDays(1));
                        }

                        lines.Add(new BondCouponRateHistory(startDate));
                    }

                    ds = lines
                        .Select(c => new
                        {
                            c.Key,
                            c.StartDate,
                            c.EndDate,
                            c.CouponRate,
                            c.CreationDate,
                            c.CreatedBy
                        })
                        .ToDataSet();

                }
            }
            return ds;
        }

        public static void UpdateBondCouponRateLine(int original_Key, DateTime StartDate, DateTime EndDate, decimal CouponRate, int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IBond instrument = (IBond)InstrumentMapper.GetInstrument(session, instrumentId);
                if (instrument != null)
                {
                    IBondCouponRateHistory crh = null;
                    if (original_Key != 0)
                    {
                        crh = instrument.CouponRates.Where(x => x.Key == original_Key).FirstOrDefault();

                        if (crh == null)
                            throw new ApplicationException(string.Format("Could not find coupon {0}", original_Key));
                        crh.LastUpdatedBy = B4F.TotalGiro.Security.SecurityManager.CurrentUser;
                    }
                    else
                    {
                        if (instrument.CouponRates.Where(x => x.StartDate == StartDate).FirstOrDefault() != null)
                            throw new ApplicationException("There is already 1 coupon found for this date");

                        crh = new BondCouponRateHistory(StartDate);
                        instrument.CouponRates.AddCouponRate(crh);
                    }
                    crh.StartDate = StartDate;
                    crh.EndDate = EndDate;
                    crh.CouponRate = CouponRate;
                    instrument.CouponRates.CheckPeriodOverlap();
                    session.InsertOrUpdate(instrument);
                }
            }
        }

        public static void DeleteBondCouponRateLine(int couponRateHistoryId, int instrumentId)
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                IBond instrument = (IBond)InstrumentMapper.GetInstrument(session, instrumentId);
                if (instrument != null)
                {
                    IBondCouponRateHistory crh = null;
                    if (couponRateHistoryId != 0)
                    {
                        crh = instrument.CouponRates.Where(x => x.Key == couponRateHistoryId).FirstOrDefault();

                        if (crh == null)
                            throw new ApplicationException(string.Format("Could not find coupon {0}", couponRateHistoryId));

                        if (instrument.CouponRates.RemoveCouponRate(crh))
                            session.Update(instrument);
                    }
                }
            }
        }

        public static int GetCashManagementFundKey()
        {
            using (IDalSession session = NHSessionFactory.CreateSession())
            {
                int instrumentID = 0;
                IList<IInstrument> instruments = InstrumentMapper.GetInstruments(session, SecCategories.CashManagementFund);
                if (instruments != null && instruments.Count > 0)
                    instrumentID = instruments[0].Key;
                return instrumentID;
            }
        }
    }
}
