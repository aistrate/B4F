using System;
using System.Collections;
using B4F.TotalGiro.Base;
using B4F.TotalGiro.Fees;
using B4F.TotalGiro.GeneralLedger.Static;
using B4F.TotalGiro.Instruments.Prices;
using B4F.TotalGiro.Orders;
using B4F.TotalGiro.Orders.Transactions;
using B4F.TotalGiro.StaticData;

namespace B4F.TotalGiro.Instruments
{
    #region Enums

    // TODO Remove seccategory
    /// <summary>
    /// The security category of the instrument
    /// </summary>
	public enum SecCategories
	{
		/// <summary>
		/// This is a stock instrument
		/// </summary>
		Stock = 1,
        /// <summary>
        /// This is a stock instrument
        /// </summary>
        Bond = 2,
        /// <summary>
        /// This is a bond instrument
        /// </summary>
        Option = 8,
        /// <summary>
        /// This is an Option
        /// </summary>
        Index = 32,
        /// <summary>
        /// This is an Index
        /// </summary>
        Cash = 64,
        /// <summary>
        /// This is cash
        /// </summary>
        MutualFund = 128,
        /// <summary>
        /// This is a Cash Management Fund
        /// </summary>
        VirtualFund = 256,
        /// <summary>
        /// This is an undefined instrument
        /// </summary>
        CashManagementFund = 512,
        /// <summary>
        /// This is a BenchMark for Performance Measuring
        /// </summary>
        BenchMark = 1024,
        /// <summary>
        /// This is an undefined instrument
        /// </summary>
        Turbo = 2048,
        /// <summary>
        /// This is an Dividend
        /// </summary>
        StockDividend = 4096,
        /// <summary>
        /// This is an unknown 
        /// </summary>
        Undefined = 0
	}

    // TODO Remove this?
    /// <summary>
    /// The known currencies in the system
    /// </summary>
	public enum KnownCurrency
	{
		/// <summary>
		/// Euro
		/// </summary>
        Euro = 600,
        /// <summary>
        /// American Dollar
        /// </summary>
		Amerikaanse_Dollar = 601,
        /// <summary>
        /// English Pound
        /// </summary>
		Engelse_Pond = 602,
        /// <summary>
        /// Swiss Frank
        /// </summary>
		Zwitserse_Frank = 603,
        /// <summary>
        /// Australian Dollar
        /// </summary>
		Australische_Dollar = 604,
        /// <summary>
        /// Canadian Dollar
        /// </summary>
		Canadese_dollar = 605,
        /// <summary>
        /// Soth African Rand
        /// </summary>
		Zuid_Afrika_Rand = 606,
        /// <summary>
        /// Sweden Kronor
        /// </summary>
		Stockholm_exchange = 607,
        /// <summary>
        /// Hongkong Dollar
        /// </summary>
		Hongkong_Dollar = 608,
        /// <summary>
        /// Japanse Yen
        /// </summary>
		Japanse_Yen = 609,
        /// <summary>
        /// Korea Won
        /// </summary>
		Korea_Won = 610,
        /// <summary>
        /// Dutch Guilder
        /// </summary>
		Nederlandse_Gulden = 611,
        /// <summary>
        /// Luxembourg Frank
        /// </summary>
		Luxemburgse_Franc = 612,
        /// <summary>
        /// Belgian Frank
        /// </summary>
		Belgische_Frank = 613,
        /// <summary>
        /// Deutsche Mark
        /// </summary>
		Duitse_Mark = 614,
        /// <summary>
        /// French Frank
        /// </summary>
		Franse_Franc = 615,
        /// <summary>
        /// Spanish Peseta
        /// </summary>
		Spaanse_Peseta = 616,
        /// <summary>
        /// Irish Pound
        /// </summary>
		Ierse_Pond = 617,
        /// <summary>
        /// Italian Lire
        /// </summary>
		Italiaanse_Lire = 618,
        /// <summary>
        /// Portugese Escudo
        /// </summary>
		Portugese_Escudo = 619,
        /// <summary>
        /// Norwegian Kroon
        /// </summary>
		Noorse_Kroon = 620

	}

    /// <summary>
    /// This enumeration lists the possible results of the size prediction of an instrument.
    /// The size is predicted using the last known price/rate of the instrument.
    /// </summary>
    public enum PredictedSizeReturnValue
    {
        /// <summary>
        /// The prediction went ok
        /// </summary>
        OK,
        /// <summary>
        /// No price/rate was found for the instrument
        /// </summary>
        NoRate,
        /// <summary>
        /// The size has been predicted using a price/rate of an old date
        /// </summary>
        OldRateDate
    }

    /// <summary>
    /// The Pricing type of the instrument
    /// </summary>
    public enum PricingTypes
    {
        /// <summary>
        /// The price is a normal amount based price
        /// </summary>
        Direct = 1,
        /// <summary>
        /// The price is defined as a percentage of the nominal value
        /// </summary>
        Percentage = 2
    }

    #endregion

    /// <summary>
    /// Interface for the <see cref="T:B4F.TotalGiro.Instruments.Instrument">Instrument</see> class
    /// </summary>
    public interface IInstrument : ITotalGiroBase<IInstrument>
	{
        bool CalculateCosts(IOrder order, IFeeFactory feeFactory);
        bool CalculateCosts(IOrderAllocation transaction, IFeeFactory feeFactory, IGLLookupRecords lookups);
        bool IsActive { get; }
        bool IsCash { get; }
        bool IsCorporateAction { get; }

        bool IsCashManagementFund { get; }
        bool IsSecurity { get; }
        bool IsTradeable { get; }
        bool IsWithPrice { get; }
        decimal PriceTypeFactor { get; }
        bool Validate();

        DateTime DisplayCurrentPriceDate { get; }
        DateTime InActiveDate { get; }
        ICountry Country { get; }
        ICurrency ToCurrency { get; }
        IInstrument ParentInstrument { get; set;  }

        IInstrument TopParentInstrument { get; }
        IInstrumentCollection ConvertedChildInstruments { get; }
        IInstrumentSymbolCollection ExternalSymbols { get; }
        IList GetInstrumentPedigree();
        int DecimalPlaces { get; }

        IPriceDetail CurrentPrice { get; set; }
        ISecCategory SecCategory { get; }
        string DisplayCurrentPrice { get; }
        PredictedSize PredictSize(Money inputAmount);
        string DisplayIsin { get; }

        string DisplayIsinWithName { get; }
        string DisplayName { get; }
        string DisplayNameWithIsin { get; }
        string DisplayToString(Decimal Quantity);
        string Name { get; set; }
        string ToString();

        DateTime CreationDate { get; set; }
	}
}
